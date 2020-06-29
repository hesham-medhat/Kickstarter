using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Kickstarter.Core.Services;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Kickstarter.API
{
    public class Functions
    {
        /// <summary>
        /// GET: /posts/{postid}/
        /// Fetches the Post of the given id passed through the request's path parameter {postid}
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetPost(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = request.PathParameters["postid"];

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
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
        /// Post: /users/{userid}/
        /// Gets the caller to follow the user whose id is {userid} given in path
        /// A Lambda function to respond to HTTP Post methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> FollowUser(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string userId = request.PathParameters["userid"];
            string followerId = request.RequestContext.Identity.User;

            var response = new APIGatewayProxyResponse() {
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };
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


        /// <summary>
        /// Put: /posts/
        /// Puts a new post to dynamodb
        /// A Lambda function to respond to HTTP Put methods from API Gateway
        /// Example payload: {
        ///    "categoryId": "1234",
        ///    "title": "post title",
        ///    "content": "post content",
        ///    "username": "username",
        ///    "tags": [
        ///        "tag1",
        ///        "tag2",
        ///        "tag3"
        ///        ]
        /// }
        /// </summary>
        public async Task<APIGatewayProxyResponse> AddPost(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var Post = JObject.Parse(request.Body);
            var response = new APIGatewayProxyResponse() {
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };
            try
            {
                string body = await PostsService.AddPostAsync(Post);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = "{ \"postId\" : " + body + " } ";
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /comments/
        /// Puts a comment to a post
        /// A Lambda function to respond to HTTP Put methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> AddComment(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var Comment = JObject.Parse(request.Body);
            var response = new APIGatewayProxyResponse() {
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };
            try
            {
                string body = await CommentsServices.AddCommentAsync(Comment);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = "{ \"commentId\" : " + body + " } ";
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }
    }
}
