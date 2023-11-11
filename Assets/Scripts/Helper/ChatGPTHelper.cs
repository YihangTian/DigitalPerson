using System;
using System.Text;
using UnityEngine;
using LitJson;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

public static class ChatGPTHelper
{
    /// <summary>
    /// zhizenzen API url
    /// </summary>
    //static string chatGPTPostUrl = "https://flag.smarttrot.com/v1/chat";
    static string chatGPTPostUrl = "https://flag.smarttrot.com/v1/chat/completions";

    private static string BCpostUrl = "https://yewu.bcwhkj.cn/api/v2.Gptliu/search";
    //private static string appkey = "64609539cab34e31bdc0c684788dc8c4";
    //private static string Token = "e3SSe9pqVASSNxHcxBmdOYcv6KjRQnNl";

    /// <summary>
    /// API Key
    /// </summary>
    static string apiSecretKey = "zk-375ae55a50beb7cf4e3ea5ed46b9470c";

    const string json_Messages = "messages";

    const string json_User = "user";
    const string json_Model = "model";
    const string json_Stream = "stream";

    const string json_Role = "role";
    const string json_Content = "content";

    const string gpt4 = "gpt-4";
    const string gpt3 = "gpt-3.5-turbo-1106";

    delegate void TextPrintEvent(string printText);
    static event TextPrintEvent textPrintEventHandler;
    static Task sendTextTask;
    /// <summary>
    /// the return for answerAction
    /// </summary>
    static string Anwsers;

    /// <summary>
    /// the speak text for digital person
    /// </summary>
    static string Speaks;

    /// <summary>
    /// speak text list return for digital person speak
    /// </summary>
    static List<string> speakList;

    /// <summary>
    /// sum text for print
    /// </summary>
    static string sum_string = "";

    /// <summary>
    /// receive from API stream
    /// </summary>
    static string responseText = null;
    static JsonData jsonData = null;
    /// <summary>
    /// main thread use this function for get ChatGPT answer
    /// </summary>
    /// <param name="question"></param>
    /// <param name="answerAction"></param>
    public static void GetChatAsync(string question, Action<string> answerAction)
    {
        Init();
        PostUrl(MakeJsonPostData(question), chatGPTPostUrl);
        //speak_Thread.Join();
        answerAction(Anwsers);
    }

    /// <summary>
    /// make a json data for get answer from chatGPT API
    /// </summary>
    /// <param name="question"></param>
    /// <returns></returns>
    static JsonData MakeJsonPostData(string question)
    {
        JsonData data = new JsonData();
        data[json_User] = "DigitalPerson";
        data[json_Model] = gpt3;
        data[json_Stream] = true;

        JsonData messageDatas = new JsonData();
        messageDatas.SetJsonType(JsonType.Array);

        JsonData messageData = new JsonData();
        messageData[json_Role] = "user";
        question = question + "，字数在三十个字以内。";
        messageData[json_Content] = question;

        messageDatas.Add(messageData);

        // allocation data content
        data[json_Messages] = messageDatas;

        return data;
    }

    /// <summary>
    /// Init request data
    /// </summary>
    static void Init()
    {
        textPrintEventHandler += SpeakPrintFunc.Instance.OnPrintText;
        Anwsers = null;
        Speaks = null;
        sum_string = null;
        responseText = null;
        speakList = new List<string>();

        sendTextTask = new Task(() =>
        {
            for(int i = 0; i < speakList.Count; i++)
            {
                int len = speakList[i].Length;
                for (int j = 0; j < len; j++)
                {
                    char c = speakList[i][j];
                    if (textPrintEventHandler != null)
                    {
                        sum_string += c;
                        textPrintEventHandler(sum_string);
                    }
                    if (c.Equals('，') || c.Equals('。') || c.Equals('？') || c.Equals('：'))
                    {
                        Thread.Sleep(1500);
                    }
                    Thread.Sleep(150);
                }
            }

        });
    }

    /// <summary>
    /// post method
    /// </summary>
    /// <param name="data"></param>
    /// <param name="postUrl"></param>
    static async void PostUrl(JsonData data, string postUrl)
    {
        try
        {
            byte[] requestData = Encoding.Default.GetBytes(data.ToJson());

            HttpWebRequest request = null;
            request = WebRequest.Create(postUrl) as HttpWebRequest;
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + apiSecretKey);
            Stream requestSteam = request.GetRequestStream();

            requestSteam.Write(requestData, 0, requestData.Length);
            requestSteam.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);

            SpeakPrintFunc.isChatGPT = true;
            SpeakPrintFunc.canTalking = true;

            //handle response text
            while (responseStreamReader.Read() != -1)
            {
                responseText = responseStreamReader.ReadLine();
                if (responseText != null)
                {
                    if (responseText.Contains("content"))
                    {
                        responseText = responseText.Substring(5);
                        JObject ans = JsonConvert.DeserializeObject(responseText) as JObject;
                        string s = ans["choices"][0]["delta"]["content"].ToString();
                        Anwsers += s;
                        Speaks += s;
                        if (Speaks.Contains("，") || Speaks.Contains("。") || Speaks.Contains("：") || Speaks.Contains("？"))
                        {
                            speakList.Add(Speaks);
                            SpeakPrintFunc.speakList.Add(Speaks);
                            SpeakPrintFunc.DisplayText = true;
                            Speaks = null;
                            if (speakList.Count == 1)
                            {
                                DictationEngine.GetAnswerSuccess = true;
                                sendTextTask.Start();
                            }
                        }
                    }
                }
            }

            await sendTextTask;
            Debug.Log("complish get answer");
            responseStreamReader.Close();
            responseStream.Close();
            SpeakPrintFunc.DisplayText = false;
            textPrintEventHandler -= SpeakPrintFunc.Instance.OnPrintText;
        }
        catch
        {
            Debug.Log("Catch + Start BCChatGPT");
            string question = DictationEngine.question + "，字数在三十个字以内。";
            jsonData = BCChatGPTHelper.GetPostInfo(question);
            BCChatGPTHelper.Post(BCpostUrl, jsonData, answer =>
            {
                Anwsers = answer;
            });
        }
    }
}