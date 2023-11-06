using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentManage : MonoBehaviour
{
    public Scrollbar Vtscrollbar;
    public ScrollRect scrollRect;
    public GameObject contentParent;
    public TextMeshProUGUI contentText;
    public static List<TextMeshProUGUI> contentTexts;
    int index;

    public static ContentManage Instance;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    private void Start()
    {
        contentTexts = new List<TextMeshProUGUI>() { contentText };
        index = 0;
    }

    /// <summary>
    /// add line 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="text"></param>
    /// <param name="Add"></param>
    public void AddLIneText(string name, string text, bool Add)
    {
        scrollRect.verticalNormalizedPosition = 0;//使滑动条滚轮在最下方
        if (text != null)
        {
            contentTexts[index].text = name + text;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentTexts[index].transform);
            if (Add)
            {
                index++;
                contentTexts.Add(Instantiate(contentText, contentParent.transform));
                contentTexts[index].text = null;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.transform);
        scrollRect.verticalNormalizedPosition = 0;//使滑动条滚轮在最下方
    }

    void ClearLineText()
    {
        for (int i = 0; i < contentTexts.Count - 1; i++)
        {
            contentTexts.RemoveAt(i);
            contentParent.transform.GetChild(i);
        }
        index = 0;

    }
}
