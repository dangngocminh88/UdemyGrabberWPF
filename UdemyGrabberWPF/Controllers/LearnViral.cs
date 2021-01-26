﻿using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UdemyGrabberWPF.Controllers
{
    public class LearnViral
    {
        private readonly MainWindow mainWindow;
        public LearnViral(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public async Task<List<string>> CreateUdemyLinkList(int maxPage)
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
                    url = "https://udemycoupon.learnviral.com/coupon-category/free100-discount/";
                }
                else
                {
                    url = $"https://udemycoupon.learnviral.com/coupon-category/free100-discount/page/{page}/";
                }
                await mainWindow.WriteInfo($"Getting coupon from {url}");
                doc = web.Load(url);
                HtmlNodeCollection linkList = doc?.DocumentNode?.SelectNodes("//a[@data-clipboard-text='Redeem Offer']");
                if (linkList != null)
                {
                    foreach (HtmlNode link in linkList)
                    {
                        string udemyLink = link?.Attributes["href"]?.Value;
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
