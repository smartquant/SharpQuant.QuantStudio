using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using SharpQuant.Common.DB;

namespace SharpQuant.UI.Controls
{
    //remark we could have it as <T> and use a service locator to resolute to specific implementation
    // or use this as a base class




    public class LookupTextbox<T>:TextBox, IUpdateAction where T:class
    {
        Action _afterLookup=null;
        string[] _propertyNames;
        Func<IList<T>> _getList; //like that we have delayed lookup possibility

        public LookupTextbox()
        {
        }

        public void Init(string[] propertyNames, IRepositoryProvider rep, string whereClause)
        {
            this.KeyDown += new KeyEventHandler(ListLookupTextbox_KeyDown);
            this.DoubleClick += new EventHandler(ListLookupTextbox_DoubleClick);
            this.ReadOnly = true;
            _propertyNames = propertyNames;

            //here this is much easier to do because we know T; in DynamicEdit we need to use reflection/emit/expressions...
            Func<IList<T>> getlist = () => { return rep.GetRepository<T>().SearchFor(whereClause).ToList(); };
            _getList = getlist;
        }

        public void SetAfterLookup(Action afterLookup)
        {
            _afterLookup = afterLookup;
        }

        void ListLookupTextbox_DoubleClick(object sender, EventArgs e)
        {
            ShowListLookup();
        }

        void ListLookupTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3) ShowListLookup();
        }

        void ShowListLookup()
        {
            //we could use a service locator here if we use T
            ListLookup<T> lookup = new ListLookup<T>(_propertyNames,_getList());
            var result = lookup.ShowDialog();

            if (lookup.SelectedItem == null) return;

            var location = lookup.SelectedItem;


            this.Text = location.ID.ToString();
            
            //force validation => this member is non-public, therefore use reflection
            this.GetType().InvokeMember("PerformControlValidation", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, this, new object[] { true });
            // force data binding update
            this.DataBindings[0].WriteValue(); 

            if (_afterLookup != null) _afterLookup.Invoke();
        }

    }
}
