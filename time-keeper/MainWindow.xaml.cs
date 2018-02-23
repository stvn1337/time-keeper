using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private static System.Timers.Timer timer;
        private static TextBox ctime;
        private static TextBox cwage;
        private static TextBox camount;
        private static bool runbool = false;

        
        public MainWindow()
        {
            InitializeComponent();
            ctime = this.currentTimeBox;
            cwage = this.hourlyWageBox;
            camount = this.amountBox;

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) //On start stop clicked...
        {
            runbool = !runbool;
            if (runbool)
            {
                timer1 = DateTime.Now;
                timer2 = DateTime.Now;
                timer = new System.Timers.Timer(1000);
                timer.Elapsed += OnTimedEvent;
                timer.Enabled = true;
            }
            else
            {
                timer.Enabled = false;
            }

            //string.Format("{0:#.00}", Convert.ToDecimal(((timer * Double.Parse(payPerHr.Text))) / 100));
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer2 = DateTime.Now;
            double wageHolder = 0;
            Application.Current.Dispatcher.Invoke(() => { double.TryParse(cwage.Text, out wageHolder); });
            if (wageHolder != 0)
            {
                double amountHrs = (timer2 - timer1).TotalHours;
                string holder = amountHrs.ToString("##.000000");
                string holder2 = (wageHolder * amountHrs).ToString("##.00");
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

        }

        private void ResetBtn_OnClickBtn_OnClick(object sender, RoutedEventArgs e)
        {
            timer1 = DateTime.Now;
        }
    }
}
