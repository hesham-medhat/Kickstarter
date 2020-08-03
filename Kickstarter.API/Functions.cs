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
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
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
                    followerId: Convert.ToInt32(followerId),
                    userId: Convert.ToInt32(userId));
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
        ///    "category": "DeepLearning",
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
        /// Example payload: {
        ///    "username": "username",
        ///    "postid": "postid",
        ///    "content": "comment content"
        /// }
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

        /// <summary>
        /// Comments: /comments/
        /// Gets up to 10 comments of a post
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetComments(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = request.QueryStringParameters["postid"];
            string lastKey;
            try
            {
            lastKey = request.QueryStringParameters["lastkey"];
            } catch (Exception)
            {
                lastKey = null;
            }

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await CommentsServices.GetCommentsAsync(id, lastKey);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /comments/{commentid}?userid=id
        /// Deletes a comment
        /// A Lambda function to respond to HTTP Delete methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> DeleteComment(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = request.PathParameters["commentid"];
            string username = request.QueryStringParameters["username"];

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };


            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await CommentsServices.DeleteCommentAsync(id, username);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            }

            return response;
        }

        /// <summary>
        /// Comments: /posts/pending/{category}
        /// Gets up to 10 posts with a category
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetPendingPosts(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string category = request.PathParameters["category"];
            string lastKey;
            try
            {
            lastKey = request.QueryStringParameters["lastkey"];
            } catch (Exception)
            {
                lastKey = null;
            }

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await PostsService.GetPendingPostsAsync(category, lastKey);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /posts/pending/{category}/{postid}
        /// Approve a pending post
        /// A Lambda function to respond to HTTP Post methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> ApprovePendingPosts(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = request.PathParameters["postid"];

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };

            try
            {
                response.StatusCode = (int)await PostsService.ApprovePendingPostsAsync(id);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /posts/
        /// Gets up to 10 posts
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetHomeBagePosts(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string lastKey;
            try
            {
            lastKey = request.QueryStringParameters["lastkey"];
            } catch (Exception)
            {
                lastKey = null;
            }

            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };
            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await PostsService.GetHomeBagePostsAsync(lastKey);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /tags/
        /// Gets top 10 tags
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetTopTags(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await TagsService.GetTopTagsAsync();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /utils/countries/
        /// Gets all countries
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetCountries(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await UtilsService.GetCountries();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Body = "Input given was in an incorrect format";
            }

            return response;
        }

        /// <summary>
        /// Comments: /utils/zones/
        /// Gets all zones of a country
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        public async Task<APIGatewayProxyResponse> GetZones(APIGatewayProxyRequest request, ILambdaContext context)
        {
            int country = Convert.ToInt32(request.QueryStringParameters["countryid"]);
            var response = new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "access-control-allow-origin", "*" }, { "Access-Control-Allow-Credentials", "true" } }
            };

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = await UtilsService.GetZones(country);
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
