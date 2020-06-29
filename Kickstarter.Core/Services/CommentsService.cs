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
                updates["comments"] = new AttributeValueUpdate()
                {
                    Action = AttributeAction.ADD,
                    Value = new AttributeValue { L = new List<AttributeValue> { new AttributeValue { M = attributes } } }
                };
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
    }
}
