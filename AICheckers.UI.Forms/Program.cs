using System;
using System.Diagnostics;
using System.Windows.Forms;
using IACheckers.UI.Forms.Game;

namespace IACheckers.UI.Forms
{
    internal static class Program
    {
        /// <sumary>
        ///     Program entry point
        /// </sumary>
        [STAThread]
        public static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Application.Run(new MainWindow());
        }
    }
}