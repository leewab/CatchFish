using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    //一个用于注册UI节点名称的辅助类，用于避免代码中直接使用UI的路径，造成UI更改容易造成很多麻烦的问题
    [DisallowMultipleComponent]
    public class UITransformRegister : MonoBehaviour
    {
        public List<TransformInfo> TransformInfoList;

        private Dictionary<string, Transform> transformDic;
        public Transform GetTransformByName(string name)
        {
            if (transformDic == null)
            {
                transformDic = new Dictionary<string, Transform>();
                foreach(var info in TransformInfoList)
                {
                    transformDic[info.name] = info.transform;
                }
            }
            if (!transformDic.ContainsKey(name))
            {
                return null;
            }
            return transformDic[name];
        }
    }

    [System.Serializable]
    public class TransformInfo
    {
        [Tooltip("UI节点的名称（由UI的制作者自定义，不过应该尽量有意义）")]
        public string name;
        [Tooltip("UI节点的Transform,应该保证这个Transform和这个Register在同一个预制体中（在GenerateRes目录中，也应该在同一个预制体上），不能跨越UIChildMaker！" +
            "为了保证规范，最好保证这个Transform不要是这个Register的上层")]
        public Transform transform;
        [Tooltip("关于该UI节点的额外描述，仅用于方便理解，在程序运行时不会用到")]
        //目前，允许description被直接序列化，之后如果需要优化内存消耗量，可以不序列话这个字段，只是在Editor工程中维护一个TransformInfo -> description的映射
        //只在Editor显示的时候显示对应的描述
        public string description;
    }
}

