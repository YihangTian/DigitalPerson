using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftChat : MonoBehaviour
{
    public GameObject LeftBubble;

    private RectTransform head_Image;
    private RectTransform content_Text;
    private RectTransform bubble_Image;
    private float HeadHight;
    private float headNameSizeY;
    public RectTransform content;

    public TMP_InputField inputField;

    public ScrollRect scrollrect;
    private float lastPos;
    public bool isadd;
    public GameObject newBubble;
    public float bubbleMaxWidth;
    private void Start()
    {
        bubbleMaxWidth = 600;
        HeadHight = 50;
        headNameSizeY = 25;
        //Line_height = LeftBubble.transform.Find("Bubble").GetComponent<GridLayoutGroup>().cellSize.y;
        //Debug.Log(Line_height);
    }

    public void AddLine(string content, bool isAdd)
    {

        if (isAdd)
        {
            newBubble = Instantiate(LeftBubble, ScrollRectParameter.content);
            Vector3 newBubblePos = newBubble.GetComponent<Transform>().localPosition;
            newBubble.GetComponent<Transform>().localPosition = new Vector3(newBubblePos.x, newBubblePos.y - ScrollRectParameter.content.sizeDelta.y, newBubblePos.z);
            print(newBubble.transform.localPosition);

            bubble_Image = newBubble.transform.Find("Bubble").GetComponent<RectTransform>();
            head_Image = newBubble.transform.Find("Head").GetComponent<RectTransform>();
            content_Text = bubble_Image.GetComponentInChildren<TextMeshProUGUI>().rectTransform;

            content_Text.GetComponent<TextMeshProUGUI>().text = content;

            if (content_Text.GetComponent<TextMeshProUGUI>().preferredWidth > bubbleMaxWidth)
            {
                content_Text.GetComponent<LayoutElement>().preferredWidth = bubbleMaxWidth;
            }

            //newBubblePos = newBubble.GetComponent<Transform>().localPosition;

            Debug.Log(bubble_Image.localPosition);
            bubble_Image.localPosition = new Vector3(bubble_Image.localPosition.x, head_Image.localPosition.y - HeadHight, bubble_Image.localPosition.z);
            Debug.Log(bubble_Image.localPosition);

            LayoutRebuilder.ForceRebuildLayoutImmediate(content_Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubble_Image);
            lastPos = ScrollRectParameter.content.sizeDelta.y;
            ScrollRectParameter.content.sizeDelta = new Vector2(ScrollRectParameter.content.sizeDelta.x, ScrollRectParameter.content.sizeDelta.y + bubble_Image.sizeDelta.y + headNameSizeY);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
        }
        else
        {
            content_Text.GetComponent<TextMeshProUGUI>().text = content;
            LayoutRebuilder.ForceRebuildLayoutImmediate(content_Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubble_Image);
            ScrollRectParameter.content.sizeDelta = new Vector2(ScrollRectParameter.content.sizeDelta.x, lastPos + bubble_Image.sizeDelta.y + headNameSizeY);
        }
        ScrollRectParameter.scrollrect.verticalNormalizedPosition = 0;
    }

    public void TestInput()
    {
        AddLine(inputField.text, isadd);
    }
}
