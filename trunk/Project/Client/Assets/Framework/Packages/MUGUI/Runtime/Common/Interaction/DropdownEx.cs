using Game.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/DropdownEx", 35)]
    [RequireComponent(typeof(RectTransform))]
    public class DropdownEx : Dropdown
    {
        public Sprite buttonPicOnClick = null;
        public Sprite buttonPicOnNotClick = null;
        public string OnClickImgName = "";
        public string OnNotClickImgName = "";

        [HideInInspector]
        // 是否点击任何列表项都能触发回调
        // 原OnValueChange只会在有变化时回调
        public bool IsAlwaysCallback { get; set; } = false;

        [HideInInspector]
        public DropdownEvent OnAnyValueClicked { get; set; } = new DropdownEvent();
        
        private ImageLoader imgLoader;
        protected override void Start()
        {
            base.Start();

            imgLoader = ImageLoader.GetOrAddImageLoader(image);

        }
        protected override void OnEnable()
        {
            base.OnEnable();

        }
        protected override GameObject CreateDropdownList(GameObject template)
        {
            if (buttonPicOnClick != null)
            {
                ReloadImage(OnClickImgName);
            }
            return base.CreateDropdownList(template);
        }
        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            if (buttonPicOnNotClick != null)
            {
                ReloadImage(OnNotClickImgName);
            }
            base.DestroyDropdownList(dropdownList);
        }

        private void ReloadImage(string imageName)
        {
            if (imgLoader == null)
            {
                return;
            }
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
        
        public new void Show()
        {
            base.Show();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            Show();
            if (!IsAlwaysCallback)
            {
                return;
            }
            var toggleRoot = transform.Find("Dropdown List/Viewport/Content");
            if (toggleRoot == null) return;
            Toggle[] toggleList = toggleRoot.GetComponentsInChildren<Toggle>(false);
            foreach (var toggle in toggleList)
            {
                toggle.onValueChanged.AddListener(x => this.OnSelectItemExtend(toggle));
            }
        }

        public void OnSelectItemExtend(Toggle toggle)
        {
            this.OnAnyValueClicked.Invoke(this.value);
        }
        
    }
}
