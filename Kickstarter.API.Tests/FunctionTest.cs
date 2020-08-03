using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Kickstarter.API;

namespace Kickstarter.API.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
        }
	/*

        [Fact]
        public async Task TetGetMethodAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions();


            request = new APIGatewayProxyRequest();
            context = new TestLambdaContext();
            response = await functions.Get(request, context);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("Hello AWS Serverless", response.Body);
        }*/
    }
}
