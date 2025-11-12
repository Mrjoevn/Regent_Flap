using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLap_New.SubForm.Mes_ChildForm
{
    public class FabricInfoDTO
    {
        public string So { get; set; }
        public string Roll { get; set; }
        public string Batch { get; set; }
        public decimal Length { get; set; }
        public string Color { get; set; }
        public string Style { get; set; }
        public string Barcode { get; set; }
        public Bitmap QRCode { get; set; }
    }
}
