using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.Controls
{
    public interface ICode
    {
        string ID { get; }
        string CODE { get; }
        string Name { get; }
        string Description { get; }
    }

    public class LookupObject : ICode
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
