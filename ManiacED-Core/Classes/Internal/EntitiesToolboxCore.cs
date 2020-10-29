using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ManiacEditor.Methods;

namespace ManiacEditor.Classes.Internal
{
    public static class EntitiesToolboxCore
    {
        public static bool MultipleObjectsSelected { get; set; } = false;
		public static string FilterText { get; set; } = "";
		public static List<int> SelectedObjectListIndexes { get; set; } = new List<int>();
		public static Visibility GetObjectListItemVisiblity(string name, ushort slotID, bool FilteredOut)
		{
			//if (MultipleObjectsSelected == true)
			//{
				//if (SelectedObjectListIndexes.Contains((int)slotID))
				//{
					//return Visibility.Visible;
				//}
				//else
				//{
					//return Visibility.Collapsed;
				//}
			//}
			//else
			//{
				if (FilteredOut) return Visibility.Collapsed;
				if (FilterText != "")
				{
					if (name.Contains(FilterText))
					{
						return Visibility.Visible;
					}
					else
					{
						return Visibility.Collapsed;
					}
				}
				else
				{
					return Visibility.Visible;
				}
			//}
		}
	}
}
