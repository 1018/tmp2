using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using CommonClassLibrary;
using System.Windows.Forms;
using System.Windows;


namespace Transport3DClassLibrary
{  
    /// <summary>
    /// 搬送3D共通インタフェース
    /// </summary>
    public class Transport3D_Draw : Symbol_Draw
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Transport3D_Draw()
        {    
            InitializeComponent();

            base.HeightStretch = true;
            base.WidthStretch = true;

            Location3D.ValueChanged += new EventHandler(SetLocation);
            Offset.ValueChanged += new EventHandler(SetLocation);
            Size3D.ValueChanged += new EventHandler(SetSize);
            DrawView.ValueChanged += new EventHandler(SetLocation);
            DrawView.ValueChanged += new EventHandler(SetSize);
        }       

        #endregion

        #region メンバ
        private bool lockSetSizeMethod = false;
        private bool lockSetLocationMethod = false;
        private bool lockSizeChangedMethod = false;
        private bool lockLocationChangedMethod = false;

        #endregion

        #region メソッド
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Transport3D_Draw
            // 
            this.Name = "Transport3D_Draw";
            this.Load += new System.EventHandler(this.Transport3D_Draw_Load);            
            
            this.ResumeLayout(false);
        }        

        #endregion

        #region プロパティ

        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        public new Point Location
        {
            get { return base.Location; }
            set { base.Location = value; }
        }
        //[Visible(true), DisplayName("衝突判定")]
        public bool HitJudge { get; set; }

        //public TransportTypeEnum TransportType { get; set; }


        double _ViewScale = 1;
        [Visible(true), DisplayName("縮尺"), Category("配置")]
        public double ViewScale
        {
            get { return _ViewScale; }
            set
            {
                _ViewScale = value;
                SetLocation(null, null);
                SetSize(null, null);

            }
        }
        Point3DFormat _Offset = new Point3DFormat(0, 0, 0);
        [Visible(true), DisplayName("位置オフセット"), TypeConverter(typeof(NestConverter)), Category("配置")]
        public Point3DFormat Offset
        {
            get { return _Offset; }
            set
            {
                _Offset = value;
                Offset.ValueChanged += new EventHandler(SetLocation);
                SetLocation(null, null);
            }
        }

        Point3DFormat _Location3D = new Point3DFormat(100, 100, 100);
        [Visible(true), DisplayName("位置"), TypeConverter(typeof(NestConverter)), Category("配置")]
        public Point3DFormat Location3D
        {
            get { return _Location3D; }
            set
            {
                _Location3D = value;
                Location3D.ValueChanged += new EventHandler(SetLocation);
                SetLocation(null, null);
            }
        }

        Size3DFormat _Size3D = new Size3DFormat(100, 10, 100);
        [Visible(true), DisplayName("サイズ"), TypeConverter(typeof(NestConverter)), Category("配置")]
        public Size3DFormat Size3D
        {
            get { return _Size3D; }
            set
            {
                _Size3D = value;
                Size3D.ValueChanged += new EventHandler(SetSize);
                SetSize(null, null);
            }
        }

        DrawViewFormat _DrawView = new DrawViewFormat(Draw3DEnum.X, Draw3DEnum.Y);
        [Visible(true), DisplayName("描画使用座標指定"), TypeConverter(typeof(NestConverter)),]
        public DrawViewFormat DrawView
        {
            get
            {
                return _DrawView;
            }
            set
            {
                _DrawView = value;
                DrawView.ValueChanged += new EventHandler(SetLocation);
                DrawView.ValueChanged += new EventHandler(SetSize);
                SetLocation(null, null);
                SetSize(null, null);
            }
        }

        #endregion

        #region イベント

        private void ParentForm_Scroll(object sender, ScrollEventArgs e)
        {
            SetLocation(null, null);
        }

        /// <summary>
        /// 3Dロケーションの変更イベント
        /// </summary>
        private void SetLocation(object sender, EventArgs e)
        {
            int autoScroll_X = 0;
            int autoScroll_Y = 0;
            if (ParentForm != null)
            {
                autoScroll_X = ParentForm.AutoScrollPosition.X;
                autoScroll_Y = ParentForm.AutoScrollPosition.Y;
            }

            if (lockSetLocationMethod) { return; }
            //Console.WriteLine("SetLocation");
            lockLocationChangedMethod = true;
            int locationX = (int)((double)(Location3D[(int)this.DrawView.XView] + Offset[(int)this.DrawView.XView]) / this.ViewScale);
            int locationY = (int)((double)(Location3D[(int)this.DrawView.YView] + Offset[(int)this.DrawView.YView]) / this.ViewScale);
            Location = new Point(locationX + autoScroll_X, locationY + autoScroll_Y);
            lockLocationChangedMethod = false;
        }

        /// <summary>
        /// 3Dサイズの変更イベント
        /// </summary>
        private void SetSize(object sender, EventArgs e)
        {
            if (lockSetSizeMethod) { return; }
            //Console.WriteLine("SetSize");
            lockSizeChangedMethod = true;
            int SizeX = (int)((double)Size3D[(int)this.DrawView.XView] / this.ViewScale);
            int SizeY = (int)((double)Size3D[(int)this.DrawView.YView] / this.ViewScale);
            Size = new Size(SizeX, SizeY);
            lockSizeChangedMethod = false;
        }

        /// <summary>
        /// ロケーションの変更イベント
        /// </summary>
        private void Transport3D_Draw_LocationChanged(object sender, EventArgs e)
        {
            if (lockLocationChangedMethod) { return; }
            lockSetLocationMethod = true;
            //Console.WriteLine("LocationChanged");
            this.Location3D[(int)this.DrawView.XView] = (int)((double)Location.X * this.ViewScale - (double)Offset[(int)this.DrawView.XView]);
            this.Location3D[(int)this.DrawView.YView] = (int)((double)Location.Y * this.ViewScale - (double)Offset[(int)this.DrawView.YView]);
            lockSetLocationMethod = false;
        }

        /// <summary>
        /// サイズの変更イベント
        /// </summary>
        private void Transport3D_Draw_SizeChanged(object sender, EventArgs e)
        {
            if (lockSizeChangedMethod) { return; }
            lockSetSizeMethod = true;
            //Console.WriteLine("SizeChanged");
            this.Size3D[(int)this.DrawView.XView] = (int)((double)Size.Width * this.ViewScale);
            this.Size3D[(int)this.DrawView.YView] = (int)((double)Size.Height * this.ViewScale);
            lockSetSizeMethod = false;
        }

        private void Transport3D_Draw_Load(object sender, EventArgs e)
        {
            SetLocation(null, null);
            SetSize(null, null);

            if (!Global.DesignMode)
            {
                if (ParentForm != null)
                {
                    ParentForm.Scroll += new ScrollEventHandler(ParentForm_Scroll);
                }
            }
            else
            {
                this.LocationChanged += new System.EventHandler(this.Transport3D_Draw_LocationChanged);
                this.SizeChanged += new System.EventHandler(this.Transport3D_Draw_SizeChanged);
            }
		
        }

        #endregion

        public enum TransportTypeEnum { Work, Conveyor, Etc };

        public enum Draw3DEnum { X, Y, Z };

        /// <summary>
        /// 3D座標構造体クラス
        /// </summary>
        public class Point3DFormat
        {
            //コンストラクタ
            public Point3DFormat(int x, int y, int z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
            public Point3DFormat() { }

            //インデクサにより整数でのプロパティ指定を可能にする
            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return X;
                        case 1:
                            return Y;
                        case 2:
                            return Z;
                        default:
                            return 0;
                    }

                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            X = value;
                            break;
                        case 1:
                            Y = value;
                            break;
                        case 2:
                            Z = value;
                            break;
                    }

                }

            }

            private int value_X = 10;
            public int X
            {
                get { return value_X; }
                set
                {
                    value_X = value;
                    OnValueChanged(null);
                }
            }

            private int value_Y = 10;
            public int Y
            {
                get { return value_Y; }
                set
                {
                    value_Y = value;
                    OnValueChanged(null);
                }
            }

            private int value_Z = 10;
            public int Z
            {
                get { return value_Z; }
                set
                {
                    value_Z = value;
                    OnValueChanged(null);
                }
            }

            public event EventHandler ValueChanged;

            protected virtual void OnValueChanged(EventArgs e)
            {
                if (ValueChanged != null)
                    ValueChanged(this, e);
            }

            public override string ToString()
            {
                return "{" + string.Format("X = {0} Y = {1} Z = {2}", X, Y, Z) + "}";
            }
        }

        /// <summary>
        /// 3Dサイズ構造体クラス
        /// </summary>
        public class Size3DFormat
        {
            //コンストラクタ
            public Size3DFormat(int width, int height, int depth)
            {
                this.Width = width;
                this.Height = height;
                this.Depth = depth;
            }
            public Size3DFormat() { }

            //インデクサにより整数でのプロパティ指定を可能にする
            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return Width;
                        case 1:
                            return Height;
                        case 2:
                            return Depth;
                        default:
                            return 0;
                    }

                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            Width = value;
                            break;
                        case 1:
                            Height = value;
                            break;
                        case 2:
                            Depth = value;
                            break;
                    }

                }

            }
                       

            private int _Width = 10;
            public int Width
            {
                get { return _Width; }
                set
                {
                    _Width = value;
                    OnValueChanged(null);
                }
            }

            private int _Height = 10;
            public int Height
            {
                get { return _Height; }
                set
                {
                    _Height = value;
                    OnValueChanged(null);
                }
            }

            private int _Depth = 10;
            public int Depth
            {
                get { return _Depth; }
                set
                {
                    _Depth = value;
                    OnValueChanged(null);
                }
            }

            public event EventHandler ValueChanged;

            protected virtual void OnValueChanged(EventArgs e)
            {
                if (ValueChanged != null)
                    ValueChanged(this, e);
            }

            public override string ToString()
            {
                return "{" + string.Format("Width = {0} Heignt = {1} Depth = {2}", Width, Height, Depth) + "}";
            }
        }

        /// <summary>
        /// 描画割り当て構造体クラス
        /// </summary>
        public class DrawViewFormat
        {
            //コンストラクタ
            public DrawViewFormat(Draw3DEnum xView, Draw3DEnum yView)
            {
                this.XView = xView;
                this.YView = yView;
            }
            public DrawViewFormat() { }

            private Draw3DEnum _XView = Draw3DEnum.X;
            [DisplayName("横方向")]
            public Draw3DEnum XView
            {
                get { return _XView; }
                set
                {
                    _XView = value;
                    OnValueChanged(null);
                }
            }

            private Draw3DEnum _YView = Draw3DEnum.Y;
            [DisplayName("縦方向")]
            public Draw3DEnum YView
            {
                get { return _YView; }
                set
                {
                    _YView = value;
                    OnValueChanged(null);
                }
            }

            public event EventHandler ValueChanged;

            protected virtual void OnValueChanged(EventArgs e)
            {
                if (ValueChanged != null)
                    ValueChanged(this, e);
            }

            public override string ToString()
            {
                return "{" + string.Format("XView = {0} YView = {1}", XView, YView) + "}";
            }
        }

    }

    





}
