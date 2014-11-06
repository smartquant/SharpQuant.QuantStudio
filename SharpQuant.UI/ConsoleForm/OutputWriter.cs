using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SharpQuant.UI.Console
{
public class OutputWriter : StringWriter
{
    // Fields
    private TextBox textBox;
    private Action<string> _appendText;

    // Methods
    public OutputWriter(TextBox textBox)
    {
        this.textBox = textBox;
        _appendText = (line) => this.textBox.AppendText(line); 
    }

    private void Add(string line)
    {
        if (this.textBox.InvokeRequired)
        {
            this.textBox.Invoke(_appendText, line);
        }
        else
        {
            try
            {
                this.textBox.AppendText(line);
            }
            catch
            {
            }
        }
    }

    private void AddLine(string line)
    {
        this.Add(string.Format("{0}{1}", line, Environment.NewLine));
    }

    public override void Write(char[] buffer)
    {
        this.Write(new string(buffer));
    }

    public override void Write(bool value)
    {
        this.Add(value.ToString());
    }

    public override void Write(char value)
    {
        this.Add(value.ToString());
    }

    public override void Write(decimal value)
    {
        this.Add(value.ToString());
    }

    public override void Write(double value)
    {
        this.Add(value.ToString());
    }

    public override void Write(int value)
    {
        this.Add(value.ToString());
    }

    public override void Write(long value)
    {
        this.Add(value.ToString());
    }

    public override void Write(float value)
    {
        this.Add(value.ToString());
    }

    public override void Write(string value)
    {
        this.Add(value);
    }

    public override void Write(uint value)
    {
        this.Add(value.ToString());
    }

    public override void Write(ulong value)
    {
        this.Add(value.ToString());
    }

    public override void WriteLine()
    {
        this.AddLine(string.Empty);
    }

    public override void WriteLine(bool value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(char value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(decimal value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(double value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(int value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(long value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(float value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(string value)
    {
        this.AddLine(value);
    }

    public override void WriteLine(uint value)
    {
        this.AddLine(value.ToString());
    }

    public override void WriteLine(ulong value)
    {
        this.AddLine(value.ToString());
    }

}

 

}
