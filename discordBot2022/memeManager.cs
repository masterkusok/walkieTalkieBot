using System.Threading.Tasks;
using Discord.WebSocket;
using System.Collections.Generic;
using System.IO;
using AngleSharp;
using System;
using Reddit;
using System.Net.Http;
using AngleSharp.Html.Parser;
using System.Net;

namespace discordBot2022
{
    class memeManager
    {
        List<string> AllPostUrls = new List<string>();
        List<string> AllMemeUrls = new List<string>();
        List<int> UsedMemesId = new List<int>();

        string applicationId = "your reddit application id here";
        string secret = "your reddit application secret here";
        string refreshToken = "your reddit application refresh token here";
        //this task gets urls of meme pictures from reddit.com, this procces may take a lot of time
        public async Task Initialize(int number)
        {
            int missedMemes = 0;
            Console.WriteLine("Initialization started");
            //creating a reddit client and recieving posts
            RedditClient client = new RedditClient(appId: applicationId, refreshToken: refreshToken, appSecret: secret);
            List<Reddit.Controllers.Post> posts = client.Subreddit("memes").Posts.GetHot(limit:number);
            for(int i = 0; i < posts.Count; i++)
            {
                //getting url from every post
                AllPostUrls.Add("https://reddit.com" + posts[i].Permalink);
            }
            //parsing every post to get the url of meme picture
            HttpClient httpClient = new HttpClient();
            HtmlParser parser = new HtmlParser();
            for (int i = 0; i < AllPostUrls.Count; i++)
            {
                var response = await httpClient.GetAsync(AllPostUrls[i]);
                if (response != null)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var document = await parser.ParseDocumentAsync(content);
                    try
                    {
                        if (document.QuerySelector(".media-element").GetAttribute("src") != null)
                        {
                            AllMemeUrls.Add(document.QuerySelector(".media-element").GetAttribute("src"));
                        }
                        else
                        {
                            //pages which don't have picture, or have video or gif instead i call missed
                            missedMemes++;
                        }
                    }
                    catch (Exception e)
                    {
                        missedMemes++;
                    }
                }
            }
            Console.WriteLine("Initialized, " + missedMemes + " memes missed");
        }

        public async Task sendMeme(ISocketMessageChannel channel)
        {
            int numberOfMeme = new Random().Next(0, AllMemeUrls.Count-1);

            if (UsedMemesId.Count == AllMemeUrls.Count)
            {
                UsedMemesId.Clear();
            }

            for(int i = 0; i < UsedMemesId.Count; i++)
            {
                if(numberOfMeme == UsedMemesId[i])
                {
                    numberOfMeme = new Random().Next(0, AllMemeUrls.Count - 1);
                    i -= i;
                }
            }

            try
            {
                //download random meme from list, send it, and then delete it
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri(AllMemeUrls[numberOfMeme]), "img.jpg");
                    client.DownloadFileCompleted += (s, e) =>
                    {
                        channel.SendFileAsync("img.jpg").Wait();
                        File.Delete("img.jpg");
                    };
                }
                UsedMemesId.Add(numberOfMeme);
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR WHILE SENDING MEME:" + "\n" + e);
                Console.WriteLine("\n Url of error meme - " + AllMemeUrls[numberOfMeme]);
                channel.SendMessageAsync("Ошибка при отправке мема");
            }
        }
    }
}
