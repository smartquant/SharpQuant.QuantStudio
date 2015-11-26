using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SharpQuant.UI.PropertyGrid
{
    public enum EditLevel
    {
        FullEdit = 0,
        AddOnly = 1,
        RemoveOnly = 2,
        ReadOnly = 3
    }

    public partial class CollectionEditorForm : Form
    {
        #region fields

        private ICollectionEditor _attachedEditor;

        private bool _isdirty = false;
        private bool _collectionChanged = false;
        private bool _updownremove = false;
        #endregion

        public CollectionEditorForm()
        {
            InitializeComponent();
        }

        public void Init(ICollectionEditor editor)
        {
            _attachedEditor = editor;
            RefreshValues();
            UpdateButtons();
        }

        #region Implementation

        private void UpdateButtons()
        {
            this.btnRemove.Enabled = (_attachedEditor.EditLevel == EditLevel.FullEdit || _attachedEditor.EditLevel == EditLevel.AddOnly);
            this.btnAdd.Enabled = (_attachedEditor.EditLevel == EditLevel.FullEdit || _attachedEditor.EditLevel == EditLevel.RemoveOnly);
        }


        protected virtual void RefreshValues()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            foreach(var item in _attachedEditor.Items())
            {
                var node = new TreeNode(_attachedEditor.ItemName(item));
                node.Tag = item;
                treeView.Nodes.Add(node);
            }
            treeView.EndUpdate();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (propertyGrid.SelectedObject != null && !_attachedEditor.Validate(propertyGrid.SelectedObject))
                return;
            NotifyParentIfDirty();
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            _attachedEditor.UndoChanges();
            _collectionChanged = false;
            NotifyParentIfDirty();
            this.Close();

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {           
            if (propertyGrid.SelectedObject != null && !_attachedEditor.Validate(propertyGrid.SelectedObject))
                return;

            treeView.BeginUpdate();
            //get the current  possition  and the parent collections to insert into
            var selTItem = treeView.SelectedNode;

            int position = (selTItem != null) ? selTItem.Index + 1 : 0;
            object newTItem = _attachedEditor.Add(position);
            var node = new TreeNode(_attachedEditor.ItemName(newTItem)) { Tag = newTItem };

            if (selTItem != null)
                treeView.Nodes.Insert(position, node);
            else //empty collection
                treeView.Nodes.Add(node);
            _collectionChanged = true;
            treeView.SelectedNode = node;

            treeView.EndUpdate();
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            treeView.BeginUpdate();
            
            var selTItem = treeView.SelectedNode;
            if (selTItem != null)
            {
                _updownremove = true;
                int selIndex = selTItem.Index;

                treeView.Nodes.Remove(selTItem);
                _attachedEditor.Remove(selTItem.Tag);
                if (treeView.Nodes.Count > selIndex) { treeView.SelectedNode = treeView.Nodes[selIndex]; }
                else if (treeView.Nodes.Count > 0) { treeView.SelectedNode = treeView.Nodes[selIndex - 1]; }
                else { this.propertyGrid.SelectedObject = null; }


                _collectionChanged = true;
            }
            treeView.EndUpdate();
        }


        private void btnUp_Click(object sender, System.EventArgs e)
        {
            treeView.BeginUpdate();
            _updownremove = true;
            var selTItem = treeView.SelectedNode;
            if (selTItem != null && selTItem.PrevNode != null)
            {
                int prevIndex = selTItem.PrevNode.Index;

                _attachedEditor.MoveItem(selTItem.Tag, -1);
                RefreshValues();
                treeView.SelectedNode = treeView.Nodes[prevIndex];

            }
            treeView.EndUpdate();
        }


        private void btnDown_Click(object sender, System.EventArgs e)
        {
            treeView.BeginUpdate();
            _updownremove = true;
            var selTItem = treeView.SelectedNode;
            if (selTItem != null && selTItem.NextNode != null)
            {
                int nextIndex = selTItem.NextNode.Index;

                _attachedEditor.MoveItem(selTItem.Tag, 1);
                RefreshValues();
                treeView.SelectedNode = treeView.Nodes[nextIndex];

            }
            treeView.EndUpdate();
        }


        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            treeView.BeginUpdate();
            var selTItem = treeView.SelectedNode;

            selTItem.Text = _attachedEditor.ItemName(selTItem.Tag);
            _isdirty = true;
            //refresh grid too
            RefreshGrid();
            treeView.EndUpdate();
        }

        private void RefreshGrid()
        {
            var temp = propertyGrid.SelectedObject;
            propertyGrid.SelectedObject = null;
            propertyGrid.SelectedObject = temp;
        }

        private void NotifyParentIfDirty()
        {
            if ((!_isdirty && !_collectionChanged) || this.Owner == null)
                return;

            //bubble up if we were called from another collection editor
            var editor = this.Owner as CollectionEditorForm;
            if (editor!=null)
            {
                editor._isdirty = _isdirty;
                editor._collectionChanged = _collectionChanged;
                editor.Refresh();
                return;
            }
            //notify top PropertyWindow if available
            var grid = this.Owner.Controls.OfType<PropertyWindow>().FirstOrDefault();
            if (grid == null)
                return;

            grid.SelectedObject.IsDirty = true;
            grid.RefreshEditedObject();
        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
         
            if (!_updownremove && propertyGrid.SelectedObject!=null && !propertyGrid.SelectedObject.Equals(e.Node.Tag) && !_attachedEditor.Validate(propertyGrid.SelectedObject))
                e.Cancel = true;
            else
                propertyGrid.SelectedObject = e.Node.Tag;
            _updownremove = false;
        }


        #endregion


    }

}
