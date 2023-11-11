using Crosstales.RTVoice;
using LitJson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using System.Numerics;
using UnityEngine.Networking;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net.Http;
using WebSocketSharp;

public class TTS_Test : MonoBehaviour
{
    const string url = "https://open.mobvoi.com/api/tts/v1";

    static string appkey = "8444F741415C5053DDA14884669A097A";
    protected static string appSecret = "6DBF2C499BDA99B0C7429B4160B88D8F";

    const string json_Text = "text";
    const string json_Appkey = "appkey";
    const string json_AudioType = "audio_type";
    const string json_Speed = "speed";
    const string json_Signature = "signature";// MD5 加密
    const string json_Timestamp = "timestamp";// MD5 加密
    const string json_Speaker = "speaker";// MD5 加密

    string question = "河海大学，位于中国江苏南京，是一所以水利为特色的大学！";
    JsonData jsonData;
    private void Start()
    {
        //print(GetMD5("8444F741415C5053DDA14884669A097A+6DBF2C499BDA99B0C7429B4160B88D8F+1699275086"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            jsonData = MakeJsonPostData(question);
            Debug.Log(jsonData.ToJson());
            PostUrl(jsonData, url);
        }
    }

    static JsonData MakeJsonPostData(string question)
    {
        long stamp = Timestamp();
        string md5_str = string.Join("+", appkey, appSecret, stamp);
        print(md5_str);
        JsonData data = new JsonData();
        data[json_Text] = question;
        data[json_Speaker] = "billy_meet";
        data[json_AudioType] = "mp3";
        data[json_Speed] = 1.0;
        data[json_Appkey] = appkey;
        //string md5_str = appkey + appSecret + stamp;
        data[json_Timestamp] = stamp;
        data[json_Signature] = GetMD5(md5_str);
        print(GetMD5(md5_str));

        return data;
    }

    static void PostUrl(JsonData data, string postUrl)
    {
        byte[] requestData = Encoding.Default.GetBytes(data.ToJson());

        HttpWebRequest request = null;
        request = WebRequest.Create(postUrl) as HttpWebRequest;
        request.Proxy = null;
        request.Method = "POST";
        request.ContentType = "application/json";
        //request.Headers.Add("Authorization", "Bearer " + appkey);
        Stream requestSteam = request.GetRequestStream();

        requestSteam.Write(requestData, 0, requestData.Length);
        requestSteam.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);

        //Debug.Log(responseStreamReader.ReadToEnd());
        while (responseStreamReader.Read() != -1)
        {
            Debug.Log(responseStreamReader.ReadLine());
        }
        responseStreamReader.Close();
        responseStream.Close();
    }



    public static long Timestamp()
    {
        DateTime time = DateTime.Now;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));//当地时区
        TimeSpan ts = time - startTime;
        var timestamp = Convert.ToInt64(ts.TotalSeconds);
        return timestamp;
    }


    public static string GetMD5(string myString)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        //byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
        byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);//
        byte[] targetData = md5.ComputeHash(fromData);
        string byte2String = null;

        for (int i = 0; i < targetData.Length; i++)
        {
            //这个是很常见的错误，你字节转换成字符串的时候要保证是2位宽度啊，某个字节为0转换成字符串的时候必须是00的，否则就会丢失位数啊。不仅是0，1～9也一样。
            //byte2String += targetData[i].ToString("x");//这个会丢失
            byte2String = byte2String + targetData[i].ToString("x2");
        }

        return byte2String;
    }

}
