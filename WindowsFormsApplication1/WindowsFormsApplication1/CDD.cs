﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    class CDD
    {
        //动态加载dll
        [DllImport("Kernel32")]
        private static extern System.IntPtr LoadLibrary(string dllfile);
        //将dll中的函数实例化出来
        [DllImport("Kernel32")]
        private static extern System.IntPtr GetProcAddress(System.IntPtr hModule, string lpProcName);
        //卸载dll
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        //定义委托,接收dll中的函数
        public delegate int pDD_btn(int btn);
        public delegate int pDD_whl(int whl);
        public delegate int pDD_key(int ddcode, int flag);
        public delegate int pDD_mov(int x, int y);
        public delegate int pDD_movR(int dx, int dy);
        public delegate int pDD_str(string str);
        public delegate int pDD_todc(int vkcode);

        public pDD_btn btn;          // 鼠标点击
        public pDD_whl whl;          // 鼠标滚轮
        public pDD_mov mov;       // 鼠标绝对移动
        public pDD_movR movR;   // 鼠标相对移动
        public pDD_key key;          // 键盘按键
        public pDD_str str;            //  键盘字符
        public pDD_todc todc;      // 标准虚拟键码转DD码

        //增强版功能
        public delegate Int32 pDD_MouseMove(IntPtr hwnd, Int32 x, Int32 y);
        public delegate Int32 pDD_SnapPic(IntPtr hwnd, Int32 x, Int32 y, Int32 w, Int32 h);
        public delegate Int32 pDD_PickColor(IntPtr hwnd, Int32 x, Int32 y, Int32 mode);
        public delegate IntPtr pDD_GetActiveWindow();

        public pDD_MouseMove MouseMove;                           //鼠标移动
        public pDD_SnapPic SnapPic;                                         //  抓图
        public pDD_PickColor PickColor;                                    //取色
        public pDD_GetActiveWindow GetActiveWindow;          //取激活窗口句柄

        private System.IntPtr m_hinst;

        //当类被回收时
        ~CDD()
        {
            if (!m_hinst.Equals(IntPtr.Zero))
            {
                bool b = FreeLibrary(m_hinst);
            }
        }


        public int Load(string dllfile)
        {
            m_hinst = LoadLibrary(dllfile);
            if (m_hinst.Equals(IntPtr.Zero))
            {
                return -2;
            }
            else
            {
                return GetDDfunAddress(m_hinst);
            }
        }

        //取函数地址返回值  -1：取通用函数地址错误 ，  0：仅取通用函数地址正确 ， 1：取通用函数和增强函数地址都正确
        private int GetDDfunAddress(IntPtr hinst)
        {
            IntPtr ptr;

            ptr = GetProcAddress(hinst, "DD_btn");
            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            btn = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_btn)) as pDD_btn;

            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            ptr = GetProcAddress(hinst, "DD_whl");
            whl = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_whl)) as pDD_whl;

            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            ptr = GetProcAddress(hinst, "DD_mov");
            mov = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_mov)) as pDD_mov;

            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            ptr = GetProcAddress(hinst, "DD_key");
            key = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_key)) as pDD_key;

            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            ptr = GetProcAddress(hinst, "DD_movR");
            movR = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_movR)) as pDD_movR;

            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            ptr = GetProcAddress(hinst, "DD_str");
            str = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_str)) as pDD_str;

            if (ptr.Equals(IntPtr.Zero)) { return -1; }
            ptr = GetProcAddress(hinst, "DD_todc");
            todc = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_todc)) as pDD_todc;

            //下面四个函数，只有在增强版中才可用
            ptr = GetProcAddress(hinst, "DD_MouseMove"); //鼠标移动
            if (!ptr.Equals(IntPtr.Zero)) MouseMove = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_MouseMove)) as pDD_MouseMove;

            ptr = GetProcAddress(hinst, "DD_SnapPic");        //抓取图片
            if (!ptr.Equals(IntPtr.Zero)) SnapPic = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_SnapPic)) as pDD_SnapPic;

            ptr = GetProcAddress(hinst, "DD_PickColor");      //取色
            if (!ptr.Equals(IntPtr.Zero)) PickColor = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_PickColor)) as pDD_PickColor;

            ptr = GetProcAddress(hinst, "DD_GetActiveWindow");    //获取激活窗口句柄
            if (!ptr.Equals(IntPtr.Zero)) GetActiveWindow = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_GetActiveWindow)) as pDD_GetActiveWindow;

            if (MouseMove == null || SnapPic == null || PickColor == null || GetActiveWindow == null)
            {
                return 0;
            }

            return 1;
        }
    }
}
