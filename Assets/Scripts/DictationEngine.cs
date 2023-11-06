using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using Crosstales.RTVoice.Tool;
using System.Collections;
using System.Threading;
using System;
using Unity.VisualScripting;

public class DictationEngine : MonoBehaviour
{
    /// <summary>
    /// Singleton pattern
    /// </summary>
    public static DictationEngine Instance;

    /// <summary>
    /// the text of button
    /// </summary>
    [SerializeField]
    TextMeshProUGUI BTText;

    /// <summary>
    /// Dictation Recognizer class
    /// </summary>
    public static DictationRecognizer m_DictationRecognizer;

    /// <summary>
    /// Phrase Recognizer class
    /// </summary>
    public static PhraseRecognizer m_PhraseRecognizer;

    /// <summary>
    /// the model of person
    /// </summary>
    public GameObject an1;
    public GameObject an1_1;
    public GameObject an2;
    public GameObject an2_1;

    /// <summary>
    /// question record to collect the question from digital person
    /// </summary>
    public static string question = null;

    /// <summary>
    /// in dictation mode
    /// </summary>
    public static bool isDictaion = false;

    /// <summary>
    /// in Awake/Phrase mode
    /// </summary>
    public static bool isAwake = true;

    /// <summary>
    /// robot Name
    /// </summary>
    public const string robotName = "小泰";

    /// <summary>
    /// Awake Words control
    /// </summary>
    string[] keywords = new string[] { "小泰小泰", "小泰", "停" };

    private void Awake()
    {
        if(Instance == null) Instance = this;
        m_DictationRecognizer = new DictationRecognizer();
        m_PhraseRecognizer = new KeywordRecognizer(keywords);
    }

    #region PhrasePart

