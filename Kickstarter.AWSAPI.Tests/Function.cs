using System;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using System.Collections.Generic;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Kickstarter.AWSAPI.Tests
{
    public class Function
    {
        private static AmazonDynamoDBClient _dynamoDBClient = new AmazonDynamoDBClient();

        public async Task TestLambdaFunction(APIGatewayProxyRequest inputRequest, ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning to process event: {inputRequest.Path}");

            foreach (var parameterKeyValue in inputRequest.PathParameters)
            {
                context.Logger.LogLine($"Path Parameter: {parameterKeyValue.Key}" +
                    $"-> Value: {parameterKeyValue.Value}");
            }

            /* Access DynamoDB */

            context.Logger.LogLine($"Connected DynamoDB was observed to have the tables:");

            var tablesListing = await _dynamoDBClient.ListTablesAsync();

            foreach (string tableName in tablesListing.TableNames)
            {
                context.Logger.LogLine(tableName);
            }

            // TODO: Add business logic processing the request

            context.Logger.LogLine("Processing request complete.");
        }

        public Result HelloWorld(APIGatewayProxyRequest inputRequest, ILambdaContext context)
        {
            Result result = new Result();

            /* Setup result */

            result.Data["Data param 1 - Hello"] = $"Hello World";
            result.Data["Data param 2 - Given APIGatewayProxyRequest body"] = inputRequest.Body;
            result.Data["Data param 3 and last"] = $"Last data parameter";

            result.Error["code"] = "200";
            result.Error["message"] = "All OK!";

            return result;
        }
    }
}