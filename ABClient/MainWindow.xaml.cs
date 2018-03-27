using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConnectWorker connection;

        public MainWindow(string clientId)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(clientId) == false)
            {
                this.id = clientId;
                messageLabel.Visibility = System.Windows.Visibility.Hidden;
                timeLabel.Visibility = System.Windows.Visibility.Hidden;
                secondsLabel.Visibility = System.Windows.Visibility.Hidden;
                //portTextBox.Text = port;
                //addrTextBox.Text = serverIp;
                //logText.Content = MessageConsts.StartConnecting;

                connection = new ConnectWorker();
                connection.message = clientId.ToString() + MessageConsts.ConnectMessage;
            }
            else
            {
                MessageBox.Show("Не найден идентификационный номер, пожалуйста перезапустите приложение", "Ошибка");
                this.Close();
            }
        }

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        byte time;
        //const string serverIp = "192.168.1.1";
        const string port = "1234";

        string id { get; set; }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (connection.Connected)
            {
                AlarmCall();
            }
            else
            {
                logText.Content = MessageConsts.NoConnection;
                connectBtn.Focus();
            }
        }

        /// <summary>
        /// Запускает отправку сигнала тревоги
        /// </summary>
        void AlarmCall()
        {
            alarmButton.Content = "Отменить?";
            alarmButton.Click -= Button_Click_1;
            alarmButton.Click += CancelAlarm;
            ShowLabels();
            TimeCounting(timeLabel);
        }

        /// <summary>
        /// Отображает лейблы с текстом при подаче сигнала тревоги
        /// </summary>
        void ShowLabels()
        {
            messageLabel.Visibility = System.Windows.Visibility.Visible;
            timeLabel.Visibility = System.Windows.Visibility.Visible;
            secondsLabel.Visibility = System.Windows.Visibility.Visible;
        }

        void ResetElements()
        {
            timeLabel.Content = 10;
            timer.Tick -= timer_Tick;
            alarmButton.Content = "Подать сигнал";
            connectBtn.Focus();
            HideLabels();
        }

        /// <summary>
        /// Делает лейблы скрытыми
        /// </summary>
        void HideLabels()
        {
            messageLabel.Visibility = System.Windows.Visibility.Hidden;
            timeLabel.Visibility = System.Windows.Visibility.Hidden;
            secondsLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Производит обратный отсчёт в секундах и выводит результат на заданный лейбл.
        /// </summary>
      void TimeCounting(Label label)
        {           
            timer.Interval = 1000;
            timer.Tick += timer_Tick;            
          
            time= Convert.ToByte(label.Content);
            
               timer.Start();          
         }

      void timer_Tick(object sender, EventArgs e)
      {
          time -= 1;
          timeLabel.Content = time.ToString() ;
          if (time == 0)
          {
              timer.Stop();
              HideLabels();
              alarmButton.Content = "Тревога!";

              //Отправка сигнала на сервер
              connection.SendMessage(this.id.ToString() + MessageConsts.AlarmMessage);
          }
      }

      void CancelAlarm(object sender, EventArgs e)
      {
          timer.Stop();
          alarmButton.Click -= CancelAlarm;
          alarmButton.Click += Button_Click_1;
          ResetElements();
      }

      private void connectBtn_Click(object sender, RoutedEventArgs e)
      {
          //ConnectToServer();
          try
          {
              connection.InitializeConnection(this.id);
              logText.Content = connection.serverResponse;
              if (connection.Connected)
              {
                  connectBtn.Click -= connectBtn_Click;
                  connectBtn.Click += disconnect_Click;
                  connectBtn.Content = "Отключиться";
              }
          }
          catch
          {
              logText.Content = MessageConsts.ConnectionFail;
          }
      }

      private void disconnect_Click(object sender, RoutedEventArgs e)
      {
          try
          {
              //connection.message = this.id.ToString() + MessageConsts.DisconnectMessage;
              connection.CloseConnection(this.id);
              connectBtn.Click -= disconnect_Click;
              connectBtn.Click += connectBtn_Click;
              connectBtn.Content = "Подключиться";
              logText.Content = MessageConsts.DisconnectionSuccess;
              ResetElements();
          }
          catch
          {
              logText.Content = MessageConsts.DisconnectionFail;
          }
      }

      private void Window_MouseDown(object sender, MouseButtonEventArgs e)
      {
          
      }

#region Сетевые методы

/*
      BackgroundWorker backWorker1 = new BackgroundWorker();
      BackgroundWorker backWorker2 = new BackgroundWorker();
      

      private void ConnectToServer(object sender, RoutedEventArgs e)
      {
          ConnectToServer();
      }

      private void ConnectToServer()
      {
          try
          {
              TcpClient client = new TcpClient();
             // client.Connect(addrTextBox.Text, Convert.ToInt32(portTextBox.Text));
             // client.ReceiveTimeout = 50;
              IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(addrTextBox.Text), Convert.ToInt32(portTextBox.Text));
              client.Connect(IP_End);

              backWorker1.RunWorkerAsync();// Получение данных
              backWorker2.WorkerSupportsCancellation = true;

              var stw = new StreamWriter(client.GetStream());
              stw.WriteLine(this.id);              
          }
          catch (Exception e)
          {
              MessageBox.Show("Не удалось подключиться к серверу!");
              return;
          }
          connectBtn.Content = "Отключиться";
      }
        */
#endregion

#region Для отображения иконки в трее
      protected override void OnSourceInitialized(EventArgs e)
      {
          base.OnSourceInitialized(e); // базовый функционал приложения в момент запуска
          createTrayIcon(); // создание нашей иконки
      }

      private System.Windows.Forms.NotifyIcon TrayIcon = null;
      private ContextMenu TrayMenu = null;
      private const string ShowWindowText = "Развернуть";
      private const string HideWindowText = "Свернуть в трей";

      private bool createTrayIcon()
      {
          bool result = false;
          if (TrayIcon == null)
          { // только если мы не создали иконку ранее
              TrayIcon = new System.Windows.Forms.NotifyIcon(); // создаем новую
              TrayIcon.Icon = ABClient.Properties.Resources.icon; // изображение для трея
              // обратите внимание, за ресурсом с картинкой мы лезем в свойства проекта, а не окна,
              // поэтому нужно указать полный namespace
              TrayIcon.Text = "Here is tray icon text."; // текст подсказки, всплывающей над иконкой
              TrayMenu = Resources["TrayMenu"] as ContextMenu; // а здесь уже ресурсы окна и тот самый x:Key

              // сразу же опишем поведение при щелчке мыши, о котором мы говорили ранее
              // это будет просто анонимная функция, незачем выносить ее в класс окна
              TrayIcon.Click += delegate(object sender, EventArgs e)
              {
                  if ((e as System.Windows.Forms.MouseEventArgs).Button == System.Windows.Forms.MouseButtons.Left)
                  {
                      // по левой кнопке показываем или прячем окно
                      ShowHideMainWindow(sender, null);
                  }
                  else
                  {
                      // по правой кнопке (и всем остальным) показываем меню
                      TrayMenu.IsOpen = true;
                      Activate(); // нужно отдать окну фокус, см. ниже
                  }
              };
              result = true;
          }
          else
          { // все переменные были созданы ранее
              result = true;
          }
          TrayIcon.Visible = true; // делаем иконку видимой в трее
          return result;
      }

      private void ShowHideMainWindow(object sender, RoutedEventArgs e)
      {
          TrayMenu.IsOpen = false; // спрячем менюшку, если она вдруг видима
          if (IsVisible)
          {// если окно видно на экране
              // прячем его
              Hide();
              // меняем надпись на пункте меню
              (TrayMenu.Items[0] as MenuItem).Header = ShowWindowText;
          }
          else
          { // а если не видно
              // показываем
              Show();
              // меняем надпись на пункте меню
              (TrayMenu.Items[0] as MenuItem).Header = HideWindowText;
              WindowState = CurrentWindowState;
              Activate(); // обязательно нужно отдать фокус окну,
              // иначе пользователь сильно удивится, когда увидит окно
              // но не сможет в него ничего ввести с клавиатуры
          }
      }

      private WindowState fCurrentWindowState = WindowState.Normal;
      public WindowState CurrentWindowState
      {
          get { return fCurrentWindowState; }
          set { fCurrentWindowState = value; }
      }

      // переопределяем встроенную реакцию на изменение состояния сознания окна
      protected override void OnStateChanged(EventArgs e)
      {
          base.OnStateChanged(e); // системная обработка
          if (this.WindowState == System.Windows.WindowState.Minimized)
          {
              // если окно минимизировали, просто спрячем
              Hide();
              // и поменяем надпись на менюшке
              (TrayMenu.Items[0] as MenuItem).Header = ShowWindowText;
          }
          else
          {
              // в противном случае запомним текущее состояние
              CurrentWindowState = WindowState;
          }
      }

      private bool fCanClose = false;
      public bool CanClose
      { // флаг, позволяющий или запрещающий выход из приложения
          get { return fCanClose; }
          set { fCanClose = value; }
      }

      // переопределяем обработчик запроса выхода из приложения
      protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
  base.OnClosing(e); // встроенная обработка
  if(!CanClose) {    // если нельзя закрывать
    e.Cancel = true; //выставляем флаг отмены закрытия
    // запоминаем текущее состояние окна
    CurrentWindowState = this.WindowState;
    // меняем надпись в менюшке
    (TrayMenu.Items[0] as MenuItem).Header = ShowWindowText;
    // прячем окно
    Hide();
  }
  else { // все-таки закрываемся
    // убираем иконку из трея
    TrayIcon.Visible = false;
  }
}

      private void MenuExitClick(object sender, RoutedEventArgs e)
      {
          CanClose = true;
          Close();
      }
#endregion

      private void Window_Closing(object sender, CancelEventArgs e)
      {
          connection.Dispose();
      }
    }      
 }