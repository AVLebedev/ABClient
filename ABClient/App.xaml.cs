using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ABClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            ShowStartWindow();
        }

        /// <summary>
        /// Выбирает, какое окно показать при старте приложения
        /// </summary>
        private void ShowStartWindow()
        {
            if (IsFirstExecution())
            {
                FirstExecutionWindow activation = new FirstExecutionWindow();
                activation.Show();
            }
            else
            {
                string clientId = GetClientId();
                MainWindow main = new ABClient.MainWindow(clientId);
                main.Show();
            }
        }

        /// <summary>
        /// Извлекает id текущего клиента из файла clientInfo.ini
        /// </summary>
        /// <returns></returns>
        private string GetClientId()
        {
            string[] clientInfo = File.ReadLines("clientInfo.ini").ToArray();
            string clientId = null;
            for (int i = 0; i < clientInfo.Length; i++)
            {
                if (clientInfo[i] == "[Identification]") clientId = clientInfo[i + 1].Split(new char[] { '=' })[1];
            }
            return clientId;
        }

        /// <summary>
        /// Проверяет, что приложение запущено впервые
        /// </summary>
        /// <returns></returns>
        private bool IsFirstExecution()
        {
            FileInfo infoFile = new FileInfo("clientInfo.ini");
            if (infoFile.Exists == false)
            {
                return true;
            }
            return false;
        }
    }
}
