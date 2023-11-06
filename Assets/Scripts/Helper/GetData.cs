using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using XCharts.Runtime;
using System;
using UnityEngine.UI;
using XUGL;

public class GetData : MonoBehaviour
{
    public static GetData Instance;
    /// <summary>
    /// 水温
    /// </summary>
    public List<double> WT = new List<double>();
    /// <summary>
    /// 浊度
    /// </summary>
    public List<double> TB = new List<double>();
    /// <summary>
    /// 溶解氧
    /// </summary>
    public List<double> DO = new List<double>();
    /// <summary>
    /// 电导率
    /// </summary>
    public List<double> CON = new List<double>();
    /// <summary>
    /// 水质
    /// </summary>
    public List<double> PH = new List<double>();
    public List<string> Id = new List<string>();
    public List<string> Date = new List<string>();

    public LineChart LPH = new LineChart();
    public LineChart LCON = new LineChart();
    public LineChart LDO = new LineChart();
    public LineChart LTB = new LineChart();
    public LineChart LWT = new LineChart();

    private Serie SWT;
    private Serie STB;
    private Serie SDO;
    private Serie SCON;
    private Serie SPH;

    public Sprite toollipBackground;
    public Color GridColor;
    public Color TextColor;
    public Color AxisLineColor;
    public Color TooltipLineColor;

    DateTime StartTime;
    DateTime EndTime;
    string start_datetime;
    string end_datetime;

    int page_Count;
    int data_Count;


