using HtmlAgilityPack;
using System.Collections.Generic;

namespace UdemyGrabberWPF.Controllers
{
    public class YoFreeSample
    {
        public List<string> CreateUdemyLinkList()
        {
            List<string> udemyLinkList = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://yofreesamples.com/courses/free-discounted-udemy-courses-list/");
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
