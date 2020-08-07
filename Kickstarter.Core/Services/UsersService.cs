using Kickstarter.Data;
using Kickstarter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;

namespace Kickstarter.Core.Services
{
    public static class UsersService
    {
        /// <summary>
        /// Returns the status code of the request
        /// </summary>
        /// <param name="followerId">User following</param>
        /// <param name="userId">User to be followed</param>
        /// <returns>HTTP status code of the operation</returns>
        public static async Task<HttpStatusCode> FollowUser(int followerId, int userId)
        {
            HttpStatusCode resultCode;
            using (var dbContext = new SQLDbContext())
            {
                if (await dbContext.User.FindAsync(userId) == null)
                {
                    resultCode = HttpStatusCode.NotFound;
                }
                else
                {
                    await dbContext.FollowerToUser.AddAsync(new FollowerToUser
                    {
                        FollowerId = followerId,
                        UserId = userId,
                    });

                    await dbContext.SaveChangesAsync();

                    resultCode = HttpStatusCode.OK;
                }
            }
            return resultCode;
        }

        private static string HashPassword (string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        private static bool ValidatePassword (string savedPasswordHash, string password) {
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i=0; i < 20; i++)
                if (hashBytes[i+16] != hash[i])
                    return false;
            return true;
        }

        public static async Task<HttpStatusCode> Register(Newtonsoft.Json.Linq.JObject newUser)
        {
            HttpStatusCode resultCode;
            using (var dbContext = new SQLDbContext())
            {
                var address = new Address{
                    Address1 = newUser["address1"].ToString(),
                    Address2 = newUser["address2"].ToString(),
                    City = newUser["city"].ToString(),
                    Postcode = newUser["postcode"].ToString(),
                    CountryId = Convert.ToInt32(newUser["countryid"].ToString()),
                    ZoneId = Convert.ToInt32(newUser["zoneid"].ToString())
                };

                var user = new User{
                    Username = newUser["username"].ToString(),
                    Password = HashPassword(newUser["password"].ToString()),
                    Firstname = newUser["firstname"].ToString(),
                    Lastname = newUser["lastname"].ToString(),
                    Email = newUser["email"].ToString(),
                    Address = address,
                    Telephone = newUser["telephone"].ToString()
                };

                await dbContext.User.AddAsync(user);
                await dbContext.SaveChangesAsync();

                resultCode = HttpStatusCode.OK;
            }
            return resultCode;
        }

        public static async Task<HttpStatusCode> SignIn(string username, string  password)
        {
            HttpStatusCode resultCode;
            using (var dbContext = new SQLDbContext())
            {
                User user = await dbContext.User.SingleOrDefaultAsync(user => user.Username == username);
                if (user == null)
                {
                    resultCode = HttpStatusCode.NotFound;
                } else {
                    string savedPasswordHash = user.Password;
                    if (ValidatePassword(savedPasswordHash, password))
                    {
                        resultCode = HttpStatusCode.OK;
                    } else {
                        resultCode = HttpStatusCode.NotFound;
                    }
                }
            }
            return resultCode;
        }

        public static async Task<string> ExpertsCategories (string username)
        {
            var dbContext = new SQLDbContext();
            var res = await dbContext.ExpertToCategory.Where(c => c.Username == username).Select(c => new {c.Category}).ToListAsync();
            return JsonConvert.SerializeObject(res);
        }

        public static async Task<string> GetAccount (string username)
        {
                    var dbContext = new SQLDbContext();
                    var user = await dbContext.User.FirstOrDefaultAsync(user => user.Username == username);
                    var address = await dbContext.Address.FirstOrDefaultAsync(address => address.AddressId == user.AddressId);
                    var zone = await dbContext.Zone.FirstOrDefaultAsync(zone => zone.ZoneId == address.ZoneId);
                    var country = await dbContext.Country.FirstOrDefaultAsync(country => country.CountryId == zone.CountryId);
                    Dictionary<string, dynamic> res = new Dictionary<string, dynamic> {{"username", user.Username}, {"firstname", user.Firstname}, {"lastname", user.Lastname}, {"email", user.Email}, {"telephone", user.Telephone}, {"address1", address.Address1}, {"address2", address.Address2}, {"city", address.City}, {"postcode", address.Postcode}, {"country", country.Name}, {"zone", zone.Name}};
                    return JsonConvert.SerializeObject(res);
        }

        public static async Task<string> GetProfile (string username)
        {
                    var dbContext = new SQLDbContext();
                    var user = await dbContext.User.FirstOrDefaultAsync(user => user.Username == username);
                    var expertises = await dbContext.ExpertToCategory.Where(u => u.Username == username).Select(u => new {u.Category}).ToListAsync();
                    var postsResult = await dbContext.Post.Where(p => p.Username == username).Select(p => new {p.PostId, p.Category, p.Username, p.Title, p.Date}).ToListAsync();
                    
                    Dictionary<string, dynamic> res = new Dictionary<string, dynamic> { {"username", username}, {"points",  user.Points}, {"expertises", expertises }, {"posts", postsResult} };
                    return JsonConvert.SerializeObject(res);




            /*string response;
            using (var dbContext = new SQLDbContext())
            {
                var client = new AmazonDynamoDBClient();
                User user = await dbContext.User.SingleOrDefaultAsync(user => user.Username == username);
                if (user == null)
                {
                    throw new KeyNotFoundException();
                } else {
                    var request = new QueryRequest
                    {
                        TableName = "posts",
                        IndexName = "username-index",
                        KeyConditionExpression = "username = :v_username",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            {":v_username", new AttributeValue { S =  username }}},
                        Limit = 5
                    };

                    var queryResponse = await client.QueryAsync(request);
                    Posts postsResult = new Posts();
                    foreach (Dictionary<string, AttributeValue> value in queryResponse.Items)
                    {
                        postsResult.Add(new Post(value));
                    }
                    Dictionary<string, string> res = new Dictionary<string, string> { {"username", username}, {"points",  user.Points.ToString()}, {"expertises", JsonConvert.SerializeObject(user.ExpertToCategory) }, {"posts", JsonConvert.SerializeObject(postsResult)} };
                    response = JsonConvert.SerializeObject(res);
                }
            }
            return response;*/
        }

        private static bool IsExpert(string username)
        {
            //TODO: check if a user is expert or not
            return true;
        }

    }

}
