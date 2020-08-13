using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kickstarter.Data;
using Kickstarter.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kickstarter.Core.Services
{
    public class Posts
    {
        public List<Post> posts = new List<Post>();
        public string lastKey;
        
        public void Add(Post post)
        {
            posts.Add(post);
        }
    }

    public class Post
    {
        public string id, username, date, category, title, content, likes, dislikes, commentsNumber;
        public List<string> tags = new List<string>(), attachments = new List<string>();
        public Post(Dictionary<string, AttributeValue> post)
        {
            id = post["id"].S;
            username = post["username"].S;
            date = post["date"].S;
            category = post["category"].S;
            tags = post["tags"].SS;
            attachments = post["attachments"].SS;
            title = post["title"].S;
            content = post["content"].S;
            likes = post["likes"].N;
            dislikes = post["dislikes"].N;
            commentsNumber = post["commentsNumber"].N;
        }
    }
    public static class PostsService
    {
        public static async Task<string> GetPostByIdAsync(string id)
        {
            string post;
            using (var client = new AmazonDynamoDBClient())
            {
                var request = new GetItemRequest
                {
                    TableName = "posts",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } }
                };
                var response = await client.GetItemAsync(request);
                
                if (response.Item.Count == 0)
                {
                    client.Dispose();
                    throw new Exception("404");
                }
                post = JsonConvert.SerializeObject(new Post(response.Item));
            }
            return post;
        }

        public static async Task<string> AddPostAsync(Newtonsoft.Json.Linq.JObject Post)
        {
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
               attributes["category"] = new AttributeValue { S = Post["category"].ToString() };
               attributes["title"] = new AttributeValue { S = Post["title"].ToString() };
               attributes["content"] = new AttributeValue { S = Post["content"].ToString() };
               attributes["date"] = new AttributeValue { S = DateTime.Now.ToString() };
               attributes["username"] = new AttributeValue { S = Post["username"].ToString() };
               attributes["tags"] = new AttributeValue
               {
                   SS =  JsonConvert.DeserializeObject<List<string>>(Post["tags"].ToString())
               };
                attributes["attachments"] = new AttributeValue
               {
                   SS =  JsonConvert.DeserializeObject<List<string>>(Post["attachments"].ToString())
               };

               //default attributes
               attributes["likes"] = new AttributeValue { N = "0" };
               attributes["dislikes"] = new AttributeValue { N = "0" };
               attributes["commentsNumber"] = new AttributeValue { N = "0" };

               string id = Guid.NewGuid().ToString();
               attributes["id"] = new AttributeValue { S = id };

               PutItemRequest request = new PutItemRequest
               {
                   TableName = "pending-review",
                   Item = attributes
               };
               await client.PutItemAsync(request);
               client.Dispose();
               response = id;
            }
            return response;
        }

        public static async Task<string> GetPendingPostsAsync(string category, string lastKey)
        {
            Dictionary<string,AttributeValue> lastKeyEvaluated = new Dictionary<string,AttributeValue> { { "id", new AttributeValue { S = lastKey } },
                                                                                                         { "category", new AttributeValue { S = category } }};
            if (lastKey == null) lastKeyEvaluated = null;
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new QueryRequest
                {
                    TableName = "pending-review",
                    IndexName = "category-index",
                    KeyConditionExpression = "category = :v_category",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_category", new AttributeValue { S =  category }}},
                    Limit = 10,
                    ExclusiveStartKey = lastKeyEvaluated,
                    ScanIndexForward = true
                };

                var queryResponse = await client.QueryAsync(request);
                client.Dispose();
                Posts postsResult = new Posts();
                if (queryResponse.LastEvaluatedKey != null && queryResponse.LastEvaluatedKey.Count != 0) {
                    postsResult.lastKey = queryResponse.LastEvaluatedKey["id"].S;
                }
                foreach (Dictionary<string, AttributeValue> value in queryResponse.Items)
                {
                    postsResult.Add(new Post(value));
                }
                response = JsonConvert.SerializeObject(postsResult);
            }
            return response;
        }

        public static async Task<HttpStatusCode> ApprovePendingPostsAsync(string id)
        {
            var dbContext = new SQLDbContext();
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new GetItemRequest
                {
                    TableName = "pending-review",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } }
                };
                var response = await client.GetItemAsync(request);

                dbContext.Post.Add(new Kickstarter.Data.Models.Post {PostId=id, Username=response.Item["username"].S, Category=response.Item["category"].S, Title=response.Item["title"].S, Date=Convert.ToDateTime(response.Item["date"].S)});
                await dbContext.SaveChangesAsync();
                
                if (response.Item.Count == 0)
                {
                    client.Dispose();
                    throw new Exception("404");
                }
                PutItemRequest putRequest = new PutItemRequest
                {
                    TableName = "posts",
                    Item = response.Item
                };
               await client.PutItemAsync(putRequest);
               DeleteItemRequest deleteRequest = new DeleteItemRequest
               {
                    TableName = "pending-review",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } }
               };
                await client.DeleteItemAsync(deleteRequest);
                client.Dispose();

                return HttpStatusCode.OK;
            }
        }

        public static async Task<string> GetPendingPostByIdAsync(string id)
        {
            string post;
            using (var client = new AmazonDynamoDBClient())
            {
                var request = new GetItemRequest
                {
                    TableName = "pending-review",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } }
                };
                var response = await client.GetItemAsync(request);
                
                if (response.Item.Count == 0)
                {
                    client.Dispose();
                    throw new Exception("404");
                }
                post = JsonConvert.SerializeObject(new Post(response.Item));
            }
            return post;
        }

        public static async Task<string> GetHomeBagePostsAsync(string lastKey)
        {
            Dictionary<string,AttributeValue> lastKeyEvaluated;
            if (lastKey == null || lastKey.Length == 0) lastKeyEvaluated = null;
            else
            {
                lastKeyEvaluated = new Dictionary<string,AttributeValue> { { "id", new AttributeValue { S = lastKey } } };
            }
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new ScanRequest
                {
                    TableName = "posts",
                    Limit = 10,
                    ExclusiveStartKey = lastKeyEvaluated
                };

                var queryResponse = await client.ScanAsync(request);
                client.Dispose();

                Posts postsResult = new Posts();
                if (queryResponse.LastEvaluatedKey != null && queryResponse.LastEvaluatedKey.Count != 0) {
                    postsResult.lastKey = queryResponse.LastEvaluatedKey["id"].S;
                }
                foreach (Dictionary<string, AttributeValue> value in queryResponse.Items)
                {
                    postsResult.Add(new Post(value));
                }
                response = JsonConvert.SerializeObject(postsResult);
            }
            return response;
        }

        public static async Task<HttpStatusCode> Vote(string postid, string username, string direction)
        {
            var dbContext = new SQLDbContext();
            var client = new AmazonDynamoDBClient();

            UserVotes userVote = await dbContext.UserVotes.SingleOrDefaultAsync(e => e.PostId == postid && e.Username == username);
            
            /*Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = postid } }
                };

                Dictionary<string, AttributeValueUpdate> likesUpdates = new Dictionary<string, AttributeValueUpdate>();
                likesUpdates["likes"] = new AttributeValueUpdate()
                {
                    Action = AttributeAction.ADD,
                    Value = new AttributeValue { N = "1" }
                };

                Dictionary<string, AttributeValueUpdate> dislikesUpdates = new Dictionary<string, AttributeValueUpdate>();
                dislikesUpdates["dislikes"] = new AttributeValueUpdate()
                {
                    Action = AttributeAction.ADD,
                    Value = new AttributeValue { N = "1" }
                };*/

            if (userVote != null) {
                if (userVote.Direction == direction) return HttpStatusCode.OK;                
                userVote.Direction = direction;
                await dbContext.SaveChangesAsync();

                /*if (direction == "up") {
                    likesUpdates["dislikes"] = new AttributeValueUpdate()
                    {
                        Action = AttributeAction.ADD,
                        Value = new AttributeValue { N = "-1" }
                    };
                    UpdateItemRequest PostRequest = new UpdateItemRequest
                    {
                        TableName = "posts",
                        Key = key,
                        AttributeUpdates = likesUpdates
                    };
                    await client.UpdateItemAsync(PostRequest);
                } else {
                    dislikesUpdates["likes"] = new AttributeValueUpdate()
                    {
                        Action = AttributeAction.ADD,
                        Value = new AttributeValue { N = "-1" }
                    };
                    UpdateItemRequest PostRequest = new UpdateItemRequest
                    {
                        TableName = "posts",
                        Key = key,
                        AttributeUpdates = dislikesUpdates
                    };
                    await client.UpdateItemAsync(PostRequest);
                }*/
            } else {
                /*if (direction == "up") {
                    UpdateItemRequest PostRequest = new UpdateItemRequest
                    {
                        TableName = "posts",
                        Key = key,
                        AttributeUpdates = likesUpdates
                    };
                    await client.UpdateItemAsync(PostRequest);
                } else {
                    UpdateItemRequest PostRequest = new UpdateItemRequest
                    {
                        TableName = "posts",
                        Key = key,
                        AttributeUpdates = dislikesUpdates
                    };
                    await client.UpdateItemAsync(PostRequest);
                }*/
                await dbContext.UserVotes.AddAsync (new UserVotes
                {
                    Username = username,
                    PostId = postid,
                    Direction = direction
                });
                await dbContext.SaveChangesAsync();
            }
            client.Dispose();
            return HttpStatusCode.OK;
        }

        public static async Task<string> GetVote(string postid, string username)
        {
            var dbContext = new SQLDbContext();
            if (username != null) {
                UserVotes userVote = await dbContext.UserVotes.SingleOrDefaultAsync(e => e.PostId == postid && e.Username == username);
                if (userVote == null) return "none";
                else return userVote.Direction;
            }

            var usersVote = await dbContext.UserVotes.Where(e => e.PostId == postid).Select(e => new {e.Username, e.Direction}).ToListAsync();
            return JsonConvert.SerializeObject(usersVote);
        }
    }
}
