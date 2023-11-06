using LitJson;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    private string postUrl = "https://flag.smarttrot.com/v1/chat/completions";
    private const string user = "json_User";
    private const string messages = "json_Messages";
    private DateTime start;
    private DateTime end;

    private void Start()
    {
        start = DateTime.Now;
        StartCoroutine(Post());
    }
    IEnumerator Post()
    {/**//**/
        WWWForm form = new WWWForm();/**/

        // 配置数据
        string apiSecretKey = "zk-375ae55a50beb7cf4e3ea5ed46b9470c";
        JsonData data = new JsonData();
        data[user] = "测试者";

        // messages
        JsonData messageDatas = new JsonData();
        messageDatas.SetJsonType(JsonType.Array);

        // 单个 message
        JsonData messageData = new JsonData();
        messageData["role"] = "json_User";
        messageData["content"] = "什么是人工智能";

        // 存入 message
        messageDatas.Add(messageData);

        // 配置内容
        data[messages] = messageDatas;

        // 编码 JSON
        var dataBytes = Encoding.Default.GetBytes(data.ToJson());
        UnityWebRequest request = UnityWebRequest.Post(postUrl, form);
        request.uploadHandler = new UploadHandlerRaw(dataBytes);

        // 发送 https
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiSecretKey);
        //request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string receiveContent = request.downloadHandler.text;
            Debug.Log(receiveContent);
        }
        end = DateTime.Now;
        Debug.Log(end - start);
    }
}

