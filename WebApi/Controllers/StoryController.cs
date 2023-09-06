using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using WebApi.Modals;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController 
    {
             
        [HttpGet("getLatestItems")]
        public List<Story> Get()
        {
            int[] result = GetIDList("https://hacker-news.firebaseio.com/v0/beststories.json");
            List<Story> mylist = new List<Story>();
            if (result.Length>0)
            {
                List<Story> stories = new List<Story>();
                for(int i=0;i<result.Length;i++)
                {
                    Story story = GetIDDetail("https://hacker-news.firebaseio.com/v0/item/" + result[i]+".json");
                    mylist.Add(story);
                }
            }
            mylist.OrderByDescending(c => c.score);
            return mylist;
        }

        public int[] GetIDList(string urlvalue)
        {
             int[] data=new int[] { };
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(urlvalue);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(urlvalue).Result;
                 data = JsonConvert.DeserializeObject<int[]>(response.Content.ReadAsStringAsync().Result);
               
            }
            catch
            {
                return null;
            }
            
            return data;
        }

        public Story GetIDDetail(string urlvalue)
        {
            Story story = new Story();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(urlvalue);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(urlvalue).Result;
                var value = JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result);
                story.title = (string?)value["title"];
                story.uri = (string?)value["url"];
                story.postedBy = (string?)value["by"];
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)value["time"]);
                story.time = dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");                 
                story.score= (int)value["score"];                
                story.commentCount = ((string?)value["type"]).Count(); ;

            }
            catch
            {
                return null;
            }

            return story;
        }
    }

  
}