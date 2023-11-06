using UnityEngine;
using UnityEngine.UI;


public class MovieCtrl : MonoBehaviour
{
    public static MovieCtrl Instance;

    public Button dance;
    public Button shame;
    public Button sayHello;
    public Button idle;
    public Button explanation;
    public Animator animator;


    float nowAnimTime = 0;
    string nowAnim = "";

    public GameObject model1;
    public GameObject model2;

    float lastAnimTiem = 0;

    public AudioSource audioSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (model1.activeInHierarchy == true)
        {
            dance.onClick.AddListener(delegate
            {
                DHStop();
                DictationEngine.Instance.ChangeTextColor();
                audioSource.Play();
                nowAnim = "Dance";
                lastAnimTiem = 41.533f;
                animator.SetBool("Dance", true);
                //animator.SetInteger("Animator", 1);
            });
            shame.onClick.AddListener(delegate
            {
                DHStop();
                DictationEngine.Instance.ChangeTextColor();
                nowAnim = "Shame";
                lastAnimTiem = 3.33f;
                animator.SetBool("Shame", true);
            });
            sayHello.onClick.AddListener(delegate
            {
                DHStop();
                DictationEngine.Instance.ChangeTextColor();
                SpeakPrintFunc.Instance.speech.Text = "您好，我是水利数字人小泰，很高兴为您服务！";
                if (SpeakPrintFunc.Instance.speech.Text != null)
                {
                    SpeakPrintFunc.Instance.speech.Speak();
                }
                ContentManage.Instance.AddLIneText(DictationEngine.robotName + "：", "您好，我是水利数字人小泰，很高兴为您服务！", true);
                nowAnim = "SayHello";
                lastAnimTiem = 26.867f;
                //animator.SetInteger("Animator", 2);
                animator.SetBool("SayHello", true);

            });
            idle.onClick.AddListener(delegate
            {
                DictationEngine.Instance.ChangeTextColor();
                DHStop();
                nowAnim = "Idle";
                lastAnimTiem = 26.867f;
                animator.SetBool("Idle", true);
            });
            explanation.onClick.AddListener(delegate
            {
                if (model1.activeInHierarchy == true)
                {
                    model1.SetActive(false);
                    model2.SetActive(true);
                }
                else
                {
                    model2.SetActive(false);
                    model1.SetActive(true);
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (nowAnim!=""&&lastAnimTiem!=0)
        {
            nowAnimTime += Time.deltaTime;
            if (nowAnimTime>= lastAnimTiem)
            {

                DHStop();
                Debug.Log(nowAnim+"播放结束");
                nowAnim = "";
                lastAnimTiem = 0;
                nowAnimTime = 0;
                if(nowAnim == "Dance")
                {
                    audioSource.Stop();
                }
            }
        }
    }  
    

    public void DHStop() 
    {
        
        audioSource.Stop();

        animator.SetBool("Dance", false);
        animator.SetBool("Shame", false);
        animator.SetBool("SayHello", false);
        animator.SetBool("Facetoface", false);
        animator.SetBool("Idle", false);
    }
}
