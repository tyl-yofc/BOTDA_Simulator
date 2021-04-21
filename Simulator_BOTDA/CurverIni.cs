using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using DevComponents.DotNetBar;
using System.Drawing;

namespace Simulator_BOTDA
{
    public class CurverIni
    {
        private  PointPairList _ppListOriginal;
        public CurverIni(TabItem tabitem,ZedGraphControl zed)
        {
            _ppListOriginal = new PointPairList();         
            InitZed(zed,tabitem);
            tabitem.AttachedControl.Controls.Add(zed);
        }
        public void InitZed(ZedGraphControl zed,TabItem ti)
        {
            zed.Width = ti.AttachedControl.Width;
            zed.Height = ti.AttachedControl.Height;

            GraphPane subpane = zed.GraphPane;
            zed.MasterPane = new MasterPane("", new RectangleF(0, 0, ti.AttachedControl.Width, ti.AttachedControl.Height))
            {
                Border = { IsVisible = false }
            };
            subpane.Border.IsVisible = false;
            subpane.Title.Text = "";
            subpane.Chart.Border.Color = Color.Black;
            subpane.Legend.Position = LegendPos.TopCenter;
            subpane.Margin.Left = 5f;
            subpane.Margin.Top = 10;
            subpane.Margin.Bottom = 0.1f;
            subpane.Margin.Right = 0;

            subpane.XAxis.IsVisible = true;
            subpane.XAxis.Color = Color.Black;
            subpane.XAxis.Title.Text = "位置(m)";
            subpane.XAxis.Title.FontSpec.FontColor = Color.Black;
            subpane.XAxis.Scale.FontSpec.FontColor = Color.Black;
            subpane.XAxis.MajorGrid.IsVisible = false;
            subpane.XAxis.MinorGrid.IsVisible = false;
            subpane.XAxis.MajorGrid.Color = Color.Black;
            subpane.XAxis.MajorTic.Color = Color.Black;
            subpane.XAxis.Scale.FontSpec.Size = 12;
            subpane.XAxis.Title.FontSpec.Size = 10;
            subpane.XAxis.Scale.MajorStepAuto = true;
            subpane.XAxis.Scale.MinorStepAuto = true;
          //  subpane.XAxis.Scale.MinorStep = 1;
            subpane.XAxis.MinorTic.Color = Color.Transparent;
            subpane.X2Axis.Scale.IsVisible = false;
            subpane.X2Axis.IsVisible = false;
            //     subpane.XAxis.Type = ZedGraph.AxisType.LinearAsOrdinal;

            subpane.YAxis.Color = Color.Black;
            subpane.YAxis.Title.Text = "频移";
            subpane.YAxis.Title.FontSpec.FontColor = Color.Black;
            subpane.YAxis.Scale.FontSpec.FontColor = Color.Black;
            subpane.YAxis.MajorGrid.IsVisible = true;
            subpane.YAxis.MinorGrid.IsVisible = false;
            subpane.YAxis.MinorGrid.Color = Color.Black;
            subpane.YAxis.MajorTic.Color = Color.Black;
            subpane.YAxis.MinorGrid.Color = Color.Black;
            subpane.YAxis.MinorTic.Color = Color.Black;
            subpane.YAxis.Scale.FontSpec.Size = 12;
            subpane.YAxis.Title.FontSpec.Size = 10;

            subpane.Legend.IsVisible = true;
            subpane.Legend.Position = LegendPos.Top;
            subpane.Legend.IsHStack = true;
            subpane.Legend.FontSpec.Size = 10;

            zed.MasterPane.PaneList.Add(subpane);
            zed.AxisChange();
            zed.Refresh();

            InitCurver(zed);
        }
        private void InitCurver(ZedGraphControl zed)
        {
            _ppListOriginal = new PointPairList();
            zed.MasterPane.PaneList[0].AddCurve("原始数据", _ppListOriginal, Color.Red, SymbolType.None);
        }
        public  void DrawCurver(BotdaData channelinfo,float refractiveIndex,float samplingResolution)
        {
            _ppListOriginal.Clear();

            for (int i = 0; i < channelinfo.frequencyDatas.Count; i++)
                _ppListOriginal.Add(i * 1.5f / refractiveIndex * samplingResolution, channelinfo.frequencyDatas[i]);        
        }
    }

}
