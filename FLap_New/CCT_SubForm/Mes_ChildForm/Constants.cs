using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLap_New.SubForm.Mes_ChildForm
{
    public class ConstantsFormQRChild
    {
        // Kích thước Form Con
        public const int WIDTH = 400;
        public const int HEGHT = 250;

        // Khoảng cách giữa các Form Con
        public const int SPACING = 20;

        // Kích thước Panel Con(QR code) trong Form Con
        public const int PANEL_WIDTH = 150;
        public const int PANEL_HEGHT = 150;

        //font chữ chung
        public static readonly Font FONT_DEFAULT = new Font("Microsoft Sans Serif", 9);
        public static readonly Font FONT_QRCODE_TEXT = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);

    }
}
