using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
        CancellationTokenSource cancellationTokenSource = null;
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
            if (!ValidateStart())
            {
                return;
            }
            ChangeState(true);
            Udemy udemy = new Udemy(Main);
            int numberEnrolled = 0;
            Progress.Value = 0;
            double progressStep = CountProgressStep();
            try
            {
                // Get Coupon of YoFreeSample
                try
                {
                    if (YoFreeSampleChk.IsChecked ?? false)
                    {
                        YoFreeSample yoFreeSample = new YoFreeSample(Main);
                        List<string> udemyLinkList = await yoFreeSample.CreateUdemyLinkList(cancellationTokenSource);
                        Progress.Value += 1;
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    await WriteInfo(ex.Message, InfoType.Error);
                }
                // Get Coupon of TutorialBar
                try
                {
                    if (TutorialBarChk.IsChecked ?? false)
                    {
                        TutorialBar tutorialBar = new TutorialBar(Main);
                        List<string> udemyLinkList = await tutorialBar.CreateUdemyLinkList(10, cancellationTokenSource);
                        Progress.Value += 1;
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    await WriteInfo(ex.Message, InfoType.Error);
                }
                // Get Coupon of LearnViral
                try
                {
                    if (LearnViral.IsChecked ?? false)
                    {
                        LearnViral learnViral = new LearnViral(Main);
                        List<string> udemyLinkList = await learnViral.CreateUdemyLinkList(10, cancellationTokenSource);
                        Progress.Value += 1;
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    await WriteInfo(ex.Message, InfoType.Error);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                Progress.Value = 100;
                MessageBox.Show($"{numberEnrolled} course enrolled");
                ChangeState(false);
            }
        }
        private bool ValidateStart()
        {
            if (string.IsNullOrEmpty(ClientId.Text))
            {
                MessageBox.Show("client_id is required");
                return false;
            }
            if (string.IsNullOrEmpty(AccessToken.Text))
            {
                MessageBox.Show("access_token is required");
                return false;
            }
            if (string.IsNullOrEmpty(CSRFToken.Text))
            {
                MessageBox.Show("X-CSRFToken is required");
                return false;
            }
            return true;
        }
        private double CountProgressStep()
        {
            int numberWebsite = 0;
            if (YoFreeSampleChk.IsChecked ?? false)
            {
                numberWebsite++;
            }
            if (TutorialBarChk.IsChecked ?? false)
            {
                numberWebsite++;
            }
            if (LearnViral.IsChecked ?? false)
            {
                numberWebsite++;
            }
            return 100 * 1.0 / numberWebsite;
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmResult = MessageBox.Show("Are you sure to stop?", "Confirm Stop!!", MessageBoxButton.YesNo);
            if (confirmResult == MessageBoxResult.Yes)
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                }
            }
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
            Scroll.ScrollToBottom();
            await Task.Delay(3);
        }
        private void ChangeState(bool inProgress)
        {
            ClientId.IsEnabled = !inProgress;
            AccessToken.IsEnabled = !inProgress;
            CSRFToken.IsEnabled = !inProgress;
            WebsiteGroup.IsEnabled = !inProgress;
            Start.IsEnabled = !inProgress;
            Stop.IsEnabled = inProgress;
            if (inProgress)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
            else
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
