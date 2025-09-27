using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MUGUI
{
    [AddComponentMenu("UI/ButtonExtend", 100)]
    [RequireComponent(typeof(Button))]
    public class ButtonExtend : MonoBehaviour
    {
        public bool ChangeImg;
        public Sprite DisableImg;
        public Sprite EnableImg;
        public string DisableImgName;
        public string EnableImgName;
        public bool ChangeText;
        public Text mText;
        public string DisableText;
        public string EnableText;
        public bool ChangeTextColor;
        public Color DisableTextColor;
        public Color EnableTextColor;

        private Button mButton;
        Color defaultColor, hoverColor, pressColor;
        private bool showEnable;
        private ImageLoader imgLoader;
        private bool inited = false;
        private string saveImgName = "";

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (this.inited) 
            {
                return;
            }
            inited = true;
            mButton = this.gameObject.GetComponent<Button>();

            if (mButton != null)
            {
                //button.onClick.AddListener(OnClick);
                showEnable = mButton.interactable;
                defaultColor = mButton.colors.normalColor;
                hoverColor = mButton.colors.highlightedColor;
                pressColor = mButton.colors.pressedColor;
            }
            if (ChangeImg)
            {
                imgLoader = ImageLoader.GetOrAddImageLoader(mButton.image);
                if (saveImgName != "")
                {
                    ReloadImage(saveImgName);
                }
            }
        }

        private void ReloadImage(string imageName)
        {
            if (imgLoader == null)
            {
                saveImgName = imageName;
                return;
            }
            saveImgName = "";
            if (imgLoader.ImageName == imageName)
            {
                return;
            }
            imgLoader.ImageName = imageName;
            imgLoader.Loaded = OnLoadDone;
            imgLoader.Reload();
        }
        private void OnLoadDone(string name, UnityEngine.Object obj)
        {
        }

        public void SetEnable(bool enable)
        {
            if (enable)
            {
                if (ChangeImg)
                {
                    ReloadImage(EnableImgName);
                }
                if (ChangeText && mText != null)
                {
                    mText.text = EnableText;
                }
                if (ChangeTextColor && mText != null)
                {
                    mText.color = EnableTextColor;
                }
            }
            else
            {
                if (ChangeImg)
                {
                    ReloadImage(DisableImgName);
                }
                if (ChangeText && mText != null)
                {
                    mText.text = DisableText;
                }
                if (ChangeTextColor && mText != null)
                {
                    mText.color = DisableTextColor;
                }
            }
        }

    }
}
