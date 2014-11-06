using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;

using SharpQuant.UI.Command;

namespace SharpQuant.UI.Utils
{
    public static class MenuBuilder
    {

        public static void AppendMenuItems(this MenuStrip root, ResourceManager resourcemanager, IEnumerable<IMenuDef> defs)
        {
            foreach (var def in defs.Where(p=>p.ParentID==0))
            {

                var elem = CreateItem(def,resourcemanager);
                root.Items.Add(elem);

                if (def.MenuType == MenuType.Menu)
                {
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                        AppendSubItems(elem as ToolStripMenuItem, resourcemanager, submenus);
    
                }
            }
        }
        public static void InsertMenuItems(this MenuStrip root, ResourceManager resourcemanager, IEnumerable<IMenuDef> defs, int index)
        {
            foreach (var def in defs.Where(p => p.ParentID == 0))
            {

                var elem = CreateItem(def, resourcemanager);
                root.Items.Insert(index,elem);

                if (def.MenuType == MenuType.Menu)
                {
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                        AppendSubItems(elem as ToolStripMenuItem, resourcemanager, submenus);

                }
            }
        }
        public static void AppendMenuItems(this ToolStripMenuItem root, ResourceManager resourcemanager, IEnumerable<IMenuDef> defs)
        {
            foreach (var def in defs.Where(p => p.ParentID == 0))
            {

                var elem = CreateItem(def, resourcemanager);
                root.DropDownItems.Add(elem);

                if (def.MenuType == MenuType.Menu)
                {
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                        AppendSubItems(elem as ToolStripMenuItem, resourcemanager, submenus);

                }
            }
        }
        public static void InsertMenuItems(this ToolStripMenuItem root, ResourceManager resourcemanager, IEnumerable<IMenuDef> defs, int index)
        {
            foreach (var def in defs.Where(p => p.ParentID == 0))
            {

                var elem = CreateItem(def, resourcemanager);
                root.DropDownItems.Insert(index, elem);

                if (def.MenuType == MenuType.Menu)
                {
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                        AppendSubItems(elem as ToolStripMenuItem, resourcemanager, submenus);

                }
            }
        }

        public static void AppendSubItems(this ToolStripMenuItem menu, ResourceManager resourcemanager,IEnumerable<IMenuDef> defs)
        {
            foreach (var def in defs)
            {
                var elem = CreateItem(def, resourcemanager);

                menu.DropDownItems.Add(elem);

                if (def.MenuType == MenuType.Menu)
                {
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                        AppendSubItems(elem as ToolStripMenuItem,resourcemanager, submenus);

                }
            }
        }

        public static void InsertSubItems(this ToolStripMenuItem menu, ResourceManager resourcemanager, IEnumerable<IMenuDef> defs, int index)
        {
            foreach (var def in defs)
            {
                var elem = CreateItem(def, resourcemanager);

                menu.DropDownItems.Insert(index,elem);

                if (def.MenuType == MenuType.Menu)
                {
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                        AppendSubItems(elem as ToolStripMenuItem, resourcemanager, submenus);

                }
            }
        }

        public static ToolStripItem CreateItem(this IMenuDef def, ResourceManager resourcemanager, Func<object> commandParameterCallback = null)
        {
            ToolStripItem elem = null;
            if (def.MenuType == MenuType.Seperator)
            {
                elem = new ToolStripSeparator();
            }
            else
            {
                elem = new ToolStripMenuItem();
                elem.Text = def.Text;
                if (!string.IsNullOrEmpty(def.Image))
                    elem.Image = (Image)resourcemanager.GetObject(def.Image);
                if (def.ShortCutKeys > 0)
                {
                    ((ToolStripMenuItem)elem).ShortcutKeys = (Keys)def.ShortCutKeys;
                }
                if (def.Command != null)
                {
                    if (commandParameterCallback == null) commandParameterCallback = () => null;
                    MenuItemCommandBinding binding = new MenuItemCommandBinding(elem as ToolStripMenuItem, def.Command, commandParameterCallback);
                }
            }

            elem.Name = def.Name;
            elem.Size = new System.Drawing.Size(def.SizeH, def.SizeV);

            return elem;
        }
    }
}
