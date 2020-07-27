using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kickstarter.Core.Services
{

    public static class TagsService
    {
        public static async Task<string> GetTopTagsAsync()
        {
            string datemonth = DateTime.Now.ToString("MMyyyy");
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new QueryRequest
                {
                    TableName = "tags",
                    KeyConditionExpression = "datemonth = :v_datemonth",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_datemonth", new AttributeValue { N =  datemonth }}},
                    Limit = 10
                };
                var queryResponse = await client.QueryAsync(request);
                List<Dictionary<string,string>> tagsResult = new List<Dictionary<string,string>>();
                foreach (Dictionary<string, AttributeValue> value in queryResponse.Items)
                {
                    tagsResult.Add(new Dictionary<string,string> {{"tag", value["tag"].S}, {"occurrences", value["occurrences"].N}});
                }
                response = JsonConvert.SerializeObject(tagsResult);
            }
            return response;
        }
    }
}
