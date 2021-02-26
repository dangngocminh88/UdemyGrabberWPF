using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UdemyGrabberWPF.Models;

namespace UdemyGrabberWPF.Controllers
{
    public class CourseMania
    {
        private readonly MainWindow mainWindow;
        public CourseMania(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public async Task<List<string>> CreateUdemyLinkList(CancellationTokenSource cancellationTokenSource)
        {
            List<string> udemyLinkList = new List<string>();
            string url = "https://api.coursemania.xyz/api/get_courses"; ;
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<dynamic> courseList = JsonConvert.DeserializeObject<List<dynamic>>(json);
                foreach(dynamic course in courseList)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    string udemyLink = course.url;
                    if(!string.IsNullOrEmpty(udemyLink))
                    {
                        udemyLinkList.Add(udemyLink);
                    }
                }
            }
            else
            {
                await mainWindow.WriteInfo(response.ReasonPhrase, InfoType.Error);
            }
            return udemyLinkList;
        }
    }
}
