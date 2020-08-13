using Kickstarter.Data;
using Kickstarter.Data.Models;

using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Kickstarter.Core.Services
{
    public static class UtilsService
    {
        /// <summary>
        /// Returns all countries available
        /// </summary>
        /// <returns>countries list as json object</returns>
        public static async Task<string> GetCountries()
        {
            var dbContext = new SQLDbContext();
            var countries = await dbContext.Country.Select(c => new {c.CountryId ,c.Name, c.IsoCode2, c.IsoCode3}).ToListAsync();
            return JsonConvert.SerializeObject(countries);
        }

        /// <summary>
        /// Returns all zones of a country
        /// </summary>
        /// <returns>countries list as json object</returns>
        public static async Task<string> GetZones(int countryId)
        {
            var dbContext = new SQLDbContext();
            var zones = await dbContext.Zone.Where(z => z.CountryId == countryId).Select(z => new {z.ZoneId ,z.Name}).ToListAsync();
            return JsonConvert.SerializeObject(zones);
        }

        /// <summary>
        /// Returns all categories available
        /// </summary>
        /// <returns>Categories list as json object</returns>
        public static async Task<string> GetCategories()
        {
            var dbContext = new SQLDbContext();
            var categories = await dbContext.Category.Select(c => new {c.Category1}).ToListAsync();
            List<string> categoriesList = new List<string>();
            foreach (var i in categories) {
                categoriesList.Add(i.Category1);
            }
            return JsonConvert.SerializeObject(categoriesList);
        }

        /// <summary>
        /// Creates a presigned url for S3 bucket
        /// </summary>
        /// <returns>Presigned url</returns>
        public static string GeneratePreSignedURL()
        {
            const string bucketName = "kickstarter-attachments";
            RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
            IAmazonS3 s3Client = new AmazonS3Client(bucketRegion);
            const string objectKey = "{filename}";

            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key        = objectKey,
                Verb       = HttpVerb.PUT,
                Expires    = DateTime.Now.AddMinutes(5)
            };

            string url = s3Client.GetPreSignedURL(urlRequest);
            return url;
        }

        /// <summary>
        /// Creates a record in RDS attachments table for recently added attachment
        /// </summary>
        /// <returns>Status code</returns>
        public static async Task<HttpStatusCode> S3ObjectCreated(string key)
        {
            using (var dbContext = new SQLDbContext())
            {
                await dbContext.Attachments.AddAsync(new Attachments
                {
                    Name = key
                });
                await dbContext.SaveChangesAsync();
            }
            return HttpStatusCode.OK;
        }
    }

}
