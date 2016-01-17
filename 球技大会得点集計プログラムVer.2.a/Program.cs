/*
球技大会得点集計プログラム　Ver.2
Created By ShiranuiNui
最終コミット日：2016/01/14

【現在の製作状況】
・メイン画面：ほぼ完成
・バレーボール得点データ入力画面：未着手（構想は固まりつつある？）
・卓球、バスケットボール、ドッチボール得点データ入力画面：未着手

・バレーボール内部処理：試行錯誤中
・卓球、バスケットボール、ドッチボール内部処理：未着手

・各種目得点データ入出力処理（バイナリデータ）：完成
・各種目得点・順位出力処理（.xlsmファイル）：未着手

【このプログラムが求める物】
１：各コートから来る試合結果を迅速に入力できるようにする
２：入力した結果を自動で計算し、各クラスの得点・順位等を迅速に算出出来るようにする
３：各クラスの得点・順位等を必要な時に.xlsmファイル形式（Excelワークブックファイル）に出力出来るようにする
４：各年度によって競技順や対戦相手が変わっても、再コンパイル無しで対応できるようにする（多分無理）
*/

namespace 球技大会得点集計プログラムVer._2.a
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Diagnostics;

    [Serializable()]
    internal abstract class PointData//得点データ基底クラス
    {
        internal int Point { get; set; } = 0;//総得点 TODO:これを使って決勝リーグ等を判定
        internal int LostPoint { get; set; } = 0;//総失点
        internal int WinLose { get; set; } = 0;//勝敗。+1が勝利、-1が敗北を意味する
    }
    [Serializable()]
    internal class VolleyBall : PointData//バレーボールデータクラス：得点データ基底クラスからの派生クラス
    {
        internal int WinSetCount { get; set; } = 0;//勝ちセット数
        internal int LoseSetCount { get; set; } = 0;//負けセット数
        internal int GetLostRate { get; set; } = 0;//得失セット数
    }
    [Serializable()]
    class TableTennis : PointData
    {
        internal int WinGameCount { get; set; } = 0;//試合勝利数
    }
    [Serializable()]
    class GameTable
    {
        internal string LeftClassName { get; set; } = "";
        internal string RightClassName { get; set; } = "";
        internal bool IsEND { get; set; } = false;
        internal bool IsNotElimination { get; set; } = false;
    }
    [Serializable()]
    class SavingDataBase//セーブ時に一時的にデータをまとめるクラス
    {
        internal Dictionary<string, VolleyBall> VolleyBall;
        internal Dictionary<string, GameTable> VolleyGameTable;
    }
    class Program
    {
        private static readonly string[] CLASSLOOPER = new string[]//クラスをループさせる時に使う定数
        {"1A","1B","1C","1D","1E","1F","1G","1H", "2A", "2B", "2C", "2D", "2E", "2F", "2G", "2H", "3A", "3B", "3C", "3D", "3E", "3F", "3G", "3H" };
        static readonly string[] GAMETABLELOOPER = new string[]
        {"1A","2A","3A","4A","5A","6A","1B","2B","3B","4B","5B","6B","1C","2C","3C","4C","5C","6C","1D","2D","3D","4D","5D","6D",};


        static readonly int[] GAMETABLENUMBER = new int[] { 1, 2, 3, 1 };
        static readonly string[] GAMETABLEALPHABET = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };

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
                VolleyBallMode.VolleyBall = InDataBase.VolleyBall;
                VolleyBallMode.VolleyGameTable = InDataBase.VolleyGameTable;
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
                Console.WriteLine("ドッチボール→4");//未着手
                Console.WriteLine("現在の集計結果の表示→5");//内部データ変更につき凍結中
                Console.WriteLine("全データの初期化→6");//種目追加待ち
                Console.WriteLine("プログラムの終了→7");//実装済み
                string strInputKey = Console.ReadLine();
                int intInputKey = ShiraAuxiliarySys.StrIntConv(strInputKey);
                switch (intInputKey)
                {
                    case 1:
                        VolleyBallMode.SelectSequence();
                        break;
                    case 3:
                        TableTennisMode.SelectSequence();
                        break;
                    case 5:
                        CompileShowMode();
                        break;
                    case 6:
                        InitializeMode();
                        break;
                    case 7:
                        ExitMode();
                        IsExit = true;
                        break;
                    default:
                        break;
                }
            }
        }
        static void CompileShowMode()//総合得点表示モード TODO:種目追加
        {
            Console.WriteLine();
            //List<Dictionary<int, VolleyBall>> list = VolleyBallData.Select(x => x.Value).ToList();
            int[] VolleyPoint = VolleyBallMode.PointOutputForShow();
            Enumerable.Range(0, 24).ToList().ForEach(x => Console.WriteLine("{0} : {1}", CLASSLOOPER[x], VolleyPoint[x]));
            Console.WriteLine();
        }
        static void InitializeMode()//初期化モード TODO:種目追加
        {
            Console.WriteLine();
            VolleyBallMode.InitializeMode();

            Console.WriteLine("初期化処理正常終了！");
            Console.WriteLine();
        }
        static void ExitMode()//終了処理モード
        {
            SavingDataBase SavingDataBase = new SavingDataBase();
            SavingDataBase.VolleyBall = VolleyBallMode.VolleyBall;
            SavingDataBase.VolleyGameTable = VolleyBallMode.VolleyGameTable;
            String CurrentDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = CurrentDir + @"\..\球技大会データ.db";
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SavingDataBase);
            fs.Close();
        }
    }
    internal static class VolleyBallMode
    {
        static readonly string[] VOLLEYBALL2015 = new string[]//去年のクラスのデータ
        {"2A","3E","3H","3F","3B","3A","1A","1F","2E", "3C", "3G", "1D", "3D", "2G", "1H", "1B", "2C","2D","1E","2H","2B","1C","2F","1G", };
        static readonly string[] GROUPALPHABET = new string[]//グループでループさせる時に使う定数
        {"A","B","C","D","E","F","G","H" };
        private static readonly string[] GROUPLOOPER = new string[]//グループでループさせる時に使う定数
        {"A1","A2","A3","B1","B2","B3","C1","C2","C3","D1","D2","D3","E1","E2","E3","F1","F2","F3","G1","G2","G3","H1","H2","H3",};
        private static readonly string[] CLASSLOOPER = new string[]//クラスをループさせる時に使う定数
        {"1A","1B","1C","1D","1E","1F","1G","1H", "2A", "2B", "2C", "2D", "2E", "2F", "2G", "2H", "3A", "3B", "3C", "3D", "3E", "3F", "3G", "3H" };


        internal static Dictionary<string, VolleyBall> VolleyBall { get; set; }//Key=クラス名 Value=バレーボールクラス毎得点データクラス
        internal static Dictionary<string, GameTable> VolleyGameTable { get; set; }//Key=試合コード Ex:（"A1") Value=バレーボール試合毎データクラス

        internal static void SelectSequence()
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
                switch (intInputKey)
                {
                    case 1:
                        UpdateMode();
                        break;
                    case 2:
                        ShowScoreMode();
                        break;
                    case 3:
                        UpdateClassDataMode();
                        break;
                    case 4:
                        InitializeMode();
                        break;
                    case 5:
                        IsExit = true;
                        break;
                }
            }
        }
        private static void UpdateMode()
        {
            string[] GameName = VolleyGameTable.Where(x => x.Value.IsEND == false).OrderBy(x => x.Key.Substring(1)).Select(x => x.Key).ToArray();

            string[] LeftClassName = VolleyGameTable.Where(x => x.Value.IsEND == false).OrderBy(x => x.Key.Substring(1)).Select(x => x.Value.LeftClassName).ToArray();

            string[] RightClassName = VolleyGameTable.Where(x => x.Value.IsEND == false).OrderBy(x => x.Key.Substring(1)).Select(x => x.Value.RightClassName).ToArray();

            for (int j = 0; j < GameName.Count(); j++)
            {

                Console.WriteLine($"{GameName[j].Substring(0, 1)}グループ 第{GameName[j].Substring(1)}試合 {LeftClassName[j]} vs {RightClassName[j]} → {j.ToString()}");
            }
            Console.WriteLine("試合を選択して下さい");//コートの選択
            int inGameNumber = ShiraAuxiliarySys.StrIntConv(Console.ReadLine());

            /*
            //TODO：バレーの場合はセット毎の得点入力が必要。入力後に計算処理も！
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
            */
            Console.WriteLine();
        }
        private static void ShowScoreMode()
        {
            string[] ClassNameSorted = VolleyBall.OrderBy(x => x.Key).Select(x => x.Key).ToArray();
            int[] PointSorted = VolleyBall.OrderBy(x => x.Key).Select(x => x.Value.Point).ToArray();
            Enumerable.Range(0, 24).ToList().ForEach(x => Console.WriteLine("{0} : {1}", ClassNameSorted[x], PointSorted[x]));
            Console.WriteLine();
        }
        private static void UpdateClassDataMode()
        {
            Console.WriteLine("クラスを入力して下さい　例）1A");
            string InputClass = Console.ReadLine();
            InputClass = ShiraAuxiliarySys.StrStrConv(InputClass);
            Console.WriteLine("クラスのグループ番号を入力して下さい");
            string InputGroup = Console.ReadLine();
            InputGroup = ShiraAuxiliarySys.StrStrConv(InputGroup);
            //VolleyBallData[InputGroup].ClassName = InputClass;
            Console.WriteLine();
        }
        internal static void InitializeMode()
        {
            Console.WriteLine("バレーボール内部データ初期化中……");
            if (VolleyBall != null) { VolleyBall.Clear(); }
            if (VolleyGameTable != null) { VolleyGameTable.Clear(); }
            VolleyBall = new Dictionary<string, VolleyBall>();//クラスデータ初期化
            Enumerable.Range(0, 24).ToList().ForEach(x => VolleyBall.Add(CLASSLOOPER[x], new VolleyBall()));

            VolleyGameTable = new Dictionary<string, GameTable>();
            Enumerable.Range(0, 24).ToList().ForEach(x => VolleyGameTable.Add(GROUPLOOPER[x], new GameTable()));
            int i = 0;
            foreach (string GroupName in GROUPALPHABET)//試合データ初期化
            {
                VolleyGameTable[GroupName + "1"].LeftClassName = VolleyGameTable[GroupName + "3"].RightClassName = VOLLEYBALL2015[i];
                VolleyGameTable[GroupName + "1"].RightClassName = VolleyGameTable[GroupName + "2"].LeftClassName = VOLLEYBALL2015[i + 1];
                VolleyGameTable[GroupName + "2"].RightClassName = VolleyGameTable[GroupName + "3"].LeftClassName = VOLLEYBALL2015[i + 2];
                i += 3;
            }
            Console.WriteLine("バレーボール内部データ初期化処理完了！");
            Console.WriteLine();
        }
        internal static int[] PointOutputForShow()
        {
            int[] Point = VolleyBall.Select(x => x.Value.Point).ToArray();
            return Point;
        }
    }
    internal static class TableTennisMode
    {
        internal static Dictionary<string, TableTennis> TableTennis { get; set; }//Key=クラス名 Value=卓球クラス毎得点データクラス
        internal static Dictionary<string, GameTable> TableTennisGameTable { get; set; }//Key=試合コード Ex:（"A1") Value=卓球試合毎データクラス

        internal static void SelectSequence()
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
    }
    internal static class ShiraAuxiliarySys//補助的な処理を詰め込んでるだけ
    {
        internal static int StrIntConv(string str)//文字列を数値型に変換するメソッド。例）"１１" → 11
        {
            str = Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Narrow);
            int outint = int.Parse(str);
            return outint;
        }
        internal static string StrStrConv(string str)//文字列を内部で使う正しい文字列（半角大文字）にするメソッド 例）"ａ１"　→ "A1"
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
        internal static void DisableCloseButton()
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


