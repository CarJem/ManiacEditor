using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ManiacEditor.Classes.Options
{
    public class DefaultInputPreferences
    {
        public List<string> GetInput(string name)
        {
            BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

            var feilds = typeof(DefaultInputPreferences).GetProperties(bindingFlags);
            foreach (var entry in feilds)
            {
                if (entry.Name == name)
                {
                    object value = entry.GetValue(this, null);
                    if (value != null && value is List<string>) return (List<string>)value;
                    else return null;

                }
            }
            return null;
        }
    }
}
