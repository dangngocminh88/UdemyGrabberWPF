using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace UdemyGrabberWPF.Controllers
{
    public class TutorialBar
    {
        public List<string> Run(int maxPage)
        {
            if (maxPage == 0)
            {
                return null;
            }

            List<string> udemyLinkList = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;

            for (int page = 1; page < maxPage; page++)
            {
                if (page == 1)
                {
                    doc = web.Load("https://www.tutorialbar.com/all-courses/");
                }
                else
                {
                    doc = web.Load("https://www.tutorialbar.com/all-courses/");
                }

                HtmlNodeCollection linkList = doc?.DocumentNode?.SelectNodes("//h3[@class='mb15 mt0 font110 mobfont100 fontnormal lineheight20']");
                if (linkList != null)
                {
                    foreach (HtmlNode link in linkList)
                    {
                        string linkPage = link?.ChildNodes["a"]?.Attributes["href"]?.Value;
                        if (!string.IsNullOrEmpty(linkPage))
                        {

                            udemyLinkList.Add(getUdemyLink(linkPage));
                        }
                    }
                }
            }
            return udemyLinkList;
        }
        private string getUdemyLink(string linkPage)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(linkPage);
            HtmlNode link = doc?.DocumentNode?.SelectNodes("//a[@class='btn_offer_block re_track_btn']")[0];
            string udemyLink = link?.Attributes["href"]?.Value;
            return udemyLink;
        }
    }
}
