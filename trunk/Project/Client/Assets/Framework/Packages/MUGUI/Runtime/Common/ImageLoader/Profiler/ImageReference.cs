using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.UI
{
    public class ImageReference
    {
        public BaseImageLoader Loader { get; private set; }

        public string ImageName { get; private set; }

        public int Count { get; set; }

        public ImageReference(BaseImageLoader loader, string imgName)
        {
            this.Loader = loader;
            this.ImageName = imgName;
        }

    }

}
