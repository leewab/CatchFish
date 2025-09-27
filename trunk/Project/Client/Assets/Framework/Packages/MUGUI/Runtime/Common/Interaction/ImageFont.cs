using System;
using Game.UI;
using MUEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MUGUI
{
    [System.Serializable]
    public class EmojiInfo
    {
        public string name;
        public int key;
        public int frameCount;
        //以下四个量是描述纹理坐标的。
        public float ulen;
        public float vlen;
        public float startu;
        public float startv;
        public float ratio;
    }

    [System.Serializable]
    public class ImageFontItem
    {
        public EmojiInfo[] _emojiinfo;
        //public Vector4[] _EmojiSelfData;
        //public float[] _EmojiUVData;
        public float _umax, _realulen, _realvlen,_ulimit;
    }
    public class ImageFont : MonoBehaviour
    {    
        //这三个成员需要通过菜单命令emoji/buildemoji来完成构建。
        public const string OutputPath = "Assets/Res/Emoji/output/";
        public const string InputPath = "Assets/Res/Emoji/input/";
        public const string IMAGEFONTNAME = "DongTai_Xiaohu.prefab";
        public static event UnityEngine.Events.UnityAction waitForImageFontLoaded;
        private static ImageFont _instance;
        private static bool _bloading=false;
        public static int emoji1num = 47;
        public static ImageFont instance
        {
            get
            {
                if(!_bloading)
                {
                    //_instance为空的时候将异步加载，所有访问这一属性的方法应当意识到这一切。
                    //但在目前情况下advanced.start时已经加载好了。
                    _bloading = true;
                    //保证res editor中不报错。
                    if (GOGUITools.GetAssetAction != null)
                        GOGUITools.GetAssetAction(IMAGEFONTNAME, (string name, UnityEngine.Object obj) => {
                            _instance = ((GameObject)obj).GetComponent<ImageFont>();
                            emoji1num = _instance.items[2]._emojiinfo.Length;
                            if (waitForImageFontLoaded != null)
                                waitForImageFontLoaded();
                        }, MUEngine.LoadPriority.Prior, MUEngine.ECacheType.AutoDestroy);
                }
                return _instance;
            }
        }
        //common数据在其数组的首个位置上。偏移要加1.
        [SerializeField]
        public Texture[] _EmojiTexture;
        [SerializeField]
        public ImageFontItem[] items;
        //建立shader变量，目的是保持引用。
        [SerializeField]
        public Shader sh1;
        [SerializeField]
        public Shader sh2;
        [SerializeField]
        public int NUM_EMOJI_SET;

        [System.NonSerialized]
        private Material[] _mat;
        public Material this[int setid]
        {
            get
            {
                if (setid != 0 && setid != 1)
                    return null;
                if (_mat == null || _mat.Length == 0)
                    _mat = new Material[2];
                if (_mat[setid] == null)
                {
                    //构建出这种材质，常量数组已经被预计算了。
                    Material var;
                    if (setid == 0)
                    {
                        var = new Material(sh1);
                        var.SetTexture("_CommonTex", _EmojiTexture[0]);
                        var.SetTexture("_EmojiTex", _EmojiTexture[1]);
                        var.SetFloat("_FrameSpeed", 3);
                        var.SetFloat("_uwidth", items[1]._realulen);
                        var.SetFloat("_vwidth", items[1]._realvlen);
                        var.SetFloat("_umax", items[1]._umax);
                        var.SetFloat("_ulimit", items[1]._ulimit);
                    }
                    else
                    {
                        var = new Material(sh2);
                        //var.SetTexture("_CommonTex", _EmojiTexture[3]);
                        //var.SetTexture("_EmojiTex", _EmojiTexture[2]);
                        var.SetFloat("_FrameSpeed", 3);
                        var.SetFloat("_uwidth", items[2]._realulen);
                        var.SetFloat("_vwidth", items[2]._realvlen);
                        var.SetFloat("_umax", items[2]._umax);
                        var.SetFloat("_ulimit", items[2]._ulimit);
                        var.SetFloat("_uwidth2", items[3]._realulen);
                        var.SetFloat("_vwidth2", items[3]._realvlen);
                        var.SetFloat("_umax2", items[3]._umax);
                        var.SetFloat("_ulimit2", items[3]._ulimit);
                    }
                    _mat[setid] = var;
                    //一定要注意将值写回。
                }
                return _mat[setid];
            }
        }

        private Texture2D tex1, tex2;
        public static WeakReference w1, w2;
        private static bool flag;
        private static Game.UI.GameCommonList grid;
        public static void LoadImage(bool _flag, Game.UI.GameCommonList _grid)
        {
            flag = _flag;
            if (!flag)
            {
                if (instance != null) instance.Release();
                return;
            }

            if (_grid == grid) return;
            if (instance == null)
                waitForImageFontLoaded += (() => { if(flag) LoadImage(true, _grid); });
            else
            {
                grid = _grid;
                GOGUITools.GetAssetAction("emoji_1.png", instance.OnGetImg,
                    LoadPriority.Prior, ECacheType.AutoDestroy);
                GOGUITools.GetAssetAction("emoji_2.png", instance.OnGetImg,
                    LoadPriority.Prior, ECacheType.AutoDestroy);
            }
        }
        private void OnGetImg(string name, Object obj)
        {
            if (name == "emoji_1.png")
            {
#if UNITY_EDITOR
                if (GameConfig.LoadFromPrefab)
                    tex1 = obj as Texture2D;
                else
#endif
                    tex1 = (obj as Sprite).texture;
                w1 = new WeakReference(tex1);
            }
            else
            {
#if UNITY_EDITOR
                if (GameConfig.LoadFromPrefab)
                    tex2 = obj as Texture2D;
                else
#endif
                    tex2 = (obj as Sprite).texture;
                w2 = new WeakReference(tex2);
            }
            if (!flag)
            {
                Release();
                return;
            }
            if (tex1 != null && tex2 != null)
            {
                Material mat= this[1];
                mat.SetTexture("_EmojiTex", tex1);
                mat.SetTexture("_CommonTex", tex2);
                if (grid != null)
                {
                    grid.Visible = true;
                    grid.BeginUpdate();
                }
            }
        }
        private void Release()
        {
            if (tex1 != null)
            {
                GOGUITools.ReleaseAssetAction("emoji_1.png", tex1);
                tex1 = null;
            }
            if (tex2 != null)
            {
                GOGUITools.ReleaseAssetAction("emoji_2.png", tex2);
                tex2 = null;
            }
            if (grid != null)
            {
                grid.EndUpdate();
                grid.Visible = false;
                grid = null;
            }
        }

        public EmojiInfo[] getemojiInfo(int setid=-1)
        {
            if (setid < -1 || setid >= NUM_EMOJI_SET)
                return null;
            return items[setid+1]._emojiinfo;
        }
        public EmojiInfo getemojiInfoItem(int setid,int emoji_id)
        {
            if (setid < 0 || setid >= NUM_EMOJI_SET)
                return null;
            if (emoji_id >= 0 && emoji_id < items[setid+1]._emojiinfo.Length)
                return items[setid+1]._emojiinfo[emoji_id];
            emoji_id -= 1000;
            if (emoji_id >=0 && emoji_id < items[0]._emojiinfo.Length)
                return items[0]._emojiinfo[emoji_id];
            return items[1]._emojiinfo[0];
        }
    }
}