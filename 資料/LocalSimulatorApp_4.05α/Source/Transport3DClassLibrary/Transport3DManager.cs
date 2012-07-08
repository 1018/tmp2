using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;


namespace Transport3DClassLibrary
{
    /// <summary>
    /// 搬送3D共通管理クラス
    /// </summary>
    public class Transport3DManager
    {
        private Dictionary<Form, List<Transport3D_Draw>> transPort3DBaseFormList = new Dictionary<Form, List<Transport3D_Draw>>();

        private static Transport3DManager singletonInstance;

        /// <summary>
        /// Transport3DManagerインスタンス作成
        /// </summary>
        /// <returns>Transport3DManagerインスタンス</returns>
        public static Transport3DManager CreateInstance()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new Transport3DManager();
            }
            return singletonInstance;
        }
        
        /// <summary>
        /// BaseForm内の全ITransport3Dを取得する
        /// </summary>
        /// <param name="baseForm">自BaseFormインスタンス</param>
        /// <returns>自BaseForm内ITransport3Dリスト</returns>
        public List<Transport3D_Draw> GetITransport3DList(Form baseForm)
        {           
            List<Transport3D_Draw> newTransportList = new List<Transport3D_Draw>();

            bool findItransport3dList = transPort3DBaseFormList.TryGetValue(baseForm, out newTransportList);

            //既にITransport3DListがあるBaseFormの場合
            if (findItransport3dList)
            {
                return newTransportList;
            }

            //ITransport3DListがないBaseFormの場合、新規作成する。            
            newTransportList = new List<Transport3D_Draw>();
            foreach (Control childControl in baseForm.Controls)
            {
                Transport3D_Draw transport3D = childControl as Transport3D_Draw;
                if (transport3D == null) { continue; }
                newTransportList.Add(transport3D);
            }
            transPort3DBaseFormList.Add(baseForm, newTransportList);
            return newTransportList; 
            

        } 


    }
    

    
}
