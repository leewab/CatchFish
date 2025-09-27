using System.Collections.Generic;
using Game.UI;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// InputController 独立Mono单例
    /// </summary>
    public class InputController : MonoSingleton<InputController>
    {
        /// <summary>
        /// 是否有触发使用Input组件
        /// </summary>
        public static bool IsFocusInputFiled = false;

        private void OnEnable()
        {
            GameInputField.FocusOnChange += OnFocusOnChange;
        }

        private void OnDisable()
        {
            GameInputField.FocusOnChange -= OnFocusOnChange;
        }

        private void Update()
        {
            //UnityEngine.Profiling.Profiler.BeginSample("handleKeyPress");
            HandleKeyPress();
            HandleMouse();
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        private void HandleKeyPress()
        {
            // if (MUGame.GameInputField.isFocusOn == true)
            // {
            //     if (Input.GetKeyDown(KeyCode.Return))
            //     {
            //         LuaInputMod.Instance().OnPressEnterWhenInput();
            //     }
            //     return;
            // }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
            }

#if UNITY_STANDALONE_WIN
            if (Input.GetKeyUp(KeyCode.PageUp))
            {
                //D.log("PageUp click Update Conf");
            }

            if (Input.GetKey(KeyCode.F10))
            {
            }

            if (Input.GetKey(KeyCode.F12))
            {
                Screen.SetResolution(960, 640, false);
            }

            for (int i = (int)KeyCode.Backspace; i <= (int)KeyCode.LeftAlt; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                }

                if (Input.GetKeyUp((KeyCode)i))
                {
                }
            }
#endif
        }

        private void HandleMouse()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            //if ( Input.GetMouseButton(1) )
            //{ // 0 is left, 1 is right, 2 is middle mouse button.
            //  // 	        float h = Input.GetAxis("Mouse X"); // The horizontal movement of the mouse.						
            //  // 	        float v = Input.GetAxis("Mouse Y"); // The vertical movement of the mouse.
            //  //             if ( h != 0 && v != 0 )
            //  //             {
            //  //                 LuaInputMod.Instance().OnMouseDrag( 1, new Vector2( h, v ) );
            //  //             }

            //    MUEngine.MURoot.MUCamera.Swipe(Input.mousePosition );
            //    //需要通知Lua端，摄像机被某个神奇的地方转动了
            //    LuaInputMod.Instance().NotifyCameraSwipe();
            //}
            //if ( Input.GetMouseButtonDown(1) )
            //{
            //    MUEngine.MURoot.MUCamera.OnSwipeStart(Input.mousePosition );
            //}
            //if ( Input.GetMouseButtonUp(1))
            //{
            //    MUEngine.MURoot.MUCamera.OnSwipeEnd( );
            //}
#endif
        }

        private void OnFocusOnChange(int instanceId, bool isFocus)
        {
            IsFocusInputFiled = isFocus;
        }


        static Dictionary<GameObject, bool> mEasyTouchDragIgnoreGameObjectDict = new Dictionary<GameObject, bool>();

        public static void AddEasyTouchDragIgnoreGameObject(GameObject obj)
        {
            mEasyTouchDragIgnoreGameObjectDict.Add(obj, true);
        }

        public static bool IsEasyTouchDragIgnoreGameObject(GameObject obj)
        {
            return mEasyTouchDragIgnoreGameObjectDict.ContainsKey(obj);
        }
        
    }
}