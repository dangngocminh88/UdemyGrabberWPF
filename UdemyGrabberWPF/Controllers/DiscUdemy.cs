using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UdemyGrabberWPF.Models;

namespace UdemyGrabberWPF.Controllers
{
    public class DiscUdemy
    {
        private readonly MainWindow mainWindow;
        public DiscUdemy(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public async Task<List<string>> CreateUdemyLinkList(int maxPage, CancellationTokenSource cancellationTokenSource)
        {
            if (maxPage == 0)
            {
                return null;
            }
            List<string> udemyLinkList = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;
            string url;
            for (int page = 1; page < maxPage; page++)
            {
                if (page == 1)
                {
                    url = "https://www.discudemy.com/all";
                }
                else
                {
                    url = $"https://www.discudemy.com/all/{page}/";
                }
                await mainWindow.WriteInfo($"Getting coupon from {url}", InfoType.Info);

                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                doc = web.Load(url);
                HtmlNodeCollection linkList = doc?.DocumentNode?.SelectNodes("//a[@class='card-header']");
                if (linkList != null)
                {
                    foreach (HtmlNode link in linkList)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        string link2 = link?.Attributes["href"]?.Value;
                        string subLink2 = link2.Substring(link2.LastIndexOf("/"));
                        string link3 = $"https://www.discudemy.com/go/{subLink2}";
                        doc = web.Load(link3);
                        string udemyLink = doc?.DocumentNode?.SelectSingleNode("//div[@class='ui segment']")?.ChildNodes["a"]?.Attributes["href"]?.Value;
                        if (!string.IsNullOrEmpty(udemyLink))
                        {
                            udemyLinkList.Add(udemyLink);
                        }
                    }
                }
            }
            return udemyLinkList;
        }
    }
}
