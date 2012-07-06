using System;

namespace CommonClassLibrary
{
    public class AbsolutePathGenerator
    {
        public AbsolutePathGenerator(string basePath)
        {
            this.BasePath = basePath;
        }

        string _BasePath;
        public string BasePath
        {
            get
            {
                return _BasePath;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException("value"); }

                _BasePath = value;

                // パスの終端に'\'を付ける
                string separatorStr = new string(new char[] { System.IO.Path.DirectorySeparatorChar });
                if (!value.EndsWith(separatorStr))
                {
                    _BasePath += separatorStr;
                }
            }
        }

        public string ToAbsolutePath(string relativePath)
        {
            Uri baseUri = new Uri(this.BasePath);
            Uri fullUri = new Uri(baseUri, relativePath);

            return FormatToWindowsPath(System.Web.HttpUtility.UrlDecode(fullUri.LocalPath));
        }

        private string FormatToWindowsPath(string path)
        {
            return path.Replace('/', '\\');
        }

        //static public void Test()
        //{
        //    AbsolutePathGenerator generator = new AbsolutePathGenerator(@"c:\hoge");
        //    string testFolderPath = @"..\testdir";
        //    System.Diagnostics.Debug.Assert(generator.ToAbsolutePath(testFolderPath) == @"c:\testdir");

        //    string testFilePath = @".\testdir\test.txt";
        //    System.Diagnostics.Debug.Assert(generator.ToAbsolutePath(testFilePath) == @"c:\hoge\testdir\test.txt");
        //}
    }
}
