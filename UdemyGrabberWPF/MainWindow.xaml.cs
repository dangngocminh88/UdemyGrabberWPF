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
                await udemy.WriteInfo("Getting coupon from https://yofreesamples.com/");
                YoFreeSample yoFreeSample = new YoFreeSample();
                List<string> udemyLinkList = yoFreeSample.CreateUdemyLinkList();
                await udemy.RunAsync(udemyLinkList);
            }

            if (TutorialBarChk.IsChecked ?? false)
            {
                await udemy.WriteInfo("Getting coupon from https://www.tutorialbar.com/");
                TutorialBar tutorialBar = new TutorialBar();
                List<string> udemyLinkList = tutorialBar.Run(10);
                await udemy.RunAsync(udemyLinkList);
            }

            MessageBox.Show("Complete");
        }
    }
}
