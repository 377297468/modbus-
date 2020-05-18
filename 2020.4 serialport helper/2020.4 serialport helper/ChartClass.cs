using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

using Modbus.Device;

namespace _2020._4_serialport_helper
{
    public class ChartClass
    {
    
        private Queue<double> dataQueue = new Queue<double>(2);
        public  Chart chart1 = null;
        public SerialPort serialPort1 = null;
        public Hashtable hash = null;
        string register_address = null;
        double  register_data ;
        public ChartArea ChartArea1;

        public ChartClass(Chart _chart1, SerialPort _serialPort1, Hashtable _hash,string _register_address,double update_register_data)
        {
            this.chart1 = _chart1;
            this.serialPort1 = _serialPort1;
            this.hash = _hash;
            this.register_address = _register_address;
            this.register_data = update_register_data;

        }


        ///  <summary>
        ///  将串口数据添加到chart中
        ///  </summary>
        ///  <param name="sender"></param>
        ///  <param name="e"></param>
        public void addchartdata()
        {
          //  UpdateQueueValue();
            this.chart1.Series[0].Points.Clear();
            // for (int i = 0; i < dataQueue.Count; i++)
            // {
            int i = 1;
                this.chart1.Series[0].Points.AddXY(i++, register_data);
          //  }
        }

        ///  <summary>
        ///  初始化图表
        ///  </summary>
        //public void InitChart()
        //{
        //    try
        //    {
        //        定义图表区域
        //        this.chart1.ChartAreas.Clear();
        //        ChartArea1 = new ChartArea("C1");
        //        this.chart1.ChartAreas.Add(ChartArea1);
        //        定义存储和显示点的容器
        //        this.chart1.Series.Clear();
        //        Series series1 = new Series("S1");
        //        series1.ChartArea = "C1";
        //        this.chart1.Series.Add(series1);

        //        this.chart1.ChartAreas[0].AxisY.Minimum = 0;
        //        this.chart1.ChartAreas[0].AxisY.Maximum = 100;
        //        this.chart1.ChartAreas[0].AxisX.Interval = 5;
        //        this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
        //        this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
        //        设置标题
        //        this.chart1.Titles.Clear();
        //        this.chart1.Titles.Add("S01");
        //        this.chart1.Titles[0].Text = "XXX显示";
        //        this.chart1.Titles[0].ForeColor = System.Drawing.Color.RoyalBlue;
        //        this.chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
        //        设置图表显示样式
        //        this.chart1.Series[0].Color = Color.Red;

        //        this.chart1.Titles[0].Text = string.Format("串口数据显示");
        //        this.chart1.Series[0].ChartType = SeriesChartType.Line;


        //        this.chart1.Series[0].Points.Clear();
        //    }
        //    catch (Exception EX)
        //    { EX.Message.ToString(); }
        //}

        //更新队列中的值
        public  void UpdateQueueValue()
        {

            if (dataQueue.Count > 100)
            {
               // 先出列
                for (int i = 0; i < 100; i++)
                {
                    dataQueue.Dequeue();
                }
            }
            //if (hash.containskey(register_address) == false)
            //{

            //    for (int i = 0; i < 100; i++)
            //    {
            //        dataqueue.enqueue(register_data);
            //    }
            //}
          
        }
    }
}

