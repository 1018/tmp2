using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using CommonClassLibrary;

namespace LocalSimulator.ProjectMaker
{
    public interface ICommand
    {
        void Execute();

        void Undo();

        void Redo();

        string Text();
    }

    //ShapeObject新規作成コマンド
    public class ShapeObjectNewCreateCommand : ICommand
    {
        private BaseForm baseForm = null;
        private Type shapeType = null;
        private ShapeManager shapeManager = null;
        private List<Point> settingPoints = null;
        private ShapeObject newObject;
        private ShapeObject[] beforeShapeCollection = null;
        private ShapeObject[] afterShapeCollection = null;
        private string symbolName;
        private string text;

        public ShapeObjectNewCreateCommand(ShapeManager shapeManager, BaseForm baseForm, Type shapeType, List<Point> settingPoints)
        {
            this.baseForm = baseForm;
            this.beforeShapeCollection = baseForm.ShapeCollection.ToArray<ShapeObject>();
            this.shapeType = shapeType;
            this.shapeManager = shapeManager;
            this.settingPoints = new List<Point>(settingPoints);
            if (shapeType == typeof(ViewObject))
            {
                this.symbolName = MainForm.Instance.SymbolListView.SelectedItems[0].Tag.ToString();
                //this.symbolName = MainForm.Instance.SymbolListView.SelectedItems[0].Text;
            }
            this.text = shapeType + "新規作成";
        }
        
        public void Execute()
        {
            this.newObject = shapeManager.NewCreate(baseForm, shapeType, settingPoints, symbolName);
            this.afterShapeCollection = baseForm.ShapeCollection.ToArray<ShapeObject>();
        }

        public void Undo()
        {
            //this.baseForm.ShapeCollection = new OriginalCollection<ShapeObject>();
            this.baseForm.ShapeCollection.Clear();
            this.baseForm.ShapeCollection.AddRange(beforeShapeCollection);
            this.baseForm.Refresh();
        }

        public void Redo()
        {
            //this.baseForm.ShapeCollection = new OriginalCollection<ShapeObject>();
            this.baseForm.ShapeCollection.Clear();
            this.baseForm.ShapeCollection.AddRange(afterShapeCollection);
            this.baseForm.Refresh();
        }

        public string Text()
        {
            return this.text;
        }
    }

    //ShapeObject移動コマンド
    public class ShapeObjectMoveCommand : ICommand
    {
        private ShapeManager shapeManager;
        private List<ShapeObject> moveObjects;
        private List<List<Point>> moveObjectsOriginPoints;
        private List<List<Point>> moveObjectsPoints;
        private string text;

        public ShapeObjectMoveCommand(ShapeManager shapeManager, List<ShapeObject> moveObjects, 
                                      List<List<Point>> moveObjectsOriginPoints, List<List<Point>> moveObjectsPoints )
        {
            this.shapeManager = shapeManager;
            this.moveObjects = moveObjects;
            this.moveObjectsPoints = new List<List<Point>>(moveObjectsPoints);
            this.moveObjectsOriginPoints = new List<List<Point>>(moveObjectsOriginPoints);
            this.text = "オブジェクト移動";
        }
        public void Execute()
        {
            shapeManager.Move(moveObjects, moveObjectsPoints);
        }

        public void Undo()
        {
            shapeManager.Move(moveObjects, moveObjectsOriginPoints);            
        }

        public void Redo()
        {
            this.Execute();
        }

        public string Text()
        {
            return text;
        }

    }

    //ShapeObject変形コマンド
    public class ShapeObjectChangeCommand : ICommand
    {
        private ShapeManager shapeManager;
        private ShapeObject changeObject;
        private List<Point> originPoints;
        private List<Point> changePoints;
        private string text;

        public ShapeObjectChangeCommand(ShapeManager shapeManager, ShapeObject changeObject, List<Point> originPoints, List<Point> changePoints)
        {
            this.shapeManager = shapeManager;
            this.changeObject = changeObject;
            this.originPoints = new List<Point>(originPoints);
            this.changePoints = new List<Point>(changePoints);
            this.text = changeObject.GetType().ToString() + "変更";
        }
        public void Execute()
        {
            shapeManager.Change(changeObject, changePoints);
        }
        public void Undo()
        {
            shapeManager.Change(changeObject, originPoints);
        }
        public void Redo()
        {
            this.Execute();
        }

        public string Text()
        {
            return text;
        }


    }

    //ShapeObject貼り付けコマンド
    public class ShapeObjectPasteCommand : ICommand
    {
        private ShapeManager shapeManager;
        private BaseForm baseForm;
        private Point pasteDistance;
        private List<object> pasteObjects;
        private ShapeObject[] beforeShapeCollection;
        private ShapeObject[] afterShapeCollection;
        private string text;

