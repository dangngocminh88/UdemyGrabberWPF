using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public async Task RunAsync(List<string> udemyLinkList)
        {
            if (udemyLinkList != null)
            {
                foreach (string udemyLink in udemyLinkList)
                {
                    await WriteInfo(udemyLink);
                    string courseId = GetCourseId(udemyLink);
                    if (string.IsNullOrEmpty(courseId))
                    {
                        await WriteInfo("Can not get course id");
                        continue;
                    }
                    bool purchased = await CheckPurchasedAsync(courseId);
                    if (!purchased)
                    {
                        await EnrollAsync(courseId, udemyLink);
                    }
                }
            }
        }
        private string GetCourseId(string udemyLink)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(udemyLink);
            HtmlNode body = doc.DocumentNode.SelectSingleNode("//body");
            string courseId = body?.Attributes["data-clp-course-id"]?.Value;
            return courseId;
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
                    await WriteInfo(purchasedDate);
                    return true;
                }
            }
            else
            {
                await WriteInfo(response.ReasonPhrase);
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
        private async Task EnrollAsync(string courseId, string udemyLink)
        {
            string url = "https://www.udemy.com/payment/checkout-submit/";
            Uri myUri = new Uri(udemyLink);
            string couponCode = HttpUtility.ParseQueryString(myUri.Query).Get("couponCode");
            if (string.IsNullOrEmpty(couponCode))
            {
                await WriteInfo("Can not find coupon");
                return;
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
                    await WriteInfo($"Purchase success {purchasedInfo}");
                }
                else
                {
                    await WriteInfo("Coupon expired");
                }
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase);
            }
        }
        public async Task WriteInfo(string content)
        {
            mainWindow.Info.Text += content + "\n";
            await Task.Delay(3);
        }
    }
}
