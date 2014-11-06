using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SharpQuant.UI.Controls
{
    partial class ListLookup<T> : Form where T:class
    {


        public ListLookup(string[] propertyNames, IEnumerable<T> list)
        {
            InitializeComponent();

            //var list = DB.TestDBContext.GetDBContext().GetRepository<T>().GetAll().Select((i,t)=>
            //    new LookupObject(){ID=i.ID,CODE=i.CODE,Name=i.Name,Description=i.Description} as ICode);

            //var list = DB.TestDBContext.GetDBContext().GetRepository<T>().GetAll();
            
            Type t = typeof(T);

            PropertyInfo pID = t.GetProperty(propertyNames[0]);
            PropertyInfo pCODE = t.GetProperty(propertyNames[1]);
            PropertyInfo pName = t.GetProperty(propertyNames[2]);
            PropertyInfo pDesc = t.GetProperty(propertyNames[3]);

            foreach (var item in list)
            {
                var index = new object[] { };
                LookupObject obj = new LookupObject()
                {
                    ID = pID.GetValue(item, index).ToString(),
                    CODE = pCODE.GetValue(item, index).ToString(),
                    Name = pName.GetValue(item, index).ToString(),
                    Description = pDesc.GetValue(item, index).ToString(),
                };
                lstDetails.Items.Add(new DetailItem(obj));
            }
        }

        ICode _selectedItem;

        public ICode SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; }
        }


        private void lstDetails_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var item = e.Item as DetailItem;
            SelectedItem = item.Location;
        }

        private void lstDetails_DoubleClick(object sender, EventArgs e)
        {
            if (this.lstDetails.SelectedItems.Count == 0) return;

            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


    class DetailItem : ListViewItem
    {

        ICode _location;

        public ICode Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public DetailItem(ICode location)
            : base(new string[3])
        {
            _location = location;
            base.SubItems[0].Text = location.CODE;
            base.SubItems[1].Text = location.Name;
            base.SubItems[2].Text = location.Description;

        }
    }


}
