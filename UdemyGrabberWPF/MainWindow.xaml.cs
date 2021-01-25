using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using UdemyGrabberWPF.Controllers;

namespace UdemyGrabberWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadToken();
        }
        private void LoadToken()
        {
            string[] lines = File.ReadAllLines("Token.txt");
            if (lines.Length > 0)
            {
                ClientId.Text = lines[0];
            }
            if (lines.Length > 1)
            {
                AccessToken.Text = lines[1];
            }
            if (lines.Length > 2)
            {
                CSRFToken.Text = lines[2];
            }
        }
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ClientId.Text))
            {
                MessageBox.Show("client_id is required");
                return;
            }
            if (string.IsNullOrEmpty(AccessToken.Text))
            {
                MessageBox.Show("access_token is required");
                return;
            }
            if (string.IsNullOrEmpty(CSRFToken.Text))
            {
                MessageBox.Show("X-CSRFToken is required");
                return;
            }

            Udemy udemy = new Udemy(Main);

            if (YoFreeSampleChk.IsChecked ?? false)
            {
                YoFreeSample yoFreeSample = new YoFreeSample(Main);
                List<string> udemyLinkList = await yoFreeSample.CreateUdemyLinkList();
                await udemy.RunAsync(udemyLinkList);
            }

            if (TutorialBarChk.IsChecked ?? false)
            {
                TutorialBar tutorialBar = new TutorialBar(Main);
                List<string> udemyLinkList = await tutorialBar.CreateUdemyLinkList(10);
                await udemy.RunAsync(udemyLinkList);
            }

            /*if (LearnViral.IsChecked ?? false)
            {
                await udemy.WriteInfo("Getting coupon from https://udemycoupon.learnviral.com/");
                TutorialBar tutorialBar = new TutorialBar();
                List<string> udemyLinkList = tutorialBar.Run(10);
                await udemy.RunAsync(udemyLinkList);
            }*/

            MessageBox.Show("Complete");
        }
        public async Task WriteInfo(string content)
        {
            Info.Text += content + "\n";
            await Task.Delay(3);
        }
    }
}
