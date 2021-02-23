using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UdemyGrabberWPF.Models;

namespace UdemyGrabberWPF.Controllers
{
    public class Udemy
    {
        private readonly MainWindow mainWindow;
        public Udemy(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public async Task<int> RunAsync(List<string> udemyLinkList, CancellationTokenSource cancellationTokenSource, double startStep, double finishStep)
        {
            int numberEnrolled = 0;
            bool successEnroll;
            if (udemyLinkList != null)
            {
                double progressStep = (finishStep - startStep) / udemyLinkList.Count;
                foreach (string udemyLink in udemyLinkList)
                {
                    await mainWindow.WriteInfo(udemyLink, InfoType.Info);
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load(udemyLink);

                    string courseId = GetCourseId(doc);
                    if (string.IsNullOrEmpty(courseId))
                    {
                        await mainWindow.WriteInfo("Can not get course id", InfoType.Error);
                        mainWindow.Progress.Value += progressStep;
                        continue;
                    }

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    bool purchased = await CheckPurchasedAsync(courseId);
                    if (!purchased)
                    {
                        if (mainWindow.chkMinimumRating.IsChecked ?? false)
                        {
                            double rating = GetRating(doc);
                            if (rating < 4.0)
                            {
                                await mainWindow.WriteInfo($"course rating is {rating}", InfoType.Error);
                                mainWindow.Progress.Value += progressStep;
                                continue;
                            }
                        }
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        successEnroll = await EnrollAsync(courseId, udemyLink);
                        if (successEnroll)
                        {
                            numberEnrolled++;
                        }
                    }
                    mainWindow.Progress.Value += progressStep;
                }
            }
            mainWindow.Progress.Value = finishStep;
            return numberEnrolled;
        }
        private string GetCourseId(HtmlDocument doc)
        {
            HtmlNode body = doc.DocumentNode.SelectSingleNode("//body");
            string courseId = body?.Attributes["data-clp-course-id"]?.Value;
            return courseId;
        }
        private double GetRating(HtmlDocument doc)
        {
            HtmlNodeCollection rating = doc.DocumentNode.SelectNodes("//span[@data-purpose='rating-number']");
            if (rating.Count > 0)
            {
                try
                {
                    return double.Parse(rating[0].InnerText);
                }
                catch (InvalidCastException)
                {

                }
            }
            return 0;
        }
        private async Task<bool> CheckPurchasedAsync(string udemyId)
        {
            string url = "https://www.udemy.com/api-2.0/course-landing-components/" + udemyId + "/me/?components=purchase";
            using HttpClient client = new HttpClient();
            CreateHeaders(client);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                dynamic courseInfo = JsonConvert.DeserializeObject(json);
                string purchasedDate = courseInfo.purchase.data.purchase_date;
                if (string.IsNullOrEmpty(purchasedDate))
                {
                    return false;
                }
                else
                {
                    await mainWindow.WriteInfo(purchasedDate, InfoType.Info);
                    return true;
                }
            }
            else
            {
                await mainWindow.WriteInfo(response.ReasonPhrase, InfoType.Error);
                return true;
            }
        }
        private void CreateHeaders(HttpClient client)
        {
            string access_token = mainWindow.AccessToken.Text;
            Random r = new Random();
            int rInt = r.Next(0, 255);

            client.DefaultRequestHeaders.Add("authorization", "Bearer " + access_token);
            client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36 Edg/87.0.664.66");
            client.DefaultRequestHeaders.Add("x-forwarded-for", rInt.ToString());
            client.DefaultRequestHeaders.Add("x-udemy-authorization", "Bearer " + access_token);
            client.DefaultRequestHeaders.Add("origin", "https://www.udemy.com");
            client.DefaultRequestHeaders.Add("referer", "https://www.udemy.com/");
            client.DefaultRequestHeaders.Add("dnt", "1");
        }
        private void AddHeaderToCheckout(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("x-checkout-client-version", "1");
            client.DefaultRequestHeaders.Add("x-checkout-is-mobile-app", "false");
            client.DefaultRequestHeaders.Add("x-checkout-version", "2");
            client.DefaultRequestHeaders.Add("X-CSRFToken", mainWindow.CSRFToken.Text);
        }
        private async Task<bool> EnrollAsync(string courseId, string udemyLink)
        {
            const string url = "https://www.udemy.com/payment/checkout-submit/";
            Uri myUri = new Uri(udemyLink);
            string couponCode = HttpUtility.ParseQueryString(myUri.Query).Get("couponCode");
            if (string.IsNullOrEmpty(couponCode))
            {
                await mainWindow.WriteInfo("Can not find coupon", InfoType.Error);
                return false;
            }

            CheckoutSubmitRequest request = new CheckoutSubmitRequest();
            request.shopping_cart.items[0].buyableId = long.Parse(courseId);
            //request.shopping_cart.items[0].purchasePrice.currency = "USD";
            request.shopping_cart.items[0].discountInfo.code = couponCode;
            string json = JsonConvert.SerializeObject(request);

            Uri baseAddress = new Uri("https://www.udemy.com");
            CookieContainer cookieContainer = new CookieContainer();
            using HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            using HttpClient client = new HttpClient(handler);
            CreateHeaders(client);
            AddHeaderToCheckout(client);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            cookieContainer.Add(baseAddress, new Cookie("X-CSRFToken", mainWindow.CSRFToken.Text));
            cookieContainer.Add(baseAddress, new Cookie("client_id", mainWindow.ClientId.Text));
            cookieContainer.Add(baseAddress, new Cookie("access_token", mainWindow.AccessToken.Text));

            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic purchasedInfo = JsonConvert.DeserializeObject(jsonResponse);
                if (purchasedInfo.message == null)
                {
                    await mainWindow.WriteInfo($"Enroll successfully {purchasedInfo}", InfoType.Sucess);
                    return true;
                }
                else
                {
                    await mainWindow.WriteInfo("Coupon expired", InfoType.Error);
                    return false;
                }
            }
            else
            {
                await mainWindow.WriteInfo(response.ReasonPhrase, InfoType.Error);
                return false;
            }
        }
    }
}
