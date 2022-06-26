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
    /// Логика взаимодействия для AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        public Models.ClientSet new_client = new Models.ClientSet();
        public int i = 0;
        Models.PrielbrusyeDBEntities _db = new Models.PrielbrusyeDBEntities();
        public AddClientWindow()
        {
            InitializeComponent();
            DataContext = new_client;
        }

        /// Добавление нового клиента в БД
        private void btn_add_client_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            bool checkPassport = true;


            
            if (string.IsNullOrWhiteSpace(TxtFIO.Text))
                errors.AppendLine("Вы не ввели ФИО");
            if (string.IsNullOrWhiteSpace(dpicker_birthday.Text.ToString()))
                errors.AppendLine("Вы не выбрали дату рождения");
            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
                errors.AppendLine("Вы не ввели эл. почту");
            if (string.IsNullOrWhiteSpace(TxtPassword.Text))
                errors.AppendLine("Вы не ввели пароль");
            if (string.IsNullOrWhiteSpace(TxtPassport.Text))
                errors.AppendLine("Вы не ввели паспортные данные");

            string textPassport = TxtPassport.Text;

            if (!(string.IsNullOrWhiteSpace(textPassport)))
            {
                if (TxtPassport.Text.Length == 11)
                {
                    for (i = 0; i < 4; i++)
                    {
                        if (!(Char.IsDigit(textPassport[i])))
                            checkPassport = false;
                    }
                    if (textPassport[4] != ' ')
                        checkPassport = false;
                    for (i = 5; i < 11; i++)
                    {
                        if (!(Char.IsDigit(textPassport[i])))
                            checkPassport = false;
                    }
                    if (!checkPassport)
                        errors.AppendLine("Вы ввели паспортные данные некорректно");
                }
                else
                    errors.AppendLine("Вы ввели паспортные данные некорректно");
            }

            string passportData = "";
            foreach (char chr in TxtPassport.Text)
            {
                if (chr != ' ') passportData += chr;
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string[] words = TxtFIO.Text.Split(' ');
            Models.ClientSet new_client = new Models.ClientSet()
            {
                Surname = words[0],
                Name = words[1],
                Patronymic = words[2],
                PassportData = passportData,
                DateOfBirthday = Convert.ToDateTime(dpicker_birthday.Text),
                Email = TxtEmail.Text,
                Address = TxtAddress.Text,
                Password = TxtPassword.Text
            };


            _db.ClientSet.Add(new_client);

            try
            {
                
                _db.SaveChanges();
                MessageBox.Show("Клиент добавлен", "Успешно", MessageBoxButton.OK, MessageBoxImage.None);
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

