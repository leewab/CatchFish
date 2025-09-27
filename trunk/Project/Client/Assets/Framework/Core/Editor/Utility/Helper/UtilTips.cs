using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
#if UNITY_EDITOR
   public class UtilTips : EditorWindow
   {
      private string title;
      private string content;
      private Action confirmCallBack;
      private Action cancelCallBack;

      public void ShowTips(string _title, string _content, Action _confirmCallBack = null, Action _cancelCallBack = null)
      {
         title = _title;
         content = _content;
         confirmCallBack = _confirmCallBack;
         cancelCallBack = _cancelCallBack;
      }

      private void OnGUI()
      {
         GUILayout.BeginHorizontal();
         GUILayout.Label(title, new GUIStyle {alignment = TextAnchor.MiddleCenter});
         GUILayout.EndHorizontal();
         GUILayout.BeginHorizontal();
         GUILayout.Label(content, new GUIStyle {alignment = TextAnchor.MiddleCenter});
         GUILayout.EndHorizontal();
         GUILayout.BeginHorizontal();
         if (GUILayout.Button("确认"))
         {
            Close();
            confirmCallBack?.Invoke();
         }
         if (cancelCallBack != null && GUILayout.Button("取消"))
         {
            Close();
            cancelCallBack?.Invoke();
         }
         GUILayout.EndHorizontal();
      }
   }
#endif
}