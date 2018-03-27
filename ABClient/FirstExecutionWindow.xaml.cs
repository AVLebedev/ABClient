using System;
using System.Collections.Generic;
using System.IO;
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

namespace ABClient
{
    /// <summary>
    /// Логика взаимодействия для FirstExecutionWindow.xaml
    /// </summary>
    public partial class FirstExecutionWindow : Window
    {
        public FirstExecutionWindow()
        {
            InitializeComponent();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string clientId = idTextBox.Text;
            string activationCode = codeTextBox.Text;

            if (string.IsNullOrWhiteSpace(clientId) || clientId == idTextBox.PlaceHolder)
                MessageBox.Show("Введите серийный номер!");
            else if(string.IsNullOrWhiteSpace(activationCode) || activationCode == codeTextBox.PlaceHolder)
                MessageBox.Show("Введите код активации!");
            else
            {
                using (var connection = new ConnectWorker() { message = clientId })
                {

                    //Подключаемся к серверу для активации
                    try
                    {
                        connection.InitializeConnection(clientId);
                        connection.SendMessage(clientId + MessageConsts.ActivateMessage + "|" + activationCode);
                        string serverResponse = connection.ListenServer();
                        if (serverResponse == "1")
                        {
                            using (StreamWriter writer = new StreamWriter("clientInfo.ini"))
                            {
                                writer.WriteLine("[Identification]");
                                writer.WriteLine("id=" + clientId);
                            }
                            MessageBox.Show(MessageConsts.ActivationSuccess);

                            connection.CloseConnection(clientId);
                            MainWindow main = new MainWindow(clientId);
                            main.Show();
                            this.Close();
                        }
                        else if (serverResponse == "0")
                        {
                            MessageBox.Show(MessageConsts.ActivationFail);
                        }

                    }
                    catch
                    {
                        MessageBox.Show(MessageConsts.ActivationFail);
                    }
                }
             }           

        }
    }
}
