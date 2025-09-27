using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game;
using Game.Core;

public class UIEffectBlack : MonoBehaviour
{
    public bool ClickClose = true;
    public bool Transparent = false;
    static List<UIEffectBlack> lastShowList = new List<UIEffectBlack>();

    Image _blackImg = null;
    public Image blackImg
    {
        get
        {
            if (_blackImg == null)
            {
                _blackImg = new GameObject("blackImage").AddComponent<Image>();
                _blackImg.color = new Color(0, 0, 0, Transparent ? 0 : 0.82f);

                Button btn = _blackImg.gameObject.AddComponent<Button>();
            }
            return _blackImg;
        }
    }
    
    bool init = true;
    void OnInit()
    {
        init = false;
    }

    void OnDestroy()
    {
        Destroy(_blackImg);
    }

    void OnEnable()
    {
        if (init) OnInit();
        if (ClickClose) EventTriggerListener.Get(blackImg.gameObject).onClick += OnCloseSelf;
        ShowBlack();
    }

    void OnDisable()
    {
        if (ClickClose) EventTriggerListener.Get(blackImg.gameObject).onClick -= OnCloseSelf;
    }

    void OnCloseSelf(GameObject go)
    {
        this.gameObject.SetActive(false);
    }

    void ShowBlack()
    {
        RectTransform rectTransform = blackImg.rectTransform;
        rectTransform.SetParent(this.gameObject.transform);
        rectTransform.SetSiblingIndex(0);
        rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        rectTransform.position = new Vector3(0, 0, 0);
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0);
        Vector2 v2 = rectTransform.GetComponentInParent<CanvasScaler>().referenceResolution;
        float s1 = v2.x / v2.y;
        float s2 = 1.0f * Screen.width / Screen.height;
        if (s1 < s2)
            v2.x = (int)Mathf.FloorToInt(v2.y * s2);
        else if (s1 > s2)
            v2.y = (int)Mathf.FloorToInt(v2.x / s2);
        rectTransform.sizeDelta = new Vector2(v2.x, v2.y);
    }

}
