using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

using System.Diagnostics;

namespace FramesPlayer
{

    static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName([MarshalAs(UnmanagedType.LPTStr)] string path,
                                                 [MarshalAs(UnmanagedType.LPTStr)] StringBuilder longPath,
                                                 int longPathLength);

        /// <summary>Главная точка входа для приложения.</summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0) Application.Run(new FormMain(args[0]));
            else Application.Run(new FormMain(""));
        }
    }

}
