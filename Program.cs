using System;
using System.Collections.Generic;

namespace Hailstone
{
    /// <summary>
    /// Contains program-related functions.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The title for the main window of the program.
        /// </summary>
        public static readonly string Title = "Hailstone";

        /// <summary>
        /// Program main entry-point.
        /// </summary>
        public static void Main()
        {
            new Window().Run();
        }
    }
}
