using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.MenuTreeView
{
    public interface ITreeMenuDef
    {
        int ID { get; }
        int ParentID { get; }
        bool IsLeaf { get;  }
        string Name { get;  }
        string Description { get;  }
        string Menus { get;  }
        string Settings { get; }

    }

    public class TreeMenuDef : ITreeMenuDef
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public bool IsLeaf { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Menus { get; set; }
        public string Settings { get; set; }

    }
}
