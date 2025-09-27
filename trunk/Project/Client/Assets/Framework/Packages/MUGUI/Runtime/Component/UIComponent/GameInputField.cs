using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameInputField : GameLabel
    {
        /// <summary>
        /// Focus事件播报
        /// </summary>
        public static Action<int, bool> FocusOnChange = null;
        
        protected override void OnInit()
        {
            base.OnInit();
            this.InputField.onValueChanged.AddListener(onValueChange);
            UIEventListener.AddSelectCallBack(OnSelectHandler);
            UIEventListener.AddDeselectCallBack(OnDeselectHandler);
        }

        protected override void OnHide()
        {
            base.OnHide();
            if (GameInputField.FocusOnChange != null) GameInputField.FocusOnChange(InstanceID, false);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.InputField.onValueChanged.RemoveAllListeners();
            this.InputField.onEndEdit.RemoveAllListeners();
        }

        private void onValueChange(string str)
        {
            this.InputField.text = str;
        }

        private void OnSelectHandler(GameObject go)
        {
            if (GameInputField.FocusOnChange != null) GameInputField.FocusOnChange(InstanceID, true);
            if (this.InputField.placeholder == null) return;
            this.InputField.placeholder.gameObject.SetActive(false);
        }

        private void OnDeselectHandler(GameObject go)
        {
            if (GameInputField.FocusOnChange != null) GameInputField.FocusOnChange(InstanceID, false);
            if (this.InputField.placeholder == null) return;
            this.InputField.placeholder.gameObject.SetActive(true);
        }
        
        public void ExecuteLuaInput()
        {
#if UNITY_TOLUA
            LuaInterface.LuaState luaState = LuaClient.Instance.GetLuaState();
            if (luaState != null)
            {
                luaState.DoString(this.InputField.text);
            }
            else
            {
                Debug.LogError("执行失败,Lua虚拟机未启动！");
            }
#endif
        }

        //设置内容，但是不会允许截断，如果内容超长，则忽略此次操作。
        //因为lua中不能有效计算字符串长度，不能估计到是否截断。
        //额外增加一个返回值，用于表示本次赋值是否成功
        public bool SetTextWithoutTrim(string value)
        {
            int limit = this.InputField.characterLimit;
            if (limit > 0 && value.Length > limit) return false;
            this.InputField.text = value;
            return true;
        }
        
        public void ActivateInputField()
        {
            this.InputField.ActivateInputField();
        }

        public void DeactivateInputField()
        {
            this.InputField.DeactivateInputField();
        }

        public void MoveTextStart(bool shift)
        {
            this.InputField.MoveTextStart(shift);
        }

        public void MoveTextEnd(bool shift)
        {
            this.InputField.MoveTextEnd(shift);
        }

        public void AddChangeCallBack(UnityEngine.Events.UnityAction<string> call)
        {
            RemoveChangeCallBack(call);
            this.InputField.onValueChanged.AddListener(call);
        }

        public void RemoveChangeCallBack(UnityEngine.Events.UnityAction<string> call)
        {
            this.InputField.onValueChanged.RemoveListener(call);
        }

        public void AddOnEditEndCallBack(UnityEngine.Events.UnityAction<string> call)
        {
            RemoveOnEditEndCallBack(call);
            this.InputField.onEndEdit.AddListener(call);
        }

        public void RemoveOnEditEndCallBack(UnityEngine.Events.UnityAction<string> call)
        {
            this.InputField.onEndEdit.RemoveListener(call);
        }
       
        #region Property   
  
        public override Text Label
        {
            get
            {
                if (gameObject)
                {
                    if (!this.mLabel)
                    {
                        this.mLabel = this.InputField.textComponent;
                        if (!this.mLabel) ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Label 失败, Text 为空!");
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Label 失败, gameObject 为空!");
                }

                return this.mLabel;
            }
        }

        
        private InputField mInputField;
        public InputField InputField
        {
            get
            {
                if (gameObject)
                {
                    if (!this.mInputField)
                    {
                        this.mInputField = gameObject.GetComponent<InputField>();
                        this.mInputField.onValueChanged.RemoveListener(onValueChange);
                        this.mInputField.onValueChanged.AddListener(onValueChange);
                        if (!this.mInputField) ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - InputField 失败, Text 为空!");
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - InputField 失败, gameObject 为空!");
                }

                return this.mInputField;
            }
        }
        
        public int MaxCharNum
        {
            set => this.InputField.characterLimit = value;
            get => this.InputField.characterLimit;
        }
        
        public bool Interactable
        {
            set => this.InputField.interactable = value;
            get => this.InputField.interactable;
        }
        
        public bool MultipleLine
        {
            set => this.InputField.lineType = value
                ? UnityEngine.UI.InputField.LineType.MultiLineNewline
                : UnityEngine.UI.InputField.LineType.SingleLine;
            get => this.InputField.lineType == UnityEngine.UI.InputField.LineType.MultiLineNewline;
        }

        public bool IsFocused => this.InputField.isFocused;

        #endregion

    }
}