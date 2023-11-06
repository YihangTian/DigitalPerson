using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;
using XUGL;

public static class ChartHelper
{
    static Sprite toollipBackground;
    static Color GridColor;
    static Color TextColor;
    static Color AxisLineColor;
    static Color TooltipLineColor;

    static List<string> Date;

    static Serie SWT;
    /// <summary>
    /// Weather
    /// </summary>
    public static List<double> WT = new List<double>();
    public static List<string> TimeList = new List<string>();

    static Title title;
    static GridCoord Gridcoord;
    static XAxis xAxis;
    static YAxis yAxis;
    static Tooltip tooltip;
    static Serie se;

    public static void SetChartProperty(LineChart lineChart, string serieName, string unit)
    {
        SWT = lineChart.GetSerie(0);
        SWT.serieName = serieName;
        SWT.lineType = LineType.Smooth;
        SetCharFormot(lineChart, serieName, unit, 50, 25, Color.red, Date);
    }

    static void SetCharFormot(LineChart lineChart, string TitleName, string yAxisName, int TitleFontSize, int AxisFontSize, Color color1, List<string> date)
    {
        //lineChart.Init();
        //GridColor = new Color(0, 255, 127, 50);
        GridColor = new Color(255, 255, 255, 255);
        TextColor = new Color(255, 255, 255, 255);
        AxisLineColor = new Color(255, 255, 255, 255);
        TooltipLineColor = new Color(255, 255, 255, 255);

        lineChart.theme.enableCustomTheme = true;
        lineChart.theme.customColorPalette.Add(new Color(255, 255, 255, 255));

        title = lineChart.EnsureChartComponent<Title>();
        Gridcoord = lineChart.EnsureChartComponent<GridCoord>();
        xAxis = lineChart.EnsureChartComponent<XAxis>();
        yAxis = lineChart.EnsureChartComponent<YAxis>();
        tooltip = lineChart.EnsureChartComponent<Tooltip>();
        se = lineChart.GetSerie(0);


        title.show = true;
        title.text = TitleName;
        title.location.top = 0;
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
        xAxis.axisLabel.rotate = 0;
        xAxis.boundaryGap = false;
        if (date.Count > 5) xAxis.splitNumber = 5;
        //else if (date.Count == 9) xAxis.axisLabel.interval = 1;
        //else xAxis.splitNumber = 0;

        xAxis.axisLabel.textStyle.fontSize = 25;
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
        yAxis.axisLabel.textStyle.fontSize = 25;
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

        //DrawLine(lineChart);
        title = null;
        Gridcoord = null;
        xAxis = null;
        yAxis = null;
        tooltip = null;
        se = null;
    }

    //static void DrawLine(LineChart chart)
    //{
    //    if (chart == null) return;

    //    chart.onDraw = delegate (VertexHelper vh) { };
    //    // or
    //    chart.onDrawBeforeSerie = delegate (VertexHelper vh, Serie serie) { };
    //    // or
    //    chart.onDrawAfterSerie = delegate (VertexHelper vh, Serie serie)
    //    {
    //        if (serie.index != 0) return;
    //        var dataPoints = serie.context.dataPoints;
    //        if (dataPoints.Count > 1)
    //        {
    //            var pos = dataPoints[5];
    //            var grid = chart.GetChartComponent<GridCoord>();
    //            var zeroPos = new Vector3(grid.context.x, grid.context.y);
    //            //竖线
    //            //var startPos = new Vector3(zeroPos.x, zeroPos.y);
    //            //var endPos = new Vector3(zeroPos.x, zeroPos.y + grid.context.height);
    //            //横线
    //            //var startPos1 = new Vector3(zeroPos.x, zeroPos.y + grid.context.height / 2);
    //            //var endPos1 = new Vector3(zeroPos.x + grid.context.width, zeroPos.y + grid.context.height / 2);
    //            //UGL.DrawLine(vh, startPos1, endPos1, chart.theme.serie.lineWidth, Color.yellow);

    //            //按比例划分
    //            var yaxis = chart.EnsureChartComponent<YAxis>();
    //            var xaxis = chart.EnsureChartComponent<XAxis>();
    //            double max = yaxis.max;
    //            double min = yaxis.min;
    //            if (max == 14)
    //            {
    //                float rate = (float)(max - min) / 11;
    //                var startPos1 = new Vector3(zeroPos.x, zeroPos.y + grid.context.height / rate);
    //                var endPos1 = new Vector3(zeroPos.x + grid.context.width, zeroPos.y + grid.context.height / rate);
    //                UGL.DrawLine(vh, startPos1, endPos1, chart.theme.serie.lineWidth, Color.yellow);
    //            }
    //            var startPos = new Vector3(pos.x, zeroPos.y);
    //            var endPos = new Vector3(pos.x, zeroPos.y + grid.context.height);

    //            UGL.DrawLine(vh, startPos, endPos, chart.theme.serie.lineWidth, Color.blue);
    //            UGL.DrawCricle(vh, pos, 20, Color.blue);
    //        }
    //    };
    //    // or
    //    chart.onDrawTop = delegate (VertexHelper vh) { };
    //}
}
