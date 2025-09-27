using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockEffectController : MonoBehaviour
{
    // private GameObject zhuozi;
    private GameObject[] gos;
    private int playQuene = 0;
    public float delayTime = 0.87f;
    private float currentTime = 0.0f;
    public float scale = 1.0f;
    public int layer = 15;
    Transform mTransform;
    public int renderQueue = 3100;
    private List<GameObject> otherAnimations = new List<GameObject>();
    private bool hasOtherAnimation = false;
    // Use this for initialization
    void Start()
    {
        mTransform = transform;
        gameObject.transform.localScale *= scale;
        gos = new GameObject[mTransform.childCount];

        for (int i = 0; i < mTransform.childCount; ++i)
        {
            gos[i] = mTransform.GetChild(i).gameObject;
            gos[i].SetActive(false);
            gos[i].layer = layer;
            if (gos[i].GetComponent<MeshRenderer>() != null)
                gos[i].GetComponent<MeshRenderer>().material.renderQueue = renderQueue + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime < delayTime)
        {
            currentTime += Time.deltaTime;
            return;
        }
        else
        {
            if (playQuene == 0)
            {
                gos[0].SetActive(true);
                gos[0].GetComponent<Animation>().playAutomatically = true;
                if (hasOtherAnimation)
                    for (int i = 0; i < otherAnimations.Count; ++i)
                    {
                        otherAnimations[i].SetActive(true);
                        otherAnimations[i].GetComponent<Animation>().Play();
                    }
            }
        }
        if (playQuene == mTransform.childCount)
            return;
        if (gos[playQuene].GetComponent<Animation>().playAutomatically && !gos[playQuene].GetComponent<Animation>().isPlaying)
        {
            gos[playQuene].GetComponent<Animation>().playAutomatically = false;
            gos[playQuene].SetActive(false);
            if (++playQuene == mTransform.childCount)
            {
                gos[mTransform.childCount - 1].SetActive(true);
                return;
            }
            gos[playQuene].SetActive(true);
            gos[playQuene].GetComponent<Animation>().playAutomatically = true;
            gos[playQuene].GetComponent<Animation>().Play();
        }
    }
    /// <summary>
    /// 重置动画
    /// </summary>
    public void Reset()
    {
        for (int i = 0; i < mTransform.childCount; ++i)
        {
            gos[i].SetActive(false);
            gos[i].GetComponent<Animation>().playAutomatically = false;
        }
        if (mTransform.childCount > 0)
            gos[0].GetComponent<Animation>().playAutomatically = true;
        playQuene = 0;
        currentTime = 0.0f;
        if (hasOtherAnimation)
            for (int i = 0; i < otherAnimations.Count; ++i)
                otherAnimations[i].SetActive(false);
    }
    /// <summary>
    /// 关闭播放动画画面时需要调用
    /// </summary>
    public void Close()
    {
        for (int i = 0; i < gos.Length; ++i)
            gos[i].SetActive(false);
        if (hasOtherAnimation)
            for (int i = 0; i < otherAnimations.Count; ++i)
                otherAnimations[i].SetActive(false);
    }
    /// <summary>
    /// 强制播放下一个动画
    /// </summary>
    public void NextAnimation()
    {
        if (playQuene != gos.Length - 1)
            ++playQuene;
    }
    /// <summary>
    /// 查找其他动画
    /// </summary>
    public void FindOtherAnimation(string prefabName)
    {
        GameObject otherAnimation = GameObject.Find(prefabName);
        if (otherAnimation != null)
        {
            hasOtherAnimation = true;
            otherAnimation.SetActive(false);
            otherAnimation.GetComponent<MeshRenderer>().material.renderQueue = renderQueue;
            otherAnimation.layer = layer;
            otherAnimation.transform.localScale *= scale;
            otherAnimations.Add(otherAnimation);
        }
    }
}