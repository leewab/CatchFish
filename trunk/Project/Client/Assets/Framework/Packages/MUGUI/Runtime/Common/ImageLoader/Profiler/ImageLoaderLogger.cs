using System.Collections.Generic;

namespace Game.UI
{
    /// <summary>
    /// 图片加载日志记录器
    /// 仅用于编辑器模式
    /// </summary>
    public static class ImageLoaderLogger
    {
        /// <summary>
        /// 注册资源加载
        /// </summary>
        /// <param name="imgLoader">图片加载器</param>
        /// <param name="imageName">图片名称</param>
        public static void Register(BaseImageLoader imgLoader, string imageName)
        {
#if UNITY_EDITOR

            Dictionary<string, ImageReference> refDict;
            if (AllImageReferences.ContainsKey(imgLoader))
            {
                refDict = AllImageReferences[imgLoader];
            }
            else
            {
                refDict = new Dictionary<string, ImageReference>();
                AllImageReferences[imgLoader] = refDict;
            }

            ImageReference imgRef;
            if (refDict.ContainsKey(imageName))
            {
                imgRef = refDict[imageName];
            }
            else
            {
                imgRef = new ImageReference(imgLoader, imageName);
                refDict[imageName] = imgRef;
            }

            imgRef.Count++;
#endif
        }

        /// <summary>
        /// 注册资源卸载
        /// </summary>
        /// <param name="imgLoader">图片加载器</param>
        /// <param name="imageName">图片名称</param>
        public static void Unregister(BaseImageLoader imgLoader, string imageName)
        {
#if UNITY_EDITOR
            if (!AllImageReferences.ContainsKey(imgLoader))
            {
                return;
            }
            Dictionary<string, ImageReference> refDict = AllImageReferences[imgLoader];
            if (!refDict.ContainsKey(imageName))
            {
                return;
            }
            ImageReference imgRef = refDict[imageName];
            imgRef.Count--;
            
            if (imgRef.Count == 0)
            {
                refDict.Remove(imageName);
            }
            if (refDict.Count == 0)
            {
                AllImageReferences.Remove(imgLoader);
            }
#endif
        }

#if UNITY_EDITOR
        private static Dictionary<BaseImageLoader, Dictionary<string, ImageReference>> AllImageReferences = new Dictionary<BaseImageLoader, Dictionary<string, ImageReference>>();

        /// <summary>
        /// 获取当前引用情况
        /// </summary>
        /// <returns>引用列表</returns>
        public static List<ImageReference> GetCurrentReference()
        {
            List<ImageReference> refList = new List<ImageReference>();
            foreach (var refDict in AllImageReferences.Values)
            {
                foreach (var imgRef in refDict.Values)
                {
                    refList.Add(imgRef);
                }
            }

            return refList;
        }

#endif

    }

}
