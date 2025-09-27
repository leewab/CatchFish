using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

class UITypeWriter: MonoBehaviour
{
    struct CharInfo
    {
        public string mCharStr;
    }
    Text mTextLabel;

    int mFirstShowCharCount = 0;

    float mShowSpeed = 0.1f;

    float mLeftShowNextCharTime = 0;

    string mFullText;

    public float ShowSpeed
    {
        get { return mShowSpeed; }
        set { mShowSpeed = value; }
    }

    public int FirstShowCharCount
    {
        get { return mFirstShowCharCount; }
        set { mFirstShowCharCount = value; }
    }
    List<CharInfo> mLeftToShowCharList = new List<CharInfo>(); 
    void Awake()
    {
        mTextLabel = GetComponent<Text>();
    }

    void Start()
    {

    }


    public bool IsShowTextFinished()
    {
        return mLeftToShowCharList.Count == 0;
    }

    public void FinishShowText()
    {
        mLeftToShowCharList.Clear();
        mTextLabel.text = mFullText;
        enabled = false;
    }


    public void BeginShowText(string str)
    {
        enabled = true;
        mFullText = str;
        mTextLabel.text = "";
        string firstStr = "";
        mLeftToShowCharList.Clear();
        if (mFirstShowCharCount >= mFullText.Length)
        {
            mTextLabel.text = str;
            mLeftToShowCharList.Clear();
            enabled = false;
            return;
        }
        ParseFullTextToChar();
        for (int i = 0; i< mFirstShowCharCount;++i)
        {
            firstStr += mLeftToShowCharList[i].mCharStr;
        }
        mLeftToShowCharList.RemoveRange(0, mFirstShowCharCount);
        mTextLabel.text = firstStr;
        mLeftShowNextCharTime = mShowSpeed;
    }

    void ParseFullTextToChar()
    {
        mLeftToShowCharList.Clear();
        int count = mFullText.Length;
        int curIndex = 0;
        string curText = mFullText;
        List<string> colorList = new List<string>();
        while (curIndex < count)
        {
            if(string.Compare(mFullText, curIndex, "<color=", 0, 7) == 0)
            {
                int endIndex = mFullText.IndexOf('>', curIndex);

                colorList.Add(mFullText.Substring(curIndex, endIndex - curIndex + 1));
                curIndex = endIndex+1;
                continue;
            }
            if (string.Compare(mFullText, curIndex, "</color>", 0, 8) == 0)
            {
                int endIndex = mFullText.IndexOf('>', curIndex);

                if(colorList.Count > 0)
                {
                    colorList.RemoveAt(colorList.Count - 1);
                }
                curIndex = curIndex + 8;
                continue;
            }
            char c = mFullText[curIndex];
            CharInfo tInfo = new CharInfo();
            if(colorList.Count == 0)
            {
                tInfo.mCharStr = c.ToString();
            }
            else
            {
                tInfo.mCharStr = colorList[colorList.Count - 1] + c.ToString() + "</color>";
            }
            
            mLeftToShowCharList.Add(tInfo);
            curIndex++;
        }
    }


    void Update()
    {
        if(mLeftToShowCharList.Count == 0)
        {
            enabled = false;
            return;
        }
        mLeftShowNextCharTime -= Time.deltaTime;
        if(mLeftShowNextCharTime > 0f)
        {
            return;
        }
        mLeftShowNextCharTime = mShowSpeed;
        string c = mLeftToShowCharList[0].mCharStr;
        mLeftToShowCharList.RemoveAt(0);
        string curText = mTextLabel.text + c;
        mTextLabel.text = curText;
    }
}

