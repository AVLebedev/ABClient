using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABClient
{
    public struct MessageConsts
    {
        public const string ConnectionSuccess = "Соединение установлено";
        public const string ConnectionFail = "Не удалось подключиться к серверу";
        public const string DisconnectionSuccess = "Соединение успешно закрыто";
        public const string DisconnectionFail = "При отключении от сервера произошла ошибка. \nСоединение разорвано";

        public const string ActivationSuccess = "Активация прошла успешно. Удачного использования!";
        public const string ActivationFail = "Ошибка активации. Попробуйте снова.";

        //public const string DisconnectMessage = "Клиент отключился";
        public const string NoConnection = "Нет соединиения c сервером";
        public const string StartConnecting = "Для установки соединения нажмите 'Подключиться'";
        public const string ServerNotResponse = "Не удалось получить ответ от сервера";
        public const string UnactiveClient = "Клиент с текущим id не активирован на сервере";

        public const string ConnectMessage = "|connect";
        public const string DisconnectMessage = "|disconnect";
        public const string AlarmMessage = "|alarm";
        public const string ActivateMessage = "|activate";
    }
}
