namespace Game
{
    public class PreloadProcessInit : PreloadProcess
    {
        public string PreloadDesc => "初始化Preload";
        
        public override void Start()
        {
            base.Log(PreloadDesc);
            this.InitPreload();
            this.Finish(PreloadDesc);
        }
        
        private void InitPreload()
        {
            GameConfig.TickTime(" 开始预加载 ");
        }
    }
}