    /// <summary>
    /// awake dictation
    /// </summary>
    /// <param name="args">event param</param>
    private void PhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);
        if (args.text.Contains(robotName))
        {
            SpeakPrintFunc.Instance.speech.Silence(); // stop talking
            MovieCtrl.Instance.DHStop(); //stop person behave

            SpeakPrintFunc.Instance.speech.Text = "我在呢！"; //put text in speaksystem
            if (SpeakPrintFunc.Instance.speech.Text != null)
            {
                SpeakPrintFunc.Instance.speech.Speak(); // startspeak
            }
            ContentManage.Instance.AddLIneText("我：", args.text, true); // add text at UI
            MovieCtrl.Instance.animator.SetBool("SayHello", true); // start greating
            ContentManage.Instance.AddLIneText(robotName + "：", "我在呢！", true); // add text at UI 

            Phrase2Dictation();

            isDictaion = true;
            isAwake = false;
            SpeakPrintFunc.isSpecialWord = true;
            SpeakPrintFunc.DisplayText = false;

            //Thread.Sleep(100); // have question on this, need or needn't to sleep 100 seconds
            DictationStart(); //start dictation mode
        }
        else if (args.text.Contains("停"))
        {
            SpeakPrintFunc.Instance.speech.Silence();
            MovieCtrl.Instance.DHStop();
            isAwake = true;
            isDictaion = false;
            SpeakPrintFunc.isTalking = false;
            SpeakPrintFunc.isSpecialWord = true;
            SpeakPrintFunc.DisplayText = false;
        }
      
    }

    /// <summary>
    /// only use in awake mode
    /// to close phrasemode open dictation mode
    /// </summary>
    void Phrase2Dictation()
    {
        if (isAwake)
        {
            m_PhraseRecognizer.OnPhraseRecognized -= PhraseRecognized;
            StopPhrase();
        }
    }
    #endregion

    #region DictationPart
    /// <summary>
    /// start dictation mode
    /// </summary>
    void DictationStart()
    {
        Debug.Log("开始识别");
        m_DictationRecognizer.DictationResult += DictationGetResult;
        m_DictationRecognizer.DictationComplete += DictationGetCompleted;
        m_DictationRecognizer.DictationHypothesis += DictationGetHypothesis;
        try
        {
            if (m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                m_DictationRecognizer.Start();
            }
        }
        catch
        {
            if (m_PhraseRecognizer.IsRunning)
            {
                PhraseRecognitionSystem.Shutdown();
                m_PhraseRecognizer.Stop();
            }
            if(m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                m_DictationRecognizer.Start();
            }
        }

    }

    /// <summary>
    /// get dictation text and start think mode
    /// </summary>
    /// <param name="text"></param>
    /// <param name="confidence"></param>
    public void DictationGetResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log(text);
        ReadyToStartPhrase();

        question = text;
        SpeakPrintFunc.Instance.Replay_AnswerSpeak();
    }

    /// <summary>
    /// 识别假想内容方法
    /// </summary>
    /// <param name="text"></param>
    private void DictationGetHypothesis(string text)
    {
        Debug.Log(text);
        if (isDictaion && !SpeakPrintFunc.isTalking)
        {
            ContentManage.Instance.AddLIneText("我：", text, false);
        }
    }

    /// <summary>
    /// 识别完成方法
    /// </summary>
    /// <param name="completionCause"></param>
    public void DictationGetCompleted(DictationCompletionCause completionCause)
    {
        if (completionCause == DictationCompletionCause.Complete)
        {
            StopDictation();
        }
        else
        {
            if (isDictaion)
            {
                if (completionCause != DictationCompletionCause.Complete)
                {
                    //Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
                    Debug.Log("开始识别");
                    StopDictation();
                    if (m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
                    {
                        m_DictationRecognizer.Start();
                    }
                }
                else
                {
                    StopDictation();
                    if (m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
                    {
                        m_DictationRecognizer.Start();
                    }
                    //Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
                }
            }
        }

    }
    #endregion


    /* --------------- KeepPhrase 和 ChangeInit 一起使用可以保证开启语言唤醒功能-------------- */
    public void ReadyToStartPhrase()
    {
        StopDictation();
        ChangeInit();
    }

    /// <summary>
    /// Delete all event method
    /// </summary>
    void ChangeInit()
    {
        /* 差了下面这三行就运行不了了， 真神奇 */
        isAwake = true;
        isDictaion = false;
        StopPhrase();
        /* 差了下面这三行就运行不了了， 真神奇 */

        m_PhraseRecognizer.OnPhraseRecognized -= PhraseRecognized;
        m_DictationRecognizer.DictationResult -= DictationGetResult;
        m_DictationRecognizer.DictationComplete -= DictationGetCompleted;
        m_DictationRecognizer.DictationHypothesis -= DictationGetHypothesis;
    }

    /* --------------- KeepPhrase 和 ChangeInit 一起使用可以保证开启语言唤醒功能-------------- */


    /* --------------- 按钮切换语言输出状态，按下可以直接关闭其他功能  ------------------------- */

    /// <summary>
    /// Voice Awake Function
    /// </summary>
    /// <returns></returns>
    public void AwakeStart()
    {
        StopDictation(); // need to quit dictation mode
        if (!m_PhraseRecognizer.IsRunning)
        {
            m_PhraseRecognizer.OnPhraseRecognized += PhraseRecognized;
            try
            {
                StartPhrase();
            }
            catch
            {

                StartPhrase();
            }
        }
        Debug.Log("开始唤醒：" + m_PhraseRecognizer.IsRunning);
        isDictaion = false;
        isAwake = true;
    }

    /// <summary>
    /// change dictation and phrase
    /// </summary>
    public void ChangedDicStateBT()
    {
        StopAllCoroutines();
        SpeakPrintFunc.isThinking = false;
        SpeakPrintFunc.DisplayText = false;
        if (an1.activeInHierarchy == true)
        {
            MovieCtrl.Instance.DHStop();
            if (!m_PhraseRecognizer.IsRunning && m_DictationRecognizer.Status == SpeechSystemStatus.Stopped && BTText.color == Color.white)
            {
                ChangeTextColor();
                AwakeStart();
                isDictaion = true;
                BTText.color = Color.green;
            }
            else if (m_PhraseRecognizer.IsRunning)
            {
                BTText.color = Color.white;
                InitPhrase();
            }
            else
            {
                StopDictation();
                InitPhrase();
                BTText.color = Color.white;
            }
        }
    }
    /// <summary>
    /// when change state init the engine and event
    /// </summary>
    public void InitPhrase()
    {
        StopCoroutine("Think_Animation");
        SpeakPrintFunc.DisplayText = false;
        isAwake = true;
        isDictaion = false;
        SpeakPrintFunc.Instance.speech.Silence();
        StopPhrase();
        m_PhraseRecognizer.OnPhraseRecognized -= PhraseRecognized;
        m_DictationRecognizer.DictationResult -= DictationGetResult;
        m_DictationRecognizer.DictationComplete -= DictationGetCompleted;
        m_DictationRecognizer.DictationHypothesis -= DictationGetHypothesis;
    }

    void StartPhrase()
    {
        if (!m_PhraseRecognizer.IsRunning)
        {
            PhraseRecognitionSystem.Restart();
            if (!m_PhraseRecognizer.IsRunning)
            {
                m_PhraseRecognizer.Start();
            }
        }
    }

    void StopPhrase()
    {
        if (m_PhraseRecognizer.IsRunning)
        {
            try
            {
                PhraseRecognitionSystem.Shutdown();
                m_PhraseRecognizer.Stop();
            }
            catch
            {
                m_PhraseRecognizer.Stop();
            }

        }
    }

    public static void StopDictation()
    {
        if (m_DictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            m_DictationRecognizer.Stop();
        }
    }

    /// <summary>
    /// when click those button that has entertainment function, the dictation will shut down
    /// </summary>
    public void ChangeTextColor()
    {
        if (m_PhraseRecognizer != null && m_DictationRecognizer != null)
        {
            SpeakPrintFunc.Instance.speech.Silence();
            if (m_PhraseRecognizer.IsRunning)
            {
                BTText.color = UnityEngine.Color.white;
                InitPhrase();
            }
            else
            {
                StopDictation();
                InitPhrase();
                BTText.color = UnityEngine.Color.white;
            }
        }
    }
    /* --------------- 按钮切换语言输出状态，按下可以直接关闭其他功能  ------------------------- */

    /// <summary>
    /// 应用退出
    /// </summary>
    public void QuitApp()
    {
        Application.Quit();
    }
}