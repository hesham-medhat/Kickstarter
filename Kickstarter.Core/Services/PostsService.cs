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
    public class Post
    {
        public string id, username, date, categoryId, title, content, likes, dislikes;
        public bool moreComments;
        public List<string> tags = new List<string>();
        public List<Dictionary<string, string>> comments = new List<Dictionary<string, string>>();
        public Post(Dictionary<string, AttributeValue> post)
        {
            id = post["id"].S;
            username = post["username"].S;
            date = post["date"].S;
            categoryId = post["categoryId"].S;
            tags = post["tags"].SS;
            title = post["title"].S;
            content = post["content"].S;
            likes = post["likes"].N;
            dislikes = post["dislikes"].N;
            moreComments = post["moreComments"].BOOL;

            foreach(AttributeValue value in post["comments"].L) {
                Dictionary<string, string> comment = new Dictionary<string, string>();
                comment["id"] = value.M["id"].S;
                comment["postid"] = value.M["postid"].S;
                comment["username"] = value.M["username"].S;
                comment["date"] = value.M["date"].S;
                comment["content"] = value.M["content"].S;
                comments.Add(comment);
            }
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
               attributes["categoryId"] = new AttributeValue { S = Post["categoryId"].ToString() };
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
               attributes["moreComments"] = new AttributeValue { BOOL = false };
               //attributes["comments"] = new AttributeValue { L = new List<AttributeValue>() };

               var CUSTOMEPOCH = 1300000000000;
               var ts = (decimal)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds - CUSTOMEPOCH;
               Random rnd = new Random();
               var randid = Math.Floor((decimal)(rnd.Next() * 512));
               ts = (ts * 64);
               string id = ((ts * 512) + (randid % 512)).ToString("0");
               attributes["id"] = new AttributeValue { S = id };

               PutItemRequest request = new PutItemRequest
               {
                   TableName = "posts",
                   Item = attributes
               };
               await client.PutItemAsync(request);
               response = id;
            }
            return response;
        }
    }
}
