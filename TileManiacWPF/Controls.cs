using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using RSDKv5;
using Color = System.Drawing.Color;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Reflection;

namespace TileManiacWPF
{
	public class EditorControls
	{
		private MainWindow Instance;
		private Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;

		public EditorControls(MainWindow _Instance)
		{
			Instance = _Instance;
			UpdateMenuItems();
		}

		public void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
		{

		}

		public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (isCombo(e, myKeyBinds.NewInstance))
			{
				Instance.newInstanceToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.Open))
			{
				Instance.OpenToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds._Save))
			{
				Instance.saveToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.SaveAs))
			{
				Instance.saveAsToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds._SaveUncompressed))
			{
				Instance.saveUncompressedToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.SaveAsUncompressed))
			{
				Instance.saveAsUncompressedToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.backupConfig))
			{
				Instance.tileConfigbinToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.backupImage))
			{
				Instance.x16TilesgifToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.ExportColMask))
			{
				Instance.exportCurrentCollisionMaskAsToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.OpenSingleColMask))
			{
				Instance.openSingleCollisionMaskToolStripMenuItem_Click_1(null, null);
			}
			if (isCombo(e, myKeyBinds.ImportFromOlderRSDK))
			{
				Instance.importFromOlderRSDKVersionToolStripMenuItem_Click(null, null);
			}

			if (isCombo(e, myKeyBinds.Copy))
			{
				Instance.copyToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.PastetoOther))
			{
				Instance.copyToOtherPathToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.Paste))
			{
				Instance.pasteToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.MirrorMode))
			{
				Instance.mirrorPathsToolStripMenuItem1.IsChecked = !Instance.mirrorPathsToolStripMenuItem1.IsChecked;
				Instance.mirrorPathsToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.RestorePathA))
			{
				Instance.pathAToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.RestorePathB))
			{
				Instance.pathBToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.RestorePaths))
			{
				Instance.bothToolStripMenuItem_Click(null, null);
			}

			if (isCombo(e, myKeyBinds.ShowPathB))
			{
				Instance.showPathBToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.ShowGrid))
			{
				Instance.showGridToolStripMenuItem.IsChecked = !Instance.showGridToolStripMenuItem.IsChecked;
				Instance.showGridToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.ClassicMode))
			{
				Instance.classicViewModeToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.WindowAlwaysOnTop))
			{
				Instance.windowAlwaysOnTop.IsChecked = !Instance.windowAlwaysOnTop.IsChecked;
				Instance.WindowAlwaysOnTop_Click(null, null);
			}

			if (isCombo(e, myKeyBinds.SplitFile))
			{
				Instance.splitFileToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.FlipTileH))
			{
				Instance.flipTileHorizontallyToolStripMenuItem_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.FlipTileV))
			{
				Instance.flipTileVerticallyToolStripMenuItem_Click(null, null);
			}

			if (isCombo(e, myKeyBinds.HomeFolderOpen))
			{
				Instance.openCollisionHomeFolderToolStripMenuItem_Click(null, null);
			}

			if (isCombo(e, myKeyBinds.About))
			{
				Instance.aboutToolStripMenuItem1_Click(null, null);
			}
			if (isCombo(e, myKeyBinds.Settings))
			{
				Instance.settingsToolStripMenuItem_Click(null, null);
			}


		}

		public void GraphicPanel_OnKeyDownLoaded(object sender, KeyEventArgs e)
		{

		}

		public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
		{

		}

		public void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
		{

		}

		public bool isCombo(KeyEventArgs e, StringCollection keyCollection, bool singleKey = false)
		{
			if (keyCollection == null) return false;
			foreach (string key in keyCollection)
			{
				if (!singleKey)
				{
					if (isComboData(e, key))
					{
						return true;
					}
				}
				else
				{
					if (isComboCode(e, key))
					{
						return true;
					}
				}

			}
			return false;
		}

		public bool isComboData(KeyEventArgs e, string key)
		{
			KeysConverter kc = new KeysConverter();

			if (e.KeyData == (Keys)kc.ConvertFromString(key)) return true;
			else return false;
		}

		public bool isComboCode(KeyEventArgs e, string key)
		{
			KeysConverter kc = new KeysConverter();

			if (e.KeyCode == (Keys)kc.ConvertFromString(key)) return true;
			else return false;
		}

		public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
		{
			string nullString = (nonRequiredBinding ? "" : "N/A");
			List<string> keyBindList = new List<string>();
			List<string> keyBindModList = new List<string>();

			if (!Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

			if (Properties.KeyBinds.Default == null) return nullString;

			var keybindDict = Properties.KeyBinds.Default[keyRefrence] as StringCollection;
			if (keybindDict != null)
			{
				keyBindList = keybindDict.Cast<string>().ToList();
			}
			else
			{
				return nullString;
			}

			if (keyBindList == null)
			{
				return nullString;
			}

			if (keyBindList.Count > 1)
			{
				string keyBindLister = "";
				foreach (string key in keyBindList)
				{
					keyBindLister += String.Format("({0}) ", key);
				}
				if (tooltip) return String.Format(" ({0})", keyBindLister);
				else return keyBindLister;
			}
			else if ((keyBindList.Count == 1))
			{
				if (tooltip) return String.Format(" ({0})", keyBindList[0]);
				else return keyBindList[0];
			}
			else
			{
				return nullString;
			}


		}

		public void UpdateMenuItems()
		{
			Instance.newInstanceMenuItem.InputGestureText = KeyBindPraser("NewInstance");
			Instance.openMenuItem.InputGestureText = KeyBindPraser("Open");
			Instance.saveMenuItem.InputGestureText = KeyBindPraser("_Save");
			Instance.saveAsMenuItem.InputGestureText = KeyBindPraser("SaveAs");
			Instance.saveAsUncompressedMenuItem.InputGestureText = KeyBindPraser("SaveAsUncompressed");
			Instance.saveUncompressedMenuItem.InputGestureText = KeyBindPraser("_SaveUncompressed");
			Instance.backupTilesConfigMenuItem.InputGestureText = KeyBindPraser("backupConfig", false, true);
			Instance.backupTilesMenuItem.InputGestureText = KeyBindPraser("backupImage", false, true);
			Instance.importMenuItem.InputGestureText = KeyBindPraser("ImportFromOlderRSDK", false, true);
			Instance.OpenSingleColMaskMenuItem.InputGestureText = KeyBindPraser("OpenSingleColMask", false, true);
			Instance.exportCurrentMaskMenuItem.InputGestureText = KeyBindPraser("ExportColMask", false, true);

			Instance.copyMenuItem.InputGestureText = KeyBindPraser("Copy");
			Instance.copyToOtherPathMenuItem.InputGestureText = KeyBindPraser("PastetoOther");
			Instance.pasteMenuItem.InputGestureText = KeyBindPraser("Paste");
			Instance.mirrorPathsToolStripMenuItem1.InputGestureText = KeyBindPraser("MirrorMode");
			Instance.restorePathAMenuItem.InputGestureText = KeyBindPraser("RestorePathA", false, true);
			Instance.restorePathBMenuItem.InputGestureText = KeyBindPraser("RestorePathB", false, true);
			Instance.restoreBothMenuItem.InputGestureText = KeyBindPraser("RestorePaths", false, true);

			Instance.showPathBToolStripMenuItem.InputGestureText = KeyBindPraser("ShowPathB");
			Instance.showGridToolStripMenuItem.InputGestureText = KeyBindPraser("ShowGrid");
			Instance.classicViewModeToolStripMenuItem.InputGestureText = KeyBindPraser("ClassicMode", false, true);
			Instance.windowAlwaysOnTop.InputGestureText = KeyBindPraser("WindowAlwaysOnTop");


			Instance.splitFileMenuItem.InputGestureText = KeyBindPraser("SplitFile", false, true);
			Instance.flipTileHMenuItem.InputGestureText = KeyBindPraser("FlipTileH", false, true);
			Instance.flipTileVMenuItem.InputGestureText = KeyBindPraser("FlipTileV", false, true);

			Instance.openCollisionHomeFolderToolStripMenuItem.InputGestureText = KeyBindPraser("HomeFolderOpen", false, true);

			Instance.aboutMenuItem.InputGestureText = KeyBindPraser("About", false, true);
			Instance.settingsMenuItem.InputGestureText = KeyBindPraser("Settings", false, true);
		}

		public void UpdateTooltips()
		{



		}
	}


}
