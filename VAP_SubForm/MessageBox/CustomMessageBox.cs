using System.Windows.Forms;

namespace CustomMessageBoxDemo
{
    public static class CustomMessageBox
    {
        public static DialogResult Show(string message, string title = "Thông báo")
        {
            using (var frm = new FrmMessageBox(title, message))
            {
                return frm.ShowDialog();
            }
        }
    }
}
