using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_AdminView
    {
        private string username = "";
        private string password = "";
        private string code = "";
        
        internal enum AdminState
        {
            ToggleLogin,
            ToggleRegister,
        }
        
        //当前的管理中心状态
        private AdminState adminState  = AdminState.ToggleLogin;
        
        internal void OnGUI()
        {
            GUILayout.BeginVertical(EditorHelper.BoxGUIStyle);
            switch (adminState)
            {
                case AdminState.ToggleLogin:
                    LoginTop();
                    LoginMiddle();
                    LoginBottom();
                    break;
                case AdminState.ToggleRegister:
                    RegisterTop();
                    RegisterMiddle();
                    RegisterBottom();
                    break;
            }
            GUILayout.EndVertical();
        }

        #region DrawAdminLogin

        private void LoginTop()
        {
            EditorHelper.DrawTitle("PM管理（登陆）");
        }

        private void LoginMiddle()
        {
            username = EditorHelper.DrawField("用户名：", username, new RectOffset(50, 80, 5, 5), new Vector2(80, 0));
            password = EditorHelper.DrawField("密  码：", password, new RectOffset(50, 80, 5, 5), new Vector2(80, 0));
            code = EditorHelper.DrawField("验证码：", code, new RectOffset(50, 80, 5, 5), new Vector2(80, 0));
            GUILayout.Space(10);
            EditorHelper.DrawButton("登              录", new RectOffset(100, 100, 5, 5), new Vector2(0, 26),() =>
            {
                Debug.Log("登录");
                Dictionary<string, string> postDataDic = new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password},
                    {"qrcode", "0123456789"},
                    {"protocol", $"{(int)ProtocolEnum.ProtocolAction.USER_LOGIN}"},
                    {"id", new System.Random().Next(10000).ToString("x8")}
                };
                HttpHelper.HttpRequestFormPost(PM_Define.AdminURL, postDataDic, "login");
            });
            EditorHelper.DrawButton("前往注册", new RectOffset(100, 100, 5, 5), new Vector2(0, 20),() =>
            {
                Debug.Log("前往注册");
                adminState = AdminState.ToggleRegister;
            });
        }

        private void LoginBottom()
        {
            var lStyle = GUI.skin.label;
            lStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Space(40);
            EditorHelper.DrawLable("@Copyright leewab 2019-09-11", lStyle);
            GUILayout.Space(20);
        }

        #endregion

        #region DrawAdminRegister

        private void RegisterTop()
        {
            EditorHelper.DrawTitle("PM管理（注册）");
        }

        private void RegisterMiddle()
        {
            username = EditorHelper.DrawField("用户名：", username, new RectOffset(50, 80, 5, 5), new Vector2(80, 0));
            password = EditorHelper.DrawField("密  码：", password, new RectOffset(50, 80, 5, 5), new Vector2(80, 0));
            code = EditorHelper.DrawField("二次密码：", code,new RectOffset(50, 80, 5, 5), new Vector2(80, 0));
            GUILayout.Space(10);
            EditorHelper.DrawButton("注              册", new RectOffset(100, 100, 5, 5), new Vector2(0, 26), () =>
            {
                Debug.Log("注册");
                if (string.IsNullOrEmpty(username) | string.IsNullOrEmpty(password)) return;
                Dictionary<string, string> postDataDic = new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password},
                    {"idcard", "0123456789"},
                    {"phone", "1234567890"},
                    {"protocol", $"{(int)ProtocolEnum.ProtocolAction.USER_REGISTER}"},
                    {"id", new System.Random().Next(10000).ToString("x8")}
                };
                HttpHelper.HttpRequestFormPost(PM_Define.AdminURL, postDataDic, "register");
            });

            EditorHelper.DrawButton("前往登录", new RectOffset(100, 100, 5, 5), new Vector2(0, 20),
                () =>
                {
                    Debug.Log("前往登录");
                    adminState = AdminState.ToggleLogin;
                });
        }

        private void RegisterBottom()
        {
            var lStyle = GUI.skin.label;
            lStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Space(40);
            EditorHelper.DrawLable("@Copyright leewab 2019-09-11", lStyle);
            GUILayout.Space(20);
        }

        #endregion
    }
}
