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
    public class Comments
    {
        public List<Comment> comments = new List<Comment>();
        public string lastKey;
        
        public void Add(Comment comment)
        {
            comments.Add(comment);
        }
    }

    public class Comment
    {
        public string id, username, date, content, postid;

        public Comment(Dictionary<string, AttributeValue> value)
        {
            id = value["id"].S;
            postid = value["postid"].S;
            username = value["username"].S;
            date = value["date"].S;
            content = value["content"].S;

        }

    }

    public static class CommentsServices
    {
        public static async Task<string> AddCommentAsync(Newtonsoft.Json.Linq.JObject Comment)
        {
            string response;
            string id = Guid.NewGuid().ToString();
            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue> { { "id", new AttributeValue { S = id } },
                                                              { "postid", new AttributeValue { S = Comment["postid"].ToString() } },
                                                              { "content", new AttributeValue { S = Comment["content"].ToString() } },
                                                              { "username", new AttributeValue { S = Comment["username"].ToString() } },
                                                              { "date", new AttributeValue { S = DateTime.Now.ToString() } }  };
            using (var client = new AmazonDynamoDBClient())
            {
               Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = Comment["postid"].ToString() } }
                };

                Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate>();
                updates["commentsNumber"] = new AttributeValueUpdate()
                {
                    Action = AttributeAction.ADD,
                    Value = new AttributeValue { N = "1" }
                };

                UpdateItemRequest PostRequest = new UpdateItemRequest
                {
                    TableName = "posts",
                    Key = key,
                    AttributeUpdates = updates
                };
               await client.UpdateItemAsync(PostRequest);
               PutItemRequest CommentRequest = new PutItemRequest
               {
                   TableName = "comments",
                   Item = attributes
               };
               await client.PutItemAsync(CommentRequest);
               response = id;
            }
            return response;
        }

        public static async Task<string> GetCommentsAsync(string id, string lastKey)
        {
            Dictionary<string,AttributeValue> lastKeyEvaluated = new Dictionary<string,AttributeValue> { { "id", new AttributeValue { S = lastKey } },
                                                                                                         { "postid", new AttributeValue { S = id } }};
            if (lastKey == null) lastKeyEvaluated = null;
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               var request = new QueryRequest
                {
                    TableName = "comments",
                    IndexName = "postid-index",
                    KeyConditionExpression = "postid = :v_Id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Id", new AttributeValue { S =  id }}},
                    Limit = 10,
                    ExclusiveStartKey = lastKeyEvaluated,
                    ScanIndexForward = true
                };

                var queryResponse = await client.QueryAsync(request);
                Comments commentsResult = new Comments();
                if (queryResponse.LastEvaluatedKey != null && queryResponse.LastEvaluatedKey.Count != 0) {
                    commentsResult.lastKey = queryResponse.LastEvaluatedKey["id"].S;
                }
                foreach (Dictionary<string, AttributeValue> value in queryResponse.Items)
                {
                    commentsResult.Add(new Comment(value));
                }
                response = JsonConvert.SerializeObject(commentsResult);
            }
            return response;
        }

        public static async Task<string> DeleteCommentAsync(string id, string username)
        {
            Dictionary<string,AttributeValue> key = new Dictionary<string,AttributeValue> { { "id", new AttributeValue { S = id } } };
            string response;
            using (var client = new AmazonDynamoDBClient())
            {
               var getRequest = new GetItemRequest
                {
                    TableName = "comments",
                    Key = key
                };
                var comment = await client.GetItemAsync(getRequest);
                
                if (comment.Item.Count == 0)
                {
                    client.Dispose();
                    throw new Exception("404");
                } else if(comment.Item["username"].S != username) {
                    client.Dispose();
                    throw new Exception("Access Denied");
                }
               var request = new DeleteItemRequest
                {
                    TableName = "comments",
                    Key = key
                };
                var queryResponse = await client.DeleteItemAsync(request);
                response = JsonConvert.SerializeObject(queryResponse);
            }
            return response;
        }
    }
}
