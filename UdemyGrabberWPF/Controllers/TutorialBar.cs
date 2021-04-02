using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UdemyGrabberWPF.ExtensionMethods;
using UdemyGrabberWPF.Models;

namespace UdemyGrabberWPF.Controllers
{
    public class TutorialBar
    {
        private readonly MainWindow mainWindow;
        public TutorialBar(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public async Task<List<string>> CreateUdemyLinkList(int maxPage, CancellationTokenSource cancellationTokenSource, List<string> checkedCourses)
        {
            if (maxPage == 0)
            {
                return null;
            }
            List<string> udemyLinkList = new();
            HtmlWeb web = new();
            HtmlDocument doc;
            string url;
            for (int page = 1; page < maxPage; page++)
            {
                if (page == 1)
                {
                    url = "https://www.tutorialbar.com/all-courses/";
                }
                else
                {
                    url = $"https://www.tutorialbar.com/all-courses/page/{page}/";
                }
                await mainWindow.WriteInfo($"Getting coupon from {url}", InfoType.Info);
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                doc = web.Load(url);
                HtmlNodeCollection linkList = doc?.DocumentNode?.SelectNodes("//h3[@class='mb15 mt0 font110 mobfont100 fontnormal lineheight20']");
                if (linkList != null)
                {
                    foreach (HtmlNode link in linkList)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        string linkPage = link?.ChildNodes["a"]?.Attributes["href"]?.Value;
                        if (!string.IsNullOrEmpty(linkPage))
                        {
                            string udemyLink = GetUdemyLink(linkPage);
                            if (udemyLink.ValidURL(checkedCourses))
                            {
                                udemyLinkList.Add(udemyLink);
                            }
                        }
                    }
                }
            }
            return udemyLinkList;
        }
        private static string GetUdemyLink(string linkPage)
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(linkPage);
            HtmlNode link = doc?.DocumentNode?.SelectNodes("//a[@class='btn_offer_block re_track_btn']")[0];
            string udemyLink = link?.Attributes["href"]?.Value;
            return udemyLink;
        }
    }
}
