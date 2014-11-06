using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

using System.Windows.Forms;

namespace SharpQuant.UI.Utils
{
    public class DynamicContextMenu
    {
        Dictionary<string, ToolStripItem> _menus;

        ToolStrip _toolstrip;
        IEnumerable<IMenuDef> _menuDefs;
        string _settings;

        public string Settings
        {
            get { return _settings; }
        }

        public DynamicContextMenu(ToolStrip toolstrip, IEnumerable<IMenuDef> menus, ResourceManager resourcemanager)
        {
            _menus = new Dictionary<string, ToolStripItem>(); 

            _toolstrip = toolstrip;
            _menuDefs = menus;

            foreach (var menu in menus)
            {
                _menus.Add(menu.Name, menu.CreateItem(resourcemanager,()=>this.Settings)); //pass the settings as an argument to ICommand
            }

        }

        public void RenderDynamicContextMenu(string menus, string settings, char sep = '|')
        {
            _settings = settings; //pass settings to command

            if ((string)_toolstrip.Tag == menus) return;

            _toolstrip.Tag = menus;

            _toolstrip.Items.Clear();
            string[] menusList = menus.Split(sep);
            foreach (string menu in menusList)
            {
                _toolstrip.Items.Add(_menus[menu]);
            }
        }


    }
}
