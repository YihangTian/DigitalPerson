using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectParameter : MonoBehaviour
{
    public static ScrollRect scrollrect;
    public static float contentYSize;

    public static RectTransform content;
    // Start is called before the first frame update
    void Start()
    {
        scrollrect = GetComponent<ScrollRect>();
        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
