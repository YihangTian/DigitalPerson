using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniExcelLibs;
using System.Linq;
using Newtonsoft.Json;
using LitJson;
using static ExcelHelper;

public static class ExcelHelper
{
    static string fileName = "CityCode.xlsx";
    //static string filePath = Application.dataPath + "/ExcelData/" + fileName;
    static string filePath = Application.dataPath + "/Resources/ExcelData/" + fileName;

    //cityCodes = GetCityCodeList(filePath);
    //ContentManage.Instance.AddLIneText("系统：", GetCityCode(GetCityCodeList(filePath), "南京").ToString(), true);
    static int citycode;
    static List<CityCode> cityCodes;
    public static void GetCityCodeList(string path)
    {
        cityCodes = MiniExcel.Query<CityCode>(path).ToList(); //按照 表格类名称类从文件中读取数据，并转换成列表
        WeaterCtr.Instance.inputField.text = cityCodes.Count().ToString();
    }

    public static int GetCityCode(string cityName)
    {
        citycode = cityCodes.Find(x => x.Name.Contains(cityName)).adcode;
        cityCodes = null;
        Debug.Log(citycode);
        return citycode;
    }

    private static void ConvertToJson()
    {
        var res = MiniExcel.Query<CityCode>(filePath);
        Debug.Log(JsonConvert.SerializeObject(res, Formatting.Indented).ToString());
    }
}
