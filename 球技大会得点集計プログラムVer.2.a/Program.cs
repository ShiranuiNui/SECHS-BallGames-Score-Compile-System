﻿//球技大会得点集計プログラム　Ver.2
//Created By ShiranuiNui
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
    class PointData//得点データ基底クラス
    {
        public string GroupLocation { get; set; } = "";//クラスのグループ番号
        public int Point { get; set; } = 0;//総得点
        public int LostPoint { get; set; } = 0;//総失点
        public int WinLose { get; set; } = 0;//勝敗。+1が勝利、-1が敗北を意味する
    }
    [Serializable()]
    class VolleyBall : PointData//バレーボールデータクラス：得点データ基底クラスからの派生クラス
    {
        public int WinSetCount { get; set; } = 0;//勝ちセット数
        public int LoseSetCount { get; set; } = 0;//負けセット数
        public int GetLostRate { get; set; } = 0;//得失セット数
    }
    [Serializable()]
    class TableTennis : PointData
    {
        public int WinGameCount { get; set; } = 0;//試合勝利数
    }
    [Serializable()]
    class GameTable
    {
        public string LeftClassGroup { get; set; }
        public string RightClassGroup { get; set; }
        public bool IsEND { get; set; } = false;
        public bool IsNotElimination { get; set; } = false;
    }
    [Serializable()]
    class SavingDataBase//セーブ時に一時的にデータをまとめるクラス
    {
        public Dictionary<string, VolleyBall> VolleyBall;
        public Dictionary<string, GameTable> VolleyGameTable;
    }
    class Program
    {
        private static readonly string[] CLASSLOOPER = new string[]//クラスをループさせる時に使う定数
        {"1A","1B","1C","1D","1E","1F","1G","1H", "2A", "2B", "2C", "2D", "2E", "2F", "2G", "2H", "3A", "3B", "3C", "3D", "3E", "3F", "3G", "3H" };
        private static readonly string[] GROUPLOOPER = new string[]//グループでループさせる時に使う定数
        {"A1","A2","A3","B1","B2","B3","C1","C2","C3","D1","D2","D3","E1","E2","E3","F1","F2","F3","G1","G2","G3","H1","H2","H3",};
        static readonly string[] GAMETABLELOOPER = new string[]
        {"1A","2A","3A","4A","5A","6A","1B","2B","3B","4B","5B","6B","1C","2C","3C","4C","5C","6C","1D","2D","3D","4D","5D","6D",};

        public static Dictionary<string, VolleyBall> VolleyBall;
        public static Dictionary<string, GameTable> VolleyGameTable;

        static void Main(string[] args)
        {
            Console.WriteLine("～球技大会得点集計プログラムVer.2～");
            ShiraAuxiliarySys.DisableCloseButton();//閉じるボタンを無効化
            try　//セーブしてあるファイルを読み込む TODO:種目追加
            {
                String CurrentDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string path = CurrentDir + @"\..\球技大会データ.db";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryFormatter f = new BinaryFormatter();
                SavingDataBase InDataBase = (SavingDataBase)f.Deserialize(fs);
                fs.Close();
                VolleyBall = InDataBase.VolleyBall;
                VolleyGameTable = InDataBase.VolleyGameTable;
            }
            catch //セーブしてあるファイルが無かったら初期化して生成
            {
                InitializeMode();
            }
            for (bool IsExit = false; IsExit == false;)//ここからメインプログラム。終了処理が行われるまで無限ループにしてある
            {
                Console.WriteLine("処理種目別コードを入力し、Enterキーを押してください");
                Console.WriteLine("バレーボール→1");//制作中
                Console.WriteLine("バスケットボール→2");//未着手
                Console.WriteLine("卓球→3");//未着手
                Console.WriteLine("ドッチボール→4");
                Console.WriteLine("現在の集計結果の表示→5");//種目追加待ち
                Console.WriteLine("全データの初期化→6");//種目追加待ち
                Console.WriteLine("プログラムの終了→7");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraAuxiliarySys.StrIntConv(strInputKey);
                //int intInputKey = 5;//Debug
                switch (intInputKey)
                {
                    case 1:
                        VolleyBallMode();
                        break;
                    case 3:
                        TableTennis();
                        break;
                    case 5:
                        CompileShowMode();
                        break;
                    case 6:
                        InitializeMode();
                        break;
                    case 7:
                        Console.WriteLine();
                        ExitMode();
                        IsExit = true;
                        break;
                    case 8:
                        DebugMode.VolleyBallSet2015();
                        break;
                    default:
                        break;

                }
            }
        }
        static void VolleyBallMode()//バレーボール処理モード TODO:集計の実装、クラス表示バグの修正
        {
            Console.WriteLine();
            Console.WriteLine("バレーボールモードに移行しました。処理別コードを入力してください");
            for (bool IsExit = false; IsExit == false;)
            {
                Console.WriteLine("点数の入力→1");//詳細情報待ち
                Console.WriteLine("現在の集計結果の表示→2");//詳細情報、上の完成待ち
                Console.WriteLine("クラスの設定→3");//実装済み
                Console.WriteLine("初期化→4");//実装済み
                Console.WriteLine("処理種目別コード入力画面に戻る→5");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraAuxiliarySys.StrIntConv(strInputKey);
                Console.WriteLine();
                //int intInputKey = 5;//Debug
                switch (intInputKey)
                {
                    case 1:

                        //string[] GameName = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Key).ToArray();//コート名の取得
                        string[] GameName = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Key).ToArray();
                        string[] LeftClassGroup = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Value.LeftClassGroup).ToArray();
                        List<string> listLeftClassName = new List<string>();//クラス名の取得
                        foreach (string strLeftClassPosition in LeftClassGroup)
                        {
                            listLeftClassName.Add(VolleyBall.Where(x => x.Value.GroupLocation == strLeftClassPosition).Select(x => x.Key).First());
                        }

                        string[] RightClassGroup = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Value.RightClassGroup).ToArray();
                        List<string> listRightClassName = new List<string>();//クラス名の取得
                        foreach (string strRightClassPosition in RightClassGroup)
                        {
                            listRightClassName.Add(VolleyBall.Where(x => x.Value.GroupLocation == strRightClassPosition).Select(x => x.Key).First());
                        }

                        for (int i = 0; i < GameName.Count(); i++)
                        {

                            Console.WriteLine("{0}コート 第{1}試合 {2} vs {3} → {4}", GameName[i].Substring(1), GameName[i].Substring(0, 1), listLeftClassName[i], listRightClassName[i], i);
                        }
                        Console.WriteLine("試合を選択して下さい");//コートの選択
                        int inGameNumber = ShiraAuxiliarySys.StrIntConv(Console.ReadLine());

                        Console.WriteLine("試合選択をやり直す場合は0を入力して下さい");
                        Console.WriteLine("{0}の点数を入力してください", listLeftClassName[inGameNumber]);
                        int LeftClassScore = ShiraAuxiliarySys.StrIntConv(Console.ReadLine());//点数の入力
                        if (LeftClassScore == 0) { Console.WriteLine(); break; }
                        VolleyBall[listLeftClassName[inGameNumber]].Point += LeftClassScore;

                        Console.WriteLine("{0}の点数を入力してください", listRightClassName[inGameNumber]);
                        int RightClassScore = ShiraAuxiliarySys.StrIntConv(Console.ReadLine());//点数の入力
                        if (LeftClassScore == 0) { Console.WriteLine(); break; }
                        VolleyBall[listRightClassName[inGameNumber]].Point += RightClassScore;

                        Console.WriteLine("入力処理が正常に終了しました");
                        VolleyGameTable[GameName[inGameNumber]].IsEND = true;
                        Console.WriteLine();
                        break;
                    case 2:
                        foreach (string Class in CLASSLOOPER)
                        {
                            Console.WriteLine("{0} : {1}", Class, VolleyBall[Class].Point);
                        }
                        Console.WriteLine();
                        break;
                    case 3:
                        Console.WriteLine("クラスを入力して下さい　例）1A");
                        string InputClass = Console.ReadLine();
                        InputClass = ShiraAuxiliarySys.StrStrConv(InputClass);
                        Console.WriteLine("クラスのグループ番号を入力して下さい");
                        string InputGroup = Console.ReadLine();
                        InputGroup = ShiraAuxiliarySys.StrStrConv(InputGroup);
                        VolleyBall[InputClass].GroupLocation = InputGroup;
                        Console.WriteLine();
                        break;
                    case 4:
                        InitializeMode();
                        break;
                    case 5:
                        IsExit = true;
                        break;
                }
                //VolleyBall["1A"].GroupLocation = "A1";
                //string kekka = VolleyBall.Where(y => y.Value.GroupLocation == "A1").Select(x => x.Key).First();
                //VolleyBall[VolleyBall.Where(y => y.Value.GroupLocation == "A1").Select(x => x.Key).First()]
            }
        } //TODO:点数入力の完成
        static void TableTennis()
        {
            Console.WriteLine();
            Console.WriteLine("卓球モードに移行しました。処理別コードを入力してください");
            for (bool IsExit = false; IsExit == false;)
            {
                Console.WriteLine("点数の入力→1");//詳細情報待ち
                Console.WriteLine("現在の集計結果の表示→2");//詳細情報、上の完成待ち
                Console.WriteLine("クラスの設定→3");//実装済み
                Console.WriteLine("初期化→4");//実装済み
                Console.WriteLine("処理種目別コード入力画面に戻る→5");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraAuxiliarySys.StrIntConv(strInputKey);
                Console.WriteLine();
            }

        }
        static void CompileShowMode()//総合得点表示モード TODO:種目追加
        {
            Console.WriteLine();
            Dictionary<string, int> CompiledData = new Dictionary<string, int>();
            foreach (string Class in CLASSLOOPER)
            {
                CompiledData.Add(Class, 0);
                CompiledData[Class] += VolleyBall[Class].Point;
                Console.WriteLine("{0} : {1}", Class, CompiledData[Class]);//TODO:種目が追加されたらここも追加
            }
            Console.WriteLine();
        }
        static void InitializeMode()//初期化モード TODO:種目追加
        {
            if (VolleyBall != null)
            {
                VolleyBall.Clear();
            }
            Console.WriteLine();
            Console.WriteLine("初期化中……");
            VolleyBall = new Dictionary<string, VolleyBall>();
            for (int i = 0; i < 24; i++)
            {
                VolleyBall.Add(CLASSLOOPER[i], new VolleyBall());
                VolleyBall[CLASSLOOPER[i]].GroupLocation = GROUPLOOPER[i];
            }
            VolleyGameTable = new Dictionary<string, GameTable>();
            string[] ABCDSortedGroupLooper = GROUPLOOPER.OrderBy(x => x.Substring(0, 1)).ToArray();
            int iGroupID = 0;
            for (int i = 0; i < 24; i++)
            {
                VolleyGameTable.Add(GAMETABLELOOPER[i], new GameTable());
                VolleyGameTable[GAMETABLELOOPER[i]].LeftClassGroup = ABCDSortedGroupLooper[iGroupID + 0];
                VolleyGameTable[GAMETABLELOOPER[i]].RightClassGroup = ABCDSortedGroupLooper[iGroupID + 1];
                i++;
                VolleyGameTable.Add(GAMETABLELOOPER[i], new GameTable());
                VolleyGameTable[GAMETABLELOOPER[i]].LeftClassGroup = ABCDSortedGroupLooper[iGroupID + 1];
                VolleyGameTable[GAMETABLELOOPER[i]].RightClassGroup = ABCDSortedGroupLooper[iGroupID + 2];
                i++;
                VolleyGameTable.Add(GAMETABLELOOPER[i], new GameTable());
                VolleyGameTable[GAMETABLELOOPER[i]].LeftClassGroup = ABCDSortedGroupLooper[iGroupID + 2];
                VolleyGameTable[GAMETABLELOOPER[i]].RightClassGroup = ABCDSortedGroupLooper[iGroupID + 0];
                iGroupID += 3;
            }
            Console.WriteLine("初期化処理正常終了！");
            Console.WriteLine();
        }
        static void ExitMode()//終了処理モード
        {
            SavingDataBase SavingDataBase = new SavingDataBase();
            SavingDataBase.VolleyBall = VolleyBall;
            SavingDataBase.VolleyGameTable = VolleyGameTable;
            String CurrentDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = CurrentDir + @"\..\球技大会データ.db";
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SavingDataBase);
            fs.Close();
        }
    }
}
static class ShiraAuxiliarySys//補助的な処理を詰め込んでるだけ
{
    public static int StrIntConv(string str)//文字列を数値型に変換するメソッド。例）"１１" → 11
    {
        str = Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Narrow);
        int outint = int.Parse(str);
        return outint;
    }
    public static string StrStrConv(string str)//文字列を内部で使う正しい文字列（半角大文字）にするメソッド 例）"ａ１"　→ "A1"
    {
        str = Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Narrow);
        str = str.ToUpper();
        return str;
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
static class DebugMode
{
    public static void VolleyBallSet2015()
    {
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2A"].GroupLocation = "A1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3E"].GroupLocation = "A2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3H"].GroupLocation = "A3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3F"].GroupLocation = "B1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3B"].GroupLocation = "B2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3A"].GroupLocation = "B3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1A"].GroupLocation = "C1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1F"].GroupLocation = "C2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2E"].GroupLocation = "C3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3C"].GroupLocation = "D1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3G"].GroupLocation = "D2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1D"].GroupLocation = "D3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["3D"].GroupLocation = "E1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2G"].GroupLocation = "E2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1H"].GroupLocation = "E3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1B"].GroupLocation = "F1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2C"].GroupLocation = "F2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2D"].GroupLocation = "F3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1E"].GroupLocation = "G1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2H"].GroupLocation = "G2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2B"].GroupLocation = "G3";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1C"].GroupLocation = "H1";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["2F"].GroupLocation = "H2";
        球技大会得点集計プログラムVer._2.a.Program.VolleyBall["1G"].GroupLocation = "H3";
    }
}

