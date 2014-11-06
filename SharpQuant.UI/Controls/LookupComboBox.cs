using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using SharpQuant.Common.DB;

namespace SharpQuant.UI.Controls
{
    public class LookupComboBox<T> : ComboBox, IUpdateAction where T : class
    {

        public LookupComboBox()
        {
        }

        public void Init(string[] propertyNames, IRepositoryProvider rep, string whereClause)
        {

            this.DataSource = rep.GetRepository<T>().SearchFor(whereClause).ToList();
       
            //defaults          
            this.DisplayMember = propertyNames[1];
            //IMPORTANT: This will throw an error if the ID property is decorated with [Browsable(false)]!!!!
            this.ValueMember = propertyNames[0];

            

            //this.Enabled = false;
            //this.DropDownStyle = ComboBoxStyle.Simple;
            
            this.SelectedValueChanged += new EventHandler(LookupComboBox_SelectedValueChanged);
        }

        void LookupComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //force validation => this member is non-public, therefore use reflection
            this.GetType().InvokeMember("PerformControlValidation", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, this, new object[] { true });
            // force data binding update
            this.DataBindings[0].WriteValue(); 

            if (_afterLookup != null) _afterLookup.Invoke();
        }

        Action _afterLookup = null;
        public void SetAfterLookup(Action afterLookup)
        {
            _afterLookup = afterLookup;
        }
    }
}
