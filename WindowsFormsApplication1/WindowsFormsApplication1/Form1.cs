using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        CDD cdd = null;
        String ip = "192.168.1.110";
        int port = 9112;
        SynchronizationContext _synContext = null;
        //方向
        int fx = 0;

        int key_dian = 200;
        int key_enter = 313;
        int key_a = 401;
        int key_s = 402;
        int key_d = 403;
        int key_w = 302;
        int key_t = 305;
        int key_shit = 500;
        public Form1()
        {
            InitializeComponent();
            _synContext = SynchronizationContext.Current;
        }

        private void start()
        {
            if (cdd.key != null)
            {
                //按下~TURTLE
                //key_down(200, 305, 307, 304, 305, 409, 303, 313);
                key_down(200);
            }
        }

        private void key_down(params int[] keyid)
        {
            foreach (int k in keyid)
            {
                cdd.key(k, 1);
                cdd.key(k, 2);
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //注册热键F1，Id号为100。HotKey.KeyModifiers.None表示没添加任何辅助键
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.None, Keys.F5);
        }

        private void Form1_Leave(object sender, EventArgs e)
        {
            //注销Id号为100的热键设定
            HotKey.UnregisterHotKey(Handle, 100);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键 
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            start();
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            new Thread(StartServer).Start();
            cdd = new CDD();
            //判断系统是32位还是64位
            string dllfile = System.Environment.CurrentDirectory;
            if (Help.WinIs64())
            {
                dllfile += "\\dd64.dll";
            }
            else
            {
                dllfile += "\\dd32.dll";
            }
            LoadDllFile(dllfile);
            label1.Text += "等待连接\r\n\r\n";
        }

        private void LoadDllFile(string dllfile)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(dllfile);
            if (!fi.Exists)
            {
                label1.Text += "加载DLL失败\r\n";
                return;
            }
            label1.Text += "加载DLL成功\r\n\r\n";
            int ret = cdd.Load(dllfile);
            if (ret == -2) 
            {
                label1.Text += "装载库时发生错误\r\n\r\n"; 
                return;
            }
            else
            {
                label1.Text += "装载库成功\r\n\r\n"; 
            }
            if (ret == -1) 
            {
                label1.Text += "取函数地址时发生错误\r\n\r\n"; 
                return;
            }
            else
            {
                label1.Text += "取函数地址成功\r\n\r\n"; 
            }
            if (ret == 0) 
            {
                label1.Text += "非增强模块\r\n\r\n";
            }
            else
            {
                label1.Text += "增强模块\r\n\r\n";
            }

            return;
        }

        private void StartServer()
        {
            IPAddress ipaddress = IPAddress.Parse(ip);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ipaddress, port));
            serverSocket.Listen(10);
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                label1.Text += "连接成功\r\n\r\n";
                ThreadPool.QueueUserWorkItem(new WaitCallback(SocketThread), clientSocket);
            }
        }

        private void SocketThread(Object socket)
        {
            //设置输入输出缓存大小
            Byte[] inbuff = new Byte[1024];
            //获取客户连接
            Socket clientSocket = (Socket)socket;
            //获取客户信息
            IPEndPoint ipEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            string clientaddress = ipEndPoint.Address.ToString();
            string clientport = ipEndPoint.Port.ToString();
            //缓存输入客户发来的信息
            while (true)
            {
                //Random ran = new Random(2);
                //label1.Text = ran.Next(1, 10) + "";
                try
                {
                    clientSocket.Receive(inbuff, 1024, SocketFlags.None);
                    string inbuffstr = Encoding.Default.GetString(inbuff);
                    if (!String.IsNullOrEmpty(inbuffstr.Trim()))
                    {
                        _synContext.Post(SetText, inbuffstr);
                    }
                    inbuff = new Byte[1024];
                }
                catch (Exception e)
                {
                    label1.Text += "连接断开\r\n\r\n";
                    clientSocket.Close();
                    return;
                }
            }
            //关闭连接
            //clientSocket.Close();
        }
        private void SetText(object Text)
        {
            string str = (string)Text;
            textBox1.Text = str.Trim();
            str = textBox1.Text;
            if (str.Length <= 3)
            {
                if (int.TryParse(str,out fx))
                {
                    wsad_up();
                    switch (fx)
                    {
                        case 1:
                            cdd.key(key_shit, 1);
                            cdd.key(key_w, 1);
                            break;
                        case 2:
                            cdd.key(key_w, 1);
                            cdd.key(key_d, 1);
                            break;
                        case 3:
                            cdd.key(key_shit, 1);
                            cdd.key(key_d, 1);
                            break;
                        case 4:
                            cdd.key(key_d, 1);
                            cdd.key(key_s, 1);
                            break;
                        case 5:
                            cdd.key(key_shit, 1);
                            cdd.key(key_s, 1);
                            break;
                        case 6:
                            cdd.key(key_s, 1);
                            cdd.key(key_a, 1);
                            break;
                        case 7:
                            cdd.key(key_shit, 1);
                            cdd.key(key_a, 1);
                            break;
                        case 8:
                            cdd.key(key_a, 1);
                            cdd.key(key_w, 1);
                            break;
                        case 10:
                            cdd.btn(1);
                            Thread.Sleep(10);
                            cdd.btn(2);
                            break;
                    }
                }
            }
            else
            {
                byte[] array = System.Text.Encoding.ASCII.GetBytes(str);

                //~
                cdd.key(key_dian, 1);
                Thread.Sleep(50);
                cdd.key(key_dian, 2);

                int t_num = 0;
                foreach (byte b in array)
                {
                    Thread.Sleep(10);
                    key_down(cdd.todc(b));
                    if (b == 84)
                        t_num++;
                }

                //enter
                Thread.Sleep(50);
                cdd.key(key_enter, 1);
                Thread.Sleep(10);
                cdd.key(key_enter, 2);
                Thread.Sleep(50);
                cdd.key(key_enter, 1);
                Thread.Sleep(10);
                cdd.key(key_enter, 2);

                //t键取消放慢模式
                Thread.Sleep(10);
                if (t_num % 2 != 0)
                {
                    cdd.key(key_t, 1);
                    Thread.Sleep(10);
                    cdd.key(key_t, 2);
                }
            }
        }


        private void wsad_up()
        {
            cdd.key(key_a, 2);
            cdd.key(key_s, 2);
            cdd.key(key_d, 2);
            cdd.key(key_w, 2);
            cdd.key(key_shit, 2);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
