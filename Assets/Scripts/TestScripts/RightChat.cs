using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightChat : MonoBehaviour
{

    public GameObject RightBubble;

    private RectTransform head_Image;
    private RectTransform content_Text;
    private RectTransform bubble_Image;

    public TMP_InputField inputField;

    public GameObject newBubble;

    private float HeadHight;
    private float lastPos;

    public bool isadd;

    public float bubbleMaxWidth;
    public float Line_height;

    private void Start()
    {
        bubbleMaxWidth = 600;
        //Line_height = LeftBubble.transform.Find("Bubble").GetComponent<GridLayoutGroup>().cellSize.y;
        //Debug.Log(Line_height);
    }

    public void AddLine(string content, bool isAdd)
    {

        if (isAdd)
        {
            newBubble = Instantiate(RightBubble, ScrollRectParameter.content);
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

            newBubblePos = newBubble.GetComponent<Transform>().localPosition;

            bubble_Image.localPosition = new Vector3(bubble_Image.localPosition.x, head_Image.localPosition.y - HeadHight, bubble_Image.localPosition.z);

            LayoutRebuilder.ForceRebuildLayoutImmediate(content_Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubble_Image);
            lastPos = ScrollRectParameter.content.sizeDelta.y;
            ScrollRectParameter.content.sizeDelta = new Vector2(ScrollRectParameter.content.sizeDelta.x, ScrollRectParameter.content.sizeDelta.y + bubble_Image.sizeDelta.y + head_Image.sizeDelta.y);

            //LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
        }
        else
        {
            content_Text.GetComponent<TextMeshProUGUI>().text = content;
            LayoutRebuilder.ForceRebuildLayoutImmediate(content_Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubble_Image);
            ScrollRectParameter.content.sizeDelta = new Vector2(ScrollRectParameter.content.sizeDelta.x, lastPos + bubble_Image.sizeDelta.y + head_Image.sizeDelta.y);
        }
        ScrollRectParameter.scrollrect.verticalNormalizedPosition = 0;
    }

    public void TestInput()
    {
        bool input = Random.Range(0, 2) == 0 ? false : true;
        AddLine(inputField.text, isadd);
    }
}
