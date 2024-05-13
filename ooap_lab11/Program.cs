using HtmlAgilityPack;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://uakino.club/";
        int pageCount = 4;

        List<Task<List<string>>> tasks = new List<Task<List<string>>>();

        for (int i = 1; i <= pageCount; i++)
        {
            tasks.Add(GetMovieTitlesAsync(baseUrl, i));
        }

        await Task.WhenAll(tasks);

        foreach (var task in tasks)
        {
            List<string> movieTitles = await task;
            Console.WriteLine("-----------------NEW PAGE-----------------");
            foreach (var title in movieTitles)
            {
                Console.WriteLine(title);
            }
        }
    }

    private static async Task<List<string>> GetMovieTitlesAsync(string baseUrl, int page)
    {
        List<string> movieTitles = new List<string>();

        using (HttpClient client = new HttpClient())
        {
            string url = $"{baseUrl}/page/{page}/";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var titleNodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='movie-title']");

                if (titleNodes != null)
                {
                    foreach (var titleNode in titleNodes)
                    {
                        movieTitles.Add(titleNode.InnerText.Trim());
                    }
                }
                else
                {
                    Console.WriteLine($"No movie titles found on page {page}");
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve page {page}. Status code: {response.StatusCode}");
            }
        }

        return movieTitles;
    }
}