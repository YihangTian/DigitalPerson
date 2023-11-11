using LitJson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

public class BCChatGPTHelper : MonoBehaviour
{
    private static List<string> answerList = new List<string>();
    private static List<string> speakList = new List<string>();

    public static string Answer;
    public static string sum_string;

    private static string json_Message = "messages";
    private static string json_Model = "model";
    private static string json_Stream = "stream";
    private static string json_Max_Tokens = "max_tokens";
    private static JsonData role = new JsonData();
    delegate void TextPrintEvent(string printText);
    static event TextPrintEvent textPrintEventHandler;
    /// <summary>
    /// the speak text for digital person
    /// </summary>
    static string Speaks;
    /// <summary>
    /// 输入信息
    /// </summary>
    private static string json_Role = "role";
    private static string json_Content = "content";

    public static Task sendTextTask;
    /// <summary>
    /// Init request data
    /// </summary>
    static void Init()
    {
        textPrintEventHandler += SpeakPrintFunc.Instance.OnPrintText;
        Answer = null;
        Speaks = null;
        sum_string = null;
        Speaks = null;
        speakList = new List<string>();

        sendTextTask = new Task(() =>
        {
            for (int i = 0; i < speakList.Count; i++)
            {
                int len = speakList[i].Length;

                for (int j = 0; j < len; j++)
                {
                    char c = speakList[i][j];
                    if (textPrintEventHandler != null)
                    {
                        sum_string += c;
                        Debug.Log(sum_string);
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

    public static async void Post(string url, JsonData str, Action<string> GetAnswer, string content_type = "application/json; charset=UTF-8", string Token = "e3SSe9pqVASSNxHcxBmdOYcv6KjRQnNl")
    {
        Init();
        SpeakPrintFunc.isChatGPT = true;
        SpeakPrintFunc.canTalking = true;

        var client = new RestClient();
        var request = new RestRequest(url, Method.POST);
        request.AddHeader("Authorization", $"Bearer {Token}");
        //client.UserAgent = "Apifox/1.0.0 (https://apifox.com)";
        request.AddHeader("Content-Type", $"{content_type}");
        request.AddParameter("application/json", str.ToJson(), ParameterType.RequestBody);
        RestResponse response = client.Execute(request);
        answerList = response.Content.Split("\n").ToList();
        for (int i = 0; i < answerList.Count; i++)
        {
            if (answerList[i].Contains("content"))
            {
                string s = answerList[i].Substring(5);
                JObject s_js = JsonConvert.DeserializeObject(s) as JObject;
                string c = s_js["choices"][0]["delta"]["content"].ToString();
                Answer += c;
                Speaks += c;
                if (Speaks.Contains("，") || Speaks.Contains("。") || Speaks.Contains("：") || Speaks.Contains("？"))
                {
                    speakList.Add(Speaks);
                    SpeakPrintFunc.speakList.Add(Speaks);
                    SpeakPrintFunc.DisplayText = true;
                    Speaks = null;
                    if (speakList.Count == 1)
                    {
                        Debug.Log("start print");
                        DictationEngine.GetAnswerSuccess = true;
                        sendTextTask.Start();
                    }
                }
            }
        }
        await sendTextTask;
        SpeakPrintFunc.DisplayText = false;

        textPrintEventHandler -= SpeakPrintFunc.Instance.OnPrintText;
        GetAnswer(Answer);
    }

    public static JsonData GetPostInfo(string answer)
    {
        JsonData data = new JsonData();

        JsonData messageData = new JsonData();
        messageData.SetJsonType(JsonType.Array);



        role[json_Role] = "user";
        role[json_Content] = answer;
        messageData.Add(role);

        data[json_Message] = messageData;
        data[json_Model] = "gpt-3.5-turbo-1106";
        data[json_Stream] = true;
        data[json_Max_Tokens] = 50;

        return data;
    }
}
