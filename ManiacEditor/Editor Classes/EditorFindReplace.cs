using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace ManiacEditor
{
	public class EditorFindReplace
	{
		public Editor Editor;

		public EditorFindReplace(Editor instance)
		{
			Editor = instance;
		}

		#region Find Unused Tiles

		public void FindUnusedTiles()
		{
			Editor.UpdateWaitingScreen(true);
			Editor.ToggleEditorButtons(false);
			List<int> UnusedTiles = new List<int> { };


			System.Threading.Thread thread = new System.Threading.Thread(() => {
				for (int i = 0; i < 1024; i++)
				{
					if (Editor.TilesToolbar != null) Editor.TilesToolbar.SelectedTileLabel.Content = "Selected Tile: " + i;
					bool Unusued = IsTileUnused(i);
					if (Unusued) UnusedTiles.Add(i);
				}
				Editor.Dispatcher.Invoke(new Action(() => ShowUnusedTiles(UnusedTiles)));
			}) { IsBackground = true };
			thread.Start();

		}

		public bool IsTileUnused(int tile)
		{
			IEnumerable<EditorLayer> AllLayers = Editor.EditorScene.AllLayers;
            bool unused = true;

			foreach (var editorLayer in Editor.EditorScene.AllLayers)
			{
				for (int x = 0; x < editorLayer.Layer.Width; x++)
				{
					for (int y = 0; y < editorLayer.Height; y++)
					{
						ushort currentTile = editorLayer.GetTileAt(new Point(x * EditorConstants.TILE_SIZE, y * EditorConstants.TILE_SIZE));
                        int tileIndex = (currentTile & 0x3ff);
                        if (tileIndex == tile) unused = false;

					}
				}
			}
			return unused;
		}

		public void ShowUnusedTiles(List<int> UnusedTiles)
		{
			if (UnusedTiles.Count != 0)
			{
				var message = "";
				int overflowMax = 32;
				int currentRowPos = 0;
				for (int i = 0; i < UnusedTiles.Count; i++)
				{
					if (i != UnusedTiles.Count - 1)
					{
						if (currentRowPos > overflowMax)
						{
							currentRowPos = 0;
							message += Environment.NewLine;
							message += string.Format("{0},", UnusedTiles[i]);
							currentRowPos++;
						}
						else
						{
							message += string.Format("{0},", UnusedTiles[i]);
							currentRowPos++;
						}

					}
					else
					{
						message += string.Format("{0}", UnusedTiles[i]);
					}
				}
				System.Windows.MessageBoxResult result = RSDKrU.MessageBox.ShowYesNo("Tiles not used are: " + Environment.NewLine + message, "Results", "Copy to Clipboard", "OK", System.Windows.MessageBoxImage.Information);
				if (result == System.Windows.MessageBoxResult.Yes)
				{
					System.Windows.Forms.Clipboard.SetText(message);
				}
			}
			else
			{
				RSDKrU.MessageBox.Show("Found Nothing", "Results", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
			}
			Editor.UpdateWaitingScreen(false);
			Editor.ToggleEditorButtons(true);
		}

		#endregion
	}
}
