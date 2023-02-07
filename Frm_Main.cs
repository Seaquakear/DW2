using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace DW
{
    public partial class Frm_Main : Form
    {
        public static string licence = "";
        public Frm_Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.label2.Text = "当前licence:" + GetLicenceFromAppConfig();
        }

        private void button_getdata_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private string GetLicenceFromAppConfig()
        {
            licence = ConfigurationManager.AppSettings["licence"];
            return licence;
        }

        private static string HttpGet(string Url)
        {
            try
            {
                string retString = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json;charset=utf-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetData()
        {
            try
            {
                this.listView1.Items.Clear();
                string nowTime = DateTime.Now.ToString("yyyy-MM-dd");
                string url = "http://api.mycodenobug.com/api/oneparam/qsgc?datetime=" + nowTime  + "&licence=" + licence;

                //get请求获取数据
                string result = HttpGet(url);
                if (result.Length > 3)
                {
                    var data = JsonConvert.DeserializeObject<List<QSGC>>(result);
                    ListViewItem item;
                    string kk = data[0].cje;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i].dm.ToString().Substring(0, 3) != "sz3" && data[i].dm.ToString().Substring(0, 5) != "sh688")
                        {
                            item = new ListViewItem();
                            item.SubItems.Add(data[i].dm);
                            item.SubItems.Add(data[i].mc);
                            item.SubItems.Add(data[i].p);
                            item.SubItems.Add(data[i].ztp);
                            item.SubItems.Add(data[i].zf);
                            item.SubItems.Add(data[i].nh);
                            item.SubItems.Add(data[i].cje);
                            item.SubItems.Add(data[i].lb);
                            item.SubItems.Add(data[i].hs);
                            listView1.Items.Add(item);
                        }
                    }

                    //for (int i = 0; i < data.Count; i++)
                    //{
                    //    item = new ListViewItem();
                    //    item.SubItems.Add(data[i].dm);
                    //    item.SubItems.Add(data[i].mc);
                    //    item.SubItems.Add(data[i].p);
                    //    item.SubItems.Add(data[i].ztp);
                    //    item.SubItems.Add(data[i].zf);
                    //    item.SubItems.Add(data[i].nh);
                    //    item.SubItems.Add(data[i].cje);
                    //    item.SubItems.Add(data[i].lb);
                    //    item.SubItems.Add(data[i].hs);
                    //    listView1.Items.Add(item);
                    //}
                }
                else
                {
                    MessageBox.Show("未到监控时间段，请在交易日9点50分后使用该程序！");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string gp_code = listView1.SelectedItems[0].SubItems[1].Text;
                FillPicture1(gp_code);
            }
        }

        private void FillPicture1(string gp_code)
        {
            string url = "https://image.sinajs.cn/newchart/daily/n/" + gp_code + ".gif";
            System.Net.WebClient _Client = new System.Net.WebClient();
            byte[] _ImageByte = _Client.DownloadData(url);
            System.IO.MemoryStream _ImageStrem = new System.IO.MemoryStream(_ImageByte);
            Image _DownImage = Image.FromStream(_ImageStrem);
            this.pictureBox1.Image = _DownImage;
        }

        public class QSGC
        {
            /// <summary>
            /// 代码
            /// </summary>
            public string dm { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            public string mc { get; set; }

            /// <summary>
            /// 价格
            /// </summary>
            public string p { get; set; }

            /// <summary>
            /// 涨停价
            /// </summary>
            public string ztp { get; set; }
            /// <summary>
            /// 涨幅
            /// </summary>
            public string zf { get; set; }
            /// <summary>
            /// 成交额
            /// </summary>
            public string cje { get; set; }

            /// <summary>
            /// 是否新高
            /// </summary>
            public string nh { get; set; }

            /// <summary>
            /// 量比
            /// </summary>
            public string lb { get; set; }

            /// <summary>
            /// 换手率
            /// </summary>
            public string hs { get; set; }
        }
    }
}
