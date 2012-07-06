using System;

namespace System.Windows.Forms
{
	public class SelectedTreeListViewItemCollection : ListView.SelectedListViewItemCollection
	{
        new public TreeListViewItem this[int index]
        {
            get { return ((TreeListViewItem)base[index]); }
        }

        public SelectedTreeListViewItemCollection(TreeListView TreeListView)
            : base((ListView)TreeListView)
        {
        }

        public bool Contains(TreeListViewItem item)
        {
            return (base.Contains((ListViewItem)item));
        }

        public int IndexOf(TreeListViewItem item)
        {
            return (base.IndexOf((ListViewItem)item));
        }
	}
}
