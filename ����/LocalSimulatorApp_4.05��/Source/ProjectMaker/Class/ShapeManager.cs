using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using CommonClassLibrary;
using System.Collections.ObjectModel;
using System.Collections;
using System.Reflection;

namespace LocalSimulator.ProjectMaker
{
    
    
    //ShapeObjectに対する要求を管理するクラス
    public class ShapeManager
    {       

        private ShapeManager()
        {
            this.ClipBoardObjects = new List<object>();            
        }

        public static ShapeManager instance = null;

        public static ShapeManager CreateInstance()
        {
            if (instance == null)
            {
                instance = new ShapeManager();
            }
            return instance;
        }

        public Point CopyPoint { get; set; }

        public Point PastePoint { get; set; }

        public List<object> ClipBoardObjects { get; set; }

        public ShapeObject NewCreate(BaseForm baseForm, Type shapeType, List<Point>settingPoints, string symbolName)
        {
            ShapeObject newObject = (ShapeObject)Activator.CreateInstance(shapeType);

            if (newObject.GetType() == typeof(ViewObject))
            {
                ViewObject vObject = (ViewObject)newObject;
                vObject.ParentForm = baseForm;
                vObject.CreateSymbol(symbolName);
            }
                   
            newObject.ParentForm = baseForm;
            
            newObject.Initialize(settingPoints);
            
            newObject.ReMake();

            
            baseForm.ShapeCollection.Add(newObject);

            
            baseForm.SelectChange(newObject);      
            
            return newObject;

        }

        public void Delete(BaseForm baseForm, ShapeObject deleteObject)
        {            
            baseForm.ShapeCollection.Remove(deleteObject);
        }

        public void Delete(BaseForm baseForm, List<ShapeObject> deleteObjects)
        {
            foreach (ShapeObject deleteObject in deleteObjects)
            {
                this.Delete(baseForm, deleteObject);
            }
        }

        public void Move(List<ShapeObject> shapeObjects, List<List<Point>> movingObjectsPoints)
        {           
            
            for (int i = 0; i < shapeObjects.Count; i++)
            {
                shapeObjects[i].Move(movingObjectsPoints[i]);
                shapeObjects[i].ReMake();
            }
            BaseForm baseForm = shapeObjects[0].ParentForm;
            baseForm.Refresh();

            //if (shapeObjects.Count == 1)
            //{
            //    MainCtrl.PropertyView_Set(shapeObjects[0]);
            //}
            //else
            //{
            //    MainCtrl.PropertyView_Set(null);
            //}

        }

        public List<Point> Change(ShapeObject changeObject, List<Point> changePoints)
        {
            changeObject.Change(changePoints);            

            BaseForm baseForm = changeObject.ParentForm;

            changeObject.ReMake();
            baseForm.Refresh();

            return changePoints;

        }
        
        public Size SnapSize(Size NowSize, int Zoom)
        {
            if (MainCtrl.SnapMode)
            {
                int WaruX = 0;
                int WaruY = 0;

                if (NowSize.Width >= 0)
                { WaruX = (int)((double)NowSize.Width / (double)(MainCtrl.GridSize.Width * Zoom) + 0.5); }
                else
                { WaruX = (int)((double)NowSize.Width / (double)(MainCtrl.GridSize.Width * Zoom) - 0.5); }

                if (NowSize.Height >= 0)
                { WaruY = (int)((double)NowSize.Height / (double)(MainCtrl.GridSize.Height * Zoom) + 0.5); }
                else
                { WaruY = (int)((double)NowSize.Height / (double)(MainCtrl.GridSize.Height * Zoom) - 0.5); }

                return new Size(WaruX * MainCtrl.GridSize.Width * Zoom, WaruY * MainCtrl.GridSize.Height * Zoom);
            }
            else
            {
                return NowSize;
            }

        }

        public Point SnapPoint(Point Pt, double Zoom)
        {
            if (MainCtrl.SnapMode)
            {
                int WaruX = (int)((double)Pt.X / (double)(MainCtrl.GridSize.Width * Zoom) + 0.5);
                int WaruY = (int)((double)Pt.Y / (double)(MainCtrl.GridSize.Height * Zoom) + 0.5);

                return new Point((int)(WaruX * MainCtrl.GridSize.Width * Zoom), (int)(WaruY * MainCtrl.GridSize.Height * Zoom));
            }
            else
            {
                return Pt;
            }
        }

