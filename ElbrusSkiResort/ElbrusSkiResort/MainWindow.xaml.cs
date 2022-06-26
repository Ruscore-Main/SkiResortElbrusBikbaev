using EasyCaptcha.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ElbrusSkiResort
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isVisibleMask = true;
        private int inc = 0;
        private DispatcherTimer timer_login;
        public static Models.EmployeeSet currentUser = null;
        public Models.PrielbrusyeDBEntities _db = new Models.PrielbrusyeDBEntities();
        public int countAttemp = 0;
        public bool isVisibleCaptcha = false;
        string answer;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void timerTicker1(object sender, EventArgs e)
        {
            inc++;
            if (inc == 10)
            {
                timer_login.Stop();
                BtnCheckCaptcha.IsEnabled = true;
                BtnUpdateCaptcha.IsEnabled = true;
                TxtCaptcha.IsEnabled = true;
            }
        }

        /// Таймер на 3 минуты
        private void timerTicker2(object sender, EventArgs e)
        {
            inc++;
            if (inc == 180)
            {
                timer_login.Stop();
                TxtLogin.IsEnabled = true;
                PswMask.IsEnabled = true;
                TxtPass.IsEnabled = true;
                BtnLogin.IsEnabled = true;
            }
        }

        private void BtnShowPsw_Click(object sender, RoutedEventArgs e)
        {
            isVisibleMask = !isVisibleMask;
            if (!isVisibleMask)
            {
                TxtPass.Visibility = Visibility.Visible;
                TxtPass.Text = PswMask.Password;
                PswMask.Visibility = Visibility.Hidden;
            }
            else
            {
                PswMask.Visibility = Visibility.Visible;
                PswMask.Password = TxtPass.Text;
                TxtPass.Visibility = Visibility.Hidden;
            }
        }

        public Models.LoginHistorySet CreateHistory(Models.EmployeeSet user, string status)
        {
            Models.LoginHistorySet login_history = new Models.LoginHistorySet();
            login_history.EmployeeSet = user;

            string month = DateTime.Now.Month.ToString();
            if (month.Length == 1)
                month = "0" + month;

            string day = DateTime.Now.Day.ToString();
            if (day.Length == 1)
                day = "0" + day;

            string date_login = DateTime.Now.Year.ToString() + "-" + month + "-" + day;
            login_history.Date = date_login;
            login_history.Time = DateTime.Now.ToShortTimeString();
            login_history.Status = status;

            return login_history;
        }

        // Обновление капчи
        public void UpdateCaptcha()
        {
            MyCaptcha.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 3);
            answer = MyCaptcha.CaptchaText;
        }

        // ПоказКапчи
        public void ShowCaptcha()
        {
            UpdateCaptcha();
            CaptchaBlock.Visibility = Visibility.Visible;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TxtLogin.Text))
                errors.AppendLine("Вы не ввели логин");

            if (string.IsNullOrWhiteSpace(PswMask.Password) && string.IsNullOrWhiteSpace(TxtPass.Text))
                errors.AppendLine("Вы не ввели пароль");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!isVisibleMask)
            {
                PswMask.Visibility = Visibility.Visible;
                PswMask.Password = TxtPass.Text;
                TxtPass.Visibility = Visibility.Hidden;
            }

            string login = TxtLogin.Text;
            string pass = PswMask.Password;
            var user = _db.EmployeeSet.Where(el => el.Login == login && el.Password == pass).FirstOrDefault();
            var userByLogin = _db.EmployeeSet.Where(x => x.Login == login).FirstOrDefault();

            if (user == null)
            {
                if (userByLogin != null)
                {
                    var login_history = CreateHistory(userByLogin, "Не успешно");
                    _db.LoginHistorySet.Add(login_history);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                countAttemp++;
                if (countAttemp >= 2)
                {
                    TxtLogin.IsEnabled = false;
                    TxtPass.IsEnabled = false;
                    PswMask.IsEnabled = false;
                    BtnLogin.IsEnabled = false;
                    isVisibleCaptcha = true;
                    ShowCaptcha();
                }
            }
            else
            {
                var login_history = CreateHistory(user, "Не успешно");
                _db.LoginHistorySet.Add(login_history);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                currentUser = user;
                Windows.EmployeeWindow employeeWindow = new Windows.EmployeeWindow(_db, currentUser, this);
                employeeWindow.Show();
                this.Hide();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCaptcha.Text == answer)
            {
                isVisibleCaptcha = false;
                TxtLogin.IsEnabled = true;
                TxtPass.IsEnabled = true;
                PswMask.IsEnabled = true;
                BtnLogin.IsEnabled = true;
                CaptchaBlock.Visibility = Visibility.Hidden;
            }
            // Блокировка из-за неправильного ввода капчи
            else
            {
                inc = 0;
                TxtCaptcha.IsEnabled = false;
                BtnUpdateCaptcha.IsEnabled = false;
                BtnCheckCaptcha.IsEnabled = false;
                timer_login = new DispatcherTimer();
                timer_login.Interval = TimeSpan.FromSeconds(1);
                timer_login.Tick += timerTicker1;
                timer_login.Start();
            }
        }

        // Кнопка обновления капчи
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateCaptcha();
        }
    }
}
