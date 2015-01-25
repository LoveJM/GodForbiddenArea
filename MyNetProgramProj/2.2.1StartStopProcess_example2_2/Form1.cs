using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace _2._2._1StartStopProcess_example2_2
{
    public partial class frm_StartStopProcess : Form
    {
        int fileIndex;
        string fileName = "Notepad.exe";
        Process process1 = new Process();
        public frm_StartStopProcess()
        {
            InitializeComponent();
            listView1.View = View.Details;//以详细列表方式展示
            //参数：列名称,宽度(像素),水平对其方式
            listView1.Columns.Add("进程ID", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("进程名称", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("占用内存", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("启动时间", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("文件名", 280, HorizontalAlignment.Left);
        }

        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            string argument = Application.StartupPath + "\\myfile" + fileIndex + ".txt";
            if (File.Exists(argument) == false)//文件不存在则创建一个 
            {
                File.CreateText(argument);
            }
            //设置要启动的应用程序名称及参数
            ProcessStartInfo ps = new ProcessStartInfo(fileName, argument);
            ps.WindowStyle = ProcessWindowStyle.Normal;//启动的应用程序模式为正常窗口
            fileIndex++;
            Process p = new Process();
            p.StartInfo = ps;//指定启动信息
            p.Start();//启动进程
            //等待启动完成，否则获取进程信息可能失败
            p.WaitForInputIdle();
            RefreshListView();
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;//光标为等待的样式
            //创建Process组件数组,并将它们与指定进程名称(Notepad)的所有进程资源相关联
            Process[] myProcess;
            myProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName));
            foreach (Process p in myProcess) 
            {
                //通过向进程主窗口发送关闭消息达到关闭进程的目的
                p.CloseMainWindow();
                //等待1000毫秒
                Thread.Sleep(1000);
                //释放与此组件相关联的所有资源
                p.Close();
            }
            fileIndex = 0;
            RefreshListView();
            this.Cursor = Cursors.Default;//终止进程后,光标转为默认光标
        }
        private void RefreshListView() 
        {
            listView1.Items.Clear();
            //创建process数据,并将它们与系统内所有进程相关联
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName));
            foreach (Process p in processes) 
            {
                //将每个进程的进程名称，占用的物理内存及启动时间加入到listview中
                ListViewItem item = new ListViewItem(
                    new string[]{
                    p.Id.ToString(),//进程ID
                    p.ProcessName,//进程名称
                    string.Format("{0}kb",p.WorkingSet64/1024f),//进程占用的物理内存
                    string.Format("{0}",p.StartTime),//进程的启动时间
                    p.MainModule.FileName//调用的完整文件名
                    }
                    );
                listView1.Items.Add(item);
            }

        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            RefreshListView();
        }
        
    }
}
