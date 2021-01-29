﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UdemyGrabberWPF.Controllers;
using UdemyGrabberWPF.Models;

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
            int numberEnrolled = 0;

            if (YoFreeSampleChk.IsChecked ?? false)
            {
                YoFreeSample yoFreeSample = new YoFreeSample(Main);
                List<string> udemyLinkList = await yoFreeSample.CreateUdemyLinkList();
                numberEnrolled += await udemy.RunAsync(udemyLinkList);
            }

            if (TutorialBarChk.IsChecked ?? false)
            {
                TutorialBar tutorialBar = new TutorialBar(Main);
                List<string> udemyLinkList = await tutorialBar.CreateUdemyLinkList(10);
                numberEnrolled += await udemy.RunAsync(udemyLinkList);
            }

            if (LearnViral.IsChecked ?? false)
            {
                LearnViral learnViral = new LearnViral(Main);
                List<string> udemyLinkList = await learnViral.CreateUdemyLinkList(10);
                numberEnrolled += await udemy.RunAsync(udemyLinkList);
            }

            MessageBox.Show($"{numberEnrolled} course enrolled");
        }
        public async Task WriteInfo(string content, InfoType infoType)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run(content));
            para.Foreground = infoType switch
            {
                InfoType.Error => Brushes.Red,
                InfoType.Sucess => Brushes.Blue,
                _ => Brushes.Black,
            };
            Info.Document.Blocks.Add(para);
            await Task.Delay(3);
        }
    }
}
