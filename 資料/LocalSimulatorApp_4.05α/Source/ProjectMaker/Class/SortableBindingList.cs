/*
 * http://d.hatena.ne.jp/siokoshou/20070504
 * 
 * 上記ページのコードを元に、ソート方法の登録機能を追加。
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CustomBindingList
{
    /// <summary>
    /// バインディングしたデータソースのソートの方法を外部に委託するクラス
    /// </summary>
    public class SortableBindingList<T> : BindingList<T>
    {
        List<SortableMethod<T>> SortableMethodList = new List<SortableMethod<T>>();

        PropertyDescriptor _sortProp = null;
        ListSortDirection _sortDir = ListSortDirection.Ascending;
        bool _isSorted = false;


        /// <summary>
        /// ユーザー定義のソートを登録する
        /// </summary>
        /// <remarks>
        /// 同一のプロパティへのソートが既に登録されているとき、
        /// 既存のソート方法に上書きされます。
        /// </remarks>
        /// <param name="sortableProperty"></param>
        /// <param name="comparer"></param>
        public void RegisterSortable(PropertyDescriptor sortableProperty, IComparer<T> comparer)
        {
            if (sortableProperty == null) { throw new ArgumentNullException("sortableProperty"); }
            if (comparer == null) { throw new ArgumentNullException("comparer"); }

            UnregisterSortable(sortableProperty);

            SortableMethodList.Add(new SortableMethod<T>(sortableProperty, comparer));
        }

        /// <summary>
        /// 指定のプロパティに対するユーザー定義のソートを削除する
        /// </summary>
        /// <param name="sortableProperty"></param>
        public void UnregisterSortable(PropertyDescriptor sortableProperty)
        {
            if (sortableProperty == null) { throw new ArgumentNullException("sortableProperty"); }

            SortableMethodList.RemoveAll(
                (item) => { return item.Property.Equals(sortableProperty); });
        }

        /// <summary>
        /// 全てのユーザー定義のソートを削除する
        /// </summary>
        public void ResetSortable()
        {
            SortableMethodList.Clear();
        }


        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            SortableMethod<T> Method = SortableMethodList.Find((item) =>
                property.Equals(item.Property));

            List<T> SortList = this.Items as List<T>;

            if (SortList != null)
            {
                if (Method != null)
                {
                    SortList.Sort(Method.GetComparer(direction));
                }
                else
                {
                    SortList.Sort(PropertyComparerFactory.Factory<T>(property, direction));
                }

                this._isSorted = true;
                this._sortProp = property;
                this._sortDir = direction;

                this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected override bool SupportsSortingCore { get { return true; } }

        protected override bool IsSortedCore { get { return this._isSorted; } }

        protected override PropertyDescriptor SortPropertyCore { get { return this._sortProp; } }

        protected override ListSortDirection SortDirectionCore { get { return this._sortDir; } }

    }

    public static class PropertyComparerFactory
    {
        public static IComparer<T> Factory<T>(PropertyDescriptor property, ListSortDirection direction)
        {
            Type seed = typeof(PropertyComparer<,>);
            Type[] typeArgs = { typeof(T), property.PropertyType };
            Type pcType = seed.MakeGenericType(typeArgs);

            IComparer<T> comparer = (IComparer<T>)Activator.CreateInstance(pcType, new object[] { property, direction });
            return comparer;
        }
    }

    public sealed class PropertyComparer<T, U> : IComparer<T>
    {
        private PropertyDescriptor _property;
        private ListSortDirection _direction;
        private Comparer<U> _comparer;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            this._property = property;
            this._direction = direction;
            this._comparer = Comparer<U>.Default;
        }

        public int Compare(T x, T y)
        {
            U xValue = (U)this._property.GetValue(x);
            U yValue = (U)this._property.GetValue(y);

            if (this._direction == ListSortDirection.Ascending)
                return this._comparer.Compare(xValue, yValue);
            else
                return this._comparer.Compare(yValue, xValue);
        }
    }

    /// <summary>
    /// ソートするプロパティと、ソートの方法を保持するクラス
    /// </summary>
    /// <remarks>
    /// プロパティをキーに、IComparerオブジェクトを持つ。
    /// 
    /// 又、ソートの方向を意識したIComparerオブジェクトを生成する。
    /// 降順ソート時には、昇順ソート時の正反対の結果を返すIComparerが必要だから。
    /// </remarks>
    public sealed class SortableMethod<T>
    {
        public SortableMethod(PropertyDescriptor property, IComparer<T> comparer)
        {
            Property = property;
            Comparer = comparer;
        }


        public PropertyDescriptor Property { get; private set; }

        private IComparer<T> Comparer { get; set; }


        /// <summary>
        /// 指定の方向へのソートを行うためのIComparerオブジェクトを取得します。
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public IComparer<T> GetComparer(ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
                return new DecidedDirectionComparer<T>(this.AscendingCompare);
            else
                return new DecidedDirectionComparer<T>(this.DescendingCompare);
        }

        /// <summary>
        /// 指定の方向へのソートを行うためのComparisonデリゲートを取得します。
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Comparison<T> GetComparison(ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
                return this.AscendingCompare;
            else
                return this.DescendingCompare;
        }

        private int AscendingCompare(T x, T y)
        {
            return Comparer.Compare(x, y);
        }

        private int DescendingCompare(T x, T y)
        {
            return Comparer.Compare(y, x);
        }
    }


    /// <summary>
    /// SortableMethodクラスにて生成されるIComparerオブジェクト
    /// </summary>
    public sealed class DecidedDirectionComparer<T> : IComparer<T>
    {
        public DecidedDirectionComparer(Comparison<T> comparison)
        {
            Comparison = comparison;
        }

        Comparison<T> Comparison { get; set; }

        #region IComparer<IIoListItem> メンバ

        public int Compare(T x, T y)
        {
            return Comparison(x, y);
        }

        #endregion
    }
}
