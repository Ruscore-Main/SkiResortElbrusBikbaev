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
    /// Логика взаимодействия для CreateOrderWindow.xaml
    /// </summary>
    public partial class CreateOrderWindow : Window
    {
        public int i = 0;
        public Models.OrderSet lastOrder = null;
        public Random rnd = new Random();
        private string numBaracode = "";
        public int counter = 0;
        private string date_start = "", time_start = "";
        private string order_code = "";
        private int time_rental = 0;
        Models.PrielbrusyeDBEntities _db;
        EmployeeWindow employeeWindow;
        public CreateOrderWindow(Models.PrielbrusyeDBEntities db, EmployeeWindow window)
        {
            InitializeComponent();
            this._db = db;
            this.employeeWindow = window;
            timer_display.Text = Timer.allTime.Hours.ToString() + ":" + Timer.allTime.Minutes.ToString() + ":" + Timer.allTime.Seconds.ToString();
            TimerMove();
            getLastOrder();

        }


        /// Вывод код последнего заказа из БД в текстовое поле
        public void getLastOrder()
        {
            foreach (Models.OrderSet order in _db.OrderSet)
            {
                lastOrder = order;
            }
            string orderCode = lastOrder.OrderCode;
            tbox_baraCode.Text = orderCode;
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

        /// Событие нажатия на кнопку, после чего сгенерируется штрих-код
        private void tbox_baraCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                numBaracode = "";
                StringBuilder errors = new StringBuilder();
                if (tbox_timeRental.Text.Length == 0)
                    errors.AppendLine("Вы не ввели время проката");
                if (tbox_baraCode.Text.Length == 0)
                    errors.AppendLine("Вы не ввели код заказа");
                if (checkString(tbox_baraCode.Text) == false)
                    errors.AppendLine("Код заказа должен состоять только из цифр");
                if (checkString(tbox_timeRental.Text) == false)
                    errors.AppendLine("Вы ввели время проката некорректно");
                if (tbox_baraCode.Text.Length != 7)
                    errors.AppendLine("Код заказа должен состоять из 7 символов");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                baracode_field.Children.Clear();
                getNumBaracode(tbox_baraCode.Text);
                generateBaracode(numBaracode);
            }
        }

        /// Состоит ли строка только из чисел?
        public bool checkString(string str)
        {
            int checkId = 0;
            for (i = 0; i < str.Length; i++)
            {
                if (str[i] != '1' && str[i] != '2' && str[i] != '3' && str[i] != '4' && str[i] != '5' &&
                    str[i] != '6' && str[i] != '7' && str[i] != '8' && str[i] != '9' && str[i] != '0')
                {
                    checkId = 1;
                    break;
                }
            }
            if (checkId == 0)
                return true;
            else
                return false;
        }

        /// Формирование номера штрих-кода
        public void getNumBaracode(string orderCode)
        {
            string date = "";
            string time = "";
            string month = "", day = "";

            if (DateTime.Now.Month.ToString().Length == 1)
                month = "0" + DateTime.Now.Month.ToString();
            else
                month = DateTime.Now.Month.ToString();
            if (DateTime.Now.Day.ToString().Length == 1)
                day = "0" + DateTime.Now.Day.ToString();
            else
                day = DateTime.Now.Day.ToString();

            date_start = DateTime.Now.Year.ToString() + "-" + month + "-" + day;
            time_start = DateTime.Now.ToShortTimeString();
            for (i = 0; i < date_start.Length; i++)
            {
                if (date_start[i] != '-')
                    date += date_start[i];
            }
            for (i = 0; i < time_start.Length; i++)
            {
                if (time_start[i] != ':')
                    time += time_start[i];
            }

            string timeRental = tbox_timeRental.Text;
            time_rental = Convert.ToInt32(tbox_timeRental.Text);
            order_code = tbox_baraCode.Text;
            numBaracode += orderCode + date + time + timeRental + rnd.Next(100000, 999999).ToString();
        }

        /// Генерация штрих-кода
        public void generateBaracode(string code)
        {
            double marginLeft = 0;
            for (i = 0; i < code.Length; i++)
            {
                Rectangle streak = new Rectangle();
                streak.VerticalAlignment = VerticalAlignment.Top;
                streak.HorizontalAlignment = HorizontalAlignment.Left;
                if (i == 0 || i == 1 || i == code.Length / 2 + 1 || i == code.Length / 2 || i == code.Length - 1 || i == code.Length - 2)
                    streak.Height = 110.5 + 20.7;
                else
                    streak.Height = 110.5;

                if (code[i] != '0')
                    streak.Fill = Brushes.Black;
                else
                    streak.Fill = Brushes.White;

                switch (code[i])
                {
                    case '1': streak.Width = 1 * 1; break;
                    case '2': streak.Width = 1 * 2; break;
                    case '3': streak.Width = 1 * 3; break;
                    case '4': streak.Width = 1 * 4; break;
                    case '5': streak.Width = 1 * 5; break;
                    case '6': streak.Width = 1 * 6; break;
                    case '7': streak.Width = 1 * 7; break;
                    case '8': streak.Width = 1 * 8; break;
                    case '9': streak.Width = 1 * 9; break;
                    case '0': streak.Width = 4; break;
                }
                if (i == 0)
                {
                    marginLeft = 100;
                    streak.Margin = new Thickness(marginLeft, 30, 0, 0);
                }
                else
                {
                    marginLeft = 7;
                    streak.Margin = new Thickness(marginLeft, 30, 0, 0);
                }
                baracode_field.Children.Add(streak);
            }
            string first = code[0].ToString();
            string second = "";
            string third = "";
            int half = (code.Length + 3) / 2;
            for (i = 1; i < half; i++)
            {
                second += code[i];
            }
            for (i = half; i < code.Length; i++)
            {
                third += code[i];
            }

            TextBlock firstLetterBaracode = new TextBlock() { Text = first, FontFamily = new FontFamily("Comic Sans MS"), FontSize = 13, Margin = new Thickness(-270, 160, 0, 0) };
            TextBlock leftHalfBaracode = new TextBlock() { Text = second, FontFamily = new FontFamily("Comic Sans MS"), FontSize = 13, Margin = new Thickness(-236, 160, 0, 0) };
            TextBlock rightHalfBaracode = new TextBlock() { Text = third, FontFamily = new FontFamily("Comic Sans MS"), FontSize = 13, Margin = new Thickness(-110, 160, 0, 0) };
            baracode_field.Children.Add(firstLetterBaracode);
            baracode_field.Children.Add(leftHalfBaracode);
            baracode_field.Children.Add(rightHalfBaracode);
        }

        /// Печать штрих-кода
        private void btn_printBaracode_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(baracode_field, "Штрих-код заказа");
                selection_panel.Visibility = Visibility.Visible;
                btn_create_order.Visibility = Visibility.Visible;
            }
        }

        private void btn_add_client_Click(object sender, RoutedEventArgs e)
        {
            new AddClientWindow().ShowDialog();
        }

        /// Добавление нескольких услуг
        private void btn_add_service_Click(object sender, RoutedEventArgs e)
        {
            counter++;
            if (counter == 1)
                lbox_services.Visibility = Visibility.Visible;
            else
            {
                lbox_services.Visibility = Visibility.Hidden;
                counter = 0;
            }
        }

        /// Обновление списка клиентов в ComboBox
        private void btn_refresh_clients_list_Click(object sender, RoutedEventArgs e)
        {
            cbox_clients.ItemsSource = _db.ClientSet.OrderBy(x => x.ClientCode).ToList();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// Поиск клиента по фамилии
        private void tbox_clients_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var ClientSet = _db.ClientSet.OrderBy(x => x.ClientCode).ToList();
            ClientSet = ClientSet.Where(x => x.Surname.Contains(tbox_clients_search.Text.ToLower())).ToList();
            cbox_clients.ItemsSource = ClientSet.ToList();
        }
        
        /// Поиск услуги по ее названию=
        private void tbox_services_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var services = _db.ServiceSet.ToList();
            services = services.Where(x => x.Name.ToLower().Contains(tbox_services_search.Text.ToLower())).ToList();
            cbox_services.ItemsSource = services.ToList();
            lbox_services.ItemsSource = services.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbox_services.ItemsSource = _db.ServiceSet.ToList();
            lbox_services.ItemsSource = _db.ServiceSet.ToList();
            cbox_clients.ItemsSource = _db.ClientSet.OrderBy(x => x.ClientCode).ToList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            employeeWindow.Show();
        }

        /// Добавление новой услуги в БД и печать электронного чека=
        private void btn_create_order_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (cbox_clients.SelectedItem == null)
                errors.AppendLine("Вы не выбрали клиента");

            if (cbox_services.SelectedItem == null)
                errors.AppendLine("Вы не выбрали услугу");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<string> services = new List<string>();
            int client_ClientCode = Convert.ToInt32(cbox_clients.SelectedValue);
            services.Add(cbox_services.SelectedValue.ToString());
            int id_client = 0;
            string client_address = "";
            int total_price = 0;
            string services_list_N = "", services_list = "";
            var more_services = lbox_services.SelectedItems.Cast<Models.ServiceSet>().ToList();

            if (more_services.Count > 0)
            {
                foreach (var service in more_services)
                {
                    services.Add(service.Name);
                }
            }

            foreach (Models.ClientSet client in _db.ClientSet)
            {
                if (client.ClientCode == client_ClientCode)
                {
                    id_client = client.ClientCode;
                    client_address = client.Address;
                }
            }

            foreach (string name in services)
            {
                foreach (Models.ServiceSet service in _db.ServiceSet)
                {
                    if (name == service.Name)
                    {
                        services_list_N += service.Id + " ";
                    }
                }
            }

            for (i = 0; i < services_list_N.Length - 1; i++)
                services_list += services_list_N[i];

            Models.OrderSet new_order = new Models.OrderSet();
            new_order.OrderCode = order_code;
            new_order.DateCreation = date_start;
            new_order.TimeOrder = time_start;
            new_order.ClientCode = id_client;
            foreach (string strService in services_list.Split(' '))
            {
                Models.OrderService orderService = new Models.OrderService()
                {
                    Service_Id = Convert.ToInt32(strService),
                    Order_Id = new_order.Id
                };

                _db.OrderService.Add(orderService);
            }
            new_order.RentalTime = time_rental;
            new_order.Status = "Новая";
            _db.OrderSet.Add(new_order);
            try
            {
                _db.SaveChanges();
                MessageBox.Show("Заказ оформлен", "Успешно", MessageBoxButton.OK, MessageBoxImage.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            StackPanel electronic_receipt = new StackPanel();
            electronic_receipt.Orientation = Orientation.Vertical;
            TextBlock headline = new TextBlock() { Text = "Электронный чек", FontFamily = new FontFamily("Times New Roman"), FontSize = 19, Margin = new Thickness(80, 80, 0, 0) };
            headline.FontWeight = FontWeights.Bold;
            TextBlock tblock_date_order = new TextBlock() { Text = $"Дата заказа: {date_start} {time_start}", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 40, 0, 0) };
            TextBlock tblock_id_client = new TextBlock() { Text = $"Код клиента: {id_client}", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 10, 0, 0) };
            TextBlock tblock_order_code = new TextBlock() { Text = $"Код заказа: {order_code}", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 10, 0, 0) };
            TextBlock tblock_client_address = new TextBlock() { Text = $"Адрес проживания клиента: {client_address}", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 10, 0, 0) };
            TextBlock tblock_services = new TextBlock() { Text = "Перечень услуг:\n", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 10, 0, 0) };
            string[] services_array = services_list.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (i = 0; i < services_array.Length; i++)
            {
                foreach (Models.ServiceSet service in _db.ServiceSet)
                {
                    if (Convert.ToInt32(services_array[i]) == service.Id)
                    {
                        tblock_services.Text += service.Name + "\n";
                        total_price += Convert.ToInt32(service.Price);
                    }
                }
            }
            TextBlock tblock_time_rental = new TextBlock() { Text = $"Время проката: {time_rental}", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 10, 0, 0) };
            TextBlock tblock_total_price = new TextBlock() { Text = $"ИТОГО: {total_price * time_rental}Р", FontFamily = new FontFamily("Times New Roman"), FontSize = 15, Margin = new Thickness(80, 40, 0, 0) };
            tblock_total_price.FontWeight = FontWeights.Bold;
            electronic_receipt.Children.Add(headline);
            electronic_receipt.Children.Add(tblock_date_order);
            electronic_receipt.Children.Add(tblock_id_client);
            electronic_receipt.Children.Add(tblock_order_code);
            electronic_receipt.Children.Add(tblock_client_address);
            electronic_receipt.Children.Add(tblock_services);
            electronic_receipt.Children.Add(tblock_time_rental);
            electronic_receipt.Children.Add(tblock_total_price);
            PrintDialog print = new PrintDialog();
            if (print.ShowDialog() == true)
            {
                print.PrintVisual(electronic_receipt, "Электронный чек");
            }
        }
    }
}
