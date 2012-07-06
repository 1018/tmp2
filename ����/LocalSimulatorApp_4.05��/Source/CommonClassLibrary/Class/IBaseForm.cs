using System.Drawing;

namespace CommonClassLibrary
{
    public interface IBaseForm
    {
        string TitleName { get; set; }
        Font Font { get; set; }
        Color BackColor { get; set; }
        int Number { get; set; }
        string Text { get; set; }
        Size BaseFormSize { get; set; }
        void SendToBack();

    }
}
