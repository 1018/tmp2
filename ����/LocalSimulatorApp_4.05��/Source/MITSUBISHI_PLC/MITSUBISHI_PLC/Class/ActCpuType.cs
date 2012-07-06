using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace MITSUBISHI_PLC
{
    public class ActCpuTypeMember
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pcSeries">PCシリーズ</param>
        /// <param name="pcName">PCタイプ</param>
        /// <param name="propertyCode">プロパティコード</param>
        /// <param name="typeCode">タイプコード1</param>
        /// <param name="simTypeCode">タイプコード2</param>
        public ActCpuTypeMember(
            string pcSeries, string pcName, ushort? propertyCode, ushort? typeCode, ushort? simTypeCode)
        {
            this.series = pcSeries;
            this.name = pcName;
            this.propertyCode = propertyCode;
            this.typeCode = typeCode;
            this.simTypeCode = simTypeCode;
        }


        string series;
        string name;
        ushort? propertyCode;
        ushort? typeCode;
        ushort? simTypeCode;


        /// <summary>
        /// CPUのシリーズ名を取得する
        /// </summary>
        public string Series
        {
            get
            {
                return this.series;
            }
        }

        /// <summary>
        /// CPUの機種名を取得する
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 各Actコントロールの通信対象CPUのプロパティ値を取得する。
        /// 
        /// プロパティ値がデータベースに入力されていないとき、この値はnullになる。
        /// </summary>
        /// <remarks>
        /// ActControl.ActCpuTypeプロパティに、この値を設定する。
        /// </remarks>
        public ushort? PropertyCode
        {
            get
            {
                return this.propertyCode;
            }
        }

        /// <summary>
        /// 各ActコントロールのGetCpuTypeメソッドにて取得される型名コードを取得する。
        /// (CPU/自ボード 接続時)
        ///  
        /// 型名コードがデータベースに入力されていないとき、この値はnullになる。
        /// </summary>
        public ushort? TypeCode
        {
            get
            {
                return this.typeCode;
            }
        }

        /// <summary>
        /// 各ActコントロールのGetCpuTypeメソッドにて取得される型名コードを取得する。
        /// (GX-Simulator 接続時)
        /// 
        /// 型名コードがデータベースに入力されていないとき、この値はnullになる。
        /// </summary>
        public ushort? SimTypeCode
        {
            get
            {
                return this.simTypeCode;
            }
        }


        /// <summary>
        /// 全ての項目の内容がデータベースに入力されているかを取得する。
        /// </summary>
        public bool IsCompletenes
        {
            get
            {
                return (PropertyCode.HasValue && TypeCode.HasValue && SimTypeCode.HasValue);
            }
        }
    }


    public static class ActCpuType
    {
        /* (データベースのKeyはCPU名を想定) */

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static ActCpuType()
        {
            List<ActCpuTypeMember> dataBaseList = new List<ActCpuTypeMember>();

            // リソース内のデータベースファイル
            string resourceText = global::MITSUBISHI_PLC.Properties.Resources.ActCpuTypeDataBase;
            string[] textLines = resourceText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string TextLine in textLines)
            {
                string[] csv = TextLine.Split(',');

                string cpuSeries;       // シリーズ名
                string cpuName;         // CPU名
                ushort? propertyCode;   // プロパティコード
                ushort? typeCode;       // タイプコード(CPU/自ボード接続時)
                ushort? simTypeCode;    // タイプコード(GX Simulator接続時)

                // 項目数は5
                if (csv.Length != 5) { return; }

                // シリーズ名
                cpuSeries = csv[0];

                // CPU名
                cpuName = csv[1];

                // プロパティコード
                ushort tmpPropertyCode;
                if (ushort.TryParse(csv[2], System.Globalization.NumberStyles.HexNumber, null, out tmpPropertyCode))
                {
                    propertyCode = tmpPropertyCode;
                }
                else
                {
                    propertyCode = null;
                }

                // タイプコード(CPU/自ボード接続時)
                ushort tmpTypeCode;
                if (ushort.TryParse(csv[3], System.Globalization.NumberStyles.HexNumber, null, out tmpTypeCode))
                {
                    typeCode = tmpTypeCode;
                }
                else
                {
                    typeCode = null;
                }

                // タイプコード(GX Simulator接続時)
                ushort tmpSimTypeCode;
                if (ushort.TryParse(csv[4], System.Globalization.NumberStyles.HexNumber, null, out tmpSimTypeCode))
                {
                    simTypeCode = tmpSimTypeCode;
                }
                else
                {
                    simTypeCode = null;
                }


                // データベースに追加
                ActCpuTypeMember Data = new ActCpuTypeMember(
                    cpuSeries, cpuName, propertyCode, typeCode, simTypeCode);

                dataBaseList.Add(Data);
            }

            dataBase = dataBaseList.ToArray();
        }


        readonly static IEnumerable<ActCpuTypeMember> dataBase;


        public static IEnumerable<ActCpuTypeMember> GetEnumerable()
        {
            return dataBase;
        }

    }
}