        public ShapeObjectPasteCommand(ShapeManager shapeManager, BaseForm baseForm, Point pasteDistance, List<object> pasteObjects)
        {            
            this.shapeManager = shapeManager;
            this.baseForm = baseForm;
            this.pasteDistance = pasteDistance;
            this.pasteObjects = pasteObjects;

            this.beforeShapeCollection = this.baseForm.ShapeCollection.ToArray<ShapeObject>();

            this.text = "オブジェクト貼り付け";
        }
        public void Execute()
        {
            shapeManager.Paste(baseForm, pasteDistance, pasteObjects);
            this.afterShapeCollection = baseForm.ShapeCollection.ToArray<ShapeObject>();
        }
        public void Undo()
        {
            //this.baseForm.ShapeCollection = new OriginalCollection<ShapeObject>();
            this.baseForm.ShapeCollection.Clear();
            this.baseForm.ShapeCollection.AddRange(beforeShapeCollection);
            this.baseForm.Refresh();
        }

        public void Redo()
        {
            //this.baseForm.ShapeCollection = new OriginalCollection<ShapeObject>();
            this.baseForm.ShapeCollection.Clear();
            this.baseForm.ShapeCollection.AddRange(afterShapeCollection);
            this.baseForm.Refresh();
        }
        public string Text()
        {
            return text;
        }

    }

    //ShapeObject削除コマンド
    public class ShapeObjectDeleteCommand : ICommand
    {
        private BaseForm baseForm;
        private List<ShapeObject> deleteObjects;
        private ShapeObject[] beforeShapeCollection;
        private ShapeObject[] afterShapeCollection;
        private string text;

        public ShapeObjectDeleteCommand(BaseForm baseForm, List<ShapeObject> deleteObjects)
        {
            this.baseForm = baseForm;
            this.deleteObjects = deleteObjects;
            this.beforeShapeCollection = this.baseForm.ShapeCollection.ToArray<ShapeObject>();
            this.text = "オブジェクト削除";
        }
        public void Execute()
        {
            this.baseForm.ShapeCollection.RemoveAll(delegate(ShapeObject shapeObject)
            {
                return shapeObject.Selected;
            });
            this.afterShapeCollection = baseForm.ShapeCollection.ToArray<ShapeObject>();
            this.baseForm.Refresh();
        }
        public void Undo()
        {
            //this.baseForm.ShapeCollection = new OriginalCollection<ShapeObject>();
            this.baseForm.ShapeCollection.Clear();
            this.baseForm.ShapeCollection.AddRange(beforeShapeCollection);
            this.baseForm.Refresh();
        }

        public void Redo()
        {
            //this.baseForm.ShapeCollection = new OriginalCollection<ShapeObject>();
            this.baseForm.ShapeCollection.Clear();
            this.baseForm.ShapeCollection.AddRange(afterShapeCollection);
            this.baseForm.Refresh();
        }
        public string Text()
        {
            return text;
        }

    }

    public class ShapeObjectPropertyChangeCommand : ICommand
    {
        ShapeManager shapeManager;

        object oldValue;
        object newValue;
        ShapeObject shapeObject;
        string propertyName;
        string text;

        public ShapeObjectPropertyChangeCommand(ShapeManager manager, ShapeObject shapeObj, string property, object newVal, object oldVal)
        {
            this.shapeManager = manager;
            this.shapeObject = shapeObj;
            this.propertyName = property;
            this.newValue = newVal;
            this.oldValue = oldVal;
            this.text = "ShapeObjectプロパティ変更";
        }


        #region ICommand メンバ

        public void Execute()
        {
            Redo();
        }

        public void Undo()
        {
            this.shapeManager.SetProperty(this.shapeObject, this.propertyName, this.oldValue);
            MainForm.Instance.PropertyView.Refresh();
        }

        public void Redo()
        {
            this.shapeManager.SetProperty(this.shapeObject, this.propertyName, this.newValue);
            MainForm.Instance.PropertyView.Refresh();
        }

        public string Text()
        {
            return this.text;
        }

        #endregion
    }

    public class SymbolPropertyChangeCommand : ICommand
    {
        ShapeManager shapeManager;

        object oldValue;
        object newValue;
        Symbol_Draw symbol;
        string propertyName;
        string text;

        public SymbolPropertyChangeCommand(ShapeManager manager, Symbol_Draw sym, string property, object newVal, object oldVal)
        {
            this.shapeManager = manager;
            this.symbol = sym;
            this.propertyName = property;
            this.newValue = newVal;
            this.oldValue = oldVal;
            this.text = "Symbol_Draw プロパティ変更";
        }


