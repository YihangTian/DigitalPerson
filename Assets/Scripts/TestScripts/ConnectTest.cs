using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;
using WebSocketSharp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Networking;

public class ConnectTest
{
    string getTokenUrl = "https://work.chatbot.cn:39755/engine-api/api/answer/token";
    string robotId = "robotId=1710487583013867520";
    string getAnswerUrl = "https://work.chatbot.cn:39755/engine-api/api/streamAnswer";
    string publicKey = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALBGQ6yVSVjLNSompRSrkDReWYNdY4pPOoiabtCrDB+yKhMIIIHLJi5Z89cqvqjvkMvrXxXzl056IOQNM+Wdh60CAwEAAQ==";
    public TMP_InputField baseField;

    public async void test()
    {
        Debug.Log((Convert.FromBase64String("MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALBGQ6yVSVjLNSompRSrkDReWYNdY4pPOoiabtCrDB+yKhMIIIHLJi5Z89cqvqjvkMvrXxXzl056IOQNM+Wdh60CAwEAAQ==")).ToString());
        string token = GetToken(getTokenUrl, "sessionId=" + DateTime.UtcNow.ToString() + "&" + robotId);

        //token = token.Replace('-', '+');
        Debug.Log(token);
        string GetPostData = "";
        string time = Timestamp().ToString();
        Debug.Log(Timestamp().ToString());
        //GetPostData = "question=" + baseField.text.UrlEncode().ToUpper() + "&channel=web&userId=1710577163293560832&sessionId=" + DateTime.UtcNow.ToString() + "&robotId=1710487583013867520&sign=" + RSAEncrypt(publicKey, token + "_" + time).UrlEncode();
        GetPostData = "question=" + baseField.text + "&channel=web&userId=1710577163293560832&sessionId=" + DateTime.UtcNow.ToString() + "&robotId=1710487583013867520&sign=" + RSAEncrypt(publicKey, token + "_" + time);
        Debug.Log(getAnswerUrl + "?" + GetPostData);
        //StartCoroutine(GetText(GetPostData, getAnswerUrl));
        Task task = Task.Run(() => {

            Debug.Log(GetAnswer(GetPostData, getAnswerUrl)) ;
        });
        await task;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 获取当前的毫秒时间戳
    /// </summary>
    /// <returns></returns>
    public static long Timestamp()
    {
        long ts = ConvertDateTimeToInt(DateTime.Now);
        return ts;
    }

    /// <summary>  
    /// 将c# DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time">时间</param>  
    /// <returns>long</returns>  
    public static long ConvertDateTimeToInt(System.DateTime time)
    {
        //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        //long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
        long t = (time.Ticks - 621356256000000000) / 10000;
        return t;
    }

    public static string GetAnswer(string postDataStr, string url) 
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
        request.Method = "GET";
        request.ContentType = "text/event-stream;charset=UTF-8";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Debug.Log("send response");
        Stream stream = response.GetResponseStream();
        Debug.Log("get stream");
        Debug.Log("start reader");
        string data = "";
        try
        {
            //获取内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (!data.Contains("final")) { 
                    data += reader.ReadLine();
                }
            }
        }
        finally
        {
            stream.Close();
            
        }
        Debug.Log("receive data");
        Debug.Log(data);

        return data;
    }

    public static string GetToken(string url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://work.chatbot.cn:39755/engine-api/api/answer/token?sessionId=9527&robotId=1710487583013867520");
        request.Method = "GET";
        request.ContentType = "application/json";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();

        StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string data = reader.ReadToEnd();
        stream.Close();
        reader.Close();

        JObject token = new JObject();
        token = JObject.Parse(data);

        return token["data"].ToString();
    }

    string RSAEncrypt(string xmlPublicKey, string content)
    {
        string encryptedContent = string.Empty;
        RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(xmlPublicKey));
        //加载公钥
        //var publicXmlKey = File.ReadAllText(pathToPublicKey);
        string XML = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
            Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(XML);
            byte[] encryptedData = rsa.Encrypt(Encoding.Default.GetBytes(content), false);
            encryptedContent = Convert.ToBase64String(encryptedData);
        }
        return encryptedContent;
    }


}
