namespace UnityEngine.UI
{
    /// <summary>
    /// 循环滚动，在向上补充项目或向下补充项目后，能正常的继续滑动，而不出现滑动跳动反复的情形
    /// </summary>
    [AddComponentMenu("UI/LoopScrollRect", 40)]
    [RequireComponent(typeof(RectTransform))]

    public class LoopScrollRect : ScrollRect
    {
        public void SetOffsetAfterInsert(Vector2 offset)
        {
            m_ContentStartPosition += offset;
        }
    }
}
