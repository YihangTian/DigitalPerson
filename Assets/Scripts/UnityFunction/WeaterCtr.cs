using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.LookDev;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using XCharts.Runtime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using XUGL;
using TMPro;

public class WeaterCtr : MonoBehaviour
{
    public static WeaterCtr Instance;

    static string fileName = "CityCode.xlsx";
    static string filePath = Application.dataPath + "/Resources/ExcelData/" + fileName;

    string cityCode;
    public LineChart LWT = new LineChart();
    public TMP_InputField cityName;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance==null)
        {
            Instance = this;
        }
    }

    public void GetDataBT()
    {
        GetWeatherData();

    }

    public void GetWeatherData()
    {
        InitData();
        ExcelHelper.GetCityCodeList(filePath);
        cityCode = ExcelHelper.GetCityCode(cityName.text).ToString();
        StartCoroutine(GetData());
    }

    /// <summary>
    /// 天气情况信息
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetData()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://restapi.amap.com/v3/weather/weatherInfo?city="+cityCode+"&key=e88c43e1f1fb4c54f56f71339f7fa806&extensions=base&output=JSON");// 101270101 成都天气
        Debug.Log(cityCode);
        yield return unityWebRequest.SendWebRequest();
        Debug.Log("请求完毕");
        if (unityWebRequest.isNetworkError || unityWebRequest.isNetworkError)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            
            Debug.Log(unityWebRequest.downloadHandler.text);
            JObject jobj = JObject.Parse(unityWebRequest.downloadHandler.text);
            Debug.Log(jobj["lives"].ToString());
            if (jobj["lives"].ToString() != "")
            {
                JArray jobj2 = JArray.Parse(jobj["lives"].ToString());
                Debug.Log(jobj2);
                Debug.Log(jobj2[0]["temperature"].ToString());
                ChartHelper.WT.Add(double.Parse(jobj2[0]["temperature"].ToString()));
                ChartHelper.TimeList.Add(DateTime.Parse(jobj2[0]["reporttime"].ToString()).ToString("MM-dd"));
            }
        }
        StartCoroutine(GetYCData());
    }

    /// <summary>
    /// 预测天气
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetYCData() 
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://restapi.amap.com/v3/weather/weatherInfo?city="+cityCode+"&key=e88c43e1f1fb4c54f56f71339f7fa806&extensions=all&output=JSON");// 101270101 成都天气
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.isNetworkError || unityWebRequest.isNetworkError)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {

            JObject jobj = JObject.Parse(unityWebRequest.downloadHandler.text);
            JArray jArray = JArray.Parse(jobj["forecasts"].ToString());
            Debug.Log(jobj["forecasts"].ToString());
            if (jArray.ToString() != "")
            {
                JArray jArray2= JArray.Parse(jArray[0]["casts"].ToString());
                for(int i = 0;i < jArray2.Count; i++)
                {
                    JObject jobjtmp = JObject.Parse(jArray2[i].ToString());
                    ChartHelper.WT.Add(double.Parse(jobjtmp["daytemp"].ToString()));
                    ChartHelper.TimeList.Add(DateTime.Parse(jobjtmp["date"].ToString()).ToString("MM-dd"));
                }

            }
        }

        ChartHelper.SetChartProperty(LWT, "气温", "°C");
        AddXData();
        AddYData();
    }

    void InitData()
    {
        ChartHelper.TimeList.Clear();
        ChartHelper.WT.Clear();

        LWT.ClearData();
    }
    void AddXData()
    {
        //for (int i = 1; i < Date.Count; i++)
        for (int i = 0; i < ChartHelper.TimeList.Count; i++)
        {
            LWT.AddXAxisData(ChartHelper.TimeList[i]);
        }
    }
    void AddYData()
    {
        //for (int i = 1; i < Temperature.Count; i++)
        Debug.Log(ChartHelper.WT.Count);
        for (int i = 0; i < ChartHelper.WT.Count; i++)
        {
            LWT.AddData(0, ChartHelper.WT[i]);
        }
    }

    /// <summary>
    /// 1小时更新一次
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateWeather()
    {

        while (true)
        {
            yield return new WaitForSeconds(3600);
            StartCoroutine(GetData());
        }
    }


}
