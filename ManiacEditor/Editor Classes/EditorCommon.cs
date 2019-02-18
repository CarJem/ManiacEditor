using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Editor_Classes
{
	public class EditorCommon
	{
		private Editor Editor;

		//Shorthanding Setting Files
		public Properties.Settings mySettings = Properties.Settings.Default;
		public Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;
		public EditorCommon(Editor instance)
		{
			Editor = instance;
		}
	}
}
