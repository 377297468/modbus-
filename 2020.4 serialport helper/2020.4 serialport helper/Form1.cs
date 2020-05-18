using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace _2020._4_serialport_helper
{
    public partial class Form1 : Form
    {
        public Form1()
        {   
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
           
        }
        
        private string[] portNames = null;
        private List<SerialPort> serialPorts;
        private byte[] portBuffer;
        Hashtable hash = new Hashtable();
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();//初始化函数

            Task.Run(() =>
            {
                

                while (true)
                    {   
                        if (send_flag == 1)//发送按钮触发发送标志位
                        {
                            button3_Click(null, new EventArgs());
                        }

                    Thread.Sleep(200);

                    try
                    { 
                        serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//绑定接收事件
                           
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    if (stop_flag == 1)//暂停按钮触发暂停标志位
                     {
                       break;
                      }

                }
              

            });
          
           
           

        }
        ChartClass chartClass;
        static string register_address;
        static string register_data ;
        static double update_register_data;
        static  byte _data;  //通过name这个静态字段间接传递出局部变量data
        /// <summary>
        /// //串口数据接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private   void   port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            string strAscii = null;
            string str1 = null;
            string str2 = null;
         

            if (!radioButton4.Checked)//如果接收模式为字符模式
            {
                string str = serialPort1.ReadExisting();//字符串方式读
                textBox1.AppendText(str);//添加内容
            }
            else//如果接收模式为数值接收
            { 
                int count = serialPort1.BytesToRead;
                Byte[] dataBuff = new Byte[count];

                serialPort1.Read(dataBuff, 0, count);
                
                string ss = null;
                

                //处理寄存器地址
                for (int i = 0; i < count; i++)
                {
                    strAscii += ((char)dataBuff[i]).ToString() + ' '; //转换为字符
                    string As = dataBuff[i].ToString("X");
                    if (As.Length == 1)   //当16进制数只有一位是，在前面补上0；
                        As = "0" +As;
                    if (i == 2 || i == 3)  //第2位和第3位是寄存器地址
                    {
                        register_address += As;
                    }
                    str1 += As + ' ';
                }
                for (int i = 0; i < count; i++)
                {
                    strAscii += ((char)dataBuff[i]).ToString() + ' '; //转换为字符
                    string s = dataBuff[i].ToString("X");
                    if (s.Length == 1)   //当16进制数只有一位是，在前面补上0；
                        s = "0" + s;
                    if (i ==3 || i ==4)//第4位和第5位是寄存器地址
                    {
                        register_data += s;
                    }
                    str2 += s + ' ';
                    
                }
                dataBuff = null;//清空接收数据缓区
                byte[] a= Hex16StringToHex16Byte(register_data);//
                register_data = null;//清空数据缓存
                double update_register_data = 0;
                if (a[1] == 0)//寄存器数据大于256，数据会又问题。暂时没解决
                { update_register_data = a[0] + a[1] * 256; }
                else


                hash[register_address] = update_register_data;//将string转成十六进制再转成十进制
                textBox2.AppendText(hash.OfType<DictionaryEntry>().First(x => x.Key.ToString() == register_address).Value.ToString());
                if (hash.ContainsKey(register_address) == false   )//判断上一次接收的寄存器数据与当前寄存器数据是否一致
                    {
                    //hash[register_address] = Convert.ToInt16(Convert.ToSByte(register_data));
                    hash[register_address] = update_register_data;
                    textBox2.AppendText(hash.OfType<DictionaryEntry>().First(x => x.Key.ToString() == register_address).Value.ToString());
                   
                }

                byte data;
                data = (byte)serialPort1.ReadByte();//此处需要强制类型转换，将(int)类型数据转换为(byte类型数据，不必考虑是否会丢失数据
                string str = Convert.ToString(data, 16).ToUpper();//转换为大写十六进制字符串
                textBox1.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//空位补“0”   
                                                                                      //上一句等同为：if(str.Length == 1)
                                                                                      //                  str = "0" + str;
                                                                                      //              else 
                                                                                      //                  str = str;
                                                                                      //              textBox1.AppendText("0x" + str);
                _data = data;
               //chartClass = new ChartClass(chart1, serialPort1, hash, register_address, update_register_data);
               // initchart();
               // chartClass.addchartdata();

            }

        }

        private void dataaa()
        {
            List<byte> datalist = new List<byte>();
            datalist.Add(_data );
            datalist.IndexOf(5);
            Hashtable hash = new Hashtable();
            hash.Add(datalist.IndexOf(5), datalist.IndexOf(8));
        }



        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            serialPorts = new List<SerialPort>();
            portNames = SerialPort.GetPortNames();
            portBuffer = new byte[1000];
            for (int i = 1; i < 20; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());
            }
            comboBox1.Text = "COM1";//串口号多额默认值
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = 9600;
        }
        /// <summary>
        /// 打开串口按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.Open();
                button1.Enabled = false;//打开串口按钮不可用
                button2.Enabled = true;//关闭串口
            }
            catch
            {
                MessageBox.Show("端口错误,请检查串口", "错误");
            }
        }

        public int send_flag = 0;
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private void button3_Click(object sender, EventArgs ex)
        {
            send_flag = 1;
             SendData();
        }
        private void SendData()
        {
            byte[] DatatoSend = new byte[200];
            //byte[] swbyte = new byte[]{//  写单保持寄存器
            //                          0x01,// 从站地址
            //                          0x06,//功能代码
            //                          0x9c,//写入开始地址，2个字节，高位
            //                          0x42,//写入开始地址，低位
            //                          0x00,//写入寄存器的数据，2个字节，高位
            //                          0x0a,//写入寄存器的数据，低位
            //                          0x87,//校验码低位   crc高低位需要注意，别搞反
            //                          0x89 ,};//校验码高位
            byte[] swbyte = new byte[]{     //读寄存器
                                      0x01,// 从站地址
                                      0x03,//功能代码
                                      0x9C,//读开始地址，2个字节
                                      0x42,
                                      0x00,//读寄存器的数据，2个字节
                                      0x03,
                                      0x8B,//校验码   CRC高低位需要注意，别搞反
                                      0x8F ,};//校验码
            if (serialPort1.IsOpen)
            {
                if (radioButton1.Checked)//发送字符
                {
                    try
                    {
                        serialPort1.WriteLine(textBox1.Text);//写数据 
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("串口数据写入错误", "错误");//出错提示
                        serialPort1.Close();
                        button1.Enabled = true;//打开串口按钮可用
                        button2.Enabled = false;//关闭串口按钮不可用}
                    }
                }
                else if (radioButton2.Checked)   //发送16进制数值
                {
                    try
                    {
                        serialPort1.Write(swbyte, 0, 8);//写数据  //16进制从第一位开始写，写到第八位
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("串口数据写入错误", "错误");//出错提示
                        serialPort1.Close();
                        button1.Enabled = true;//打开串口按钮可用
                        button2.Enabled = false;//关闭串口按钮不可用}
                    }


                }

            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
           
                try
                {
                    serialPort1.Close();//关闭串口
                    button1.Enabled = true;//打开串口按钮可用
                    button2.Enabled = false;//关闭串口按钮不可用
                }
                catch (Exception err)//一般情况下关闭串口不会出错，所以不需要加处理程序
                {

                }
            
        }


        public int stop_flag = 0;//暂停标志位

        /// <summary>
        /// 暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            stop_flag = 1;
        }


        /// <summary>
        /// 此方法用于将16进制的字符串转换成16进制的字节数组
        /// </summary>
        /// <param name="_hex16ToString">要转换的16进制的字符串。</param>
        public static byte[] Hex16StringToHex16Byte(string _hex16String)
        {
            //去掉字符串中的空格。
            _hex16String = _hex16String.Replace(" ", "");
            if (_hex16String.Length / 2 == 0)
            {
                _hex16String += " ";
            }
            //声明一个字节数组，其长度等于字符串长度的一半。
            byte[] buffer = new byte[_hex16String.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                //为字节数组的元素赋值。
                buffer[i] = Convert.ToByte((_hex16String.Substring(i * 2, 2)), 16);
            }
            //返回字节数组。
            return buffer;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        // /// <summary>
        ///// 初始化图表
        /////  </summary>
        //public void initchart()
        //{
        //    try
        //    {
        //        //定义图表区域
        //        this.chart1.ChartAreas.Clear();
        //        ChartArea chartArea1 = new ChartArea("c1");
        //        this.chart1.ChartAreas.Add(chartArea1);
        //        //定义存储和显示点的容器
        //        this.chart1.Series.Clear();
        //        Series series1 = new Series("s1");
        //        series1.ChartArea = "c1";
        //        this.chart1.Series.Add(series1);

        //        this.chart1.ChartAreas[0].AxisY.Minimum = 0;
        //        this.chart1.ChartAreas[0].AxisY.Minimum = 100;
        //        this.chart1.ChartAreas[0].AxisX.Interval = 5;
        //        this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
        //        this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
        //       // 设置标题
        //        this.chart1.Titles.Clear();
        //        this.chart1.Titles.Add("s01");
        //        this.chart1.Titles[0].Text = "xxx显示";
        //        this.chart1.Titles[0].ForeColor = System.Drawing.Color.RoyalBlue;
        //        this.chart1.Titles[0].Font = new System.Drawing.Font("microsoft sans serif", 12f);
        //        //设置图表显示样式
        //        this.chart1.Series[0].Color = Color.Red;

        //        this.chart1.Titles[0].Text = string.Format("串口数据显示");
        //        this.chart1.Series[0].ChartType = SeriesChartType.Line;


        //        this.chart1.Series[0].Points.Clear();
        //    }
        //    catch (Exception ex)
        //    { ex.Message.ToString(); }
        //}
    }
}
