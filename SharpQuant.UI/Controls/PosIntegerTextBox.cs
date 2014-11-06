using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace SharpQuant.UI.Controls
{
    public class PosIntegerTextBox : TextBox
    {
        public PosIntegerTextBox()
        {
            this.TextChanged += new EventHandler(HandleTextChanged);
            this.KeyPress += new KeyPressEventHandler(HandleKeyPress);
        }

        void HandleKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        void HandleTextChanged(object sender, EventArgs e)
        {
            validateText(this);
        }

        private bool _myTextBoxChanging = false;

        private void validateText(TextBox box)
        {
            // stop multiple changes;
            if (_myTextBoxChanging)
                return;
            _myTextBoxChanging = true;

            string text = box.Text;
            if (text == "")
                return;
            string validText = "";
            bool hasPeriod = false;
            int pos = box.SelectionStart;
            for (int i = 0; i < text.Length; i++)
            {
                bool badChar = false;
                char s = text[i];
                if (s == '.')
                {
                    if (hasPeriod)
                        badChar = true;
                    else
                        hasPeriod = true;
                }
                else if (s < '0' || s > '9')
                    badChar = true;

                if (!badChar)
                    validText += s;
                else
                {
                    if (i <= pos)
                        pos--;
                }
            }

            // trim starting 00s
            while (validText.Length >= 2 && validText[0] == '0')
            {
                if (validText[1] != '.')
                {
                    validText = validText.Substring(1);
                    if (pos < 2)
                        pos--;
                }
                else
                    break;
            }

            if (pos > validText.Length)
                pos = validText.Length;
            box.Text = validText;
            box.SelectionStart = pos;
            _myTextBoxChanging = false;
        }

    }
}
