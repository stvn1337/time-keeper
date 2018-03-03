using iTextSharp.text;
using iTextSharp.text.pdf;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using time_keeper.Helpers;
using DateTime = System.DateTime;

namespace time_keeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static DateTime timer1;
        private static DateTime timer2;
        private static Timer timer;
        private static TextBox ctime;
        private static TextBox cwage;
        private static TextBox camount;
        private static bool runbool;
        private static System.Windows.Controls.Image image;
        private static bool getTextLen;

        public string SelectedCurrency { get { return currencyCmb.SelectedValue.ToString(); } }

        public MainWindow()
        {
            InitializeComponent();
            ctime = this.currentTimeBox;
            cwage = this.hourlyWageBox;
            camount = this.amountBox;
            image = this.Image;
            ctime.IsReadOnly = true;
            camount.IsReadOnly = true;

            foreach (var currency in Enum.GetValues(typeof(Currency)))
                currencyCmb.Items.Add(currency);
            currencyCmb.SelectedIndex = 0;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) //On start stop clicked...
        {
            Application.Current.Dispatcher.Invoke(() => getTextLen = cwage.Text.Length > 0);

            if (getTextLen)
            {
                runbool = !runbool;
                if (runbool)
                {
                    if (timer1 == DateTime.MinValue)
                        timer1 = DateTime.Now;
                    timer2 = DateTime.Now;
                    timer = new System.Timers.Timer(1000);
                    timer.Elapsed += OnTimedEvent;
                    timer.Enabled = true;
                    image.Source =
                        new BitmapImage(new Uri("pack://application:,,,/time-keeper;component/resources/Button-Turn-On-icon.png"));
                }
                else
                {
                    timer.Enabled = false;
                    image.Source =
                        new BitmapImage(new Uri("pack://application:,,,/time-keeper;component/resources/Button-Turn-Off-icon.png"));
                }
            }
            else
            {
                MessageBox.Show("Please enter a pay rate..");
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer2 = DateTime.Now;
            double wageHolder = 0;
            Application.Current.Dispatcher.Invoke(() => double.TryParse(cwage.Text, out wageHolder));
            if (wageHolder != 0)
            {
                double amountHrs = (timer2 - timer1).TotalHours;
                string holder = amountHrs.ToString("0.000000");
                string holder2 = (wageHolder * amountHrs).ToString("0.00");
                Console.WriteLine(holder);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ctime.Text = holder;
                    camount.Text = holder2;
                });
            }
        }

        public void PrintBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var output = new FileStream("Receipt.pdf", FileMode.Create);
            var writer = PdfWriter.GetInstance(document, output);
            document.Open();
            BaseFont bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            var welcomeParagraph = new iTextSharp.text.Paragraph("TimeKeeper Receipt")
            {
                Font = new iTextSharp.text.Font(bf1, 24)
            };

            document.Add(welcomeParagraph);
            Application.Current.Dispatcher.Invoke(() =>
            {
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf1, 10, iTextSharp.text.Font.NORMAL);
                document.Add(new iTextSharp.text.Paragraph($"Amount of time = {ctime.Text} hours", font));
                document.Add(new iTextSharp.text.Paragraph($"Pay Rate  = {cwage.Text} {SelectedCurrency} / hour", font));
                document.Add(new iTextSharp.text.Paragraph($"Total Amount  {camount.Text} {SelectedCurrency}", font));
            });
            document.Close();
            System.Diagnostics.Process.Start("Receipt.pdf");
        }

        private void ResetBtn_OnClickBtn_OnClick(object sender, RoutedEventArgs e)
        {
            timer1 = DateTime.Now;
            Application.Current.Dispatcher.Invoke(() =>
                {
                    ctime.Text = 0.ToString();
                    camount.Text = 0.ToString();
                });
        }

        /// <summary>
        /// When currency item has changed
        /// </summary>
        private void CurrencyCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountCurrencyLbl.Content = SelectedCurrency;
        }
    }
}