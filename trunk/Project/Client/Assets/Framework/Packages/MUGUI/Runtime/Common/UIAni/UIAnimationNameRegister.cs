using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MUGame
{
    //一个存粹的数据脚本，用于指定某个节点对应的界面，对应的UI动画名称
    //用于让动画的制作者显式地指定某个界面的动画状态机trigger名称，避免在代码中写死名称导致不必要的复杂性
    [DisallowMultipleComponent]
    public class UIAnimationNameRegister : MonoBehaviour
    {
        //暂时只需要指定入场动画和出场动画，其它的特殊动画仍然需要在代码中指定名称，不过它们的数量目前很少
        public string enterName;
        public string exitName;
    }
}

