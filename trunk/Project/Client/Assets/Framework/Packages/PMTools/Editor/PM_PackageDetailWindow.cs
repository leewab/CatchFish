using Framework.Core;

namespace Framework.PM
{
    public class PM_PackageDetailWindow : BaseEditorWindow
    {
        private bool mIsAdmin = false;
        private PM_PackageDetailView mPackageDetailView;
        private PM_PackageDetailModule mPackageDetailModule;

        public void Init(C2S_PB.C2S_PackageInfo _info, bool _isAdmin = false)
        {
            mPackageDetailModule.PackageInfo = _info;
        }
        
        protected override void Awake()
        {
            mPackageDetailView = new PM_PackageDetailView();
            mPackageDetailModule = new PM_PackageDetailModule();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            mPackageDetailView.OnGUI(mPackageDetailModule);
        }

        public override void Clear()
        {
            base.Clear();
            mPackageDetailView.Clear();
            mPackageDetailView = null;
            mPackageDetailModule = null;
        }
    }
}