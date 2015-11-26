using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Linq;
using System.Text;

using SharpQuant.Common.Validation;
using SharpQuant.UI.UserMessage;

namespace SharpQuant.UI.PropertyGrid
{

    public class CollectionEditor : CollectionEditorBase<object>
    {
    }

    public interface ICollectionEditor
    {
        System.Collections.IEnumerable Items();
        string ItemName(object item);
        EditLevel EditLevel { get; set; }

        object Add(int position);
        void Remove(object item);
        void MoveItem(object item, int step);
        void UndoChanges();

        bool Validate(object item);
    }

    public abstract class CollectionEditorBase<T> : System.Drawing.Design.UITypeEditor, ICollectionEditor where T:class
    {
        #region fields
        
        private ITypeDescriptorContext _context;
        private IWindowsFormsEditorService edSvc = null;
        protected IList<T> _collection;
        protected IList<T> _backup = new List<T>();
        private EditLevel _EditLevel = EditLevel.FullEdit;

        //Set these in in derived class
        protected IValidationProvider _validationProvider;
        protected IUserMessageService _message;

        #endregion



        public virtual T Factory(ITypeDescriptorContext context)
        {
            Type t = typeof(T);
            T instance = (t.GetConstructor(Type.EmptyTypes) != null) ? (T)Activator.CreateInstance(t, new object[0])
            : Activator.CreateInstance<T>();
            return instance;
        }

        public virtual string FormName()
        {
            var t = typeof(T);
            return string.Format("{0}Editor",t.Name);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                object originalValue = value;
                _context = context;
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                _collection = value as IList<T>;
                _backup = new List<T>(_collection);

                if (edSvc != null)
                {
                    CollectionEditorForm collEditorFrm = CreateForm();
                    collEditorFrm.Init(this);

                    context.OnComponentChanging();
                    if (edSvc.ShowDialog(collEditorFrm) == DialogResult.OK)
                    {
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


        protected virtual CollectionEditorForm CreateForm()
        {
            var frm = new CollectionEditorForm();
            frm.Text = FormName();

            return frm;
        }

        #region ICollectionEditor

        public EditLevel EditLevel
        {
            get { return _EditLevel; }
            set { _EditLevel = value; }

        }


        public System.Collections.IEnumerable Items()
        {
            foreach (var item in _collection)
                yield return item;
        }

        public virtual string ItemName(object obj)
        {
            var converter = TypeDescriptor.GetConverter(obj);
            var text = converter.ConvertToString(obj);

            return string.IsNullOrEmpty(text) ? "New" : text;
        }

        public virtual object Add(int position)
        {
            T item = Factory(_context);
            _collection.Insert(position, item);
            return item;

        }

        public virtual void Remove(object item)
        {
            _collection.Remove((T)item);
        }

        public void MoveItem(object item, int step)
        {
            int index = _collection.IndexOf((T)item);
            if (index > -1 && index < _collection.Count && index + step > -1 && index + step < _collection.Count)
            {
                int pos = index + step;

                T obj = _collection[pos];
                _collection[pos] = _collection[index];
                _collection[index] = obj;
            }

        }

        public virtual void UndoChanges()
        {
            _collection.Clear();
            foreach (T item in _backup)
                _collection.Add(item);
        }

        public virtual bool Validate(object item)
        {
            if (_validationProvider != null && _message!=null)
            {
                var validator = _validationProvider.GetValidator<T>();
                var value = item as T;
                if (validator != null && value != null)
                {
                    if (validator.IsValid(value))
                        return true;
                    var msg = validator.GetErrorMessages(value);
                    _message.ShowUserMessage(SharpQuant.UI.UserMessage.UserMessageCategories.Error, "Validation error", "Entity has some invalid values:\r\n" + string.Join("\r\n", msg));
                    return false;
                }
            }
            return true;
        }

        #endregion




    }
}

