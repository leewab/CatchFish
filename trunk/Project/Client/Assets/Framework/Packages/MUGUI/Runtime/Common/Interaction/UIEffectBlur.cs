using UnityEngine;
using System.Collections.Generic;

public class UIEffectBlur : MonoBehaviour
{
    static List<BoxBlurSprite> G_ShowBlurSpriteStack = new List<BoxBlurSprite>();

    BoxBlurSprite boxBlurSprite = null;
    bool init = true;
    void OnInit()
    {
        init = false;
        GameObject blurImage = new GameObject("blurImage");
        blurImage.transform.parent = this.gameObject.transform;
        blurImage.transform.SetAsLastSibling();
        boxBlurSprite = blurImage.AddComponent<BoxBlurSprite>();
        boxBlurSprite.raycastTarget = false;
        RectTransform rectTransform = boxBlurSprite.rectTransform;
        rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        rectTransform.position = new Vector3(0, 0, 0);
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0);
    }

}
