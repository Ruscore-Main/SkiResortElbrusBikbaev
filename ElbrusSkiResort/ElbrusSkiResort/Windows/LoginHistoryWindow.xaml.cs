using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElbrusSkiResort.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginHistoryWindow.xaml
    /// </summary>
    public partial class LoginHistoryWindow : Window
    {
        Models.PrielbrusyeDBEntities _db;
        EmployeeWindow employeeWindow;
        public int i = 0, l = 0;
        public LoginHistoryWindow(Models.PrielbrusyeDBEntities db, EmployeeWindow window)
        {
            InitializeComponent();
            TimerMove();
            this._db = db;
            this.employeeWindow = window;
            var allLogin = _db.EmployeeSet.ToList();

            allLogin.Insert(0, new Models.EmployeeSet
            {
                Login = "Все логины"
            });

            cbox_filter_login.ItemsSource = allLogin;
            dgrid_login_history.ItemsSource = _db.LoginHistorySet.ToList();
        }


        /// <summary>
        /// Отображение таймера
        /// </summary>
        public async void TimerMove()
        {
            while (Timer.allTime.TotalSeconds > 0)
            {
                await Task.Delay(1000);
                timer_display.Text = Timer.allTime.Hours.ToString() + ":" + Timer.allTime.Minutes.ToString() + ":" + Timer.allTime.Seconds.ToString();
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Фильтрация данных таблиц по логину и сортировка по дате
        /// </summary>
        public void DataGridFilter()
        {
            string dateString = "";
            var sortByDates = new List<Models.LoginHistorySet>();
            var login_history = _db.LoginHistorySet.ToList();

            if (cbox_filter_login.SelectedIndex > 0)
                login_history = login_history.Where(p => p.EmployeeSet == cbox_filter_login.SelectedItem as Models.EmployeeSet).ToList();

            if (ccbox_sort_date.IsChecked.Value)
            {
                int size = login_history.Count();
                int[] massIntDate = new int[size];
                int ident = 0;

                foreach (var date in login_history)
                {
                    dateString = "";
                    string shortDateString = date.Date;
                    string[] masShortDateString = shortDateString.Split(new char[] { '-' });

                    for (i = 0; i < masShortDateString.Length; i++)
                        dateString += masShortDateString[i];

                    massIntDate[ident] = Convert.ToInt32(dateString);
                    ident++;
                }

                Array.Sort(massIntDate, (a, b) => b - a);

                for (i = 0; i < massIntDate.Length; i++)
                {
                    foreach (var date in login_history)
                    {
                        dateString = "";
                        string shortDateString = date.Date;
                        string[] masShortDateString = shortDateString.Split(new char[] { '-' });
                        for (l = 0; l < masShortDateString.Length; l++)
                            dateString += masShortDateString[l];

                        if (massIntDate[i] == Convert.ToInt32(dateString))
                        {
                            sortByDates.Add(new Models.LoginHistorySet() { Id = date.Id, Date = date.Date, Time = date.Time, Status = date.Status, EmployeeSet = date.EmployeeSet});
                            break;
                        }
                    }
                }
            }
            if (ccbox_sort_date.IsChecked == false)
                dgrid_login_history.ItemsSource = login_history.ToList();
            else
                dgrid_login_history.ItemsSource = sortByDates;
        }

        private void cbox_filter_login_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridFilter();
        }

        private void ccbox_sort_date_Checked(object sender, RoutedEventArgs e)
        {
            DataGridFilter();
        }

        // Закрытие окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            employeeWindow.Show();
        }

        private void ccbox_sort_date_Unchecked(object sender, RoutedEventArgs e)
        {
            DataGridFilter();
        }
    }
}
