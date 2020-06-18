using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Kickstarter.Core.Services;
using System;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Kickstarter.API
{
    public class Functions
    {
        /// <summary>
        /// GET: /getPost/{postid}/
        /// Fetches the Post of the given id passed through the request's path parameter {postid}
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetPost(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = request.PathParameters["postid"];

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await PostsService.GetPostByIdAsync(id);
            }
            catch (Exception e)
            {
                int statusCode = response.StatusCode = Convert.ToInt32(e.Message);
                response.Body = statusCode switch
                {
                    (int)HttpStatusCode.NotFound => "Requested post does not exist",
                    _ => "Failed to load post",
                };
            }

            return response;
        }

        /// <summary>
        /// GET: /followUser/{userid}/
        /// Gets the caller to follow the user whose id is {userid} given in path
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> FollowUser(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string userId = request.PathParameters["postid"];
            string followerId = request.RequestContext.Identity.User;

            var response = new APIGatewayProxyResponse();
            try
            {
                response.StatusCode = (int)await UsersService.FollowUser(
                    followerId: Convert.ToUInt32(followerId),
                    userId: Convert.ToUInt32(userId));
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format for the user id";
            }

            return response;
        }
    }
}
