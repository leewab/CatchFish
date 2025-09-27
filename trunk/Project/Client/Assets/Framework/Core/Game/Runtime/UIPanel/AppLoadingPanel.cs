using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class AppLoadingPanel : GamePanel
    {
        [SerializeField] private Text txtTips;
        [SerializeField] private Slider sliProgress;
        
        public void ShowLoadingTips(string content)
        {
            txtTips.text = content;
        }

        public void ShowLoadingProgress(float progress)
        {
            sliProgress.value = progress;
        }

    }
}