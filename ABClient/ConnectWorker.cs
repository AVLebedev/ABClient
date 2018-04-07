using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABClient
{


    /// <summary>
    /// Предоставляет методы для сетевого взаимодействия клиента с сервером
    /// </summary>
    public class ConnectWorker : IDisposable
    {
        // Ожидание нового пользователя
        //private string UserName = "Неизвестный";
        private StreamWriter swSender;
        private StreamReader srReceiver;
        private TcpClient tcpServer;
        // Нужно обновить форму с сообщениями из другого потока
        private delegate void UpdateLogCallback(string strMessage);
        // Необходимо задать форму на "отключенное" состояние из другого потока
        private delegate void CloseConnectionCallback(string strReason);
        private Thread thrMessaging;
        public bool Connected;

        public string message { get; set; }
        public string serverResponse { get; set; }

        private string clientId;

        /// <summary>
        /// Происходит при выходе из приложения-клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (Connected == true)
            {
                // Закрытие потоков, событий
                Connected = false;
                swSender.Close();
                srReceiver.Close();
                tcpServer.Close();
            }
        }

        MainWindow mainWindow;
        public void InitializeConnection(string id, MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeConnection(id);
        }

        public void InitializeConnection(string id)
        {
            clientId = id;
            try
            {
                // Получение локального IPv4
                string strHostName = System.Net.Dns.GetHostName(); ;
                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                IPAddress[] addr = ipEntry.AddressList;
                int i = 0;
                IPAddress ipAddr = addr[i];
                while (addr[i].AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    i++;
                    ipAddr = addr[i];
                }
                int port = 1234;
                // Конвертируем адрес
                //ipAddr = IPAddress.Parse(ip);
                // Старт соеднинения
                tcpServer = new TcpClient();
                tcpServer.Connect(ipAddr, port);

                // Отслеживание поключения
                this.Connected = true;
                this.serverResponse = MessageConsts.ConnectionSuccess;

                // Отправка своего id на сервер
                this.message = id + MessageConsts.ConnectMessage;
                swSender = new StreamWriter(tcpServer.GetStream());
                swSender.WriteLine(message);
                swSender.Flush();

                // Старт нити для общения
                thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
                thrMessaging.Start();
            }
            catch (Exception e)
            {
                ConnectionFailHandler();
            }
        }

        /// <summary>
        /// Закрытие текущего соденинения с отправкой сообщения на сервер
        /// </summary>
        /// <param name="Reason">текущий номер клиента</param> 
        public void CloseConnection(string id)
        {
            try
            {
                SendMessage(id + MessageConsts.DisconnectMessage);
            }
            catch
            {
                serverResponse = MessageConsts.DisconnectionFail;
            }
            finally
            {
                CloseConnection();
            }
        }

       /// <summary>
       /// Закрывает текущее соединения без отправки сообщения на сервер
       /// </summary>
        public void CloseConnection()
        {
            if (this.Connected == true)
            {
                this.Connected = false;
                swSender.Close();
                srReceiver.Close();
                tcpServer.Close();
            }
        }

       /// <summary>
       /// Отправляет запрос на сервер
       /// </summary>
       /// <param name="msg">текст запроса</param>
        public void SendMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg) == false)
            {
                swSender.WriteLine(msg);
                swSender.Flush();
            }
        }

       /// <summary>
       /// Ожидает ответа от сервера в синхронном режиме, возвращает ответ при получении
       /// </summary>
       /// <returns></returns>
        public string ListenServer()
        {            
            // Получение ответа от сервера
            srReceiver = new StreamReader(tcpServer.GetStream());
            string response = null;
            while(response == null) 
                response= srReceiver.ReadLine();
            return response;
        }

        private void ConnectionFailHandler()
        {
            this.Connected = false;
            this.serverResponse = MessageConsts.ConnectionFail;
            mainWindow.Dispatcher.Invoke(new Action(() => { mainWindow.NoConnection(); }));
        }

        private void ReceiveMessages()
        {
            try
            {
                // Получение ответа от сервера
                srReceiver = new StreamReader(tcpServer.GetStream());
                // Если символ ответа 1, то подключились
                string ConResponse = srReceiver.ReadLine();
                if (ConResponse[0] == '1')
                {
                    // Обновление состояния формы
                    serverResponse = MessageConsts.ConnectionSuccess;
                }
                else if (ConResponse[0] == '0')
                {
                    serverResponse = MessageConsts.UnactiveClient;
                    CloseConnection();
                    return;
                }
                else
                {
                    string Reason = MessageConsts.NoConnection;
                    Reason += ConResponse.Substring(2, ConResponse.Length - 2);
                    serverResponse = Reason;
                    return;
                }
                while (Connected)
                {
                    //try
                    //{
                        serverResponse = srReceiver.ReadLine();
                    //}
                    //catch (IOException e)
                    //{
                    //    return;
                    //}
                }
            }
            catch (IOException e)
            {
                ConnectionFailHandler();
                
            }
        }

        public void Dispose()
        {
            this.CloseConnection(clientId);
        }
    }
}
