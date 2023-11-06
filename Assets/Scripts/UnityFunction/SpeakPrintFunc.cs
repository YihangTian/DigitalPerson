using Crosstales.RTVoice.Tool;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeakPrintFunc : MonoBehaviour
{
    public static SpeakPrintFunc Instance;

    public SpeechText speech;

    public static bool DisplayText = false;
    public static bool isChatGPT = false;
    public static bool isBadou = false;
    public static bool canTalking = false;
    public static bool startTalking = false;
    public static bool isSpeaking = false;

    public static bool sayHello = true;
    public static bool isThinking = false;
    public static bool isSpecialWord = true;
    public static bool isTalking = false;

    public static List<string> speakList = new List<string>();

    static string answertext = null;

    private string[] ellips = new string[] { "", ".", "..", "..." };


    //speakList = new List<string> { "河海大学是中国最早开","设水利类专业的高校，以","水利、工农业产业技术","和管理科学为优势和特","色，各类学科协调发展","。" };
    //speakList = new List<string> { "河海大学是中国最早开设水利类专业的高校，", "以水利、工农业产业技术", "和管理科学为优势和特色，", "各类学科协调发展。"};

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        speech.OnSpeechTextComplete += Speech_OnSpeechTextComplete;
        speech.OnSpeechTextStart += Speech_OnStartSpeek;
    }

    private void Update()
    {
        if (DisplayText)
        {
            StopCoroutine("Think_Animation");
            ContentManage.Instance.AddLIneText(DictationEngine.robotName + "：", answertext, false);
        }
        if (canTalking)
        {
            if (speakList != null && speakList.Count > 0)
            {
                speech.Text = speakList[0];

                if (speech.Text != null)
                {
                    speech.Speak();
                    startTalking = true;
                    speakList.RemoveAt(0);
                    canTalking = false;
                    sayHello = false;
                    DictationEngine.Instance.AwakeStart();
                }
            }
        }
    }


    /// <summary>
    /// print event user
    /// </summary>
    /// <param name="text"></param>
    public void OnPrintText(string text)
    {
        answertext = text;
    }

    /// <summary>
    /// start speak event
    /// </summary>
    private void Speech_OnStartSpeek()
    {
        //if (!DictationEngine.m_PhraseRecognizer.IsRunning)
        //{
        //    DictationEngine.m_PhraseRecognizer.Start();
        //}
        if (!sayHello)
        {
            MovieCtrl.Instance.animator.SetBool("Facetoface", true);
        }
        if (!DictationEngine.isAwake)
        {
            isTalking = true;
            DictationEngine.isDictaion = false;
        }
    }

    /// <summary>
    /// complete speak event
    /// </summary>
    private void Speech_OnSpeechTextComplete()
    {
        if (isChatGPT || isBadou)
        {
            if (speech.Text != null && speakList.Count > 0)
            {
                speech.Text = speakList[0];
                speech.Speak();
                speakList.RemoveAt(0);
            }
            else
            {
                Debug.Log("说完话后执行了");
                DictationEngine.Instance.ReadyToStartPhrase();
                Thread.Sleep(100);
                DictationEngine.Instance.AwakeStart();
                ContentManage.Instance.AddLIneText(DictationEngine.robotName + "：", answertext, true);
                speech.Text = null;
                isChatGPT = false;
                isBadou = false;
                isTalking = false;
                MovieCtrl.Instance.DHStop();
            }
        }
        else
        {
            MovieCtrl.Instance.DHStop();
        }
    }

    /// <summary>
    /// from API getAnswer(BaDuo and XingHuo)
    /// </summary>
    public void Replay_AnswerSpeak()
    {
        DictationEngine.StopDictation();
        speakList = new List<string>();
        ContentManage.Instance.AddLIneText("我：", DictationEngine.question, true);
        StartCoroutine("Think_Animation");
        AnwserSelection.GetAnswer(DictationEngine.question, answer =>
        {
            DictationEngine.Instance.AwakeStart();
            isSpecialWord = false;
            StopCoroutine("Think_Animation");
            isThinking = false;
            sayHello = false;
        });
    }

    /// <summary>
    /// Think Text Animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator Think_Animation()
    {
        isThinking = true;
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            ContentManage.Instance.AddLIneText(DictationEngine.robotName + "：", ellips[i], false);
            i = (i + 1) % ellips.Length;
        }
    }
}
