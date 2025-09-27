using System;
using Game;
using Game.Core;
using Utils;

namespace MUGame.Functions
{
    public static class Net
    {
        public static SafeAction<msg_base> Send;
        public static SafeAction<string, int> Connect;
        public static SafeAction<string, int> CrossConnect;
        public static SafeAction SelectServer;
        public static SafeAction Disconnect;
        public static SafeAction Reconnect;
        public static SafeAction CrossDisconnect;
        public static SafeAction CrossReconnect;
        public static SafeAction ReturnToLogin;
        public static SafeAction SwitchAccount;
        public static SafeAction ToggleToActive;
        public static SafeAction ToggleToLogin;
        public static SafeAction RestartLogin;
        public static SafeAction TryConnectServer;
        public static SafeAction OnGotToken;
        public static SafeAction ToggleToGoto;

        //qxm
        public static SafeAction<int, byte[],int> SendById;
    }
}
