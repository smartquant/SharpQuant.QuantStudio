using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpQuant.Common;

namespace SharpQuant.UI
{

    /// <summary>
    /// This class is used to pass information around UI objects (kind of a message)
    /// </summary>
    public class SettingsList: SettingsDictionary
    {

        static SettingsList()
        {
            _itemSeparator = ";";
            _valueSeparator = "|";
        }

        public new static SettingsList Deserialize(string s)
        {

            SettingsList list = new SettingsList();

            string[] items = s.Split(new string[] { _itemSeparator }, StringSplitOptions.None);
            foreach (var item in items)
            {
                string[] entry = item.Split(new string[] { _valueSeparator }, StringSplitOptions.None);
                if (entry.Length != 2) return list;
                list.Add(entry[0], entry[1]);
            }

            return list;
        }

        public override string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this)
            {
                sb.Append(item.Key); sb.Append(_valueSeparator); sb.Append(item.Value); sb.Append(_itemSeparator);
            }
            if (sb.Length > 1) return sb.ToString(0, sb.Length - 1);
            return string.Empty;
        } 
    }
    


}
