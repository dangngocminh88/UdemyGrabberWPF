using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;
using UdemyGrabberWPF.Models;

namespace UdemyGrabberWPF.Controllers
{
    public class YoFreeSample
    {
        private readonly MainWindow mainWindow;
        public YoFreeSample(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public async Task<List<string>> CreateUdemyLinkList()
        {
            const string url = "https://yofreesamples.com/courses/free-discounted-udemy-courses-list/";
            await mainWindow.WriteInfo($"Getting coupon from {url}", InfoType.Info);
            List<string> udemyLinkList = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection linkList = doc?.DocumentNode?.SelectNodes("//a[@class='btn btn-md btn-success']");
            if (linkList != null)
            {
                foreach (HtmlNode link in linkList)
                {
                    udemyLinkList.Add(link.Attributes["href"].Value);
                }
            }
            return udemyLinkList;
        }
    }
}
