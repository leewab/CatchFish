using Game.Core;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Asset文件存储于Assets/Framework/Core/Editor/AssetData/下，
    /// 当创建或更新之后会同步拷贝到StreamingAsset下的AssetData文件夹下
    /// 真正读取使用的Asset文件是StreamingAsset下的AssetData中的文件，包括编辑器中的读取
    /// </summary>
    public class BaseAsset : ScriptableObject
    {
        /// <summary>
        /// 模块是否可用
        /// </summary>
        [Header("是否启用")]
        [Tooltip("是否开启该Asset数据模块")]
        [SerializeField] public bool Enable = false;

        /// <summary>
        /// 模块宏定义
        /// </summary>
        [Header("宏定义")]
        [Tooltip("宏定义设定")]
        [SerializeField] public string[] ScirptingDefineSymbols = null;
        
        /// <summary>
        /// 模块目标平台
        /// </summary>
        [Header("目标平台")]
        [Tooltip("目标平台")]
        [SerializeField] public TargetPlatformEnum BuildTargetGroup = TargetPlatformEnum.Standalone;

        /// <summary>
        /// Asset数据更新操作
        /// </summary>
        [Header("数据更新")]
        [Tooltip("选择Update更新Asset中的所有数据，包括宏定义更新，File文件更新")]
        [SerializeField] public OperaEnum AssetUpdate = OperaEnum.None;
        
        /// <summary>
        /// 编辑Asset文件之后唤醒
        /// </summary>
        protected virtual void OnValidateAwake()
        {
            
        }
        
        /// <summary>
        /// 当前数据刷新
        /// </summary>
        protected virtual void OnAssetDataRefresh()
        {
            
        }

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            OnValidateAwake();
            if (AssetUpdate == OperaEnum.Update)
            {
                AddDefineSymbols();
                OnAssetDataRefresh();
            }
            AssetUpdate = OperaEnum.None;
        }

        protected virtual void OnEnable()
        {
            ScirptingDefineSymbols = GetDefineSymbols();
        }

        private string[] GetDefineSymbols()
        {
            string allSymbols = UnityEditorUtil.GetScriptingDefineSymbols(BuildTargetGroup);
            return allSymbols?.Split(';');
        }

        private void AddDefineSymbols()
        {
            if (ScirptingDefineSymbols != null && ScirptingDefineSymbols.Length > 0)
            {
                UnityEditorUtil.AddScriptingDefineSymbols(BuildTargetGroup, ScirptingDefineSymbols);
            }
        }

        private void RemoveDefineSymbols()
        {
            if (ScirptingDefineSymbols != null && ScirptingDefineSymbols.Length > 0)
            {
                UnityEditorUtil.RemoveScriptingDefineSymbols(BuildTargetGroup, ScirptingDefineSymbols);
            }
        }

#endif
        
    }
    
    public enum OperaEnum
    {
        None,
        Update,
    }

}
