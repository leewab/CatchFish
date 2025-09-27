using System.Collections.Generic;
using Game;
using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
#if UNITY_EDITOR
    
    [InitializeOnLoad]
    public class EditorRuntimeInitiator
    {
        private static IEnumerable<IBaseEditorRuntime> editorRuntimes = new List<IBaseEditorRuntime>();

        static EditorRuntimeInitiator()
        {
            CollectEditorRuntime();
            OnAwake();
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            // Debug.Log("EditorInitiator");
        }

        private static void CollectEditorRuntime()
        {
            editorRuntimes = AssemblyHelper.GetDerivedClassInstancesWithInterface<IBaseEditorRuntime>();
        }

        private static void OnAwake()
        {
            if (editorRuntimes == null) return;
            foreach (var editorRun in editorRuntimes)        
            {
                editorRun?.Awake();
            }
        }

        private static void OnUpdate()
        {
            if (editorRuntimes == null) return;
            foreach (var editorRun in editorRuntimes)        
            {
                editorRun?.Update();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Initiator()
        {
            FormatLua.FormatLuaFile_EventDefine();
        }

    }
    
#endif
}