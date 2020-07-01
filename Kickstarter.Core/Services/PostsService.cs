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
    public class Posts
    {
        public List<Post> posts = new List<Post>();
        public string lastKey;
        
        public void Add(Post post)
        {
            posts.Add(post);
        }
    }

    public class Post
    {
        public string id, username, date, category, title, content, likes, dislikes, commentsNumber;
        public List<string> tags = new List<string>();
        public Post(Dictionary<string, AttributeValue> post)
        {
            id = post["id"].S;
            username = post["username"].S;
            date = post["date"].S;
            category = post["category"].S;
            tags = post["tags"].SS;
            title = post["title"].S;
            content = post["content"].S;
            likes = post["likes"].N;
            dislikes = post["dislikes"].N;
            commentsNumber = post["commentsNumber"].N;
        }
    }
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
                
                if (response.Item.Count == 0)
                {
                    client.Dispose();
                    throw new Exception("404");
                }
                post = JsonConvert.SerializeObject(new Post(response.Item));
            }
            return post;
        }

        public static async Task<string> AddPostAsync(Newtonsoft.Json.Linq.JObject Post)
        {
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
               attributes["category"] = new AttributeValue { S = Post["category"].ToString() };
               attributes["title"] = new AttributeValue { S = Post["title"].ToString() };
               attributes["content"] = new AttributeValue { S = Post["content"].ToString() };
               attributes["date"] = new AttributeValue { S = DateTime.Now.ToString() };
               attributes["username"] = new AttributeValue { S = Post["username"].ToString() };
               attributes["tags"] = new AttributeValue
               {
                   SS =  JsonConvert.DeserializeObject<List<string>>(Post["tags"].ToString())
               };

               //default attributes
               attributes["likes"] = new AttributeValue { N = "0" };
               attributes["dislikes"] = new AttributeValue { N = "0" };
               attributes["commentsNumber"] = new AttributeValue { N = "0" };

               string id = Guid.NewGuid().ToString();
               attributes["id"] = new AttributeValue { S = id };

               PutItemRequest request = new PutItemRequest
               {
                   TableName = "pending-review",
                   Item = attributes
               };
               await client.PutItemAsync(request);
               response = id;
            }
            return response;
        }

        public static async Task<string> GetPendingPostsAsync(string category, string lastKey)
        {
            Dictionary<string,AttributeValue> lastKeyEvaluated = new Dictionary<string,AttributeValue> { { "id", new AttributeValue { S = lastKey } },
                                                                                                         { "category", new AttributeValue { S = category } }};
            if (lastKey == null) lastKeyEvaluated = null;
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new QueryRequest
                {
                    TableName = "pending-review",
                    IndexName = "category-index",
                    KeyConditionExpression = "category = :v_category",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_category", new AttributeValue { S =  category }}},
                    Limit = 10,
                    ExclusiveStartKey = lastKeyEvaluated,
                    ScanIndexForward = true
                };

                var queryResponse = await client.QueryAsync(request);
                Posts postsResult = new Posts();
                if (queryResponse.LastEvaluatedKey != null && queryResponse.LastEvaluatedKey.Count != 0) {
                    postsResult.lastKey = queryResponse.LastEvaluatedKey["id"].S;
                }
                foreach (Dictionary<string, AttributeValue> value in queryResponse.Items)
                {
                    postsResult.Add(new Post(value));
                }
                response = JsonConvert.SerializeObject(postsResult);
            }
            return response;
        }

        public static async Task<HttpStatusCode> ApprovePendingPostsAsync(string id)
        {
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new GetItemRequest
                {
                    TableName = "pending-review",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } }
                };
                var response = await client.GetItemAsync(request);
                
                if (response.Item.Count == 0)
                {
                    client.Dispose();
                    throw new Exception("404");
                }
                PutItemRequest putRequest = new PutItemRequest
                {
                    TableName = "posts",
                    Item = response.Item
                };
               await client.PutItemAsync(putRequest);
               DeleteItemRequest deleteRequest = new DeleteItemRequest
               {
                    TableName = "pending-review",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = id } } }
               };
                await client.DeleteItemAsync(deleteRequest);
                return HttpStatusCode.OK;
            }
        }
    }
}
