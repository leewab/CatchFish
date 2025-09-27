using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class UILoginView : GamePanel
    {
        private GameLabel mTxtLoginTitle;
        private GameButton mBtnLoginEnter;

        protected override void OnInit()
        {
            base.OnInit();
            this.mBtnLoginEnter = Make<GameButton>("Btn_EnterGame");
            this.mBtnLoginEnter.AddClickCallBack(OnClickEnterGameEvent);
        }

        private void OnClickEnterGameEvent(GameObject obj)
        {
            // Message.Send();
            // Message.OnResponse();
            UIManager.Instance.Close(this);
            Debug.Log("Click EnterGame");
            SceneManager.LoadScene("Game002");
#if UNITY_TOLUA
            LuaModule.Instance.EnterGame("id=100,name='Game002'");
#endif
            // CommandHandler.Instance.Execute("print(\"我执行成功啦！\")");
        }
        
    }
}