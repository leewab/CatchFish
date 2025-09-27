using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    /// <summary>
    /// Simple toggle -- something that has an 'on' and 'off' states: checkbox, toggle button, radio button, etc.
    /// </summary>
    [AddComponentMenu("UI/ToggleEx", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class ToggleEx : Toggle
    {
        public List<Graphic> listGraphic = new List<Graphic>();
        public bool switchShow = false;

        protected override void Start()
        {
            base.Start();

            UnityAction<bool> valueChange = new UnityAction<bool>(ToggleExOnValueChange);
            this.onValueChanged.AddListener(valueChange);

            ToggleExOnValueChange(isOn);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            ToggleExOnValueChange(isOn);
        }
        public void ToggleExOnValueChange(bool flag)
        {
            bool instant = toggleTransition == ToggleTransition.None;
            int count = listGraphic.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    listGraphic[i].CrossFadeAlpha(flag ? 1f : 0f, instant ? 0f : 0.1f, true);
                }
            }
            if (switchShow)
            {
                this.targetGraphic.gameObject.SetActive(!flag);
                this.graphic.gameObject.SetActive(flag);
            }
        }
    }
}
