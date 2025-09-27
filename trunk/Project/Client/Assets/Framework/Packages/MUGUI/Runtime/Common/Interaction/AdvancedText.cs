using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using System;
using Game.UI;
using UnityEngine.Events;

namespace MUGUI
{
    class AdvancedTextCopy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public AdvancedText target;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (target != null) target.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (target != null) target.OnPointerUp(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (target != null) target.OnPointerExit(eventData);
        }
    }
    public class AdvancedText : Text, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region SerializeField
        [SerializeField]
        [TextArea(5, 10)]
        private string s_OriginalText;
        [SerializeField]
        private int s_setId;
        [SerializeField]
        private bool s_auto;
        [SerializeField]
        private bool s_copy;
        [SerializeField]
        private bool s_input;
        private bool bhasstarted = false;
        private float _pixelsPerUnit_when_start = -1;
        public bool mSpaceReplace = true;
        public float pixelsPerUnit_when_start
        {
            get
            {
                if (_pixelsPerUnit_when_start < 0)
                    _pixelsPerUnit_when_start = pixelsPerUnit;
                return _pixelsPerUnit_when_start;
            }
        }
#if UNITY_EDITOR
        protected void OnValidate()
        {
            base.OnValidate();
            if (bhasstarted)
            {
                //序列化数据发生，并不会自动更新效果。
                SetId = s_setId;
                OriginalText = s_OriginalText;
                auto = s_auto;
                //imagefont已经不支持这种修改。
            }
        }
