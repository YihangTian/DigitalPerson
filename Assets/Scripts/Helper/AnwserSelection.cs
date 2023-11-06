using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AnwserSelection
{
    /// <summary>
    /// Text print thread
    /// </summary>
    static Thread print_Thread;

    /// <summary>
    /// chatGPT get answer task
    /// </summary>
    static Task chatGPTTask;

    /// <summary>
    /// baduo get answer task
    /// </summary>
    static Task baduoTask;

    /// <summary>
    /// text print event
    /// </summary>
    /// <param name="answer"></param>
    delegate void TextPrintEvent(string answer);

    /// <summary>
    /// text print handler
    /// </summary>
    static event TextPrintEvent textPrintEventHandler;

    /// <summary>
    /// handle badou answer
    /// </summary>
    public static List<string> baduoAnwser_List = new List<string>();

    /// <summary>
    /// buffer chatGPT answer
    /// </summary>
    static List<string> bufferList = new List<string>();

    /// <summary>
    /// sum text for print
    /// </summary>
    static string sum_string = "";

    /// <summary>
    /// the last answer for digital person speak
    /// </summary>
    static string robatAnswer = null;

    /// <summary>
    /// record the global answer from user
    /// </summary>
    static string question_Record = "";

    static QuestionTypeEnum questionTypeEnum;

    /// <summary>
    /// Get answer method
    /// 
    /// handle the define from developer
    /// </summary>
    /// <param name="question"> 问题 </param>
    /// <param name="Speak"> 匿名方法处理 </param>
    public static async void GetAnswer(string question, Action<string> Speak)
    {
        Init(question);
        //再次调用唤醒识别

        questionTypeEnum = GetQType.GetQusetionType(question_Record);
        switch (questionTypeEnum)
        {
            case QuestionTypeEnum.Operation:
                break;
            case QuestionTypeEnum.Action:
                break;
            case QuestionTypeEnum.Think:
                await baduoTask;

                Task task = Task.Run(async () =>
                {
                    if (baduoAnwser_List.Contains("很抱歉，我暂时无法回答您的问题！") || baduoAnwser_List.Contains("小泰：您是否想咨询以下问题，您可点击相关问题，也可重新咨询"))
                    {
                        chatGPTTask.Start();
                        await chatGPTTask;
                    }
                    else
                    {
                        //Handle Badou answer
                        BadouPrinter();
                        print_Thread.Join();
                    }
                });
                await task;
                //main thread speak
                Speak(robatAnswer);
                Debug.Log("mainTask complish");
                break;
        }

        //if (question.Contains("小蔡小蔡") || question.Contains("小太小太") || question.Contains("小泰小泰") || question.Contains("小肽小肽"))
        //{
        //    robatAnswer = "我在呢！";
        //}
        //else if (question.Contains("当前水位") || question.Contains("实时水位"))
        //{
        //    if (question.Contains("当前水位"))
        //    {
        //        robatAnswer = "当前水位为30m";
        //    }
        //    else
        //    {
        //        robatAnswer = "实时水位为30m";
        //    }
        //}
        //else
        //{
            
        //}
    }

    private static void ChatGPTConnectionTask()
    {
        ChatGPTHelper.GetChatAsync(question_Record, answer =>
        {
            Debug.Log(answer);
            robatAnswer = answer;
            Debug.Log("GetAnswer"+ answer);
        });
    }

    /// <summary>
    /// Baduo connection task, use thread
    /// it also have answer handler
    /// </summary>
    static void BaduoConnectionTask()
    {
        BaDuoConnectionHeper.GetAnswer(question_Record, answer =>
        {
            baduoAnwser_List = answer;
            robatAnswer = baduoAnwser_List[0];
            string tmpString = null;
            int tmpInt = robatAnswer.Count();
            for (int i = 0; i < tmpInt; i++)
            {
                tmpString += robatAnswer[i];
                if (tmpString.Contains("，") || tmpString.Contains("。") || tmpString.Contains("：") || tmpString.Contains("？"))
                {
                    bufferList.Add(tmpString);
                    tmpString = null;
                }
                else if (i == tmpInt - 1 && tmpString != null)
                {
                    bufferList.Add(tmpString);
                    tmpString = null;
                }
            }
        });
    }

    /// <summary>
    /// GetAnswer init parameter
    /// </summary>
    /// <param name="question"></param>
    static void Init(string question)
    {
        robatAnswer = "";
        sum_string = "";
        baduoAnwser_List = new List<string>();
        bufferList = new List<string>();
        question_Record = question;
        chatGPTTask = new Task(ChatGPTConnectionTask);
        baduoTask = new Task(BaduoConnectionTask);
        textPrintEventHandler += SpeakPrintFunc.Instance.OnPrintText;
        baduoTask.Start();
    }

    /// <summary>
    /// Badou printer method
    /// </summary>
    static void BadouPrinter()
    {
        Thread.Sleep(2000);
        string Speaks = "";
        print_Thread = new Thread(() => {
            SpeakPrintFunc.isBadou = true;
            SpeakPrintFunc.canTalking = true;
            SpeakPrintFunc.DisplayText = true;
            SpeakPrintFunc.speakList = bufferList;

            int badou_Count = robatAnswer.Count();
            for (int i = 0; i < badou_Count; i++)
            {
                if (textPrintEventHandler != null)
                {
                    sum_string += robatAnswer[i];
                    Speaks += robatAnswer[i];
                    textPrintEventHandler(sum_string);
                }
                if (robatAnswer[i].Equals('，') || robatAnswer[i].Equals('。') || robatAnswer[i].Equals('？') || robatAnswer[i].Equals('：'))
                {
                    Thread.Sleep(2000);
                }
                Thread.Sleep(200);
            }
            SpeakPrintFunc.DisplayText = false;
            textPrintEventHandler -= SpeakPrintFunc.Instance.OnPrintText;
        });
        print_Thread.Start();
    }
}
