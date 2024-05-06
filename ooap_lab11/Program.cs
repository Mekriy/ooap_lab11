using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://uakino.club/";
        int pageCount = 4;

        Task<List<string>> getMovieTitlesTask = GetMovieTitlesAsync(baseUrl, pageCount);
        await getMovieTitlesTask.ContinueWith(task =>
        {
            List<string> movieTitles = task.Result;
            foreach (var title in movieTitles)
            {
                Console.WriteLine(title);
            }
        });
        await getMovieTitlesTask;
    }

    static async Task<List<string>> GetMovieTitlesAsync(string baseUrl, int pageCount)
    {
        List<string> movieTitles = new List<string>();

        using (HttpClient client = new HttpClient())
        {
            for (int i = 1; i <= pageCount; i++)
            {
                string url = $"{baseUrl}/page/{i}/";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync();
                    HtmlDocument htmlDoc = new HtmlDocument();
                    
                    htmlDoc.LoadHtml(htmlContent);
                    htmlDoc.OptionDefaultStreamEncoding = Encoding.UTF8;

                    var titleNodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='movie-title']");

                    foreach (var titleNode in titleNodes)
                    {
                        movieTitles.Add(titleNode.InnerText.Trim());
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve page {i}. Status code: {response.StatusCode}");
                }
            }
        }

        return movieTitles;
    }
}