        #region ICommand メンバ

        public void Execute()
        {
            Redo();
        }

        public void Undo()
        {
            this.shapeManager.SetProperty(this.symbol, this.propertyName, this.oldValue);
            MainForm.Instance.PropertyView.Refresh();

            // BaseFormを再描画する必要があるが、
            // Symbol_DrawからBaseFormを取得出来ないため
            // 全てのBaseFormを再描画する
            IEnumerable<BaseForm> baseForms = MainForm.Instance.MdiChildren.OfType<BaseForm>();
            foreach (BaseForm frm in baseForms)
            {
                frm.Invalidate();
            }
        }

        public void Redo()
        {
            this.shapeManager.SetProperty(this.symbol, this.propertyName, this.newValue);
            MainForm.Instance.PropertyView.Refresh();

            // BaseFormを再描画する必要があるが、
            // Symbol_DrawからBaseFormを取得出来ないため
            // 全てのBaseFormを再描画する
            IEnumerable<BaseForm> baseForms = MainForm.Instance.MdiChildren.OfType<BaseForm>();
            foreach (BaseForm frm in baseForms)
            {
                frm.Invalidate();
            }
        }

        public string Text()
        {
            return this.text;
        }

        #endregion
    }

    public class BaseFormPropertyChangeCommand : ICommand
    {
        BaseFormManager baseFormManager;

        object oldValue;
        object newValue;
        BaseForm baseForm;
        string propertyName;
        string text;

        public BaseFormPropertyChangeCommand(BaseFormManager manager, BaseForm baseFrm, string property, object newVal, object oldVal)
        {
            this.baseFormManager = manager;
            this.baseForm = baseFrm;
            this.propertyName = property;
            this.newValue = newVal;
            this.oldValue = oldVal;
            this.text = "BaseFormプロパティ変更";
        }

        #region ICommand メンバ

        public void Execute()
        {
            Redo();
        }

        public void Undo()
        {
            this.baseFormManager.SetProperty(this.baseForm, this.propertyName, this.oldValue);
            this.baseForm.Refresh();
            MainForm.Instance.PropertyView.Refresh();
        }

        public void Redo()
        {
            this.baseFormManager.SetProperty(this.baseForm, this.propertyName, this.newValue);
            this.baseForm.Refresh();
            MainForm.Instance.PropertyView.Refresh();
        }

        public string Text()
        {
            return this.text;
        }

        #endregion
    }

    public static class CommandManager
    { 
        private static UndoStack _undoCommandStack = new UndoStack();

        private static RedoStack _redoCommandStack = new RedoStack();

        //プロパティ
        public static UndoStack UndoCommandStack
        {
            get
            {
                return _undoCommandStack;
            }
            set
            {
                _undoCommandStack = value;
            }
        }

        public static RedoStack RedoCommandStack
        {
            get
            {
                return _redoCommandStack;
            }
            set
            {
                _redoCommandStack = value;
            }
        }

        //クラス
        public class UndoStack
        {
            private Stack<ICommand> commandStack = new Stack<ICommand>();

            public void Push(ICommand pushCommand)
            {
                commandStack.Push(pushCommand);
                CommandManager.RedoCommandStack.Clear();
                MainForm.Instance.UndoToolStripSplitButton.Enabled = true;
                MainForm.Instance.updated = true;
            }

            public void Push(ICommand pushCommand, bool redoDeleteFlag)
            {
                commandStack.Push(pushCommand);
                if (redoDeleteFlag == true)
                {
                    CommandManager.RedoCommandStack.Clear();
                }
                MainForm.Instance.UndoToolStripSplitButton.Enabled = true;
            }

            public ICommand Pop()
            {
                ICommand popCommand = commandStack.Pop();
                if (commandStack.Count == 0)
                {
                    MainForm.Instance.UndoToolStripSplitButton.Enabled = false;
                }
                return popCommand;
            }

            public int Count
            {
                get { return commandStack.Count; }
            }

        }

        public class RedoStack
        {
            private Stack<ICommand> commandStack = new Stack<ICommand>();

            public void Push(ICommand pushCommand)
            {
                commandStack.Push(pushCommand);
                MainForm.Instance.RedoToolStripSplitButton.Enabled = true;
                MainForm.Instance.updated = true;
            }
            public ICommand Pop()
            {
                ICommand popCommand = commandStack.Pop();
                if (commandStack.Count == 0)
                {
                    MainForm.Instance.RedoToolStripSplitButton.Enabled = false;
                }
                return popCommand;
            }

            public int Count
            {
                get { return commandStack.Count; }
            }

            public void Clear()
            {
                commandStack = new Stack<ICommand>();
            }

        }



    }    
}