#endif
        private bool _auto;
        public bool auto
        {
            get { return _auto; }
            set
            {
                if (auto == value) return;
                _auto = value;
                SetVerticesDirty();
            }
        }
        private int _setId = -1;
        public int SetId
        {
            get { return _setId; }
            set
            {
                if (Application.isPlaying && value != _setId && ImageFont.instance != null)
                {
                    if (value < -1 || value >= ImageFont.instance.NUM_EMOJI_SET)
                        value = 0;
                    if (value == _setId)
                        return;
                    _setId = value;
                    material = ImageFont.instance[_setId];
                    var ort = m_OriginText;
                    m_OriginText = "";
                    OriginalText = ort;
                }
            }
        }

        protected override void Start()
        {

            //clone出的AdvanvedText上没有OnValidate,并且这个函数是在prefab被装载的时候执行。
            base.Start();
            bhasstarted = true;

            //imagefont已经不支持这种修改。
            if (ImageFont.instance == null)
                ImageFont.waitForImageFontLoaded += (() => { SetId = s_setId; });
            else
                SetId = s_setId;
            OriginalText = s_OriginalText;
            auto = s_auto;
            if (s_copy&&Application.isPlaying)
            {
                //确保可拷贝的对象一定有parent。
                //并且响应区域就是parent的区域
                var parent = transform.parent.gameObject;
                var com = parent.GetComponent<AdvancedTextCopy>();
                if(com ==null)
                    com = parent.AddComponent<AdvancedTextCopy>();
                com.target = this;
            }
        }

        private string m_OriginText = "";
        public string OriginalText
        {
            get { return m_OriginText; }
            set
            {
                bool e1 = string.IsNullOrEmpty(m_OriginText);
                bool e2 = string.IsNullOrEmpty(value);
                if (e1 && e2) return;
                if (m_OriginText.Equals(value)) return;
                if (e2)
                {
                    m_OriginText = "";
                    base.text = "";
                }
                else
                {
                    m_OriginText = value;
                    base.text = ParseText();
                    //当标记改变了内容 可能传给base.text的内容仍然不变。
                    SetVerticesDirty();
                }
                s_OriginalText = m_OriginText;
            }
        }
        /*
       public override void SetVerticesDirty()
        {
            sb.Length = 0;
            var trans = transform;
            while (trans != null)
            {
                temp.Push(trans.name + "/");
                trans = trans.parent;
            }
            while (temp.Count > 0)
            {
                sb.Append(temp.Pop());
            }
            sb.Append("   SetVerticesDirty");
            print(sb.ToString());
            base.SetVerticesDirty();
        }
        */
        public override string text
        {
            set
            {
                OriginalText = value;
            }
        }
        #endregion
        #region EmojiLink
        private class SpriteTagInfo
        {
            public EmojiInfo emi;
            public int startpos;
            public int lenth;
            public bool commonimg;
        }
        void DrawEmoji()
        {
            Rect re = rectTransform.rect;
            Vector3[] position = { new Vector2(re.xMin,re.yMax),
                                       new Vector2(re.xMax, re.yMax),
                                       new Vector2(re.xMax, re.yMin),
                                       new Vector2(re.xMin,re.yMin) };
            int len = verts.Count;
            Color32 emojicolor = new Color(1.0f, 1.0f, 1.0f, color.a);
            float unitsPerPixel = 1 / pixelsPerUnit_when_start;
            float deta_down = fontSize * 0.15f;
            Vector2 tempuv = new Vector2();
            Vector3 temppos = new Vector2();
            int numemoji = m_Sprite_list.Count;

            for (int image_index = 0; image_index < numemoji; image_index++)
            {
                SpriteTagInfo curtag = m_Sprite_list[image_index];
                int basepos = curtag.startpos * 4;
                if (basepos + 4 > len)
                    break;
                EmojiInfo ei = curtag.emi;

                if (ei == null || ei.name.Equals("toumingguangbiao"))
                    continue;
                Vector2[] squareuv = { new Vector2(ei.startu, ei.startv+ei.vlen),
                                       new Vector2(ei.startu+ei.ulen, ei.startv+ei.vlen),
                                       new Vector2(ei.startu+ei.ulen, ei.startv),
                                       new Vector2(ei.startu, ei.startv) };
                AddIndex(mPositions.Count, 1);
                for (int i = 0; i < 4; i++)
                {
                    if (auto)
                        mPositions.Add(position[i]);
                    else
                    {
                        temppos = verts[basepos + i].position * unitsPerPixel + roundingOffset;
                        temppos.y -= deta_down;
                        mPositions.Add(temppos);
                    }
                    //在uv0.x中放入图像id,并将原值缩半，注意用时恢复。
                    tempuv.x = squareuv[i].x * 0.5f + (ei.frameCount + 1.0f);
                    tempuv.y = squareuv[i].y;
                    if (!curtag.commonimg)
                        tempuv.y += 2.0f;
                    //uv0.y是一个标志。
                    mUVs.Add(tempuv);
                    mColors.Add(emojicolor);
                }
            }
        }
        private class HrefInfo
        {
            public int startpos_linkedtext;
            public int endpos_linkedtext;
            //下面是顶点上的索引。
            public int startIndex;
            public int endIndex;
            public bool bUnderline;
            //下划线所包含的内容。
            public string content;
            public string parameter;
            public Color32 color;//这个颜色指的是此链接所覆盖的首个文字的颜色。
            public readonly List<Rect> boxes = new List<Rect>();
        }
        private string GetLinkedText(string text)
        {
            sb.Length = 0;
            m_HrefInfos.Clear();
            var indexText = 0;
            foreach (Match match in s_HrefRegex.Matches(text))
            {
                sb.Append(text.Substring(indexText, match.Index - indexText));
                bool bUnderLine = true;
                if (match.Groups[1].Value.Equals("noline"))
                    bUnderLine = false;
                string content = match.Groups[2].Value;
                if (content.Length > 0)
                {
                    var hrefInfo = new HrefInfo
                    {
                        startpos_linkedtext = sb.Length,//一次处理后的文本起始索引
                        endpos_linkedtext = sb.Length + content.Length - 1,
                        startIndex = sb.Length * 4, // 相应的顶点索引
                        endIndex = (sb.Length + content.Length - 1) * 4 + 3,
                        bUnderline = bUnderLine,
                        content = content,
                        //在目前的正则表达模式下，组3 4和组2是一样的。
                        parameter = match.Groups[4].Value
                    };
                   
                    if (bUnderLine)
                    {
                        //[unity 2018.4] color标签对应的顶点数据中的color不再保证是<color>标签所指定的值。
                        //startIndex应忽略内容开头处的<color=#xxxxxx>标签。
                        int startIndex = hrefInfo.startIndex;
                        int endIndex = hrefInfo.endIndex;
                        int curpos = 0;
                        int content_lenth = content.Length;
                        while (startIndex + 60 < endIndex 
                            && curpos + 15 < content_lenth
                            && content.Substring(curpos, 8).Equals("<color=#")
                            && content[curpos + 14] == '>')
                        {
                            startIndex += 60;
                            curpos += 15;
                        }
                        hrefInfo.startIndex = startIndex;
                        hrefInfo.endIndex = endIndex;
                    }
                    if (mSpaceReplace)
                    {
                        int numspace = 0;
                        foreach (var x in content)
                            if (x == ' ') numspace++;
                        hrefInfo.endIndex -= numspace * 4;
                    }

                    m_HrefInfos.Add(hrefInfo);
                    sb.Append(content);
                }
                indexText = match.Index + match.Length;
            }
            sb.Append(text.Substring(indexText, text.Length - indexText));
            return sb.ToString();
        }
        void DrawHyperLink()
        {
            //2017年9月15日 13:36:18 使用图片的方式处理下划线。
            //由于下划线使用了图片机制 所以依赖于ImageFont。
            if (ImageFont.instance == null) return;
            var whiteblock = ImageFont.instance.getemojiInfoItem(0, 1010);
            if (whiteblock == null) return;
            byte alpha = ((Color32)color).a;

            Vector2 centerUV = new Vector2((whiteblock.startu + whiteblock.ulen / 2) * 0.5f + 2.0f, whiteblock.startv + whiteblock.vlen / 2);

            // 计算出每个超链接的所有包围盒。
            foreach (var hrefInfo in m_HrefInfos)
            {
                hrefInfo.boxes.Clear();
                if (hrefInfo.endIndex >= mPositions.Count)
                { print("hrefInfo.endIndex >= toFill.currentVertCount"); break; }

                //以该链接的首个字符初始化包围盒。
                hrefInfo.color = mColors[hrefInfo.startIndex + 3];
                hrefInfo.color.a = alpha;
                var lastminpos = mPositions[hrefInfo.startIndex + 3];
                var bounds = new Bounds(lastminpos, Vector3.zero);
                var lastmaxpos = mPositions[hrefInfo.startIndex + 1];
                bounds.Encapsulate(lastmaxpos);

                float max_deta_down = fontSize * 0.3f;
                for (int i = hrefInfo.startIndex + 4; i <= hrefInfo.endIndex; i += 4)
                {
                    //注意以一个字符的矩形区域为单位加入到包围盒中。
                    //TODO 需要合理方式以判断是否发生换行，目前是经验性的。
                    curPosition = mPositions[i + 3];
                    bool b_newline = (curPosition.x < lastmaxpos.x) && mPositions[i + 2].x < lastmaxpos.x
                        && ((lastminpos.y - curPosition.y) > max_deta_down);
                    lastminpos = curPosition;
                    lastmaxpos = mPositions[i + 1];
                    if (b_newline)
                    {
                        // 换行重新添加包围框.注意换行的检测方法
                        //即起点在末点之前，注意顶点自身的排列顺序。
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(lastminpos, Vector3.zero);
                    }
                    else
                        bounds.Encapsulate(lastminpos);
                    bounds.Encapsulate(lastmaxpos);
                }
                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
            }

            //只是更动顶点位置,应用deta_down，然后加入,注意效率,计算不冗余。
            //最末一行的下划线应当进行clamp,以保证它可以显示出来。
            //TODO 最后一行的下划线在靠进下边界时太细。
            float linewidth = fontSize * 0.07f;
            float deta_down = 2;
            foreach (var hrefInfo in m_HrefInfos)
            {
                if (!hrefInfo.bUnderline || hrefInfo.endIndex >= mPositions.Count)
                    continue;
                var nbox = hrefInfo.boxes.Count;
                AddIndex(mPositions.Count, nbox);
                for (int i = 0; i < nbox; i++)
                {
                    var curbox = hrefInfo.boxes[i];
                    float startx = curbox.x, starty = curbox.y, width = curbox.width;
                    float miny = starty - deta_down;
                    //rectTransform.rect确实给出了中心坐标系下的范围。
                    if (miny < rectTransform.rect.yMin)
                        miny = rectTransform.rect.yMin;
                    float maxy = miny + linewidth;
                    mPositions.Add(new Vector3(startx, miny, 0.0f));
                    mPositions.Add(new Vector3(startx + width, miny, 0.0f));
                    mPositions.Add(new Vector3(startx + width, maxy, 0.0f));
                    mPositions.Add(new Vector3(startx, maxy, 0.0f));
                    for (int t = 0; t < 4; t++)
                    {
                        mColors.Add(hrefInfo.color);
                        mUVs.Add(centerUV);
                    }
                }
            }
        }
        #endregion
        #region EventHanle
        public UnityAction<string> OnClick;
        //全局长按回调
        [NonSerialized]
        public static UnityAction<string> OnCopy;
        public static float checktime = 0.4f;
        bool CanLongTap;
        bool HasLongTap;
        static int currentstamp = 0;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || HasLongTap)
                return;
            CanLongTap = false;
            Vector2 lp;
            //eventData.position是屏幕坐标下的位置。
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in m_HrefInfos)
            {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp) && OnClick != null)
                    {
                        OnClick(hrefInfo.parameter);
                        return;
                    }
                }
            }
        }
        public override float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                settings.scaleFactor = pixelsPerUnit_when_start;
                return cachedTextGeneratorForLayout.GetPreferredWidth(m_Text, settings) / pixelsPerUnit_when_start;
            }
        }
        public override float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(rectTransform.rect.size.x, 0.0f));
                settings.scaleFactor = pixelsPerUnit_when_start;
                return cachedTextGeneratorForLayout.GetPreferredHeight(m_Text, settings) / pixelsPerUnit_when_start;
            }
        }
        public bool IsClickHyperLinkContent()
        {
            //使得长按时lua层click处理者不会做处理(它们有这样的约定)。
            if (HasLongTap)
                return true;
            Vector2 lp = Vector2.zero;
            //eventData.position是屏幕坐标下的位置。
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, Input.mousePosition, UIRoot.UICamera, out lp);
            foreach (var hrefInfo in m_HrefInfos)
            {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void LongTap(int stamp)
        {
            if (CanLongTap && stamp == currentstamp)
            {
                HasLongTap = true;
                OnCopy(s_OriginalText);
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            CanLongTap = true;
            HasLongTap = false;
            currentstamp++;
            if (s_copy && OnCopy != null)
            {
                //临时函数包裹当前时间戳 如果过期则拒绝。
                Game.TimerHandler.SetTimeout(() => { this.LongTap(currentstamp); }, checktime, false, false);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //鼠标按下 然后移动则立刻产生Up事件 但未产生click
            CanLongTap = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CanLongTap = false;
        }

        #endregion
        
        #region Core
        
        public static void InitRegex(string tagRegex,string tagRegex2,string hrefRegex)
        {
            m_originTagRegex =new Regex(tagRegex, RegexOptions.Singleline);
            m_originTagRegex2 =new Regex(tagRegex2, RegexOptions.Singleline);
            s_HrefRegex =new Regex(hrefRegex, RegexOptions.Singleline);
        }

        static Regex m_originTagRegex =
        new Regex(@"\[/(.*?)\]", RegexOptions.Singleline);
        static Regex m_originTagRegex2 =
        new Regex(@"(\[/(.*?)\])|( )", RegexOptions.Singleline);
        static Regex s_HrefRegex =
        new Regex(@"\[/http(noline)?_(.*?)(?<!(\[/\d+))_(.*?)\]", RegexOptions.Singleline);
        static int[] mNoramlIndexes = new int[6] { 0, 1, 2, 0, 2, 3 };
        static int[] mTempIndexes = new int[6];
        static StringBuilder sb = new StringBuilder();
        static Vector3 curPosition = new Vector3();
        static List<Vector3> mPositions = new List<Vector3>();
        static List<Color32> mColors = new List<Color32>();
        static List<Vector2> mUVs = new List<Vector2>();
        static List<int> mIndexes = new List<int>();
        static IList<UIVertex> verts;
        static Vector3 roundingOffset;

        List<SpriteTagInfo> m_Sprite_list = new List<SpriteTagInfo>();
        List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

        private void AddIndex(int indexoffset, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int t = 0; t < 6; t++)
                    mTempIndexes[t] = mNoramlIndexes[t] + indexoffset;
                mIndexes.AddRange(mTempIndexes);
                indexoffset += 4;
            }
        }
        private string ParseText()
        {

            m_Sprite_list.Clear();
            string linkedtext = GetLinkedText(OriginalText);

            //先对文本中的超链接进行解析。
            //然后再解析表情标记。
            int lastpos = 0;
            sb.Length = 0;

            //这表明表情集还没有真正设定。
            if (ImageFont.instance == null) return linkedtext;

            int mainpos = 1;
            Regex curreg = m_originTagRegex;
            if (mSpaceReplace)
            {
                curreg = m_originTagRegex2;
                mainpos = 2;
            }

            foreach (Match match in curreg.Matches(linkedtext))
            {
                SpriteTagInfo tempSpriteTag = new SpriteTagInfo();
                int emoji_id = 0;
                float emoji_height = 0;
                float emoji_ratio = 1;
                if (match.Groups.Count > mainpos)
                {
                    string var = match.Groups[mainpos].Value;
                    if (mSpaceReplace&&var.Equals(""))
                    {
                       emoji_ratio = 0.3f;
                       tempSpriteTag.commonimg = true;
                       tempSpriteTag.emi =
                       ImageFont.instance.getemojiInfoItem(1, 1018);
                    }
                    else
                    {
                        string[] sAttry = var.Split('_');
                        //目前对[/*]的规定是先给出id_大小_长宽比。
                        //后两者默认为fontsize与1.
                        //但注意到这使标记不再是固定长度，因此要处理好已有的链接偏移。
                        //tryparse失败后不会写入结果。
                        if (sAttry.Length > 0)
                            int.TryParse(sAttry[0], out emoji_id);
                            //得到id后可以在此处理无效值并设默认ratio.
                        if (s_setId > 0)
                        {
						    int emoji1num = ImageFont.emoji1num;
                            tempSpriteTag.commonimg = emoji_id >= emoji1num;
                            tempSpriteTag.emi = 
                                ImageFont.instance.getemojiInfoItem(emoji_id / emoji1num + 1, emoji_id% emoji1num);
                        }
                        else
                        {
                            tempSpriteTag.commonimg = emoji_id >= 1000;
                            tempSpriteTag.emi =
                                ImageFont.instance.getemojiInfoItem(_setId, emoji_id);
                        }
                        if (tempSpriteTag.emi == null)
                            emoji_ratio = 1;
                        else
                            emoji_ratio = tempSpriteTag.emi.ratio;
                        if (sAttry.Length > 1)
                            float.TryParse(sAttry[1], out emoji_height);
                        if (sAttry.Length > 2)
                            float.TryParse(sAttry[2], out emoji_ratio);
                    }
                }

                sb.Append(linkedtext.Substring(lastpos, match.Index - lastpos));
                tempSpriteTag.startpos = sb.Length;

                lastpos = match.Index + match.Length;

                if (emoji_height < 1) emoji_height = fontSize;
                string imgstr = string.Format("<quad name={0} size={1} width={2} />", "xxx", emoji_height, emoji_ratio);
                tempSpriteTag.lenth = imgstr.Length;
                m_Sprite_list.Add(tempSpriteTag);
                sb.Append(imgstr);

                //在这里要注意到因为[/*_*_*]被替换成更长的形式，每个链接对应的字符区域实际上发生了改变。
                //然而，可以直接使用忽略后的结果。
                int deta = match.Length * 4;
                foreach (var hrefInfo in m_HrefInfos)
                {
                    if (hrefInfo.startpos_linkedtext >= lastpos)
                        hrefInfo.startIndex -= deta;
                    if (hrefInfo.endpos_linkedtext >= lastpos)
                        hrefInfo.endIndex -= deta;
                }
                if (auto) break;
            }
            sb.Append(linkedtext.Substring(lastpos));
            return sb.ToString();
        }
        public string FastTextParse(string OriginalText)
        {
            //此函数依赖于 ImageFont.instance。
            //这个函数并不是用来作文本解析，只是因为需要在真正UI绑定前就得到文本所占区域大小的信息，所以要得到一种在这方面等效的表述。
            //并且，需要效率，不进行与此无关的处理。
            sb.Length = 0;
            int indexText = 0;
            foreach (Match match in s_HrefRegex.Matches(OriginalText))
            {
                sb.Append(OriginalText.Substring(indexText, match.Index - indexText));
                string content = match.Groups[2].Value;
                if (content.Length > 0)
                    sb.Append(content);
                indexText = match.Index + match.Length;
            }
            sb.Append(OriginalText.Substring(indexText, OriginalText.Length - indexText));
            string linkedtext = sb.ToString();

            //先对文本中的超链接进行解析。
            //然后再解析表情标记。
            int lastpos = 0;
            sb.Length = 0;
            //这表明表情集还没有真正设定。
            if (ImageFont.instance == null) return linkedtext;
            Regex curreg = m_originTagRegex;
            int mainpos = 1;
            if(mSpaceReplace)
            {
                curreg = m_originTagRegex2;
                mainpos = 2;
            }
            foreach (Match match in curreg.Matches(linkedtext))
            {
                int emoji_id = 0;
                float emoji_height = 0;
                float emoji_ratio = 1;
                if (match.Groups.Count > mainpos)
                {
                    string var = match.Groups[mainpos].Value;
                    if (mSpaceReplace && var.Equals(""))
                    {
                       emoji_ratio = 0.3f;
                    }
                    else
                    {
                        string[] sAttry = var.Split('_');
                        if (sAttry.Length > 0)
                            int.TryParse(sAttry[0], out emoji_id);
                        var emi = ImageFont.instance.getemojiInfoItem(s_setId, emoji_id);
                        if (emi == null)
                            emoji_ratio = 1;
                        else
                            emoji_ratio = emi.ratio;
                        if (sAttry.Length > 1)
                            float.TryParse(sAttry[1], out emoji_height);
                        if (sAttry.Length > 2)
                            float.TryParse(sAttry[2], out emoji_ratio);
                    }
                }
                if (emoji_height < 1) emoji_height = fontSize;
                sb.Append(linkedtext.Substring(lastpos, match.Index - lastpos));
                lastpos = match.Index + match.Length;
                string imgstr = string.Format("<quad name={0} size={1} width={2} />", "xxx", emoji_height, emoji_ratio);
                sb.Append(imgstr);
            }
            sb.Append(linkedtext.Substring(lastpos));
            return sb.ToString();
        }

        public float GetFirstLineHeight()
        {
            Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            settings.scaleFactor = pixelsPerUnit_when_start;
            cachedTextGenerator.Populate(text, settings);
            UILineInfo[] t = cachedTextGenerator.GetLinesArray();
            if (t.Length > 0)
            {
                float height = t[0].height/ pixelsPerUnit_when_start;
                return height;
            }
            return 0f;
        }

        private void DrawText()
        {
            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            settings.scaleFactor = pixelsPerUnit_when_start;
            cachedTextGenerator.Populate(text, settings);
            verts = cachedTextGenerator.verts;
            int lenth = verts.Count - 4;
            Rect inputRect = rectTransform.rect;
            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
            refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);
            // Determine fraction of pixel to offset text mesh.
            Vector2 _roundingOffset = PixelAdjustPoint(refPoint) - refPoint;
            roundingOffset = new Vector3(_roundingOffset.x, _roundingOffset.y, 0);
            // Apply the offset to the vertices
            float unitsPerPixel = 1 / pixelsPerUnit_when_start;
            Color32 gcolor = (Color32)color;
            byte alpha = gcolor.a;

            int leappos = -1;   //在何处跳跃
            int leapsteps = 0;  //跳跃的长度
            int emojipos = -1;  //当次跳跃对应的表情位置
            for (int i = -1; i < lenth; ++i)
            {
                if (i == leappos)
                {
                    i += leapsteps;
                    emojipos++;
                    if (emojipos < m_Sprite_list.Count)
                    {
                        leappos = m_Sprite_list[emojipos].startpos * 4;
                        leapsteps = m_Sprite_list[emojipos].lenth * 4 - 1;
                    }
                    continue;
                }
                var currentvert = verts[i];
                mPositions.Add(currentvert.position * unitsPerPixel + roundingOffset);
                //采用color属性配置的文字alpha不会使用全局alpha.
                currentvert.color.a = alpha;
                mColors.Add(s_input ? gcolor : currentvert.color);
                mUVs.Add(currentvert.uv0);
            }
            AddIndex(0, mPositions.Count / 4);
            m_DisableFontTextureRebuiltCallback = false;
        }
        protected override void UpdateGeometry()
        {
            /*
            var components = GetComponents<IMeshModifier>();
            components[0].ModifyMesh(s_VertexHelper);
            Type t = typeof(VertexHelper);
            FieldInfo fi = t.GetField("m_Uv2S", 
            BindingFlags.NonPublic | BindingFlags.Instance);
            uv2 = fi.GetValue(s_VertexHelper) as List<Vector2>;*/

            if (!(m_OriginText.Length > 0 && rectTransform != null && rectTransform.rect.width >= 0
                && rectTransform.rect.height >= 0 && font != null))
            {
                workerMesh.Clear();
                //这种情况下不需要渲染。
                //字体为空的时候可以不渲染，因为字体稍后就会加载出来。
                canvasRenderer.SetMesh(workerMesh);
                return;
            }
            mPositions.Clear();
            mColors.Clear();
            mUVs.Clear();
            mIndexes.Clear();

            DrawText();
            if (!s_input)
            {
                DrawHyperLink();
                DrawEmoji();
            }
            verts = null;
            workerMesh.Clear();
            if (mPositions.Count < 65000)
            {
                //也许顶点数会超限
                workerMesh.SetVertices(mPositions);
                workerMesh.SetColors(mColors);
                workerMesh.SetUVs(0, mUVs);
                workerMesh.SetTriangles(mIndexes, 0);
                workerMesh.RecalculateBounds();
            }
            canvasRenderer.SetMesh(workerMesh);
        }
        
#endregion

    }
}