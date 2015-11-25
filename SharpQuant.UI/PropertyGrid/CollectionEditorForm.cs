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

        public delegate void InstanceEventHandler(object sender, object instance);
        public event InstanceEventHandler InstanceCreated;
        public event InstanceEventHandler DestroyingInstance;
        public event InstanceEventHandler ItemRemoved;
        public event InstanceEventHandler ItemAdded;
        private IList _collection = null;
        private Type lastItemType = null;
        private ArrayList backupList = null;
        private EditLevel _EditLevel = EditLevel.FullEdit;
        private CollectionEditor attachedEditor = null;
        private bool _isdirty = false;
        private bool _collectionChanged = false;

        #endregion

        #region Properties

        public IList Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                backupList = new ArrayList(value);
                ProccessCollection(value);
                RefreshValues();
            }
        }

        [Category("Behavior")]
        public EditLevel EditLevel
        {
            get { return _EditLevel; }
            set
            {
                if (value != _EditLevel)
                {
                    _EditLevel = value;
                    OnEditLevelChanged(new EventArgs());
                }
            }
        }

        Func<object> _ObjectFactory;
        public Func<object> ObjectFactory
        {
            get { return _ObjectFactory; }
            set 
            { 
                _ObjectFactory = 
                () => { var obj = value(); OnInstanceCreated(obj); return obj; }; 
            }
        }

        #endregion

        public CollectionEditorForm()
        {
            InitializeComponent();
            _ObjectFactory = () => CreateInstance();
        }

        #region Collection Item


        protected virtual object CreateInstance()
        {

            PropertyInfo pi = _collection.GetType().GetProperty("Item", new Type[] { typeof(int) });
            var itemType = pi.PropertyType;

            ConstructorInfo ci = itemType.GetConstructor(new Type[0]);
            var instance = ci.Invoke(new object[] { });

            OnInstanceCreated(instance);
            return instance;
        }


        protected virtual void DestroyInstance(object instance)
        {
            OnDestroyingInstance(instance);
            if (instance is IDisposable) { ((IDisposable)instance).Dispose(); }
            instance = null;
        }


        protected virtual void OnDestroyingInstance(object instance)
        {
            if (DestroyingInstance != null)
            {
                DestroyingInstance(this, instance);
            }
        }


        protected virtual void OnInstanceCreated(object instance)
        {
            if (InstanceCreated != null)
            {
                InstanceCreated(this, instance);
            }
        }

        protected virtual void OnItemRemoved(object item)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(this, item);
            }
        }

        protected virtual void OnItemAdded(object Item)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, Item);
            }
        }


        #endregion

        #region Implementation

        protected virtual void RefreshValues()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(GenerateTItemArray(this.Collection));

            treeView.EndUpdate();
        }

        protected virtual EditLevel SetEditLevel(IList collection)
        {
            return EditLevel.FullEdit;
        }

        private void SetCollectionEditLevel(IList collection)
        {
            EditLevel el = SetEditLevel(collection);

            switch (el)
            {
                case EditLevel.FullEdit:
                    {
                        this.btnRemove.Enabled = Remove_CanEnable();
                        this.btnAdd.Enabled = Add_CanEnable();
                        break;
                    }
                case EditLevel.AddOnly:
                    {
                        this.btnRemove.Enabled = Remove_CanEnable() && false;
                        this.btnAdd.Enabled = Add_CanEnable();
                        break;
                    }
                case EditLevel.RemoveOnly:
                    {
                        this.btnAdd.Enabled = Add_CanEnable() && false;
                        this.btnRemove.Enabled = Remove_CanEnable();
                        break;
                    }
                case EditLevel.ReadOnly:
                    {
                        this.btnRemove.Enabled = Remove_CanEnable() && false;
                        this.btnAdd.Enabled = Add_CanEnable() && false;
                        break;
                    }
            }


        }

        private bool Add_CanEnable()
        {
            if (this.EditLevel == EditLevel.FullEdit || this.EditLevel == EditLevel.AddOnly) { return true; }
            return false;
        }

        private bool Remove_CanEnable()
        {
            if (this.EditLevel == EditLevel.FullEdit || this.EditLevel == EditLevel.RemoveOnly) { return true; }
            return false;
        }


        private void ProccessCollection(IList collection)
        {
            SetCollectionEditLevel(collection);
        }


        private void btnOK_Click(object sender, System.EventArgs e)
        {
            NotifyParentIfDirty();
            this.Close();
        }


        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            UndoChanges(backupList, Collection);
            _collectionChanged = false;
            NotifyParentIfDirty();
            this.Close();

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            treeView.BeginUpdate();
            if (Collection != null)
            {
                //create new  item
                object newCollItem = ObjectFactory();
                TItem newTItem = CreateTItem(newCollItem);

                //get the current  possition  and the parent collections to insert into
                TItem selTItem = (TItem)treeView.SelectedNode;

                if (selTItem != null)
                {
                    int position = selTItem.Index + 1;

                    IList coll;
                    TreeNodeCollection TItemColl;

                    if (selTItem.Parent != null)
                    {
                        coll = (((TItem)selTItem.Parent).SubItems);
                        TItemColl = selTItem.Parent.Nodes;
                    }
                    else
                    {
                        coll = Collection;
                        TItemColl = treeView.Nodes;
                    }


                    coll.Insert(position, newCollItem);
                    TItemColl.Insert(position, newTItem);


                }
                else //empty collection
                {
                    Collection.Add(newCollItem);
                    treeView.Nodes.Add(newTItem);

                }

                OnItemAdded(newCollItem);
                _collectionChanged = true;
                treeView.SelectedNode = newTItem;

            }
            treeView.EndUpdate();	
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            treeView.BeginUpdate();
            TItem selTItem = (TItem)treeView.SelectedNode;
            if (selTItem != null)
            {
                int selIndex = selTItem.Index;
                TItem parentTitem = (TItem)selTItem.Parent;
                if (parentTitem != null)
                {
                    parentTitem.Nodes.Remove(selTItem);
                    parentTitem.SubItems.Remove(selTItem.Value);
                    if (parentTitem.Nodes.Count > selIndex) { treeView.SelectedNode = parentTitem.Nodes[selIndex]; }
                    else if (parentTitem.Nodes.Count > 0) { treeView.SelectedNode = parentTitem.Nodes[selIndex - 1]; }
                    else { treeView.SelectedNode = parentTitem; }
                }
                else
                {
                    treeView.Nodes.Remove(selTItem);
                    Collection.Remove(selTItem.Value);
                    if (treeView.Nodes.Count > selIndex) { treeView.SelectedNode = treeView.Nodes[selIndex]; }
                    else if (treeView.Nodes.Count > 0) { treeView.SelectedNode = treeView.Nodes[selIndex - 1]; }
                    else { this.propertyGrid.SelectedObject = null; }
                }

                OnItemRemoved(selTItem.Value);
                _collectionChanged = true;
            }
            treeView.EndUpdate();
        }


        private void btnUp_Click(object sender, System.EventArgs e)
        {
            treeView.BeginUpdate();
            TItem selTItem = (TItem)treeView.SelectedNode;
            if (selTItem != null && selTItem.PrevNode != null)
            {
                int prevIndex = selTItem.PrevNode.Index;
                TItem fatherTitem = (TItem)selTItem.Parent;
                if (fatherTitem != null)
                {


                    MoveItem(fatherTitem.SubItems, fatherTitem.SubItems.IndexOf(selTItem.Value), -1);
                    SetProperties(fatherTitem, fatherTitem.Value);
                    treeView.SelectedNode = fatherTitem.Nodes[prevIndex];

                }
                else
                {

                    MoveItem(Collection, Collection.IndexOf(selTItem.Value), -1);
                    treeView.Nodes.Clear();
                    treeView.Nodes.AddRange(GenerateTItemArray(this.Collection));
                    treeView.SelectedNode = treeView.Nodes[prevIndex];
                }
            }
            treeView.EndUpdate();
        }


        private void btnDown_Click(object sender, System.EventArgs e)
        {
            treeView.BeginUpdate();
            TItem selTItem = (TItem)treeView.SelectedNode;
            if (selTItem != null && selTItem.NextNode != null)
            {
                int nextIndex = selTItem.NextNode.Index;
                TItem fatherTitem = (TItem)selTItem.Parent;

                if (fatherTitem != null)
                {
                    MoveItem(fatherTitem.SubItems, fatherTitem.SubItems.IndexOf(selTItem.Value), 1);
                    SetProperties(fatherTitem, fatherTitem.Value);
                    treeView.SelectedNode = fatherTitem.Nodes[nextIndex];

                }
                else
                {
                    MoveItem(Collection, Collection.IndexOf(selTItem.Value), 1);
                    treeView.Nodes.Clear();
                    treeView.Nodes.AddRange(GenerateTItemArray(this.Collection));
                    treeView.SelectedNode = treeView.Nodes[nextIndex];
                }
            }
            treeView.EndUpdate();
        }


        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            treeView.BeginUpdate();
            TItem selTItem = (TItem)treeView.SelectedNode;

            SetProperties(selTItem, selTItem.Value);
            _isdirty = true;

            treeView.EndUpdate();
        }

        private void NotifyParentIfDirty()
        {
            if ((!_isdirty && !_collectionChanged) || this.Owner == null)
                return;
            var grid = this.Owner.Controls.OfType<PropertyWindow>().FirstOrDefault();
            if (grid == null)
                return;

            grid.SelectedObject.IsDirty = true;
            grid.RefreshEditedObject();
        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            propertyGrid.SelectedObject = ((TItem)e.Node).Value;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TItem ti = (TItem)e.Node;
            if (ti.Value.GetType() != lastItemType)
            {
                lastItemType = ti.Value.GetType();
                IList coll;
                if (ti.Parent != null) { coll = ((TItem)ti.Parent).SubItems; }
                else { coll = Collection; }
                ProccessCollection(coll);

            }
        }


        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            //TODO: get editor from Type; use custom only if type has no specific editor
            if (attachedEditor != null)
            {
                attachedEditor.CollectionChanged -= new CollectionEditor.CollectionChangedEventHandler(ValChanged);
                attachedEditor = null;
            }

            if (e.NewSelection.Value is IList)
            {
                attachedEditor = (CollectionEditor)e.NewSelection.PropertyDescriptor.GetEditor(typeof(System.Drawing.Design.UITypeEditor)) as CollectionEditor;
                if (attachedEditor != null)
                {
                    attachedEditor.CollectionChanged += new CollectionEditor.CollectionChangedEventHandler(ValChanged);
                }
            }



        }


        private void ValChanged(object sender, object instance, object value)
        {
            treeView.BeginUpdate();
            TItem ti = (TItem)treeView.SelectedNode;
            SetProperties(ti, instance);
            treeView.EndUpdate();
        }

        private void UndoChanges(IList source, IList dest)
        {
            foreach (object o in dest)
            {
                if (!source.Contains(o))
                {
                    DestroyInstance(o);
                    OnItemRemoved(o);
                }

            }

            dest.Clear();
            CopyItems(source, dest);
        }

        private void CopyItems(IList source, IList dest)
        {
            foreach (object o in source)
            {
                dest.Add(o);
                OnItemAdded(o);
            }
        }

        protected virtual void OnEditLevelChanged(EventArgs e)
        {
            switch (EditLevel)
            {
                case EditLevel.FullEdit:
                    {
                        this.btnAdd.Enabled = true;
                        this.btnRemove.Enabled = true;
                        break;
                    }
                case EditLevel.AddOnly:
                    {
                        this.btnAdd.Enabled = true;
                        this.btnRemove.Enabled = false;
                        break;
                    }
                case EditLevel.RemoveOnly:
                    {
                        this.btnAdd.Enabled = false;
                        this.btnRemove.Enabled = true;
                        break;
                    }
                case EditLevel.ReadOnly:
                    {
                        this.btnAdd.Enabled = false;
                        this.btnRemove.Enabled = false;
                        break;
                    }
            }
        }

        #endregion

        #region TItem Related

        private void MoveItem(IList list, int index, int step)
        {

            if (index > -1 && index < list.Count && index + step > -1 && index + step < list.Count)
            {
                int poss = index + step;

                object possObject = list[poss];
                list[poss] = list[index];
                list[index] = possObject;
                possObject = null;
            }

        }


        protected internal TItem[] GenerateTItemArray(IList collection)
        {
            TItem[] ti = new TItem[0];

            if (collection != null && collection.Count > 0)
            {
                ti = new TItem[collection.Count];

                for (int i = 0; i < collection.Count; i++)
                {
                    ti[i] = CreateTItem(collection[i]);
                }
            }
            return ti;
        }

        /// <summary>
        /// Creates a new TItem objectfor a collection item.
        /// </summary>
        /// <param name="reffObject">The collection item for wich to create an TItem object.</param>
        /// <returns>A TItem object referencing the collection item received as parameter.</returns>
        protected virtual TItem CreateTItem(object reffObject)
        {
            TItem ti = new TItem(this, reffObject);
            SetProperties(ti, reffObject);
            return ti;
        }

        /// <summary>
        /// When implemented by a class, customize a TItem object in respect to it's corresponding collection item.
        /// </summary>
        /// <param name="titem">The TItem object to be customized in respect to it's corresponding collection item.</param>
        /// <param name="reffObject">The collection item for which it customizes the TItem object.</param>
        protected virtual void SetProperties(TItem titem, object reffObject)
        {
            // set the display name 
            //PropertyInfo pi = titem.Value.GetType().GetProperty("Name");

            //if (pi != null)
            //{
            //    var text = pi.GetValue(titem.Value, null);

            //    titem.Text = text == null ? "New" : text.ToString();
            //}
            //else
            //{
            //    titem.Text = titem.Value.GetType().Name;
            //}

            var converter = TypeDescriptor.GetConverter(reffObject);
            var text = converter.ConvertToString(reffObject);

            titem.Text = string.IsNullOrEmpty(text) ? "New" : text;


        }



        #endregion


    }



    #region TItem

    public class TItem : TreeNode
    {
        private object _Value;
        private CollectionEditorForm ced = null;
        private IList _SubItems = null;


        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public IList SubItems
        {
            get { return _SubItems; }
            set
            {
                _SubItems = value;
                this.Nodes.Clear();
                if (value != null)
                {
                    this.Nodes.AddRange(ced.GenerateTItemArray(value));
                }

            }
        }


        public TItem(CollectionEditorForm ced, object Value)
        {
            this.ced = ced;
            this._Value = Value;
        }


        public TItem(CollectionEditorForm ced, object Value, int ImageIndex)
        {
            this.ced = ced;
            this._Value = Value;
            this.ImageIndex = ImageIndex;
        }
        public TItem(CollectionEditorForm ced, object Value, int ImageIndex, int SelectedImageIndex)
        {
            this.ced = ced;
            this._Value = Value;
            this.ImageIndex = ImageIndex;
            this.SelectedImageIndex = SelectedImageIndex;
        }



    }

    #endregion
}
