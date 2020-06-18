using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace graduation_project
{
    public class Functions
    {
        private async Task<string> GetPostByIdAsync(string id)
        {
            string post;
            using (var client = new AmazonDynamoDBClient())
            {

                var request = new GetItemRequest
                {
                    TableName = "posts",
                    Key = new Dictionary<string,AttributeValue>() { { "id", new AttributeValue { S = id } } },
                };
                var response = await client.GetItemAsync(request);
                post = JsonConvert.SerializeObject(response.Item);
            }
            return post;
        }


        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = request.PathParameters["postid"];
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = await GetPostByIdAsync(id),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
            return response;
        }
    }
}