        public void Cut(BaseForm baseForm)
        {
            this.Copy(baseForm);

            //選択されているオブジェクトを削除
            List<ShapeObject> selectObjects = new List<ShapeObject>();

            selectObjects = baseForm.ShapeCollection.FindAll(delegate(ShapeObject selectObject)
            {
                return selectObject.Selected;
            });

            ShapeObjectDeleteCommand command = new ShapeObjectDeleteCommand(baseForm, selectObjects);
            command.Execute();
            CommandManager.UndoCommandStack.Push(command);
            //this.Delete(baseForm, selectObjects);
            //baseForm.Refresh();
            MainForm.Instance.PropertyView.SelectedObject = null;

        }

        public void Copy(BaseForm baseForm)
        {
            int minX = 0;
            int minY = 0;

            //選択されているオブジェクトを抽出
            List<ShapeObject> copyObjects = baseForm.ShapeCollection.FindAll(delegate(ShapeObject shapeObject)
            {
                return shapeObject.Selected;
            });

            if (copyObjects.Count == 0) { return; }

            this.ClipBoardObjects = new List<object>();

            foreach (ShapeObject obj in copyObjects)
            {
                if (obj.ShapeType == "ViewObject")
                {
                    SymbolDataSerializeFormat objSerialize = (SymbolDataSerializeFormat)SerializeSupport.Serialize(((ViewObject)obj).ControlInstance);

                    if (objSerialize != null)
                    {
                        ClipBoardObjects.Add(objSerialize);
                    }
                }
                else
                {
                    ShapeDataSerializeFormat objSerialize = (ShapeDataSerializeFormat)SerializeSupport.Serialize(obj);
                    if (objSerialize != null)
                    {
                        ClipBoardObjects.Add(objSerialize);
                    }
                }
            }           

            List<Point> copyPoints = new List<Point>();

            //全ポイント抽出
            foreach (ShapeObject copyObject in copyObjects)
            {
                copyPoints.AddRange(copyObject.NowPoints());
            }

            //最小X,Yを抜き出す
            copyPoints.Sort(delegate(Point p1, Point p2)
            {
                return p1.X.CompareTo(p2.X);
            });
            minX = copyPoints[0].X;
            
            copyPoints.Sort(delegate(Point p1, Point p2)
            {
                return p1.Y.CompareTo(p2.Y);
            });
            minY = copyPoints[0].Y;

            this.CopyPoint = new Point(minX, minY);

        }

        public void Paste(BaseForm baseForm, Point pasteDistance, List<object> pasteObjects)
        {
            
            foreach (object pasteObject in pasteObjects)
            {
                if (pasteObject.GetType() == typeof(ShapeDataSerializeFormat))
                {
                    ShapeDataSerializeFormat obj = (ShapeDataSerializeFormat)pasteObject;
                    ShapeObject MyShape = (ShapeObject)SerializeSupport.Deserialize(pasteObject, Type.GetType("LocalSimulator.ProjectMaker." + obj.Type));

                    List<Point> newPoints = new List<Point>();
                    List<Point> nowPoints = MyShape.NowPoints();
                    foreach (Point nowPoint in nowPoints)
                    {
                        Point newPoint = new Point(pasteDistance.X + nowPoint.X, pasteDistance.Y + nowPoint.Y);
                        newPoints.Add(newPoint);
                    }

                    MyShape.ParentForm = baseForm;

                    MyShape.Move(newPoints);
                    MyShape.ReMake();
                    MyShape.Selected = true;
                    baseForm.ShapeCollection.Add(MyShape);

                    //PropertyGrid更新
                    if (pasteObjects.Count == 1) { MainCtrl.PropertyView_Set(MyShape); }

                }
                else if (pasteObject.GetType() == typeof(SymbolDataSerializeFormat))
                {
                    Symbol_Draw Symbol = (Symbol_Draw)SerializeSupport.Deserialize(pasteObject);

                    ViewObject MyShape = new ViewObject();
                    //MyShape.ParentForm = baseForm;
                    MyShape.ControlInstance = Symbol;
                    MyShape.Location = Symbol.DrawLocation;
                    Symbol.Tag = MyShape;
                    MyShape.Size = Symbol.DrawSize;

                    Symbol.SymbolName = baseForm.GetSymbolName(Symbol);

                    List<Point> newPoints = new List<Point>();
                    List<Point> nowPoints = MyShape.NowPoints();
                    foreach (Point nowPoint in nowPoints)
                    {
                        Point newPoint = new Point(pasteDistance.X + nowPoint.X, pasteDistance.Y + nowPoint.Y);
                        newPoints.Add(newPoint);
                    }

                    
                    MyShape.ParentForm = baseForm;

                    MyShape.Move(newPoints);
                    MyShape.ReMake();
                    MyShape.Selected = true;
                    baseForm.ShapeCollection.Add(MyShape);     

                    MainCtrl.RefreshControlBmp(Symbol, MyShape);

                    //PropertyGrid更新
                    if (pasteObjects.Count == 1) { MainCtrl.PropertyView_Set(MyShape); }
                }
            }
            
            baseForm.Refresh();

        }

