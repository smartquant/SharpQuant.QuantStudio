using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpQuant.UI.UserMessage
{
    public partial class UserMessageForm : Form
    {
        
        private UserMessageForm()
        {
            InitializeComponent();
        }

        public UserMessageForm(UserMessageCategories category, string caption, string message)
            : this()
        {
            // Set icon
            switch (category)
            {
                case UserMessageCategories.Error:
                    this.pictureBox.Image = global::SharpQuant.UI.Resources.badge_circle_cross_24;
                    break;

                case UserMessageCategories.Warning:
                    this.pictureBox.Image = global::SharpQuant.UI.Resources.warning_24;
                    break;

                case UserMessageCategories.Information:
                    this.pictureBox.Image = global::SharpQuant.UI.Resources.badge_square_check_24;
                    break;
                case UserMessageCategories.YesNo:
                    this.pictureBox.Image = global::SharpQuant.UI.Resources.badge_square_check_24;
                    btnOk.Visible = true;
                    btnOk.DialogResult=System.Windows.Forms.DialogResult.Yes;
                    btnCancel.Text = "No";
                    btnCancel.DialogResult = System.Windows.Forms.DialogResult.No;
                    btnOk.Text = "Yes";
                    break;
                case UserMessageCategories.OkCancel:
                    btnOk.Visible = true;
                    btnCancel.Text = "Cancel";
                    btnOk.Text = "Ok";
                    this.pictureBox.Image = global::SharpQuant.UI.Resources.badge_square_check_24;
                    break;
            }

            this.Text = caption;
            this.lblMessage.Text = message;

        }
    }
}
