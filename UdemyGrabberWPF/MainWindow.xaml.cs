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
                // Get Coupon of CourseMania
                if (CourseMania.IsChecked ?? false)
                {
                    try
                    {
                        WebsiteProcessingInfo.Content = "Getting coupon from coursemania.xyz";
                        CourseMania courseMania = new CourseMania(Main);
                        List<string> udemyLinkList = await courseMania.CreateUdemyLinkList(cancellationTokenSource);
                        Progress.Value += 1;
                        WebsiteProcessingInfo.Content = "Grabbing courses from coursemania.xyz";
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
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
                // Get Coupon of DiscUdemy
                if (DiscUdemy.IsChecked ?? false)
                {
                    try
                    {
                        WebsiteProcessingInfo.Content = "Getting coupon from discudemy.com";
                        DiscUdemy discUdemy = new DiscUdemy(Main);
                        List<string> udemyLinkList = await discUdemy.CreateUdemyLinkList(10, cancellationTokenSource);
                        Progress.Value += 1;
                        WebsiteProcessingInfo.Content = "Grabbing courses from discudemy.com";
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
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
                // Get Coupon of LearnViral
                if (LearnViral.IsChecked ?? false)
                {
                    try
                    {
                        WebsiteProcessingInfo.Content = "Getting coupon from learnviral.com";
                        LearnViral learnViral = new LearnViral(Main);
                        List<string> udemyLinkList = await learnViral.CreateUdemyLinkList(10, cancellationTokenSource);
                        Progress.Value += 1;
                        WebsiteProcessingInfo.Content = "Grabbing courses from learnviral.com";
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
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
                // Get Coupon of TutorialBar
                if (TutorialBarChk.IsChecked ?? false)
                {
                    try
                    {
                        WebsiteProcessingInfo.Content = "Getting coupon from tutorialbar.com";
                        TutorialBar tutorialBar = new TutorialBar(Main);
                        List<string> udemyLinkList = await tutorialBar.CreateUdemyLinkList(10, cancellationTokenSource);
                        Progress.Value += 1;
                        WebsiteProcessingInfo.Content = "Grabbing courses from tutorialbar.com";
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
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
                // Get Coupon of YoFreeSample
                if (YoFreeSampleChk.IsChecked ?? false)
                {
                    try
                    {
                        WebsiteProcessingInfo.Content = "Getting coupon from yofreesamples.com";
                        YoFreeSample yoFreeSample = new YoFreeSample(Main);
                        List<string> udemyLinkList = await yoFreeSample.CreateUdemyLinkList(cancellationTokenSource);
                        Progress.Value += 1;
                        WebsiteProcessingInfo.Content = "Grabbing courses from yofreesamples.com";
                        numberEnrolled += await udemy.RunAsync(udemyLinkList, cancellationTokenSource, Progress.Value, Progress.Value + progressStep - 1);
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
            if (chkMinimumRating.IsChecked ?? false)
            {
                if (!double.TryParse(txtMinimumRating.Text, out _))
                {
                    MessageBox.Show("Can not check minimum rating");
                    return false;
                }
            }
            return true;
        }
        private double CountProgressStep()
        {
            int numberWebsite = 0;
            if (CourseMania.IsChecked ?? false)
            {
                numberWebsite++;
            }
            if (DiscUdemy.IsChecked ?? false)
            {
                numberWebsite++;
            }
            if (LearnViral.IsChecked ?? false)
            {
                numberWebsite++;
            }
            if (TutorialBarChk.IsChecked ?? false)
            {
                numberWebsite++;
            }
            if (YoFreeSampleChk.IsChecked ?? false)
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

        private void TxtMinimumRating_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtMinimumRating.Text, out _))
            {
                txtMinimumRating.Text = "0";
            }
        }

        public async Task WriteInfo(string content, InfoType infoType)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run(content));
            para.Foreground = infoType switch
            {
                InfoType.Error => Brushes.Red,
                InfoType.Success => Brushes.Blue,
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