        public void SetProperty(ShapeObject shapeObj, string propertyRoot, object setValue)
        {
            //設定するプロパティまでのルート
            string[] nameRoot = propertyRoot.Split(',');
            for (int i = 0; i < nameRoot.Length; i++)
            {
                nameRoot[i] = nameRoot[i].Trim();
            }

            //値を設定するオブジェクトを取得
            object[] instanceRoot = new object[nameRoot.Length];
            object component = shapeObj;

            for (int i = 0; i < nameRoot.Length; i++)
            {
                Type componentType = component.GetType();

                // 配列アクセス
                if (nameRoot[i].IndexOf('[') == 0)
                {
                    string arrayIndexStr = nameRoot[i].Trim('[', ']');
                    int arrayIndex = int.Parse(arrayIndexStr);

                    instanceRoot[i] = ((IList)component)[arrayIndex];
                }
                else
                {
                    PropertyInfo pi = componentType.GetProperty(nameRoot[i]);
                    instanceRoot[i] = pi.GetValue(component, null);
                }

                component = instanceRoot[i];
            }

            //新しい値を設定
            instanceRoot[instanceRoot.Length - 1] = setValue;

            //反対向きにセットしていく(ルートに値型が含まれる可能性があるため)
            for (int i = nameRoot.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                    component = shapeObj;
                else
                    component = instanceRoot[i - 1];


                Type componentType = component.GetType();

                // 配列アクセス
                if (nameRoot[i].IndexOf('[') == 0)
                {
                    string arrayIndexStr = nameRoot[i].Trim('[', ']');
                    int arrayIndex = int.Parse(arrayIndexStr);

                    ((IList)component)[arrayIndex] = instanceRoot[i];
                }
                else
                {
                    PropertyInfo pi = componentType.GetProperty(nameRoot[i]);
                    pi.SetValue(component, instanceRoot[i], null);
                }
            }

            shapeObj.ReMake();
            shapeObj.ParentForm.Refresh();
        }

        public void SetProperty(Symbol_Draw symbol, string propertyRoot, object setValue)
        {
            //設定するプロパティまでのルート
            string[] nameRoot = propertyRoot.Split(',');
            for (int i = 0; i < nameRoot.Length; i++)
            {
                nameRoot[i] = nameRoot[i].Trim();
            }

            //値を設定するオブジェクトを取得
            object[] instanceRoot = new object[nameRoot.Length];
            object component = symbol;

            for (int i = 0; i < nameRoot.Length; i++)
            {
                Type componentType = component.GetType();

                // 配列アクセス
                if (nameRoot[i].IndexOf('[') == 0)
                {
                    string arrayIndexStr = nameRoot[i].Trim('[', ']');
                    int arrayIndex = int.Parse(arrayIndexStr);

                    instanceRoot[i] = ((IList)component)[arrayIndex];
                }
                else
                {
                    PropertyInfo pi = componentType.GetProperty(nameRoot[i]);
                    instanceRoot[i] = pi.GetValue(component, null);
                }

                component = instanceRoot[i];
            }

            //新しい値を設定
            instanceRoot[instanceRoot.Length - 1] = setValue;

            //反対向きにセットしていく(ルートに値型が含まれる可能性があるため)
            for (int i = nameRoot.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                    component = symbol;
                else
                    component = instanceRoot[i - 1];


                Type componentType = component.GetType();

                // 配列アクセス
                if (nameRoot[i].IndexOf('[') == 0)
                {
                    string arrayIndexStr = nameRoot[i].Trim('[', ']');
                    int arrayIndex = int.Parse(arrayIndexStr);

                    ((IList)component)[arrayIndex] = instanceRoot[i];
                }
                else
                {
                    PropertyInfo pi = componentType.GetProperty(nameRoot[i]);
                    pi.SetValue(component, instanceRoot[i], null);
                }
            }
        }

    }

    
   
}
