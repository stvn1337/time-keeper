using iTextSharp.text;
using iTextSharp.text.pdf;
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
    public partial class MainWindow 
    {
        private static DateTime _timer1;
        private static DateTime _timer2;
        private static Timer _timer;
        private static TextBox _ctime;
        private static TextBox _cwage;
        private static TextBox _camount;
        private static bool _runbool;
        private static System.Windows.Controls.Image _image;
        private static bool _getTextLen;

        public string SelectedCurrency { get { return currencyCmb.SelectedValue.ToString(); } }

        public MainWindow()
        {
            InitializeComponent();
            _ctime = currentTimeBox;
            _cwage = hourlyWageBox;
            _camount = amountBox;
            _image = Image;
            _ctime.IsReadOnly = true;
            _camount.IsReadOnly = true;

            foreach (var currency in Enum.GetValues(typeof(Currency)))
                currencyCmb.Items.Add(currency);
            currencyCmb.SelectedIndex = 0;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) //On start stop clicked...
        {
            Application.Current.Dispatcher.Invoke(() => _getTextLen = _cwage.Text.Length > 0);

            if (_getTextLen)
            {
                _runbool = !_runbool;
                if (_runbool)
                {
                    if (_timer1 == DateTime.MinValue)
                        _timer1 = DateTime.Now;
                    _timer2 = DateTime.Now;
                    _timer = new Timer(1000);
                    _timer.Elapsed += OnTimedEvent;
                    _timer.Enabled = true;
                    _image.Source =
                        new BitmapImage(new Uri("pack://application:,,,/time-keeper;component/resources/Button-Turn-On-icon.png"));
                }
                else
                {
                    _timer.Enabled = false;
                    _image.Source =
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
            _timer2 = DateTime.Now;
            double wageHolder = 0;
            Application.Current.Dispatcher.Invoke(() => double.TryParse(_cwage.Text, out wageHolder));
            if (Math.Abs(wageHolder) > 0)
            {
                double amountHrs = (_timer2 - _timer1).TotalHours;
                string holder = amountHrs.ToString("0.000000");
                string holder2 = (wageHolder * amountHrs).ToString("0.00");
                Console.WriteLine(holder);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _ctime.Text = holder;
                    _camount.Text = holder2;
                });
            }
        }

        public void PrintBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var output = new FileStream("Receipt.pdf", FileMode.Create);
            PdfWriter.GetInstance(document, output);
            document.Open();
            var bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            var welcomeParagraph = new Paragraph("TimeKeeper Receipt")
            {
                Font = new Font(bf1, 24)
            };

            document.Add(welcomeParagraph);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Font font = new Font(bf1, 10, Font.NORMAL);
                document.Add(new Paragraph($"Amount of time = {_ctime.Text} hours", font));
                document.Add(new Paragraph($"Pay Rate  = {_cwage.Text} {SelectedCurrency} / hour", font));
                document.Add(new Paragraph($"Total Amount  {_camount.Text} {SelectedCurrency}", font));
            });
            document.Close();
            System.Diagnostics.Process.Start("Receipt.pdf");
        }

        private void ResetBtn_OnClickBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _timer1 = DateTime.Now;
            Application.Current.Dispatcher.Invoke(() =>
                {
                    _ctime.Text = 0.ToString();
                    _camount.Text = 0.ToString();
                });
        }

        /// <summary>
        /// When currency item has changed
        /// </summary>
        /// <param name="sender">Currency Cmb Selection Box</param>
        /// <param name="e">The current currency selection</param>
        private void CurrencyCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountCurrencyLbl.Content = SelectedCurrency;
        }
    }
}