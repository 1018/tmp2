using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace LocalSimulator.ProjectMaker
{
    public class GroupListViewItem : ListViewItem
    {
        public GroupListViewItem(string items, int imageIndex) : base(items, imageIndex) { }

        public string GroupName { get; set; }
    }

    

    public class GourListViewItemCollection : List<GroupListViewItem>
    {
        public List<GroupListViewItem> GetGroupItems(string groupName)
        {
            List<GroupListViewItem> result = new List<GroupListViewItem>();

            result = this.FindAll(delegate(GroupListViewItem item) { return item.GroupName == groupName; });


            
            return result;

        }

        public List<string> GetGroupNames()
        {
            List<string> result = new List<string>();

            foreach (GroupListViewItem item in this)
            {
                string matchString = result.Find(delegate(string s) { return item.GroupName == s; });

                //グループリスト内にないグループ名の場合、グループリストに追加
                if (String.IsNullOrEmpty(matchString) && !String.IsNullOrEmpty(item.GroupName))
                {
                    result.Add(item.GroupName);
                }
                
            }

            return result;

        }

        public new ListViewItem[] ToArray()
        {
            ListViewItem[] result = new ListViewItem[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i];
            }

            return result;
        }


    }


}

