using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpQuant.UI.Command;

namespace SharpQuant.UI.Utils
{
    public interface IToolStripDef
    {
        ICommand Command { get; set; }
        string Image { get; set; }
        string Name { get; set; }
        int SizeH { get; set; }
        int SizeV { get; set; }
        string Text { get; set; }
        string ToolTip { get; set; }
    }

    public class ToolStripDef : IToolStripDef
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public int SizeH { get; set; }
        public int SizeV { get; set; }
        public string Image { get; set; }
        public string ToolTip { get; set; }

        public ICommand Command { get; set; }
    }
}
