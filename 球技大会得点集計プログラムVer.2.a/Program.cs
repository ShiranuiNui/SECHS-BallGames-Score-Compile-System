//球技大会得点集計プログラム　Ver.2
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
    class PointData//得点データ本体クラス
    {
        public string GroupLocation { get; set; } = "";
        public int Point { get; set; } = 0;
        public int WinLose { get; set; } = 0;
    }
    class PointData2 : PointData
    {
    }
    [Serializable()]
    class GameTable
    {
        public string LeftClassGroup { get; set; }
        public string RightClassGroup { get; set; }
        public bool IsEND { get; set; } = false;
    }
    [Serializable()]
    class SavingDataBase//セーブ時に一時的にデータをまとめるクラス
    {
        public Dictionary<string, PointData> VolleyBall;
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
        static readonly string[] GAMETABLEAUXLOOPER = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        //A1vsA2 A2vsA3 A3vsA1
        private static Dictionary<string, PointData> VolleyBall;
        private static Dictionary<string, GameTable> VolleyGameTable;

        static void Main(string[] args)
        {
            Console.WriteLine("～球技大会得点集計プログラムVer.2～");
            ShiraAuxiliarySys.DisableCloseButton();//閉じるボタンを無効化
            try　//セーブしてあるファイルを読み込む
            {
                String CurrentDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string path = CurrentDir + @"\..\球技大会データ.db";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryFormatter f = new BinaryFormatter();
                SavingDataBase InDataBase = (SavingDataBase)f.Deserialize(fs);
                fs.Close();
                VolleyBall = (Dictionary<string, PointData>)InDataBase.VolleyBall;
                VolleyGameTable = (Dictionary<string,GameTable>)InDataBase.VolleyGameTable;
            }
            catch //セーブしてあるファイルが無かったら初期化して生成
            {
                InitializeMode();
            }
            for (;;)//ここからメインプログラム。無限ループにしてある
            {
                Console.WriteLine("処理種目別コードを入力し、Enterキーを押してください");
                Console.WriteLine("バレーボール→1");//制作中
                Console.WriteLine("現在の集計結果の表示→5");//未実装
                Console.WriteLine("全データの初期化→6");//未実装
                Console.WriteLine("プログラムの終了→7");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraAuxiliarySys.StrIntConv(strInputKey);
                //int intInputKey = 5;//Debug
                switch (intInputKey)
                {
                    case 1:
                        Console.WriteLine();
                        VolleyBallMode();
                        break;
                    case 6:
                        InitializeMode();
                        break;
                    case 7:
                        Console.WriteLine();
                        ExitMode();
                        break;
                }
            }
        }
        static void VolleyBallMode()//バレーボール処理モード
        {
            Console.WriteLine("バレーボールモードに移行しました。処理別コードを入力してください");
            for (bool IsExit = false; IsExit == false;)
            {
                Console.WriteLine("点数の入力→1");//未実装
                Console.WriteLine("現在の集計結果の表示→2");//未実装
                Console.WriteLine("クラスの設定→3");//実装済み
                Console.WriteLine("初期化→4");//実装済み
                Console.WriteLine("処理種目別コード入力画面に戻る→5");//実装済み
                Console.WriteLine();
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraAuxiliarySys.StrIntConv(strInputKey);
                //int intInputKey = 5;//Debug
                switch (intInputKey)
                {
                    case 1:
                        Console.WriteLine("試合を選択して下さい");
                        var GameName = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Key).ToArray();
                        var LeftClassGroup = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Value.LeftClassGroup).ToArray();
                        var RightClassGroup = VolleyGameTable.Where(x => x.Value.IsEND == false).Select(x => x.Value.RightClassGroup).ToArray();
                        //コート（ゲーム名）,左クラス,右クラス
                        for(int i = 0; i < GameName.Count();i++)
                        {
                            Console.WriteLine("{0}コート {1} vs {2} → {3}", GameName[i], LeftClassGroup[i], RightClassGroup[i], i);
                        }
                        break;
                    case 2:
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
                        Console.WriteLine();
                        break;
                }
                //VolleyBall["1A"].GroupLocation = "A1";
                //string kekka = VolleyBall.Where(y => y.Value.GroupLocation == "A1").Select(x => x.Key).First();
                //VolleyBall[VolleyBall.Where(y => y.Value.GroupLocation == "A1").Select(x => x.Key).First()]
            }
        }
        static void InitializeMode()//初期化モード
        {
            if (VolleyBall != null)
            {
                VolleyBall.Clear();
            }
            Console.WriteLine();
            Console.WriteLine("初期化中……");
            VolleyBall = new Dictionary<string, PointData>();
            for (int i = 0; i < 24; i++)
            {
                VolleyBall.Add(CLASSLOOPER[i], new PointData());
                VolleyBall[CLASSLOOPER[i]].GroupLocation = GROUPLOOPER[i];
            }
            VolleyGameTable = new Dictionary<string, GameTable>();
            int iGroupID = 0;
            for (int i = 0; i < 24; i++)
            {
                VolleyGameTable.Add(GAMETABLELOOPER[i], new GameTable());
                VolleyGameTable[GAMETABLELOOPER[i]].LeftClassGroup = GROUPLOOPER[iGroupID + 0];
                VolleyGameTable[GAMETABLELOOPER[i]].RightClassGroup = GROUPLOOPER[iGroupID + 1];
                i++;
                VolleyGameTable.Add(GAMETABLELOOPER[i], new GameTable());
                VolleyGameTable[GAMETABLELOOPER[i]].LeftClassGroup = GROUPLOOPER[iGroupID + 1];
                VolleyGameTable[GAMETABLELOOPER[i]].RightClassGroup = GROUPLOOPER[iGroupID + 2];
                i++;
                VolleyGameTable.Add(GAMETABLELOOPER[i], new GameTable());
                VolleyGameTable[GAMETABLELOOPER[i]].LeftClassGroup = GROUPLOOPER[iGroupID + 2];
                VolleyGameTable[GAMETABLELOOPER[i]].RightClassGroup = GROUPLOOPER[iGroupID + 0];
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
            Environment.Exit(0);
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