    void Start()
    {
        if (Instance == null) Instance = this;
        GetTime();
        StartCoroutine("GetPage_DataCount");
        InitData();

    }
    IEnumerator Getdata()
    {
        //页数获取API string Count = "http://data.gkaitech.com/water_data/get_data_count_r?data_type=ST_WATERQUALITY_R&STCD=00501523070900175071&&start_datetime=2023-08-11T13:27:00&end_datetime=2024-08-12T13:27:00";
        //string Api = "http://data.gkaitech.com/water_data/get_data_r?data_type=ST_WATERQUALITY_R&STCD=00501523070900175071&&start_datetime=2022-01-05T03:59:48&end_datetime=2024-11-05T03:59:48&page_id=1";
        for (int page = 1; page <= page_Count; page++)
        {
            string Api = "http://data.gkaitech.com/water_data/get_data_r?data_type=ST_WATERQUALITY_R&STCD=00501523070900175071&&start_datetime=" + start_datetime + "&end_datetime=" + end_datetime + "&page_id=" + page;
            print(Api);
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(Api);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(unityWebRequest.error);
            }
            else
            {
                string json = unityWebRequest.downloadHandler.text;

                var jobj = JObject.Parse(json);
                var jArray = (JArray)jobj["content"];

                if (jobj["content"].ToString() != "")
                {
                    int length = jArray.Count;
                    print(length);
                    for (int i = 0; i < length; i++)
                    {
                        WT.Add(retain(jobj["content"][i]["WT"]));
                        TB.Add(retain(jobj["content"][i]["TB"]));
                        DO.Add(retain(jobj["content"][i]["DO"]));
                        CON.Add(retain(jobj["content"][i]["CON"]));
                        PH.Add(retain(jobj["content"][i]["PH"]));
                        Id.Add(jobj["content"][i]["STCD"].ToString());
                        Date.Add(jobj["content"][i]["TM"].ToString());
                    }
                }
            }
        }
        ChartChange();
        AddDate();
        AddData();
        SetChartProperty();
    }

    IEnumerator GetPage_DataCount()
    {
        string CountApi = "http://data.gkaitech.com/water_data/get_data_count_r?data_type=ST_WATERQUALITY_R&STCD=00501523070900175071&&start_datetime=" + start_datetime + "&end_datetime=" + end_datetime;
        UnityWebRequest Request = UnityWebRequest.Get(CountApi);
        yield return Request.SendWebRequest();
        if (Request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(Request.downloadHandler.text);
        }
        else
        {
            var jobj = JObject.Parse(Request.downloadHandler.text);
            page_Count = (int)jobj["content"]["page_count"];
            data_Count = (int)jobj["content"]["data_count"];
        }
        StartCoroutine("Getdata");
    }

    void GetTime()
    {
        EndTime = DateTime.Now;
        StartTime = EndTime.AddDays(-5);
        start_datetime = StartTime.ToString("yyyy-MM-ddTHH:mm:ss");
        end_datetime = EndTime.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    void AddDate()
    {
        //for (int i = 1; i < Date.Count; i++)
        for (int i = 0; i < Date.Count; i++)
        {
            Date[i] = Date[i].Replace("T", "");
            Date[i] = Date[i].Replace("+", "");
            Date[i] = Date[i].Replace("-", "/");
            DateTime time = DateTime.Parse(Date[i]);
            string Time = time.ToString("HH:mm");

            LWT.AddXAxisData(Time);
            LTB.AddXAxisData(Time);
            LDO.AddXAxisData(Time);
            LCON.AddXAxisData(Time);
            LPH.AddXAxisData(Time);
        }
    }
    void AddData()
    {
        //for (int i = 1; i < Temperature.Count; i++)
        for (int i = 0; i < WT.Count; i++)
        {
            LWT.AddData(0, WT[i]);
            LTB.AddData(0, TB[i]);
            LDO.AddData(0, DO[i]);
            LCON.AddData(0, CON[i]);
            LPH.AddData(0, PH[i]);
            //STemperature.AddData(Temperature[i]);
            //STurbidity.AddData(Temperature[i]);
            //SDO.AddData(DO[i]);
            //SConductivity.AddData(Conductivity[i]);
            //SSaturation.AddData(Saturation[i]);
        }
    }
    void ChartChange()
    {
        for (int i = 0; i < TB.Count; i++)
        {
            if (TB[i] > 5)
            {
                LTB.EnsureChartComponent<YAxis>().minMaxType = Axis.AxisMinMaxType.Default;
            }
        }
    }
    void InitData()
    {
        WT.Clear();
        TB.Clear();
        DO.Clear();
        CON.Clear();
        PH.Clear();

        LWT.ClearData();
        LTB.ClearData();
        LDO.ClearData();
        LCON.ClearData();
        LPH.ClearData();
    }

    double retain(JToken jobj)
    {
        string s = jobj.ToString();
        double f = double.Parse(s);
        return Math.Round(f, 3);
    }

    void SetChartProperty()
    {
        double max = 0;
        for (int i = 0; i < TB.Count; i++)
        {
            if (TB[i] > max) max = TB[i];
            print("max+++++++++++++++" + max);
        }

        SPH = LPH.GetSerie(0);
        SPH.serieName = "pH";
        SPH.lineType = LineType.Smooth;
        SetCharFormot(LPH, "pH", "pH", 50, 50,Color.red, Date, max);
        //SetCharFormot(LPH, "pH", "mol·L", 80, 60);

        SCON = LCON.GetSerie(0);
        SCON.serieName = "电导率";
        SCON.lineType = LineType.Smooth;
        SetCharFormot(LCON, "电导率", "uS/cm", 50, 50,Color.blue, Date, max);

        SDO = LDO.GetSerie(0);
        SDO.serieName = "溶解氧";
        SDO.lineType = LineType.Smooth;
        SetCharFormot(LDO, "溶解氧", "mg/L", 50, 50,Color.green, Date, max);

        STB = LTB.GetSerie(0);
        STB.serieName = "浊度";
        STB.lineType = LineType.Smooth;
        SetCharFormot(LTB, "浊度", "NTU", 50, 50,Color.white, Date, max);

        SWT = LWT.GetSerie(0);
        SWT.serieName = "水温";
        SWT.lineType = LineType.Smooth;
        SetCharFormot(LWT, "水温", "°C", 50, 50,Color.yellow, Date, max);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lineChart"></param>
    /// <param name="TitleName">图表名称</param>
    /// <param name="yAxisName">y轴计量单位</param>
    /// <param name="TitleFontSize"></param>
    /// <param name="AxisFontSize"></param>
    public void SetCharFormot(LineChart lineChart, string TitleName, string yAxisName, int TitleFontSize, int AxisFontSize,Color color1, List<string> date, double max)
    {
        //lineChart.Init();
        //GridColor = new Color(0, 255, 127, 50);
        GridColor = new Color(255, 255, 255, 255);
        TextColor = new Color(255, 255, 255, 255);
        AxisLineColor = new Color(255, 255, 255, 255);
        TooltipLineColor = new Color(255, 255, 255, 255);

        lineChart.theme.enableCustomTheme = true;
        lineChart.theme.customColorPalette.Add(new Color(255, 255, 255, 255));

        var title = lineChart.EnsureChartComponent<Title>();
        var Gridcoord = lineChart.EnsureChartComponent<GridCoord>();
        var xAxis = lineChart.EnsureChartComponent<XAxis>();
        var yAxis = lineChart.EnsureChartComponent<YAxis>();
        var tooltip = lineChart.EnsureChartComponent<Tooltip>();
        var se = lineChart.GetSerie(0);


        title.show = true;
        title.text = TitleName;
        title.labelStyle.textStyle.fontSize = TitleFontSize;//80
        title.labelStyle.textStyle.color = TextColor;

        Gridcoord.left = 0.08f;
        Gridcoord.right = 0.05f;
        Gridcoord.top = 0.18f;
        Gridcoord.bottom = 0.12f;

        xAxis.type = Axis.AxisType.Category;
        xAxis.splitLine.show = true;
        xAxis.axisLine.show = true;
        xAxis.axisLine.showArrow = true;
        xAxis.axisLine.arrow.width = 16;
        xAxis.axisLine.arrow.height = 20;
        xAxis.axisLine.arrow.dent = 5;
        xAxis.axisLine.arrow.offset = 30;
        xAxis.axisLabel.textStyle.autoAlign = true;
        xAxis.axisLabel.textStyle.autoWrap = true;
        xAxis.axisLabel.rotate = 90;
        xAxis.boundaryGap = false;
        if (date.Count > 5) xAxis.splitNumber = 5;
        //else if (date.Count == 9) xAxis.axisLabel.interval = 1;
        //else xAxis.splitNumber = 0;

        xAxis.axisLabel.textStyle.fontSize = 50;
        xAxis.axisLabel.textStyle.color = TextColor;
        xAxis.axisLine.lineStyle.color = AxisLineColor;
        xAxis.axisLine.lineStyle.width = 2;
        xAxis.axisLabel.offset = new Vector3(0, 0, 0);
        xAxis.axisTick.show = false;
        xAxis.axisTick.inside = true;
        xAxis.axisTick.lineStyle.color = AxisLineColor;
        xAxis.axisTick.lineStyle.length = 12;

        yAxis.axisLine.show = true;
        yAxis.axisLine.showArrow = true;
        yAxis.axisLine.arrow.width = 16;
        yAxis.axisLine.arrow.height = 20;
        yAxis.axisLine.arrow.dent = 5;
        yAxis.axisLine.arrow.offset = 30;
        yAxis.axisName.show = true;
        yAxis.axisName.name = yAxisName;
        yAxis.axisName.labelStyle.textStyle.fontSize = AxisFontSize;//60
        yAxis.axisName.labelStyle.textStyle.color = TextColor;
        yAxis.axisLabel.offset = new Vector3(0, 15, 0);
        yAxis.axisName.labelStyle.offset = new Vector3(0, 80, 0);
        yAxis.axisLine.lineStyle.color = AxisLineColor;
        yAxis.axisLine.lineStyle.width = 2;
        //lineChart.EnsureChartComponent<YAxis>().splitNumber = 5;
        yAxis.axisLabel.textStyle.fontSize = 50;
        //yAxis.axisLabel.rotate = 180;
        yAxis.axisLabel.textStyle.color = TextColor;
        yAxis.axisTick.show = false;
        yAxis.axisTick.inside = true;
        yAxis.axisTick.lineStyle.color = AxisLineColor;
        yAxis.axisTick.lineStyle.length = 12;

        yAxis.splitLine.show = true;
        xAxis.splitLine.lineStyle.color = GridColor;
        xAxis.splitLine.lineStyle.opacity = 0.8f;
        xAxis.splitLine.lineStyle.type = LineStyle.Type.Dotted;
        yAxis.splitLine.lineStyle.color = GridColor;
        yAxis.splitLine.lineStyle.opacity = 0.8f;
        yAxis.splitLine.lineStyle.type = LineStyle.Type.Dotted;

        tooltip.backgroundImage = toollipBackground;
        tooltip.offset = new Vector2(0, 50);
        tooltip.trigger = Tooltip.Trigger.Axis;
        tooltip.lineStyle.type = LineStyle.Type.Solid;
        tooltip.lineStyle.color = TooltipLineColor;
        tooltip.lineStyle.opacity = 0.2f;


        se.show = true;
        se.symbol.show = true;
        se.itemStyle.show = true;
        se.lineStyle.show = true;
        se.lineStyle.color = color1;
        se.lineStyle.opacity = 1;
        se.animation.enable = true;
        se.coordSystem = "GridCoord";

        if (TitleName == "浊度")
        {

            if (max > 2)
            {
                yAxis.minMaxType = Axis.AxisMinMaxType.Default;
            }
            else
            {
                yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
                yAxis.min = 0;
                yAxis.max = max + 0.5;
            }
            yAxis.splitNumber = 2;

        }
        else if (TitleName == "pH")
        {
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.min = 0;
            yAxis.max = 14;
            yAxis.splitNumber = 4;
        }
        else if (TitleName == "水温") {
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.min = 0;
            yAxis.max = 50;
        }else
        {
            yAxis.minMaxType = Axis.AxisMinMaxType.Default;
        }

        DrawLine(lineChart);
    }

    void DrawLine(LineChart chart)
    {
        if (chart == null) return;

        chart.onDraw = delegate (VertexHelper vh) { };
        // or
        chart.onDrawBeforeSerie = delegate (VertexHelper vh, Serie serie) { };
        // or
        chart.onDrawAfterSerie = delegate (VertexHelper vh, Serie serie)
        {
            if (serie.index != 0) return;
            var dataPoints = serie.context.dataPoints;
            if (dataPoints.Count > 1)
            {
                print(dataPoints.Count);
                var pos = dataPoints[5];
                var grid = chart.GetChartComponent<GridCoord>();
                var zeroPos = new Vector3(grid.context.x, grid.context.y);
                print(grid.context.x+ "     x      " + grid.context.y);
                print(grid.context.position);//网格零点位置
                print(grid.context.height);
                print(grid.context.width);
                //竖线
                //var startPos = new Vector3(zeroPos.x, zeroPos.y);
                //var endPos = new Vector3(zeroPos.x, zeroPos.y + grid.context.height);
                //横线
                //var startPos1 = new Vector3(zeroPos.x, zeroPos.y + grid.context.height / 2);
                //var endPos1 = new Vector3(zeroPos.x + grid.context.width, zeroPos.y + grid.context.height / 2);
                //UGL.DrawLine(vh, startPos1, endPos1, chart.theme.serie.lineWidth, Color.yellow);

                //按比例划分
                var yaxis = chart.EnsureChartComponent<YAxis>();
                var xaxis = chart.EnsureChartComponent<XAxis>();
                double max = yaxis.max;
                double min = yaxis.min;
                if (max == 14) {
                    float rate = (float)(max - min) / 11;
                    var startPos1 = new Vector3(zeroPos.x, zeroPos.y + grid.context.height / rate);
                    var endPos1 = new Vector3(zeroPos.x + grid.context.width, zeroPos.y + grid.context.height / rate);
                    UGL.DrawLine(vh, startPos1, endPos1, chart.theme.serie.lineWidth, Color.yellow);
                }
                var startPos = new Vector3(pos.x, zeroPos.y);
                var endPos = new Vector3(pos.x, zeroPos.y + grid.context.height);

                UGL.DrawLine(vh, startPos, endPos, chart.theme.serie.lineWidth, Color.blue);
                UGL.DrawCricle(vh, pos, 20, Color.blue);
            }
        };
        // or
        chart.onDrawTop = delegate (VertexHelper vh) { };
    }
}
