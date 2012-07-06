using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CommonClassLibrary;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace LocalSimulator.ProjectMaker
{
    //ShapeObject作成用クラス（描画部分は別クラス）
    public class ShapeObjectMaking
    {
        private List<Point> settingPoints = new List<Point>();
        private IDrawing drawObject = null;
        private BaseForm baseForm = null;
        private Type makeObjectType = null;
        private ShapeManager shapeManager = null;

        public ShapeObjectMaking(BaseForm baseForm, MouseEventArgs e)
        {
            MainCtrl.IsShapeMaking = true;
            this.shapeManager = ShapeManager.CreateInstance();

            //型分岐
            if (MainCtrl.CircleButtonSelect == true)
            {
                this.makeObjectType = typeof(CircleObject);
            }
            else if (MainCtrl.SquareButtonSelect == true)
            {
                this.makeObjectType = typeof(SquareObject);
            }
            else if (MainCtrl.TextButtonSelect == true)
            {
                this.makeObjectType = typeof(TextObject);
            }
            else if (MainCtrl.LineButtonSelect == true)
            {
                this.makeObjectType = typeof(LineObject);
            }
            else if (MainCtrl.LinesButtonSelect == true)
            {
                this.makeObjectType = typeof(LinesObject);
            }
            else if (MainCtrl.PolygonButtonSelect == true)
            {
                this.makeObjectType = typeof(PolygonObject);
            }
            else if (MainCtrl.SymbolViewListSelect == true)
            {
                this.makeObjectType = typeof(ViewObject);
            }


            if (makeObjectType == typeof(ViewObject))
            {


                this.baseForm = baseForm;
                settingPoints.Add(this.shapeManager.SnapPoint(e.Location, baseForm.Zoom));

                //オブジェクト作成                
                ShapeObjectNewCreateCommand newCreateCommand = new ShapeObjectNewCreateCommand(shapeManager, baseForm, makeObjectType, settingPoints);
                newCreateCommand.Execute();
                CommandManager.UndoCommandStack.Push(newCreateCommand);
                MainCtrl.IsShapeMaking = false;

            }
            else
            {
                this.baseForm = baseForm;

                baseForm.Paint += new PaintEventHandler(this.BaseForm_Paint);
                baseForm.MouseMove += new MouseEventHandler(BaseForm_MouseMove);
                baseForm.MouseClick += new MouseEventHandler(BaseForm_MouseClick);
                baseForm.PreviewKeyDown += new PreviewKeyDownEventHandler(BaseForm_PreviewKeyDown);

                //settingPoints.Add(this.shapeManager.SnapPoint(e.Location, baseForm.Zoom));
            }

        }

        private void BaseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                baseForm.Paint -= new PaintEventHandler(this.BaseForm_Paint);
                baseForm.MouseMove -= new MouseEventHandler(this.BaseForm_MouseMove);
                baseForm.MouseClick -= new MouseEventHandler(this.BaseForm_MouseClick);
                baseForm.PreviewKeyDown -= new PreviewKeyDownEventHandler(this.BaseForm_PreviewKeyDown);

                MainCtrl.IsShapeMaking = false;
                baseForm.Refresh();
            }
            if (e.KeyCode == Keys.Enter && (makeObjectType == typeof(LinesObject) || makeObjectType == typeof(PolygonObject)))
            {
                baseForm.Paint -= new PaintEventHandler(this.BaseForm_Paint);
                baseForm.MouseMove -= new MouseEventHandler(this.BaseForm_MouseMove);
                baseForm.MouseClick -= new MouseEventHandler(this.BaseForm_MouseClick);
                baseForm.PreviewKeyDown -= new PreviewKeyDownEventHandler(this.BaseForm_PreviewKeyDown);

                //オブジェクト作成
                ShapeManager shapeManager = ShapeManager.CreateInstance();
                ShapeObjectNewCreateCommand newCreateCommand = new ShapeObjectNewCreateCommand(shapeManager, baseForm, makeObjectType, settingPoints);
                newCreateCommand.Execute();
                CommandManager.UndoCommandStack.Push(newCreateCommand);

                MainCtrl.IsShapeMaking = false;

            }

        }

        private void BaseForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                settingPoints.Add(this.shapeManager.SnapPoint(e.Location, baseForm.Zoom));

                if (settingPoints.Count == 2 && makeObjectType != typeof(LinesObject) && makeObjectType != typeof(PolygonObject))
                {
                    baseForm.Paint -= new PaintEventHandler(this.BaseForm_Paint);
                    baseForm.MouseMove -= new MouseEventHandler(this.BaseForm_MouseMove);
                    baseForm.MouseClick -= new MouseEventHandler(this.BaseForm_MouseClick);
                    baseForm.PreviewKeyDown -= new PreviewKeyDownEventHandler(this.BaseForm_PreviewKeyDown);

                    //オブジェクト作成
                    ShapeManager shapeManager = ShapeManager.CreateInstance();
                    ShapeObjectNewCreateCommand newCreateCommand = new ShapeObjectNewCreateCommand(shapeManager, baseForm, makeObjectType, settingPoints);
                    newCreateCommand.Execute();
                    CommandManager.UndoCommandStack.Push(newCreateCommand);
                    MainCtrl.IsShapeMaking = false;
                }
            }
        }

        private void BaseForm_MouseMove(object sender, MouseEventArgs e)
        {
            settingPoints.Add(this.shapeManager.SnapPoint(e.Location, baseForm.Zoom));
            baseForm.Refresh();
            settingPoints.Remove(this.shapeManager.SnapPoint(e.Location, baseForm.Zoom));
        }

        private void BaseForm_Paint(object sender, PaintEventArgs e)
        {
            if (makeObjectType == typeof(CircleObject))
            {
                drawObject = new CircleDrawing(Color.Black, 1, DashStyle.Solid, Color.Transparent, settingPoints, true);
            }
            else if (makeObjectType == typeof(SquareObject))
            {
                drawObject = new SquareDrawing(Color.Black, 1, DashStyle.Solid, Color.Transparent, settingPoints);
            }
            else if (makeObjectType == typeof(TextObject))
            {
                drawObject = new SquareDrawing(Color.Black, 1, DashStyle.Solid, Color.Transparent, settingPoints);
            }
            else if (makeObjectType == typeof(LineObject))
            {
                drawObject = new LineDrawing(Color.Black, 1, DashStyle.Solid, Color.Black, 1, DashStyle.Solid, settingPoints);
            }
            else if (makeObjectType == typeof(LinesObject))
            {
                drawObject = new LinesDrawing(Color.Black, 1, DashStyle.Solid, Color.Black, 1, DashStyle.Solid, settingPoints);
            }
            else if (makeObjectType == typeof(PolygonObject))
            {
                drawObject = new PolygonDrawing(Color.Black, 1, DashStyle.Solid, Color.Transparent, settingPoints);
            }


            drawObject.Draw(e.Graphics);
        }



    }

    //ShapeObject移動用クラス
    public class ShapeObjectMoving
    {
        private BaseForm baseForm = null;
        private Point moveStartMousePoint;
        private List<List<Point>> moveObjectsStartPoints = new List<List<Point>>();
        private List<List<Point>> moveObjectsPoints = new List<List<Point>>();

        private ShapeManager shapeManager;
        private List<ShapeObject> moveObjects = new List<ShapeObject>();

        public ShapeObjectMoving(BaseForm baseForm, MouseEventArgs e)
        {
            MainCtrl.IsShapeMoving = true;

            this.shapeManager = ShapeManager.CreateInstance();
            this.baseForm = baseForm;

            this.moveStartMousePoint = ZoomManager.ToBasePoint(e.Location, baseForm.Zoom);

            for (int i = 0; i < baseForm.ShapeCollection.Count; i++)
            {
                if (baseForm.ShapeCollection[i].Selected == true)
                {
                    this.moveObjectsStartPoints.Add(baseForm.ShapeCollection[i].NowPoints());
                    this.moveObjects.Add(baseForm.ShapeCollection[i]);
                }
            }

            baseForm.MouseMove += new MouseEventHandler(BaseForm_MouseMove);
            baseForm.MouseUp += new MouseEventHandler(BaseForm_MouseUp);

        }

        private void BaseForm_MouseMove(object sender, MouseEventArgs e)
        {
            //マウスを移動した距離の計算
            Point nowPoint = ZoomManager.ToBasePoint(e.Location, baseForm.Zoom);
            Point moveDistance = new Point(nowPoint.X - this.moveStartMousePoint.X,
                                           nowPoint.Y - this.moveStartMousePoint.Y);

            //Snap補正
            Point snap0 = shapeManager.SnapPoint(new Point(moveObjectsStartPoints[0][0].X + moveDistance.X, moveObjectsStartPoints[0][0].Y + moveDistance.Y), 1);
            int snapX = snap0.X - (moveObjectsStartPoints[0][0].X + moveDistance.X);
            int snapY = snap0.Y - (moveObjectsStartPoints[0][0].Y + moveDistance.Y);

            List<Point> movePoints = new List<Point>();
            moveObjectsPoints = new List<List<Point>>();

            //各オブジェクト移動処理
            for (int i = 0; i < this.moveObjects.Count; i++)
            {
                movePoints = new List<Point>();

                //移動させる座標計算
                if (MainCtrl.SnapMode)
                {
                    for (int j = 0; j < moveObjectsStartPoints[i].Count; j++)
                    {
                        int newXPoint = moveObjectsStartPoints[i][j].X + moveDistance.X + snapX;
                        int newYPoint = moveObjectsStartPoints[i][j].Y + moveDistance.Y + snapY;

                        Point newPoint = new Point(newXPoint, newYPoint);
                        movePoints.Add(newPoint);
                    }
                }
                else
                {
                    for (int j = 0; j < moveObjectsStartPoints[i].Count; j++)
                    {
                        int newXPoint = moveObjectsStartPoints[i][j].X + moveDistance.X;
                        int newYPoint = moveObjectsStartPoints[i][j].Y + moveDistance.Y;

                        Point newPoint = new Point(newXPoint, newYPoint);
                        movePoints.Add(newPoint);
                    }
                }

                //移動リストに追加
                moveObjectsPoints.Add(movePoints);
            }


            //画面外判断
            bool isOut = false;
            PointF offset = PointF.Empty;

            foreach (List<Point> moveObjPoints in moveObjectsPoints)
            {
                foreach (Point movePt in moveObjPoints)
                {
                    Rectangle baseFormRect = baseForm.ClientRectangle;
                    PointF realPt = ZoomManager.MagnifyPoint((PointF)movePt, baseForm.Zoom);

                    if (!baseFormRect.Contains(Point.Ceiling(realPt)))
                    {
                        isOut = true;

                        PointF moveOffset = PointF.Empty;

                        // ← |
                        if (baseFormRect.X > realPt.X)
                            moveOffset.X = realPt.X - baseFormRect.X;

                        // | →
                        if (baseFormRect.Right < realPt.X)
                            moveOffset.X = realPt.X - baseFormRect.Right;

                        // ↑ ＿
                        if (baseFormRect.Y > realPt.Y)
                            moveOffset.Y = realPt.Y - baseFormRect.Y;

                        // ￣ ↓
                        if (baseFormRect.Bottom < realPt.Y)
                            moveOffset.Y = realPt.Y - baseFormRect.Bottom;

                        if (MainCtrl.SnapMode)
                        {
                            //moveOffset = shapeManager.SnapPoint(moveOffset, baseForm.Zoom);

                            //if (movePt.X - moveOffset.X < 0)
                            //    moveOffset = new Point(0, moveOffset.Y);

                            //if (movePt.Y - moveOffset.Y < 0)
                            //    moveOffset = new Point(moveOffset.X, 0);
                        }


                        if (Math.Abs(offset.X) < Math.Abs(moveOffset.X))
                            offset.X = moveOffset.X;

                        if (Math.Abs(offset.Y) < Math.Abs(moveOffset.Y))
                            offset.Y = moveOffset.Y;
                    }
                }
            }

            //画面外有
            if (isOut)
            {
                offset = ZoomManager.ToBasePoint(offset, baseForm.Zoom);
                for (int i = 0; i < moveObjectsPoints.Count; i++)
                {
                    for (int j = 0; j < moveObjectsPoints[i].Count; j++)
                    {
                        moveObjectsPoints[i][j] = new Point(
                            moveObjectsPoints[i][j].X - (int)Math.Round(offset.X),
                            moveObjectsPoints[i][j].Y - (int)Math.Round(offset.Y));
                    }
                }

                //System.Diagnostics.Debug.WriteLine(moveObjectsPoints[0][0]);
            }

            shapeManager.Move(moveObjects, moveObjectsPoints);
        }

        private void BaseForm_MouseUp(object sender, MouseEventArgs e)
        {
            baseForm.MouseMove -= new MouseEventHandler(BaseForm_MouseMove);
            baseForm.MouseUp -= new MouseEventHandler(BaseForm_MouseUp);

            ShapeObjectMoveCommand shapeObjectMoveCommand = new ShapeObjectMoveCommand(shapeManager, moveObjects, moveObjectsStartPoints, moveObjectsPoints);
            CommandManager.UndoCommandStack.Push(shapeObjectMoveCommand);
            MainCtrl.IsShapeMoving = false;

            if (moveObjects.Count == 1)
            {
                MainCtrl.PropertyView_Set(moveObjects[0]);
            }
            else
            {
                MainCtrl.PropertyView_Set(null);
            }

        }


    }

    //ShapeObject微調整移動用クラス
    public class ShapeObjectLittleMoving
    {
        private BaseForm baseForm = null;
        private List<List<Point>> moveObjectsStartPoints = new List<List<Point>>();
        private List<List<Point>> moveObjectsPoints = new List<List<Point>>();

        private ShapeManager shapeManager;
        private List<ShapeObject> moveObjects = new List<ShapeObject>();

        public ShapeObjectLittleMoving(BaseForm baseForm, PreviewKeyDownEventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();


            MainCtrl.IsShapeMoving = true;

            this.shapeManager = ShapeManager.CreateInstance();
            this.baseForm = baseForm;

            int littleMoveX = 0;
            int littleMoveY = 0;


            if (e.KeyCode == Keys.Up)
            {
                if (MainCtrl.SnapMode) { littleMoveY = -(MainCtrl.GridSize.Width); }
                else { littleMoveY = -1; }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (MainCtrl.SnapMode) { littleMoveY = MainCtrl.GridSize.Width; }
                else { littleMoveY = 1; }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (MainCtrl.SnapMode) { littleMoveX = -(MainCtrl.GridSize.Height); }
                else { littleMoveX = -1; }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (MainCtrl.SnapMode) { littleMoveX = MainCtrl.GridSize.Height; }
                else { littleMoveX = 1; }
            }

            //開始地点と選択オブジェクト登録

            for (int i = 0; i < baseForm.ShapeCollection.Count; i++)
            {
                if (baseForm.ShapeCollection[i].Selected == true)
                {
                    this.moveObjectsStartPoints.Add(baseForm.ShapeCollection[i].NowPoints());
                    this.moveObjects.Add(baseForm.ShapeCollection[i]);
                }
            }

            //Snap補正
            Point snap0 = shapeManager.SnapPoint(new Point(moveObjectsStartPoints[0][0].X + littleMoveX, moveObjectsStartPoints[0][0].Y + littleMoveY), 1);
            int snapX = snap0.X - (moveObjectsStartPoints[0][0].X + littleMoveX);
            int snapY = snap0.Y - (moveObjectsStartPoints[0][0].Y + littleMoveY);

            //移動先画面外判断
            Rectangle baseFormRect = this.baseForm.ClientRectangle;
            for (int i = 0; i < this.moveObjectsStartPoints.Count; i++)
            {
                if (MainCtrl.SnapMode)
                {
                    for (int j = 0; j < moveObjectsStartPoints[i].Count; j++)
                    {
                        int moveX = this.moveObjectsStartPoints[i][j].X + littleMoveX + snapX;
                        int moveY = this.moveObjectsStartPoints[i][j].Y + littleMoveY + snapY;

                        PointF realPt = ZoomManager.MagnifyPoint(new PointF(moveX, moveY), this.baseForm.Zoom);

                        if (!baseFormRect.Contains(Point.Ceiling(realPt)))
                        {
                            if (realPt.X < baseFormRect.X || baseFormRect.Right < realPt.X)
                            {
                                littleMoveX = 0;
                            }
                            if (realPt.Y < baseFormRect.Y || baseFormRect.Bottom < realPt.Y)
                            {
                                littleMoveY = 0;
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < moveObjectsStartPoints[i].Count; j++)
                    {
                        int moveX = this.moveObjectsStartPoints[i][j].X + littleMoveX;
                        int moveY = this.moveObjectsStartPoints[i][j].Y + littleMoveY;

                        PointF realPt = ZoomManager.MagnifyPoint(new PointF(moveX, moveY), this.baseForm.Zoom);

                        if (!baseFormRect.Contains(Point.Ceiling(realPt)))
                        {
                            if (realPt.X < baseFormRect.X || baseFormRect.Right < realPt.X)
                            {
                                littleMoveX = 0;
                            }
                            if (realPt.Y < baseFormRect.Y || baseFormRect.Bottom < realPt.Y)
                            {
                                littleMoveY = 0;
                            }
                        }
                    }
                }

                if (littleMoveX == 0 && littleMoveY == 0)
                {
                    break;
                }
            }

            for (int i = 0; i < this.moveObjectsStartPoints.Count; i++)
            {
                List<Point> movePoints = new List<Point>();

                //移動させる座標計算
                if (MainCtrl.SnapMode)
                {
                    for (int j = 0; j < moveObjectsStartPoints[i].Count; j++)
                    {
                        int moveX = this.moveObjectsStartPoints[i][j].X + littleMoveX + snapX;
                        int moveY = this.moveObjectsStartPoints[i][j].Y + littleMoveY + snapY;
                        Point movePoint = new Point(moveX, moveY);
                        movePoints.Add(movePoint);
                    }
                }
                else
                {
                    for (int j = 0; j < moveObjectsStartPoints[i].Count; j++)
                    {
                        int moveX = this.moveObjectsStartPoints[i][j].X + littleMoveX;
                        int moveY = this.moveObjectsStartPoints[i][j].Y + littleMoveY;
                        Point movePoint = shapeManager.SnapPoint(new Point(moveX, moveY), 1);
                        movePoints.Add(movePoint);
                    }
                }
                moveObjectsPoints.Add(movePoints);

            }

            ShapeObjectMoveCommand shapeObjectMoveCommand = new ShapeObjectMoveCommand(shapeManager, moveObjects, moveObjectsStartPoints, moveObjectsPoints);

            shapeObjectMoveCommand.Execute();

            CommandManager.UndoCommandStack.Push(shapeObjectMoveCommand);
            MainCtrl.IsShapeMoving = false;

        }

    }

    public class ShapeObjectChanging
    {
        private BaseForm baseForm;
        private Point changeStartMousePoint;
        private ShapeManager shapeManager;
        private ShapeObject changeObject;
        private List<Point> originPoints;
        private string changePointName;
        private List<Point> changePoints;

        public ShapeObjectChanging(BaseForm baseForm, ShapeObject changeObject, MouseEventArgs e)
        {
            MainCtrl.IsShapeChanging = true;

            this.shapeManager = ShapeManager.CreateInstance();
            this.baseForm = baseForm;
            this.changeObject = changeObject;
            this.changePointName = changeObject.JudgeModifyRange(e.Location);

            //初期座標を取得する。
            this.changeStartMousePoint = ZoomManager.ToBasePoint(e.Location, baseForm.Zoom);
            this.originPoints = changeObject.NowPoints();

            baseForm.MouseMove += new MouseEventHandler(BaseForm_MouseMove);
            baseForm.MouseUp += new MouseEventHandler(BaseForm_MouseUp);


        }
        private void BaseForm_MouseMove(object sender, MouseEventArgs e)
        {
            //マウスを移動した距離の計算
            Point nowPoint = ZoomManager.ToBasePoint(e.Location, baseForm.Zoom);
            Point moveDistance = new Point(nowPoint.X - this.changeStartMousePoint.X,
                                           nowPoint.Y - this.changeStartMousePoint.Y);

            changePoints = changeObject.ChangePoint(changePointName, originPoints, moveDistance);
            shapeManager.Change(changeObject, changePoints);


        }

        private void BaseForm_MouseUp(object sender, MouseEventArgs e)
        {
            baseForm.MouseMove -= new MouseEventHandler(BaseForm_MouseMove);
            baseForm.MouseUp -= new MouseEventHandler(BaseForm_MouseUp);

            ShapeObjectChangeCommand shapeObjectChangeCommand = new ShapeObjectChangeCommand(shapeManager, changeObject, originPoints, changePoints);

            CommandManager.UndoCommandStack.Push(shapeObjectChangeCommand);

            MainCtrl.IsShapeChanging = false;

            MainCtrl.PropertyView_Set(changeObject);


        }

    }



}
