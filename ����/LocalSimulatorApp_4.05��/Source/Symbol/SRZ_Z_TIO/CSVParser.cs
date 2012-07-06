using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace YaneLib2010
{
	/// <summary>
	/// CSVなどをparseするクラス。
	/// 
	/// 区切り記号をカンマ以外に変更することも可能。
	/// また、区切り記号をスペースにして、かつ
	/// "c:\program files\" 1 mail
	/// のようにダブルコーテーションで囲むことも可能。
	/// (この場合、 c:\program files\ と 1 と mail の3つの要素に分解される)
	/// 
	/// </summary>
	public class CSVParser
	{
		// 要素の区切り記号。CSV形式であれば","だろう。ディフォルトではそうなっている。
		// 複数設定できる。
		public List<string> element_separators = new List<string>{","};

		// 行の区切り記号。CSV形式であれば改行だろう。ディフォルトではそうなっている。
		// 複数設定できる。
		public List<string> line_separators = new List<string> {"\r\n"};

		// 引用のための記号。CSV形式であれば '"' だろう。ディフォルトではそうなっている。
		// 複数設定できる。quote部分は、同じquote記号が出現するまでがひとつのブロックとして扱われる。
		// また、改行もquote出来る。詳しくはUnitTestのコードを見ること。
		public List<string> quote_strings = new List<string> { "\"" };

		// 空行を無視するのか(default = true)
		public bool ignore_black_line = true;

		// quote記号が閉じていないときに例外を投げるのか(default = false)
		public bool throw_exception = false;

		// ParseFileで読み込むときのファイルのencodingを指定する。
		// defaultではsjis。ExcelではCSVで書き出したときはsjisと決まっているので
		// ここを下手に変更しないほうが無難。
		public Encoding Encode = Encoding.GetEncoding("Shift_JIS");

		/// <summary>
		/// 入力文字列を↑のoption設定に基づきparseして返す。
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public List<List<string>> Parse(string input)
		{
			var ret = new List<List<string>>();

			// 愚直に一文字ずつスキャンしていく。
			var sb = new StringBuilder();
			int isQuote = int.MaxValue; // "…"で囲まれた範囲なのか。その場合、引用記号がquote_stringsの何番目かが入る。
			var line = new List<string>();

			for(int i=0;i< input.Length;)
			{
				if (isQuote == int.MaxValue) // Quote中は要素区切り記号と改行記号は無視する
				{
					// 要素区切り記号か？
					for (int j = 0; j < element_separators.Count; ++j)
					{
						string match = element_separators[j];
						Debug.Assert(match.Length != 0);
						if (i + match.Length > input.Length)
							continue;
						if (input.Substring(i, match.Length) == match)
						{
							// 要素区切り記号が見つかった
							line.Add(sb.ToString());
							i += match.Length;
							sb = new StringBuilder();
							goto Next;
						}
					}

					// 行区切り記号か？
					for (int j = 0; j < line_separators.Count; ++j)
					{
						string match = line_separators[j];
						Debug.Assert(match.Length != 0);
						if (i + match.Length > input.Length)
							continue;
						if (input.Substring(i, match.Length) == match)
						{
							// 行区切り記号が見つかった
							line.Add(sb.ToString());
							i += match.Length;
							sb = new StringBuilder();
							if (ignore_black_line && line.Count == 1 && line[0].Length == 0)
							{
								// 空行を無視するオプションをつけていて、かつ空行なのか？
							}
							else
							{
								ret.Add(line);
							}
							line = new List<string>();
							goto Next;
						}
					}
					// quote記号か？
					// quote中でないので、すべてのquote記号を調べる。
					for (int j = 0; j < quote_strings.Count; ++j)
					{
						string match = quote_strings[j];
						Debug.Assert(match.Length != 0);
						if (i + match.Length > input.Length)
							continue;
						if (input.Substring(i, match.Length) == match)
						{
							// quote記号が見つかった
							i += match.Length;
							isQuote = j;
							goto Next;
						}
					}
				} else {
					// quote記号中なので対応するquote記号に遭遇するかだけチェックする。

					string match = quote_strings[isQuote];
					Debug.Assert(match.Length != 0);
					if (i + match.Length > input.Length)
						continue;
					if (input.Substring(i, match.Length) == match)
					{
						// quote記号が見つかった
						i += match.Length;
						isQuote = int.MaxValue;
						goto Next;
					}
				}
				
				sb.Append(input[i++]); // 一文字追加

			Next:
				;
			}

			// 最後に解析バッファに残っている行を出力して終わり。
			// 上のソースからコピペして無駄な行をコメントアウト
			{
				// 行区切り記号が見つかった
				line.Add(sb.ToString());
			//	i += match.Length;
			//	sb = new StringBuilder();
				if (ignore_black_line && line.Count == 1 && line[0].Length == 0)
				{
					// 空行を無視するオプションをつけていて、かつ空行なのか？
				}
				else
				{
					ret.Add(line);
				}
			//	line = new List<string>();
			//	goto Next;
			}

			if (throw_exception && isQuote != int.MaxValue)
				throw new Exception("quote記号が閉じられていない。");

			return ret;	
		}

		// ストリームから直接parseする
		// csv、Excelで書き出すとSJISなのよね。
		// Stream OpenするときにSJISのencode指定しないとハマる。
		public List<List<string>> Parse(StreamReader sr)
		{

			// そんな大きなCSVを食わせることはないだろうし、
			// quote記号で改行をまたぐときの処理が面倒なので、
			// 全部連結してから渡せばいいんじゃないかと。

			/*
			var ret = new List<List<string>>();
      while(!sr.EndOfStream)
      {
      	var line = sr.ReadLine();
				ret.AddRange(Parse(line));
      }
			return ret;
			 */

			var sb = new StringBuilder();
			while (!sr.EndOfStream)
			{
				sb.AppendLine( sr.ReadLine() );
			}
			return Parse(sb.ToString());
		}

		/// <summary>
		/// ファイル名を指定して直接parseする。
		/// ファイルが存在しないなどの場合、例外が飛ぶ。
		/// ファイル形式はsjisと仮定。(this.Encodeで変更はできる)
		/// </summary>
		/// <remarks>
		/// 取得のテスト
			//var parser = new CSVParser();
			//var result = parser.ParseFile(fileSelector1.FilePath);
			//foreach (var v in result)
			//{
			//  foreach (var val in v)
			//  {
			//    Console.Write(val);
			//  }
			//  Console.WriteLine();
			//}
		/// </remarks>
		/// <param name="path"></param>
		/// <returns></returns>
		public List<List<string>> ParseFile(string path)
		{
			// これ、sjisでもうまくparseしよるんか？

			// Encoding指定しないとutf-8でないとうまくいかない。
			// 英数字しか使ってなかったからutf-8扱いで、たまたまうまく動いてたのか…。
			using (var sr = new StreamReader(path , Encode ))
			{
				return Parse(sr);
			}
		}

		/// <summary>
		/// UnitTest
		/// </summary>
		public static void UnitTest()
		{
			try
			{

				{
					var parser = new CSVParser();
					var ret = parser.Parse("abc,def,123\r\nbcd,efg,234");
					Debug.Assert(ret[1][2] == "234");
				}
				{
					var parser = new CSVParser();
					parser.element_separators = new List<string> {" "}; // スペース区切りに変更してみる
					var ret = parser.Parse("\"c:\\program files\\\" 123 mail");
					Debug.Assert(ret[0][1] == "123");
				}
				{
					var parser = new CSVParser();
					parser.ignore_black_line = true; // 空行を無視してみる
					var ret = parser.Parse("123,abc\r\n\r\n345,XXX,def");
					Debug.Assert(ret[1][2] == "def"); // 2行目の3つ目の要素
				}
				{
					using (var sw = new StreamWriter("test.csv"))
					{
						sw.WriteLine("123,456,789");
						sw.WriteLine("ABC,DEF,GHI");
					}
					var parser = new CSVParser();
					List<List<string>> ret;
					using (var sr = new StreamReader("test.csv"))
					{
						ret = parser.Parse(sr);
					}
					Debug.Assert(ret[1][2] == "GHI"); // 2行目の3つ目の要素
				}
				{
					var parser = new CSVParser();
					parser.quote_strings = new List<string> {"\"", "'"}; // " と ' をquote用の文字列にする。
					var ret = parser.Parse("\"123'456\"'78\"9A\r\nBC'");
					Debug.Assert(ret[0][0] == "123'45678\"9A\r\nBC"); // quote記号は同じquote記号が見つかるまで有効。
				}
				{
					var parser = new CSVParser();
					var ret = parser.Parse("\"123");
					Debug.Assert(ret[0][0] == "123"); // quote記号は同じquote記号が見つかるまで有効。

					try
					{
						parser.throw_exception = true; // 例外を投げる設定にしてみる。quote記号が閉じられていないので例外が飛ぶ。
						ret = parser.Parse("\"123");
						Debug.Assert(false);
					}
					catch
					{
						// 例外が飛ばなければおかしい
						Debug.Assert(true);
					}

				}

			} catch
			{
				Debug.Assert(false); // 例外が飛んできたらおかしい。
			}
		}
	}
}

