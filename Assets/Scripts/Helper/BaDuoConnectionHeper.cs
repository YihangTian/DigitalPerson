using LitJson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

public static class BaDuoConnectionHeper
{
    /// <summary>
    /// baduo Get Url
    /// </summary>
    static string initUrl = "https://demo.chatbot.cn/engine-api/api/robot/session/init/";
    static string answerUrl = "https://demo.chatbot.cn/engine-api/api/answer/";
    //static string initUrl = "http://192.168.0.123:8000/engine-api/api/robot/session/init/";
    //static string answerUrl = "http://192.168.0.123:8000/engine-api/api/answer/";

    /// <summary>
    /// sessionID this can random generate
    /// </summary>
    static string sessionID = null;


    public static void GetAnswer(string Question, Action<List<string>> Speak)
    {
        sessionID = DateTime.Now.ToString();
        sessionID = DateTime.Now.DayOfYear.ToString();
        JsonData jsonData = new JsonData();
        jsonData["robotId"] = "1691025765191258112";
        jsonData["sessionId"] = sessionID;
        PostUrl(jsonData, initUrl + sessionID, data => {

        });

        jsonData["question"] = Question;
        jsonData["channel"] = "web";
        jsonData["robotId"] = "1691025765191258112";
        jsonData["sessionId"] = sessionID;

        PostUrl(jsonData, answerUrl + sessionID, data => {
            var Answer = JObject.Parse(data);
            Speak(new List<string> { Answer["answers"][0]["plainText"].ToString()});
        });
    }


    static void PostUrl(JsonData jsonData, string url, Action<string> onResult)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(jsonData.ToJson());
        HttpWebRequest request = null;
        request = WebRequest.Create(url) as HttpWebRequest;
        request.Proxy = null;
        request.Method = "POST";
        request.ContentType = "application/json";
        Stream requestStream = request.GetRequestStream();
        requestStream.Write(requestData, 0, requestData.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);
        string responseText = responseStreamReader.ReadToEnd();
        responseStreamReader.Close();
        responseStream.Close();

        onResult(responseText);
    }
}
