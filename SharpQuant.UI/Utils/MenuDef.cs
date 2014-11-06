using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpQuant.UI.Command;

namespace SharpQuant.UI.Utils
{
    public enum MenuType
    {
        Menu,
        Command,
        Seperator
    }

    public interface IMenuDef
    {
        ICommand Command { get; set; }
        string Description { get; set; }
        int ID { get; set; }
        string Image { get; set; }
        MenuType MenuType { get; set; }
        string Name { get; set; }
        int ParentID { get; set; }
        int ShortCutKeys { get; set; }
        int SizeH { get; set; }
        int SizeV { get; set; }
        string Text { get; set; }
    }

    public class MenuDef : IMenuDef
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public MenuType MenuType { get; set; }
        public string Name { get; set; } //name of the control
        public string Text { get; set; } //menu text to be displayed
        public string Description { get; set; } //e.g. tooltip help
        public int SizeH { get; set; }
        public int SizeV { get; set; }
        public string Image { get; set; }
        public int ShortCutKeys { get; set; }

        public ICommand Command { get; set; }

    }
}
