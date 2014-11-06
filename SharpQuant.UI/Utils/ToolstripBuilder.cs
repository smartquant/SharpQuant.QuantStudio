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
    public static class ToolStripBuilder
    {

        public static ToolStripItem CreateItem(this IToolStripDef def, ResourceManager resourcemanager)
        {
            var button = new ToolStripButton();
            button.Name = def.Name;
            button.Text = def.Text;

            if (!string.IsNullOrEmpty(def.Image))
            {
                Image img = (Image)resourcemanager.GetObject(def.Image);
                button.Image = img;
            }
            if (!string.IsNullOrEmpty(def.ToolTip)) button.ToolTipText = def.ToolTip;

            if (def.Command != null)
            {
                var binding = new ToolStripButtonCommandBinding(button, def.Command, () => null);
            }
            return button;
        }

        public static void AppendItems(this ToolStrip toolstrip, IEnumerable<IToolStripDef> defs, ResourceManager resourcemanager)
        {
            foreach (var def in defs)
            {
                var item = CreateItem(def, resourcemanager);
                
                toolstrip.Items.Add(item);
            }
        }

    }
}
