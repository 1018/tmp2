using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace YaneLib2010
{
	/// <summary>
	/// CSV�Ȃǂ�parse����N���X�B
	/// 
	/// ��؂�L�����J���}�ȊO�ɕύX���邱�Ƃ��\�B
	/// �܂��A��؂�L�����X�y�[�X�ɂ��āA����
	/// "c:\program files\" 1 mail
	/// �̂悤�Ƀ_�u���R�[�e�[�V�����ň͂ނ��Ƃ��\�B
	/// (���̏ꍇ�A c:\program files\ �� 1 �� mail ��3�̗v�f�ɕ��������)
	/// 
	/// </summary>
	public class CSVParser
	{
		// �v�f�̋�؂�L���BCSV�`���ł����","���낤�B�f�B�t�H���g�ł͂����Ȃ��Ă���B
		// �����ݒ�ł���B
		public List<string> element_separators = new List<string>{","};

		// �s�̋�؂�L���BCSV�`���ł���Ή��s���낤�B�f�B�t�H���g�ł͂����Ȃ��Ă���B
		// �����ݒ�ł���B
		public List<string> line_separators = new List<string> {"\r\n"};

		// ���p�̂��߂̋L���BCSV�`���ł���� '"' ���낤�B�f�B�t�H���g�ł͂����Ȃ��Ă���B
		// �����ݒ�ł���Bquote�����́A����quote�L�����o������܂ł��ЂƂ̃u���b�N�Ƃ��Ĉ�����B
		// �܂��A���s��quote�o����B�ڂ�����UnitTest�̃R�[�h�����邱�ƁB
		public List<string> quote_strings = new List<string> { "\"" };

		// ��s�𖳎�����̂�(default = true)
		public bool ignore_black_line = true;

		// quote�L�������Ă��Ȃ��Ƃ��ɗ�O�𓊂���̂�(default = false)
		public bool throw_exception = false;

		// ParseFile�œǂݍ��ނƂ��̃t�@�C����encoding���w�肷��B
		// default�ł�sjis�BExcel�ł�CSV�ŏ����o�����Ƃ���sjis�ƌ��܂��Ă���̂�
		// ����������ɕύX���Ȃ��ق�������B
		public Encoding Encode = Encoding.GetEncoding("Shift_JIS");

		/// <summary>
		/// ���͕����������option�ݒ�Ɋ�Â�parse���ĕԂ��B
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public List<List<string>> Parse(string input)
		{
			var ret = new List<List<string>>();

			// �𒼂Ɉꕶ�����X�L�������Ă����B
			var sb = new StringBuilder();
			int isQuote = int.MaxValue; // "�c"�ň͂܂ꂽ�͈͂Ȃ̂��B���̏ꍇ�A���p�L����quote_strings�̉��Ԗڂ�������B
			var line = new List<string>();

			for(int i=0;i< input.Length;)
			{
				if (isQuote == int.MaxValue) // Quote���͗v�f��؂�L���Ɖ��s�L���͖�������
				{
					// �v�f��؂�L�����H
					for (int j = 0; j < element_separators.Count; ++j)
					{
						string match = element_separators[j];
						Debug.Assert(match.Length != 0);
						if (i + match.Length > input.Length)
							continue;
						if (input.Substring(i, match.Length) == match)
						{
							// �v�f��؂�L������������
							line.Add(sb.ToString());
							i += match.Length;
							sb = new StringBuilder();
							goto Next;
						}
					}

					// �s��؂�L�����H
					for (int j = 0; j < line_separators.Count; ++j)
					{
						string match = line_separators[j];
						Debug.Assert(match.Length != 0);
						if (i + match.Length > input.Length)
							continue;
						if (input.Substring(i, match.Length) == match)
						{
							// �s��؂�L������������
							line.Add(sb.ToString());
							i += match.Length;
							sb = new StringBuilder();
							if (ignore_black_line && line.Count == 1 && line[0].Length == 0)
							{
								// ��s�𖳎�����I�v�V���������Ă��āA����s�Ȃ̂��H
							}
							else
							{
								ret.Add(line);
							}
							line = new List<string>();
							goto Next;
						}
					}
					// quote�L�����H
					// quote���łȂ��̂ŁA���ׂĂ�quote�L���𒲂ׂ�B
					for (int j = 0; j < quote_strings.Count; ++j)
					{
						string match = quote_strings[j];
						Debug.Assert(match.Length != 0);
						if (i + match.Length > input.Length)
							continue;
						if (input.Substring(i, match.Length) == match)
						{
							// quote�L������������
							i += match.Length;
							isQuote = j;
							goto Next;
						}
					}
				} else {
					// quote�L�����Ȃ̂őΉ�����quote�L���ɑ������邩�����`�F�b�N����B

					string match = quote_strings[isQuote];
					Debug.Assert(match.Length != 0);
					if (i + match.Length > input.Length)
						continue;
					if (input.Substring(i, match.Length) == match)
					{
						// quote�L������������
						i += match.Length;
						isQuote = int.MaxValue;
						goto Next;
					}
				}
				
				sb.Append(input[i++]); // �ꕶ���ǉ�

			Next:
				;
			}

			// �Ō�ɉ�̓o�b�t�@�Ɏc���Ă���s���o�͂��ďI���B
			// ��̃\�[�X����R�s�y���Ė��ʂȍs���R�����g�A�E�g
			{
				// �s��؂�L������������
				line.Add(sb.ToString());
			//	i += match.Length;
			//	sb = new StringBuilder();
				if (ignore_black_line && line.Count == 1 && line[0].Length == 0)
				{
					// ��s�𖳎�����I�v�V���������Ă��āA����s�Ȃ̂��H
				}
				else
				{
					ret.Add(line);
				}
			//	line = new List<string>();
			//	goto Next;
			}

			if (throw_exception && isQuote != int.MaxValue)
				throw new Exception("quote�L���������Ă��Ȃ��B");

			return ret;	
		}

		// �X�g���[�����璼��parse����
		// csv�AExcel�ŏ����o����SJIS�Ȃ̂�ˁB
		// Stream Open����Ƃ���SJIS��encode�w�肵�Ȃ��ƃn�}��B
		public List<List<string>> Parse(StreamReader sr)
		{

			// ����ȑ傫��CSV��H�킹�邱�Ƃ͂Ȃ����낤���A
			// quote�L���ŉ��s���܂����Ƃ��̏������ʓ|�Ȃ̂ŁA
			// �S���A�����Ă���n���΂����񂶂�Ȃ����ƁB

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
		/// �t�@�C�������w�肵�Ē���parse����B
		/// �t�@�C�������݂��Ȃ��Ȃǂ̏ꍇ�A��O����ԁB
		/// �t�@�C���`����sjis�Ɖ���B(this.Encode�ŕύX�͂ł���)
		/// </summary>
		/// <remarks>
		/// �擾�̃e�X�g
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
			// ����Asjis�ł����܂�parse�����񂩁H

			// Encoding�w�肵�Ȃ���utf-8�łȂ��Ƃ��܂������Ȃ��B
			// �p���������g���ĂȂ���������utf-8�����ŁA���܂��܂��܂������Ă��̂��c�B
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
					parser.element_separators = new List<string> {" "}; // �X�y�[�X��؂�ɕύX���Ă݂�
					var ret = parser.Parse("\"c:\\program files\\\" 123 mail");
					Debug.Assert(ret[0][1] == "123");
				}
				{
					var parser = new CSVParser();
					parser.ignore_black_line = true; // ��s�𖳎����Ă݂�
					var ret = parser.Parse("123,abc\r\n\r\n345,XXX,def");
					Debug.Assert(ret[1][2] == "def"); // 2�s�ڂ�3�ڂ̗v�f
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
					Debug.Assert(ret[1][2] == "GHI"); // 2�s�ڂ�3�ڂ̗v�f
				}
				{
					var parser = new CSVParser();
					parser.quote_strings = new List<string> {"\"", "'"}; // " �� ' ��quote�p�̕�����ɂ���B
					var ret = parser.Parse("\"123'456\"'78\"9A\r\nBC'");
					Debug.Assert(ret[0][0] == "123'45678\"9A\r\nBC"); // quote�L���͓���quote�L����������܂ŗL���B
				}
				{
					var parser = new CSVParser();
					var ret = parser.Parse("\"123");
					Debug.Assert(ret[0][0] == "123"); // quote�L���͓���quote�L����������܂ŗL���B

					try
					{
						parser.throw_exception = true; // ��O�𓊂���ݒ�ɂ��Ă݂�Bquote�L���������Ă��Ȃ��̂ŗ�O����ԁB
						ret = parser.Parse("\"123");
						Debug.Assert(false);
					}
					catch
					{
						// ��O����΂Ȃ���΂�������
						Debug.Assert(true);
					}

				}

			} catch
			{
				Debug.Assert(false); // ��O�����ł����炨�������B
			}
		}
	}
}

