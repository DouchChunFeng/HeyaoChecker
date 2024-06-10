using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HeyaoChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)192 | (System.Net.SecurityProtocolType)768 | (System.Net.SecurityProtocolType)3072;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            Read_Data_FromFile();
        }
        public void Log(string str)
        {
            string result = "\r\n[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + str;
            textBox3.AppendText(result);
            try { File.AppendAllText(Application.StartupPath + @"\logs.txt", result); }
            catch { }
        }
		
        public void Refresh_Data_ToFile()
        {
            string write_to_file = "";
            foreach (ListViewItem ls in listView1.Items)
            {
                write_to_file += ls.SubItems[5].Text + "," + ls.SubItems[6].Text + "\n";
            }
            File.WriteAllText(Application.StartupPath + @"\data.txt", write_to_file);
        }
        public void Read_Data_FromFile()
        {
            if (File.Exists(Application.StartupPath + @"\data.txt"))
            {
                listView1.Items.Clear();
                string[] lines = File.ReadAllLines(Application.StartupPath + @"\data.txt");
                foreach (string s in lines)
                {
                    if (s.Length <= 0) { continue; }
                    string[] split = s.Split(',');
                    ListViewItem ls = new ListViewItem();
                    ls.Text = "#";
                    ls.SubItems.Add("待更新");
                    ls.SubItems.Add("/");
                    ls.SubItems.Add("/");
                    ls.SubItems.Add("/");
                    ls.SubItems.Add(split[0]);
                    ls.SubItems.Add(split[1]);
                    listView1.Items.Add(ls);
                }
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count < 1)
                {
                    添加项目AToolStripMenuItem.Visible = true;
                    编辑该项EToolStripMenuItem.Visible = false;
                    删除该项DToolStripMenuItem.Visible = false;
                }
                else
                {
                    添加项目AToolStripMenuItem.Visible = false;
                    编辑该项EToolStripMenuItem.Visible = true;
                    删除该项DToolStripMenuItem.Visible = true;
                }
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
        }
        private void 添加项目AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl_Form form = new UserControl_Form();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(MousePosition.X, MousePosition.Y);
            form.Text = "添加项目";
            if (form.ShowDialog() == DialogResult.OK)
            {
                ListViewItem ls = new ListViewItem();
                ls.Text = "#";
                ls.SubItems.Add("待更新");
                ls.SubItems.Add("/");
                ls.SubItems.Add("/");
                ls.SubItems.Add("/");
                ls.SubItems.Add(form.input1);
                ls.SubItems.Add(form.input2);
                listView1.Items.Add(ls);
                Refresh_Data_ToFile();
            }
        }
        private void 编辑该项EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem ls = listView1.Items[listView1.SelectedIndices[0]];
            UserControl_Form form = new UserControl_Form();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(MousePosition.X, MousePosition.Y);
            form.Text = "编辑项目";
            form.input1 = ls.SubItems[5].Text;
            form.input2 = ls.SubItems[6].Text;
            if (form.ShowDialog() == DialogResult.OK)
            {
                ls.SubItems[5].Text = form.input1;
                ls.SubItems[6].Text = form.input2;
                Refresh_Data_ToFile();
            }
        }
        private void 删除该项DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定删除?", "你确定", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                listView1.Items.Remove(listView1.Items[listView1.SelectedIndices[0]]);
                Refresh_Data_ToFile();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "启动运行")
            {
                if (textBox2.Text.Length < 1) { MessageBox.Show("无时间设置。", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (listView1.Items.Count < 1) { MessageBox.Show("列表为空。", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                textBox2.Enabled = false;
                comboBox1.Enabled = false;

                switch (comboBox1.SelectedIndex)
                {
                    case 0: //秒
                        timer1.Interval = Int32.Parse(textBox2.Text) * 1000;
                        break;
                    case 1: //分
                        timer1.Interval = Int32.Parse(textBox2.Text) * 60000;
                        break;
                    case 2: //时
                        timer1.Interval = Int32.Parse(textBox2.Text) * 3600000;
                        break;
                }
                timer1.Start();
                label3.Text = "运行中";
                label3.ForeColor = Color.Green;
                Log("已启动检测");
                button1.Text = "停止运行";
                refresh_data();
            }
            else
            {
                textBox2.Enabled = true;
                comboBox1.Enabled = true;
                timer1.Stop();
                label3.Text = "已停止";
                label3.ForeColor = Color.Red;
                Log("已停止检测");
                button1.Text = "启动运行";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            refresh_data();
        }

        public void refresh_data()
        {
            foreach (ListViewItem ls in listView1.Items)
            {
                string data = "wxappAid=3086825&wxappId=101&itemId=103&contentList=%5B%7B%22key%22%3A%22v2%22%2C%22value%22%3A%22" + ls.SubItems[5].Text + "%22%7D%5D";
                string result_data = webRequest.post("https://i.qz.fkw.com/appAjax/wxAppConnectionQuery.jsp?cmd=search", data).Replace("\r\n", "");
                if (result_data == null) { continue; }

                JObject jobj = JObject.Parse(result_data);
                JToken jtoken = jobj["queryDataList"][0]["content"];
                if ( ls.SubItems[3].Text.Length > 1 && !ls.SubItems[3].Text.Equals(jtoken["v5"].ToString(), StringComparison.OrdinalIgnoreCase) )
                {
                    Log(string.Format("批次{0} {4}的头壳有记录更新辣! 状态 {1}=>{2}, {3}", jtoken["v0"].ToString(), ls.SubItems[2].Text, jtoken["v3"].ToString(), jtoken["v4"].ToString(), jtoken["v1"].ToString()));

                    string notify_data = string.Format("title=头壳{0} {2}辣!&desp=## {4}的头壳记录更新辣!\n***\n- 批次: {0}\n- 状态: {1} => {2}\n- 时间: {5}\n- 信息: {3}", jtoken["v0"].ToString(), ls.SubItems[2].Text, jtoken["v3"].ToString(), jtoken["v4"].ToString(), jtoken["v1"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    webRequest.post("https://sctapi.ftqq.com/" + ls.SubItems[6].Text + ".send", notify_data);
                    
                }
                ls.Text = jtoken["v0"].ToString();
                ls.SubItems[1].Text = jtoken["v1"].ToString();
                ls.SubItems[2].Text = jtoken["v3"].ToString();
                ls.SubItems[3].Text = jtoken["v5"].ToString();
                ls.SubItems[4].Text = jtoken["v4"].ToString();
            }
        }

    }
}
