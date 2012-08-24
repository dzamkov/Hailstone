using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Hailstone.UI;
using Hailstone.Interface;

namespace Hailstone
{
    /// <summary>
    /// Contains program-related functions.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program main entry-point.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Initialize();
            Application.EnableVisualStyles();
            MainForm form = new MainForm();
            Timer trender = new Timer();
            trender.Interval = 15;
            trender.Tick += delegate { form.Render(); };
            trender.Start();
            Timer tupdate = new Timer();
            tupdate.Interval = 10;
            tupdate.Tick += delegate { form.Update(0.01); };
            tupdate.Start();
            Application.Run(form);
        }

        /// <summary>
        /// Creates the interface for the program.
        /// </summary>
        public static void Initialize()
        {
            new DoubleTypeInterface().Register();
            new IntegerTypeInterface().Register();
            new StringTypeInterface().Register();
            new AutoTypeInterface(typeof(Settings.Color)).Register();
            new AutoTypeInterface(typeof(Settings.ExtendedColor)).Register();
            new AutoTypeInterface(typeof(Settings)).Register();
        }
    }
}
