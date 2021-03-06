﻿using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UdemyGrabberWPF.ExtensionMethods;
using UdemyGrabberWPF.Models;

namespace UdemyGrabberWPF.Controllers
{
    public class LearnViral
    {
        private readonly MainWindow mainWindow;
        public LearnViral(MainWindow mainWindow)
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
                    url = "https://udemycoupon.learnviral.com/coupon-category/free100-discount/";
                }
                else
                {
                    url = $"https://udemycoupon.learnviral.com/coupon-category/free100-discount/page/{page}/";
                }
                await mainWindow.WriteInfo($"Getting coupon from {url}", InfoType.Info);

                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                doc = web.Load(url);
                HtmlNodeCollection linkList = doc?.DocumentNode?.SelectNodes("//a[@data-clipboard-text='Redeem Offer']");
                if (linkList != null)
                {
                    foreach (HtmlNode link in linkList)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        string udemyLink = link?.Attributes["href"]?.Value;
                        if (!string.IsNullOrEmpty(udemyLink) && udemyLink.ValidURL(checkedCourses))
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
