namespace Game.UI
{
    public interface IRes
    {
        /// <summary>
        /// 加载、释放图片
        /// </summary>
        public void ReleaseImage();
        public void ReloadImage();

        /// <summary>
        /// 加载是否字体，更新文本
        /// </summary>
        // public void ReleaseFont();
        // public void ReloadFond();
        
    }
}