using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kickstarter.Core.Services
{
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
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } },
                };
                var response = await client.GetItemAsync(request);
                post = JsonConvert.SerializeObject(response.Item);
            }
            return post;
        }
    }
}
