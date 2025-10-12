using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FLap_New.SubForm;

namespace FLap_New
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmIndex());
            Application.Run(new frmFabricInMes());
        }
    }
}
