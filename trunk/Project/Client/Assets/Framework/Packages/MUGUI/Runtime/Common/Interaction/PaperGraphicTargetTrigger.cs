using UnityEngine;

namespace Game.UI
{
    //该类主要方便在动画中管理PaperGraphicTarget
    public class PaperGraphicTargetTrigger : MonoBehaviour
    {
        [SerializeField]
        private PaperGraphicTarget[] targetArray;
        
        public void EnterPaperState(int index)
        {
            if(index >=0 && index < targetArray.Length && targetArray[index] != null)
            {
                targetArray[index].SetPaperState(true);
            }
        }

        public void ExitPaperState(int index)
        {
            if (index >= 0 && index < targetArray.Length && targetArray[index] != null)
            {
                targetArray[index].SetPaperState(false);
            }
        }

    }
}

