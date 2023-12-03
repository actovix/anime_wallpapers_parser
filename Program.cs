using System.Net;
using System.Text.RegularExpressions;

namespace Program
{
    public class AnimePars
    {
        public static Regex subPagesRegex = new Regex(@"https://wallhaven.cc/w/(\w)+");
        public static Regex picRegex = new Regex(@"https://w.wallhaven.cc/full/(\S)+\.\w+");
        public static Regex nameOfPicRegex = new Regex(@"\w+\.\w+$");
        public static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                throw new Exception("Args needed ===> [pageToParsCount] [pathToSave]");
            }
            int pageToParsCount = int.Parse(args[0]);
            string pathToSave = args[1];
            DirectoryInfo di = new DirectoryInfo(pathToSave);
            
            string siteUrl = @"https://wallhaven.cc/search?categories=010&purity=100&topRange=1M&sorting=toplist&order=desc&ai_art_filter=1&page=";

            int downloadedCount = 1;
            using(WebClient webClient = new WebClient())
            {
                for(int x = 0;x < pageToParsCount;x++)
                {
                    string pageHtml = webClient.DownloadString(siteUrl + GetMainPageNum().ToString());
                    foreach(string subPageUrl in GetSubPagesUrls(pageHtml))
                    {
                        string subPageHtml = webClient.DownloadString(subPageUrl);
                        string picUrl = GetPicUrl(subPageHtml);
                        System.Console.Write($"{downloadedCount} " + picUrl + " => ");
                        webClient.DownloadFile(picUrl, pathToSave + "/" + nameOfPicRegex.Match(picUrl).Value);
                        System.Console.WriteLine($"downloaded");
                        downloadedCount++;
                    }
                }
            }
            
        }

        public static int GetMainPageNum()
        {
            int pageNum = int.Parse(File.ReadAllText("/home/matthew/anime_pars.config"));
            File.WriteAllText("/home/matthew/anime_pars.config", (pageNum + 1).ToString());
            return pageNum++;
        }

        public static string GetPicUrl(string source)
        {
            return picRegex.Match(source).Value;
        }

        public static string[] GetSubPagesUrls(string htmlSource)
        {
            string[] urls;
            urls = subPagesRegex.Matches(htmlSource).Select(x => x.Value).ToArray();
            System.Console.WriteLine(urls.Length);
            return urls;
        }
    }
}