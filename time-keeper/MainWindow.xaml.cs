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
        private static TextBox payeeName;
        private static TextBox payerName;
        private static bool runbool;
        private static System.Windows.Controls.Image image;
        private static bool getTextLen;
        BaseFont bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        Font font;

        public string SelectedCurrency { get { return currencyCmb.SelectedValue.ToString(); } }

        public MainWindow()
        {
            font = new Font(bf1, 10, Font.NORMAL);
            InitializeComponent();
            ctime = this.currentTimeBox;
            cwage = this.hourlyWageBox;
            camount = this.amountBox;
            image = this.Image;
            payeeName = this.payeeNameBox;
            payerName = this.payerNameBox;
            ctime.IsReadOnly = true;
            camount.IsReadOnly = true;

            payeeName.Text = Environment.UserName;

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

        private PdfPTable CreateTable()
        {
            var table = new PdfPTable(3);
            table.AddCell(CreateCellPhrase("Amount of Time"));
            table.AddCell(CreateCellPhrase("Pay Rate"));
            table.AddCell(CreateCellPhrase("Total Amount"));
            table.AddCell(CreateCellPhrase(ctime.Text));
            table.AddCell(CreateCellPhrase($"{cwage.Text} {SelectedCurrency} / h"));
            table.AddCell(CreateCellPhrase($"{camount.Text} {SelectedCurrency}"));

            return table;
        }

        private PdfPCell CreateCellPhrase(string text)
        {
            var cell = new PdfPCell();
            cell.HorizontalAlignment = 1;
            cell.Phrase = new Phrase(text, font);
            return cell;

        }



        public void PrintBtn_OnClick(object sender, RoutedEventArgs e)
        {

            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var output = new FileStream("Receipt.pdf", FileMode.Create);
            var writer = PdfWriter.GetInstance(document, output);
            document.Open();

            var welcomeParagraph = new Paragraph("TimeKeeper Receipt")
            {
                Font = new Font(bf1, 48,2),
                Alignment = 1,
                
            };


            document.Add(welcomeParagraph);
            document.Add(new Paragraph(Environment.NewLine));
            document.Add(new Paragraph(Environment.NewLine));
            Application.Current.Dispatcher.Invoke(() =>
            {
                document.Add(CreateTable());

                document.Add(new Paragraph(Environment.NewLine));
                document.Add(new Paragraph($"Payer : {payerName.Text}", font) { Alignment = 1 });
                document.Add(new Paragraph($"Payee : {payeeName.Text}", font) { Alignment = 1 });
                document.Add(new Paragraph(Environment.NewLine));
                document.Add(new Paragraph(Environment.NewLine));
                document.Add(new Paragraph(Environment.NewLine));
                document.Add(new Paragraph(Environment.NewLine));
                document.Add(new Paragraph($"Printed on {DateTime.Now}", font) { Alignment = 1 });
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