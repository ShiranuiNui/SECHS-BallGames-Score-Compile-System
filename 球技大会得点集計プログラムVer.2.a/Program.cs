﻿//球技大会得点集計プログラム　Ver.2
//Created By ShiranuiNui---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace 球技大会得点集計プログラムVer._2.a
{
    [Serializable()]
    class PointData
    {
        public string GroupLocation { get; set; } = "";
        public int Point { get; set; } = 0;
        public int WinLose { get; set; } = 0;
    }
    class PointData2 : PointData
    {
    }
    [Serializable()]
    class SavingDataBase
    {
        public object VolleyBall;
    }
    class Program
    {
        static readonly string[] CLASSLOOPER = new string[]
        {"1A","1B","1C","1D","1E","1F","1G","1H", "2A", "2B", "2C", "2D", "2E", "2F", "2G", "2H", "3A", "3B", "3C", "3D", "3E", "3F", "3G", "3H" };

        private static Dictionary<string, PointData> VolleyBall;

        static void Main(string[] args)
        {
            Console.WriteLine("～球技大会得点集計プログラムVer.2～");
            DisableCloseButton();
            try
            {
                String CurrentDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string path = CurrentDir + @"\..\球技大会データ.db";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryFormatter f = new BinaryFormatter();
                //読み込んで逆シリアル化する
                SavingDataBase InDataBase = (SavingDataBase)f.Deserialize(fs);
                fs.Close();
                VolleyBall = (Dictionary<string, PointData>)InDataBase.VolleyBall;
            }
            catch
            {
                Console.WriteLine("初期化中……");

                VolleyBall = new Dictionary<string, PointData>();
                for (int i = 0; i < 24; i++)
                {
                    VolleyBall.Add(CLASSLOOPER[i], new PointData());
                }
                Console.WriteLine("初期化処理正常終了！");
            }
            for (;;)
            {
                Console.WriteLine("処理種目別コードを入力し、Enterキーを押してください");
                Console.WriteLine("バレーボール→1");//制作中
                Console.WriteLine("全データの初期化→5");//未実装
                Console.WriteLine("プログラムの終了→6");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraConv.StrIntConv(strInputKey);
                //int intInputKey = 5;//Debug
                switch (intInputKey)
                {
                    case 1:
                        Console.WriteLine();
                        VolleyBallMode();
                        break;
                    case 6:
                        Console.WriteLine();
                        ExitMode();
                        break;
                }
            }
        }
        static void VolleyBallMode()
        {
            Console.WriteLine("バレーボールモードに移行しました。処理別コードを入力してください");
            for (bool IsExit = false; IsExit == false;)
            {
                Console.WriteLine("点数の入力→1");//未実装
                Console.WriteLine("クラスの設定→2");//未実装
                Console.WriteLine("初期化→3");//実装済み
                Console.WriteLine("処理種目別コード入力画面に戻る→4");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraConv.StrIntConv(strInputKey);
                //int intInputKey = 5;//Debug
                switch (intInputKey)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        Console.WriteLine();
                        VolleyBall.Clear();
                        for (int i = 0; i < 24; i++)
                        {
                            VolleyBall.Add(CLASSLOOPER[i], new PointData());
                        }
                        Console.WriteLine("初期化処理正常終了！");
                        Console.WriteLine();
                        break;
                    case 4:
                        IsExit = true;
                        Console.WriteLine();
                        break;
                }
                //VolleyBall["1A"].GroupLocation = "A1";
                //string kekka = VolleyBall.Where(y => y.Value.GroupLocation == "A1").Select(x => x.Key).First();
            }
        }
        static void ExitMode()
        {
            SavingDataBase SavingDataBase = new SavingDataBase();
            SavingDataBase.VolleyBall = VolleyBall;
            String CurrentDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = CurrentDir + @"\..\球技大会データ.db";
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SavingDataBase);
            fs.Close();
            Environment.Exit(0);
        }
        //閉じるボタン無効化API処理↓
        [DllImport("USER32.DLL")]
        private static extern IntPtr
          GetSystemMenu(IntPtr hWnd, UInt32 bRevert);
        [DllImport("USER32.DLL")]
        private static extern UInt32
          RemoveMenu(IntPtr hMenu, UInt32 nPosition, UInt32 wFlags);
        public static void DisableCloseButton()
        {
            UInt32 SC_CLOSE = 0x0000F060;
            UInt32 MF_BYCOMMAND = 0x00000000;

            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            if (hWnd != IntPtr.Zero)
            {
                IntPtr hMenu = GetSystemMenu(hWnd, 0);
                RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
            }
        }
        //ここまで↑
    }

}
static class ShiraConv
{
    public static int StrIntConv(string str)
    {
        str = Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Narrow);
        int outint = int.Parse(str);
        return outint;
    }

}

