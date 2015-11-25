using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.PropertyGrid
{

    public class CollectionEditor : CollectionEditorBase<object>
    {
        public override Func<object> Factory
        {
            get 
            { 
                return null; 
            }
        }
    }

    public abstract class CollectionEditorBase<T> : System.Drawing.Design.UITypeEditor
    {

        public delegate void CollectionChangedEventHandler(object sender, object instance, object value);
        public event CollectionChangedEventHandler CollectionChanged;

        private ITypeDescriptorContext _context;

        private IWindowsFormsEditorService edSvc = null;

        public abstract Func<T> Factory { get; }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                object originalValue = value;
                _context = context;
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    CollectionEditorForm collEditorFrm = CreateForm();
                    collEditorFrm.ItemAdded += new CollectionEditorForm.InstanceEventHandler(ItemAdded);
                    collEditorFrm.ItemRemoved += new CollectionEditorForm.InstanceEventHandler(ItemRemoved);

                    collEditorFrm.Collection = (System.Collections.IList)value;


                    context.OnComponentChanging();
                    if (edSvc.ShowDialog(collEditorFrm) == DialogResult.OK)
                    {
                        OnCollectionChanged(context.Instance, value);
                        context.OnComponentChanged();
                    }



                }
            }

            return value;
        }


        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }


        private void ItemAdded(object sender, object item)
        {

            if (_context != null && _context.Container != null)
            {
                IComponent icomp = item as IComponent;
                if (icomp != null)
                {
                    _context.Container.Add(icomp);
                }
            }

        }


        private void ItemRemoved(object sender, object item)
        {
            if (_context != null && _context.Container != null)
            {
                IComponent icomp = item as IComponent;
                if (icomp != null)
                {
                    _context.Container.Remove(icomp);
                }
            }

        }


        protected virtual void OnCollectionChanged(object instance, object value)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, instance, value);
            }
        }


        protected virtual CollectionEditorForm CreateForm()
        {
            var frm = new CollectionEditorForm();
            var factory = Factory;

            if (factory!=null)
                frm.ObjectFactory = ()=>(object)Factory();

            return frm;
        }


    }
}

