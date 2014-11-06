using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Resources;


using SharpQuant.UI.Utils;

namespace SharpQuant.UI.MenuTreeView
{
    public partial class MenuTreeViewControl : UserControl
    {
        public MenuTreeViewControl()
        {
            InitializeComponent();
        }


        public void Init(IEnumerable<ITreeMenuDef> items, IEnumerable<IMenuDef> menus, ResourceManager rm)
        {
            InitializeTreeView(items);

            _dynMenu = new DynamicContextMenu(ctxDynamicContext, menus, rm);
        }


        #region treeview

        class GroupNode : TreeNode
        {

            public GroupNode(string group, string tooltip)
            {
                base.Text = group;
                base.ToolTipText = tooltip;
            }
        }

        class LeefNode : TreeNode
        {
            ITreeMenuDef _item;

            public ITreeMenuDef MenuItem
            {
                get { return _item; }
            }
            public LeefNode(ITreeMenuDef item)
            {
                _item = item;
                base.Text = item.Name;
                base.ToolTipText = item.Description;
            }
        }


        private void addSubitems(int parentID, TreeNode node, IEnumerable<ITreeMenuDef> defs)
        {
            foreach (var def in defs.Where(p => p.ParentID == parentID))
            {
                if (!def.IsLeaf)
                {
                    var treenode = new GroupNode(def.Name, def.Description);
                    node.Nodes.Add(treenode);
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                    {
                        addSubitems(def.ID,treenode, submenus);
                    }
                }
                else
                {
                    var leefNode = new LeefNode(def);
                    node.Nodes.Add(leefNode);
                }
            }
        }

        private void InitializeTreeView(IEnumerable<ITreeMenuDef> defs)
        {

            treeview.Nodes.Clear();
            
            treeview.BeginUpdate();

            foreach (var def in defs.Where(p => p.ParentID == 0))
            {
                if (!def.IsLeaf)
                {
                    var treenode = new GroupNode(def.Name, def.Description);
                    treeview.Nodes.Add(treenode);
                    var submenus = defs.Where(p => p.ParentID == def.ID);
                    if (submenus.Count() > 0)
                    {
                        addSubitems(def.ID,treenode, submenus);
                    }
                }
                else
                {
                    var leefNode = new LeefNode(def);
                    treeview.Nodes.Add(leefNode);
                }
            }

            treeview.EndUpdate();
        }



        #endregion


        #region dynamic context menu


        DynamicContextMenu _dynMenu; 

     
        private void tvInstruments_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                LeefNode node = this.treeview.SelectedNode as LeefNode;
                if (node != null)
                {
                    _dynMenu.RenderDynamicContextMenu(node.MenuItem.Menus, node.MenuItem.Settings);
                    ctxDynamicContext.Show(this, e.Location.X, e.Location.Y + 20);
                }
            }
            
        }

        #endregion

    }
}