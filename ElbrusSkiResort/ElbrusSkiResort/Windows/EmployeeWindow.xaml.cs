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
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        Models.PrielbrusyeDBEntities _db;
        Models.EmployeeSet currentUser;
        MainWindow mainWindow;
        public EmployeeWindow(Models.PrielbrusyeDBEntities db, Models.EmployeeSet user, MainWindow window)
        {
            InitializeComponent();
            this._db = db;
            this.currentUser = user;
            this.mainWindow = window;

            if (Timer.limiter)
            {
                Timer.TimerStart();
                TimerWork();
                Timer.limiter = false;
            }

            DataContext = currentUser;
            switch (currentUser.Post)
            {
                case "Продавец":
                    {
                        SellerActions.Visibility = Visibility.Visible;
                        break;
                    }
                case "Старший смены":
                    {
                        ShiftSupervisorActions.Visibility = Visibility.Visible;
                        break;
                    }
                case "Администратор":
                    {
                        AdminActions.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        /// Работа таймера
        public async void TimerWork()
        {
            while (Timer.allTime.TotalSeconds > 0)
            {
                await Task.Delay(1000);
                Timer.allTime -= new TimeSpan(0, 0, 1);
                timer_display.Text = Timer.allTime.Hours.ToString() + ":" + Timer.allTime.Minutes.ToString() + ":" + Timer.allTime.Seconds.ToString();
            }
        }

        /// Отображение таймера
        public async void TimerMove()
        {
            while (Timer.allTime.TotalSeconds > 0)
            {
                await Task.Delay(1000);
                timer_display.Text = Timer.allTime.Hours.ToString() + ":" + Timer.allTime.Minutes.ToString() + ":" + Timer.allTime.Seconds.ToString();
            }
        }

        // Сформировать заказ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateOrderWindow createOrderWindow = new CreateOrderWindow(_db, this);
            createOrderWindow.Show();
            this.Hide();
        }

        // Принять товар
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        // История авторизаций
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LoginHistoryWindow loginHistoryWindow = new LoginHistoryWindow(_db, this);
            loginHistoryWindow.Show();
            this.Hide();
        }

        // Выход в главное меню
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Timer.limiter = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.Show();
        }
    }
}
