using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.Docking
{
    public interface IWindowDef
    {
        string ClassName { get; set; }
        string AssemblyName { get; set; }
        string TabText { get; set; }
        string ButtonImage { get; set; }
        string ButtonText { get; set; }
        string ButtonName { get; set; }
        string ButtonTooltip { get; set; }
        string MenuText { get; set; }
        int DockAreas { get; set; }
        string Icon { get; set; }

        Type ViewType { get; set; }
    }
    public class WindowDef : IWindowDef
    {
        public string ClassName { get; set; }
        public string AssemblyName { get; set; }
        public string TabText { get; set; }
        public string ButtonImage { get; set; }
        public string ButtonText { get; set; }
        public string ButtonName { get; set; }
        public string ButtonTooltip { get; set; }
        public string MenuText { get; set; }
        public int DockAreas { get; set; }
        public string Icon { get; set; }

        public Type ViewType { get; set; }
    }
}
