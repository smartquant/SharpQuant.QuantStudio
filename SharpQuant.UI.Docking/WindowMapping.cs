using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.Docking
{
    public class WindowMapping
    {
        /// <summary>
        /// The type is not necassary same as QDockingWindow, because we can use this to wrap/reference UserControls
        /// </summary>
        public Type Type { get; set; }
        public QDockingWindow Window { get; set; }
    }
}
