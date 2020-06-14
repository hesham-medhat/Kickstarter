using System;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace API
{
    public class Function
    {
        public void TestLambdaFunction (APIGatewayProxyRequest inputRequest, ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning to process event: {inputRequest.Path}");

            foreach (var parameterKeyValue in inputRequest.PathParameters)
            {
                context.Logger.LogLine($"Path Parameter: {parameterKeyValue.Key}" +
                    $"-> Value: {parameterKeyValue.Value}");
            }

            // TODO: Add business logic processing the request

            context.Logger.LogLine("Processing request complete.");
        }
    }
}