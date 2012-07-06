using System;

namespace System.Windows.Forms
{
	public interface ITreeListViewItemComparer : System.Collections.IComparer
	{
        SortOrder SortOrder
        {
            get;
            set;
        }

        int Column
        {
            get;
            set;
        }
	}
}
