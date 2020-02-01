using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Diagnostics;
using RSDKvB;
using RSDKv5;
using System.Drawing.Imaging;
using System.Media;
using Color = System.Drawing.Color;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Clipboard = System.Windows.Clipboard;
using Grid = System.Windows.Controls.Grid;
using System.Threading;
using ManiacEditor.Interfaces;
using MessageBox = System.Windows.MessageBox;
using TileConfig = RSDKv5.Tileconfig;


namespace ManiacEditor
{
	public partial class MainWindow : Window
	{
		bool lockRadioButtons = false; //for locking radio button updates when switching single select options

		RSDKv5.Tileconfig.CollisionMask TileClipboard;

        public static MainWindow Instance;

		List<BitmapImage> ColImges = new List<BitmapImage>(); //List of images, saves memory
		List<BitmapImage> ColImgesNoCol = new List<BitmapImage>(); //List of images, saves memory
		List<BitmapImage> ColActivatedImges = new List<BitmapImage>(); //List of images, saves memory

		List<Bitmap> CollisionListImgA = new List<Bitmap>();
		List<Bitmap> CollisionListImgB = new List<Bitmap>();

		Grid[,] CollisionViewerBackgrounds = new Grid[16,16];
		TextBlock[,] CollisionViewerLabels = new TextBlock[16,16];
		Color CollisionColor = Color.FromArgb(255, 0, 255, 0);

		public int curColisionMask; //What Collision Mask are we editing?

		public string filepath; //Where is the file located?
		public string folderpath; //Where is the folder located?
		public string bitmappath; //Where is the image located?

		bool showPathB = false; //should we show Path A or Path B?

		public bool hasModified = false; //For intergrating tools to know that we have saved/made edits to this config.
		bool imageIsModified = false;
		bool indexedImagedLoaded = false;


		bool mouseIsDownL { get => System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed; }
		bool mouseIsDownR { get => System.Windows.Input.Mouse.RightButton == System.Windows.Input.MouseButtonState.Pressed; }


		public int viewerSetting = 0;
		public bool showGrid = false;
		public bool classicMode = false;
		public int viewAppearanceMode = 0;
		public bool mirrorMode = false;
		public int listSetting = 0;
        public bool freezeGrid = true;

		bool changingModes = false; //To prevent updating the radio buttons until after we change the viewer mode

		public RSDKv5.Tileconfig tcf; //The ColllisionMask Data
		public RSDKv5.Tileconfig tcfBak; //Backup ColllisionMask Data

		List<Bitmap> Tiles = new List<Bitmap>(); //List of all the 16x16 Stage Tiles
		List<Bitmap> IndexedTiles = new List<Bitmap>(); //List of all the 16x16 Stage Tiles (Preserving Color Pallete)

		public EditorControl EditorControls { get => GetEditorUIControls(); }

        private EditorControl GetEditorUIControls()
        {
            if (Interfaces.Base.MainEditor.Instance != null) return Interfaces.Base.MainEditor.Instance.EditorControls;
            else return null;

        }

		//Winform Components
		public PictureBoxNearestNeighbor overlayPicBox = new PictureBoxNearestNeighbor();
		public PictureBoxNearestNeighbor TilePicBox = new PictureBoxNearestNeighbor();
		public PictureBoxNearestNeighbor CollisionPicBox = new PictureBoxNearestNeighbor();
		public ManiacEditor.Interfaces.Multipurpose.Controls.RetroEDTileList CollisionList = new ManiacEditor.Interfaces.Multipurpose.Controls.RetroEDTileList();

		public bool IsClosed { get; private set; }

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			IsClosed = true;
		}

		public MainWindow()
		{
			InitializeComponent();
			InitalizeViewer();
            UpdateThemeColors();
			Instance = this;

			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/1.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/2.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/3.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/4.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/5.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/6.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/7.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/8.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/9.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/10.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/11.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/12.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/13.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/14.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/15.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/16.png", UriKind.Relative)));
			ColImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/0.png", UriKind.Relative)));


			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_1_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_2_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_3_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_4_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_5_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_6_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_7_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_8_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_9_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_10_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_11_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_12_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_13_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_14_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_15_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_16_NoCol.png", UriKind.Relative)));
			ColImgesNoCol.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/_0_NoCol.png", UriKind.Relative)));

			ColActivatedImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/Red.png", UriKind.Relative)));
			ColActivatedImges.Add(new BitmapImage(new Uri(@"/ManiacEditor;component/TileManiac/Resources/Green.png", UriKind.Relative)));


			Viewer1.Source = ColImges[16];
			Viewer2.Source = ColImges[16];
			Viewer3.Source = ColImges[16];
			Viewer4.Source = ColImges[16];
			Viewer5.Source = ColImges[16];
			Viewer6.Source = ColImges[16];
			Viewer7.Source = ColImges[16];
			Viewer8.Source = ColImges[16];
			Viewer9.Source = ColImges[16];
			Viewer10.Source = ColImges[16];
			Viewer11.Source = ColImges[16];
			Viewer12.Source = ColImges[16];
			Viewer13.Source = ColImges[16];
			Viewer14.Source = ColImges[16];
			Viewer15.Source = ColImges[16];
			Viewer16.Source = ColImges[16];


			RGBox0.Source = ColActivatedImges[0];
			RGBox1.Source = ColActivatedImges[0];
			RGBox2.Source = ColActivatedImges[0];
			RGBox3.Source = ColActivatedImges[0];
			RGBox4.Source = ColActivatedImges[0];
			RGBox5.Source = ColActivatedImges[0];
			RGBox6.Source = ColActivatedImges[0];
			RGBox7.Source = ColActivatedImges[0];
			RGBox8.Source = ColActivatedImges[0];
			RGBox9.Source = ColActivatedImges[0];
			RGBoxA.Source = ColActivatedImges[0];
			RGBoxB.Source = ColActivatedImges[0];
			RGBoxC.Source = ColActivatedImges[0];
			RGBoxD.Source = ColActivatedImges[0];
			RGBoxE.Source = ColActivatedImges[0];
			RGBoxF.Source = ColActivatedImges[0];

			LoadSettings();
		}

		void LoadSettings()
		{
			if (Properties.Defaults.Default.TileManiacListSetting == 0)
			{
				uncheckListViews();
				collisionViewRadioButton.IsChecked = true;
				lockRadioButtons = false;
				listSetting = 0;
			}
			else if (Properties.Defaults.Default.TileManiacListSetting == 1)
			{
				uncheckListViews();
				tileViewRadioButton.IsChecked = true;
				lockRadioButtons = false;
				listSetting = 1;
			}

			if (Properties.Defaults.Default.TileManiacRenderViewerSetting == 0)
			{
				unCheckModes();
				tileViewButton.IsChecked = true;
				TilePicBox.Visible = true;
				PicBoxHost2Tile.Visibility = Visibility.Visible;
				changingModes = false;
				viewerSetting = 0;
			}
			else if (Properties.Defaults.Default.TileManiacRenderViewerSetting == 1)
			{
				unCheckModes();
				colllisionViewButton.IsChecked = true;
				CollisionPicBox.Visible = true;
				PicBoxHost3Collision.Visibility = Visibility.Visible;
				changingModes = false;
				viewerSetting = 1;
			}
			else if (Properties.Defaults.Default.TileManiacRenderViewerSetting == 2)
			{
				unCheckModes();
				overlayViewButton.IsChecked = true;
				overlayPicBox.Visible = true;
				PicBoxHost1Overlay.Visibility = Visibility.Visible;
				changingModes = false;
				viewerSetting = 2;
			}
			if (Properties.Defaults.Default.TileManiacShowGrid)
			{
				showGridToolStripMenuItem.IsChecked = true;
				showGrid = true;
			}
			if (Properties.Defaults.Default.TileManiacClassicMode)
			{
				CollisionViewer.IsEnabled = false;
				CollisionViewer.Visibility =  Visibility.Hidden;
				ClassicMode.Visibility = Visibility.Visible;
				ClassicMode.IsEnabled = true;
				viewSettingsToolStripMenuItem.IsEnabled = false;
				classicMode = true;
			}
			else
			{
				CollisionViewer.IsEnabled = true;
				CollisionViewer.Visibility =  Visibility.Visible;
				ClassicMode.Visibility = Visibility.Hidden;
				ClassicMode.IsEnabled = false;
			}
			switch (Properties.Defaults.Default.TileManiacViewAppearanceMode)
			{
				case 0:
					overlayToolStripMenuItem.IsChecked = true;
					collisionToolStripMenuItem.IsChecked = false;
					viewAppearanceMode = 0;
					break;
				case 1:
					collisionToolStripMenuItem.IsChecked = true;
					overlayToolStripMenuItem.IsChecked = false;
					viewAppearanceMode = 1;
					break;
			}
			if (Properties.Defaults.Default.TileManiacMirrorMode)
			{
				mirrorPathsToolStripMenuItem1.IsChecked = true;
				mirrorMode = true;
				UpdateMirrorModeStatusLabel();

			}
			if (Properties.Defaults.Default.TileManiacWindowAlwaysOnTop)
			{
				windowAlwaysOnTop.IsChecked = true;
				this.Topmost = true;
			}
		}

		public void LoadTileConfigViaIntergration(TileConfig tilesConfig, string scenePath, int selectedTile = 0)
		{
			indexedImagedLoaded = false;
			curColisionMask = 0; // Set the current collision mask to zero (avoids rare errors)
			//tcf = tilesConfig;
			//tcfBak = tilesConfig;
			tcf = new TileConfig(Path.Combine(scenePath));
			tcfBak = new TileConfig(Path.Combine(scenePath));
			string tileBitmapPath = Path.Combine(Path.GetDirectoryName(scenePath), "16x16tiles.gif"); // get the path to the stage's tileset
			LoadTileSet(new Bitmap(tileBitmapPath)); // load each 16x16 tile into the list
			bitmappath = tileBitmapPath;

			CollisionList.Images.Clear();

			for (int i = 0; i < 1024; i++)
			{
				if (listSetting == 0)
				{
					CollisionListImgA.Add(tcf.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
					CollisionList.Images.Add(CollisionListImgA[i]);
				}
				else
				{
					CollisionListImgA.Add(Tiles[i]);
					CollisionList.Images.Add(Tiles[i]);
				}

			}

			for (int i = 0; i < 1024; i++)
			{
				if (listSetting == 0)
				{
					CollisionListImgB.Add(tcf.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
					CollisionList.Images.Add(CollisionListImgB[i]);
				}
				else
				{
					CollisionListImgB.Add(Tiles[i]);
					CollisionList.Images.Add(Tiles[i]);
				}
			}
			CollisionList.SelectedIndex = selectedTile;
			CollisionList.Refresh();

			curColisionMask = selectedTile;
			RefreshUI(); //update the UI

		}

		public void SetCollisionIndex(int index)
		{
			CollisionList.SelectedIndex = index;
			CollisionList.Refresh();

			RefreshUI(); //update the UI
		}

        public void SetGridActivation(bool enabled)
        {
            if (enabled)
            {
                Thread thread = new Thread(() => {
                    Thread.Sleep(1000);
                    freezeGrid = false;
                });
                thread.Start();
            }
            else
            {
                freezeGrid = true;
            }

        }

		public void OpenToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            SetGridActivation(false);
            OpenFileDialog dlg = new OpenFileDialog
			{
				Title = "Open RSDKv5 Tileconfig",
				DefaultExt = ".bin",
				Filter = "RSDKv5 Tileconfig Files|Tileconfig*.bin|All Files|*"
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				indexedImagedLoaded = false;
				curColisionMask = 0; // Set the current collision mask to zero (avoids rare errors)
				filepath = dlg.FileName;
				tcf = new RSDKv5.Tileconfig(dlg.FileName);
				tcfBak = new RSDKv5.Tileconfig(dlg.FileName);
				string tileBitmapPath = Path.Combine(Path.GetDirectoryName(filepath), "16x16tiles.gif"); // get the path to the stage's tileset
				LoadTileSet(new Bitmap(tileBitmapPath)); // load each 16x16 tile into the list
				bitmappath = tileBitmapPath;

				CollisionList.Images.Clear();

				for (int i = 0; i < 1024; i++)
				{
					if (listSetting == 0)
					{
						CollisionListImgA.Add(tcf.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
						CollisionList.Images.Add(CollisionListImgA[i]);
					}
					else
					{
						CollisionListImgA.Add(Tiles[i]);
						CollisionList.Images.Add(Tiles[i]);
					}

				}

				for (int i = 0; i < 1024; i++)
				{
					if (listSetting == 0)
					{
						CollisionListImgB.Add(tcf.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
						CollisionList.Images.Add(CollisionListImgB[i]);
					}
					else
					{
						CollisionListImgB.Add(Tiles[i]);
						CollisionList.Images.Add(Tiles[i]);
					}
				}
				CollisionList.SelectedIndex = curColisionMask - 1;
				CollisionList.Refresh();

				RefreshUI(); //update the UI
			}
            SetGridActivation(true);
        }

		public void saveToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (filepath != null) //Did we open a file?
			{
                SetGridActivation(false);
                Save16x16Tiles();
				tcf.Write(filepath);
				hasModified = true;
                SetGridActivation(true);
            }
			else //if not then use "Save As..."
			{
				saveAsToolStripMenuItem_Click(null, e);
			}
		}

		public void saveAsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            SetGridActivation(false);
            SaveFileDialog dlg = new SaveFileDialog
			{
				Title = "Save RSDKv5 Tileconfig As...",
				DefaultExt = ".bin",
				Filter = "RSDKv5 Tileconfig Files|Tileconfig*.bin"
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tcf.Write(dlg.FileName); //Write the data to a file
			}
            SetGridActivation(true);
        }

		public void BackupCollisionData()
		{
			try
			{
				if (filepath != null) //Did we open a file?
				{
					string filepathBackup = filepath + ".bak";
					string filepathBackupReserve = filepathBackup;
					int i = 1;
					while ((File.Exists(filepathBackup + ".bin")))
					{
						filepathBackup = filepathBackupReserve + i.ToString();
						i++;
					}
					filepathBackup = filepathBackup + ".bin";
					File.Copy(filepath, filepathBackup);
				}
				else
				{
					MessageBox.Show("No Collision Present!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				ShowError($@"Failed to Save 16x16Tiles Image to file!" + Environment.NewLine + "Error: " + ex.Message.ToString());
			}

		}

		public void Save16x16Tiles(bool isBackup = false)
		{
			try
			{
				if (isBackup)
				{
					string tileBitmapPath = Path.Combine(Path.GetDirectoryName(filepath), "16x16tiles.gif"); // get the path to the stage's tileset
					string tileBitmapBackupPath = Path.Combine(Path.GetDirectoryName(filepath), "16x16tiles.bak"); // get the path to the stage's backup tileset
					string tileBitmapBackupPathReserved = tileBitmapBackupPath; //Perserve this for checking the file each tile
					int i = 1;
					while ((File.Exists(tileBitmapBackupPath + ".gif")))
					{
						tileBitmapBackupPath = tileBitmapBackupPathReserved + i.ToString();
						i++;
					}
					File.Copy(tileBitmapPath, tileBitmapPath);
				}
				else
				{
					if (imageIsModified && indexedImagedLoaded)
					{
						if (!Properties.Defaults.Default.TileManiacAllowDirect16x16TilesGIFEditing)
						{
							if (Properties.Defaults.Default.TileManiacPromptForChoiceOnImageWrite)
							{
								MessageBoxResult result = MessageBox.Show("You have made changes that require the 16x16Tiles.gif to be modifed. While this feature should normally work just fine, it may cause some issues, which is why you may choose if you want to or not. So do you want to save directly to the 16x16Tiles.gif? (Click No will save to 16x16Tiles_Copy.gif, and Cancel with not write this file at all) (You also can change this dialog's visibility in options)", "Saving 16x16Tiles.gif", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
								if (result == MessageBoxResult.Yes)
								{
									SaveTileSet("16x16Tiles.gif");
									imageIsModified = false;
								}
								else if (result == MessageBoxResult.No)
								{
									SaveTileSet("16x16Tiles_Copy.gif");
									imageIsModified = false;
								}
								else if (result == MessageBoxResult.Cancel)
								{
									imageIsModified = true; //We Didn't Change Anything, keep reminding the user
								}
							}
							else
							{
								SaveTileSet("16x16Tiles_Copy.gif");
								imageIsModified = false;
							}

						}
						else
						{
							SaveTileSet("16x16Tiles.gif");
							imageIsModified = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShowError($@"Failed to Save 16x16Tiles Image to file! We will still try to save your collision however (if this is not a backup)" + Environment.NewLine + "Error: " + ex.Message.ToString());
			}

		}

		private void ShowError(string message, string title = "Error!")
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public void splitFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog
			{
				Description = "Select Folder to Export to..."
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				for (int i = 0; i < 1024; i++)
				{
					BinaryWriter Writer1 = new BinaryWriter(File.Create(dlg.SelectedPath + "//CollisionMaskPathA" + (i + 1) + ".rcm"));
					BinaryWriter Writer2 = new BinaryWriter(File.Create(dlg.SelectedPath + "//CollisionMaskPathB" + (i + 1) + ".rcm"));
					tcf.CollisionPath1[i].WriteUnc(Writer1);
					tcf.CollisionPath2[i].WriteUnc(Writer2);
					Writer1.Close();
					Writer2.Close();
				}
				RefreshUI();
			}

		}

		public void LoadTileSet(Bitmap TileSet, bool indexedMode = false)
		{
			if (!indexedMode) Tiles.Clear(); // Clear the previous images, since we load the entire file!
			else IndexedTiles.Clear(); // Clear the previous images, since we load the entire file!

			int tsize = TileSet.Height; //Height of the image in pixels
			for (int i = 0; i < (tsize / 16); i++) //We divide by 16 to get the "height" in blocks
			{
				Rectangle CropArea = new Rectangle(0, (i * 16), 16, 16); //we then get tile at Y: i * 16, 
																		 //we have to multiply i by 16 to get the "true Tile value" (1* 16 = 16, 2 * 16 = 32, etc.)
				if (!indexedMode)
				{
					Bitmap CroppedImage = CropImage(TileSet, CropArea); // crop that image
					Tiles.Add(CroppedImage); // add it to the tile list
				}
				else
				{
					Bitmap CroppedImageIndexed = CropImage(TileSet, CropArea, true); // crop that indexed image
					IndexedTiles.Add(CroppedImageIndexed); // add it to the indexed tile list
				}
			}
		}

		public void SaveTileSet(string path)
		{
			Bitmap bmp = mergeImages(IndexedTiles.ToArray());
			if (true)
			{

			}
			bmp.Save(Path.Combine(Path.GetDirectoryName(filepath), path));
		}

		public Bitmap mergeImages(Bitmap[] images)
		{
			Bitmap mergedImg = new Bitmap(16, 16384, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
			{
				Palette = IndexedTiles[0].Palette
			};
			for (int i = 0; i < IndexedTiles.Count; i++)
			{
				var bitmapData = IndexedTiles[i].LockBits(new Rectangle(0, 0, IndexedTiles[i].Width, IndexedTiles[i].Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
				for (int h = 0; h < 16; h++)
				{
					for (int w = 0; w < 16; w++)
					{
						int indexColor = GetIndexedPixel(w, h, bitmapData);
						SetPixel(mergedImg, w, h + (16 * i), indexColor);
					}
				}
				IndexedTiles[i].UnlockBits(bitmapData);
			}

			return mergedImg;
		}

		public unsafe Byte GetIndexedPixel(int x, int y, BitmapData bmd)
		{
			byte* p = (byte*)bmd.Scan0.ToPointer();
			int offset = y * bmd.Stride + x;
			return p[offset];
		}

		private static void SetPixel(Bitmap bmp, int x, int y, int paletteEntry)
		{
			BitmapData data = bmp.LockBits(new Rectangle(new System.Drawing.Point(x, y), new System.Drawing.Size(1, 1)), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
			byte b = Marshal.ReadByte(data.Scan0);
			Marshal.WriteByte(data.Scan0, (byte)(b & 0xf | (paletteEntry)));
			bmp.UnlockBits(data);
		}

		public Bitmap CropImage(Bitmap source, Rectangle section, bool indexed = false)
		{
			// An empty bitmap which will hold the cropped image


			Bitmap bmp = new Bitmap(section.Width, section.Height);
			if (indexed)
			{
				bmp = source.Clone(section, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
			}
			else
			{
				Graphics g = Graphics.FromImage(bmp);

				// Draw the given area (section) of the source image
				// at location 0,0 on the empty bitmap (bmp)
				g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
			}
			return bmp;
		}

		private Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height, bool fixOffset = true)
		{
			Bitmap result = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(result))
			{
				if (fixOffset) g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				g.DrawImage(sourceBMP, 0, 0, width, height);
			}
			return result;
		}

        public void SetEditorOnlyButtonsState(bool enabled)
        {
            SlopeNUD.IsEnabled = enabled;
            PhysicsNUD.IsEnabled = enabled;
            MomentumNUD.IsEnabled = enabled;
            SpecialNUD.IsEnabled = enabled;
            UnknownNUD.IsEnabled = enabled;
            IsCeilingButton.IsEnabled = enabled;

            cb00.IsEnabled = enabled;
            cb01.IsEnabled = enabled;
            cb02.IsEnabled = enabled;
            cb03.IsEnabled = enabled;
            cb04.IsEnabled = enabled;
            cb05.IsEnabled = enabled;
            cb06.IsEnabled = enabled;
            cb07.IsEnabled = enabled;
            cb08.IsEnabled = enabled;
            cb09.IsEnabled = enabled;
            cb10.IsEnabled = enabled;
            cb11.IsEnabled = enabled;
            cb12.IsEnabled = enabled;
            cb13.IsEnabled = enabled;
            cb14.IsEnabled = enabled;
            cb15.IsEnabled = enabled;

            CollisionViewer.IsEnabled = enabled;

            if (swapPathButton != null) swapPathButton.IsEnabled = enabled;
            GotoNUD.IsEnabled = enabled;
        }

        public void UpdateThemeColors()
        {
            if (OldColViewer != null)
            {
                if (App.Skin == Skin.Dark) OldColViewer.Opacity = 0.5;
                else OldColViewer.Opacity = 1.0;
            }
            if (App.Skin == Skin.Dark) CollisionColor = Color.FromArgb(20, 148, 20);
            else CollisionColor = Color.FromArgb(255, 0, 255, 0);
            this.CollisionList.Refresh();
            this.CollisionList.vScrollBar1Host.Refresh();
            this.CollisionList.vScrollBar1Host.scroller.Refresh();
            System.Drawing.Color ListBackColor = (App.Skin == Skin.Dark ? EditorTheming.darkTheme0 : System.Drawing.Color.White);
            this.CollisionList.BackColor = ListBackColor;
        }

        public void RefreshUI()
		{
            if (EditorControls != null) EditorControls.TileManiac_UpdateMenuItems();
            UpdateThemeColors();
            if (tcf != null)
            {

                TilePicBox.Image = ResizeBitmap(Tiles[curColisionMask], 96, 96); //update the tile preview 
                Bitmap Overlaypic = new Bitmap(16, 16);
                Bitmap Collisionpic = new Bitmap(16, 16);
                GetRawSlopeNUD();
                if (!showPathB) //if we are showing Path A then refresh the values accordingly
                {
                    Collisionpic = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionColor);
                    CollisionPicBox.Image = Collisionpic;
                    Overlaypic = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor, Tiles[curColisionMask]);
                    PhysicsNUD.Value = tcf.CollisionPath1[curColisionMask].CeilingAngle;
                    MomentumNUD.Value = tcf.CollisionPath1[curColisionMask].LWallAngle;
                    UnknownNUD.Value = tcf.CollisionPath1[curColisionMask].RWallAngle;
                    SpecialNUD.Value = tcf.CollisionPath1[curColisionMask].Behaviour;
                    IsCeilingButton.IsChecked = tcf.CollisionPath1[curColisionMask].IsCeiling;

                    RefreshPathA();

                }

                if (showPathB) //if we are showing Path B then refresh the values accordingly
                {
                    Collisionpic = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 255, 0));
                    CollisionPicBox.Image = Collisionpic;
                    Overlaypic = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionColor, Tiles[curColisionMask]);
                    SlopeNUD.Value = tcf.CollisionPath2[curColisionMask].FloorAngle;
                    PhysicsNUD.Value = tcf.CollisionPath2[curColisionMask].CeilingAngle;
                    MomentumNUD.Value = tcf.CollisionPath2[curColisionMask].LWallAngle;
                    UnknownNUD.Value = tcf.CollisionPath2[curColisionMask].RWallAngle;
                    SpecialNUD.Value = tcf.CollisionPath2[curColisionMask].Behaviour;
                    IsCeilingButton.IsChecked = tcf.CollisionPath2[curColisionMask].IsCeiling;

                    RefreshPathB();

                }

                overlayPicBox.Image = Overlaypic;
                overlayPicBox.Image = ResizeBitmap(Overlaypic, 96, 96);
                CollisionPicBox.Image = ResizeBitmap(new Bitmap(CollisionPicBox.Image), 96, 96);

                if (classicMode)
                {
                    CollisionViewer.IsEnabled = false;
                    CollisionViewer.Visibility = Visibility.Hidden;
                    ClassicMode.IsEnabled = true;
                    ClassicMode.Visibility = Visibility.Visible;
                }
                else
                {
                    if (viewAppearanceMode == 0) UpdateColllisionViewer(Overlaypic);
                    else if (viewAppearanceMode == 1) UpdateColllisionViewer(Collisionpic);
                    CollisionViewer.IsEnabled = true;
                    CollisionViewer.Visibility = Visibility.Visible;
                    ClassicMode.IsEnabled = false;
                    ClassicMode.Visibility = Visibility.Hidden;

                }

                RefreshCollisionList();
            }

            SetEditorOnlyButtonsState(tcf != null);


        }

		public void UpdateColllisionViewer(System.Drawing.Image image)
		{
			Bitmap viewImage = new Bitmap(image);
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					CollisionViewerBackgrounds[x,y].Background = new SolidColorBrush(ColorConvertToMedia(viewImage.GetPixel(x, y)));
					CollisionViewerLabels[x,y].Foreground = new SolidColorBrush(ColorConvertToMedia(GetTextColor(viewImage.GetPixel(x, y))));
				}
			}
		}

		public void InitalizeViewer()
		{
			// 
			// CollisionList
			// 
			this.CollisionList.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.CollisionList.BackColor = System.Drawing.SystemColors.Window;
			this.CollisionList.Location = new System.Drawing.Point(0, 0);
			this.CollisionList.Dock = DockStyle.Fill;
			this.CollisionList.Margin = new System.Windows.Forms.Padding(4);
			this.CollisionList.Name = "CollisionList";
			this.CollisionList.ScrollValue = 0;
			this.CollisionList.SelectedIndex = -1;
			this.CollisionList.Size = new System.Drawing.Size(157, 452);
			this.CollisionList.TabIndex = 36;
			this.CollisionList.SelectedIndexChanged += new System.EventHandler(this.CollisionList_SelectedIndexChanged);

			// 
			// overlayPicBox
			// 
			this.overlayPicBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.overlayPicBox.BackColor = System.Drawing.Color.Transparent;
			this.overlayPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.overlayPicBox.InitialImage = null;
			this.overlayPicBox.Dock = DockStyle.Fill;
			this.overlayPicBox.Location = new System.Drawing.Point(0, 0);
			this.overlayPicBox.Margin = new System.Windows.Forms.Padding(2);
			this.overlayPicBox.Name = "overlayPicBox";
			this.overlayPicBox.Size = new System.Drawing.Size(96, 96);
			this.overlayPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.overlayPicBox.TabIndex = 71;
			this.overlayPicBox.TabStop = false;
			this.overlayPicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPicBox_Paint);
			// 
			// TilePicBox
			// 
			this.TilePicBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.TilePicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.TilePicBox.Location = new System.Drawing.Point(0, 0);
			this.TilePicBox.Margin = new System.Windows.Forms.Padding(2);
			this.TilePicBox.Name = "TilePicBox";
			this.TilePicBox.Dock = DockStyle.Fill;
			this.TilePicBox.Size = new System.Drawing.Size(96, 96);
			this.TilePicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.TilePicBox.TabIndex = 1;
			this.TilePicBox.TabStop = false;
			this.TilePicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPicBox_Paint);
			// 
			// CollisionPicBox
			// 
			this.CollisionPicBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.CollisionPicBox.BackColor = System.Drawing.Color.Transparent;
			this.CollisionPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.CollisionPicBox.InitialImage = null;
			this.CollisionPicBox.Location = new System.Drawing.Point(0, 0);
			this.CollisionPicBox.Margin = new System.Windows.Forms.Padding(2);
			this.CollisionPicBox.Dock = DockStyle.Fill;
			this.CollisionPicBox.Name = "CollisionPicBox";
			this.CollisionPicBox.Size = new System.Drawing.Size(96, 96);
			this.CollisionPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.CollisionPicBox.TabIndex = 0;
			this.CollisionPicBox.TabStop = false;
			this.CollisionPicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPicBox_Paint);


			PicBoxHost1Overlay.Child = overlayPicBox;
			PicBoxHost2Tile.Child = TilePicBox;
			PicBoxHost3Collision.Child = CollisionPicBox;
			TilesListHost.Child = CollisionList;


			overlayPicBox.Show();
			TilePicBox.Show();
			CollisionPicBox.Show();
			CollisionList.Show();

			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					Border border = new Border()
					{
						BorderBrush = (SolidColorBrush)FindResource("DisabledText"),
						BorderThickness = new Thickness(1),
						Visibility = Visibility.Visible
					};
					Grid.SetColumn(border, x);
					Grid.SetRow(border, y);
					Grid background = new Grid()
					{
						Background = (SolidColorBrush)FindResource("NormalBackground"),
						Margin = new Thickness(1)
					};
					Grid.SetColumn(background, x);
					Grid.SetRow(background, y);
					CollisionViewerBackgrounds[x,y] = background;
					TextBlock CollisionPosition = new TextBlock()
					{
						Text = GetCollisionSection(y),
						Foreground = (SolidColorBrush)FindResource("NormalText"),
						HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
						VerticalAlignment = System.Windows.VerticalAlignment.Center
					};
					CollisionPosition.MouseDown += CollisionViewer_MouseDown;
					background.MouseDown += CollisionViewer_MouseDown;
					CollisionPosition.MouseMove += CollisionViewer_MouseMove;
					background.MouseMove += CollisionViewer_MouseMove;
					CollisionPosition.MouseUp += CollisionViewer_MouseUp;
					background.MouseUp += CollisionViewer_MouseUp;
					Grid.SetColumn(CollisionPosition, x);					
					Grid.SetRow(CollisionPosition, y);
					CollisionViewerLabels[x,y] = CollisionPosition;
					CollisionViewer.Children.Add(border);
					CollisionViewer.Children.Add(background);
					CollisionViewer.Children.Add(CollisionPosition);

				}
			}
		}

		public Color GetTextColor(Color bg)
		{
			int nThreshold = 100;
			int bgDelta = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) +
										  (bg.B * 0.114));

			Color foreColor = (255 - bgDelta < nThreshold) ? Color.Black : Color.White;
			return foreColor;
		}

		public string GetCollisionSection(int y)
		{
			switch (y)
			{
				case 0:
					return "0";
				case 1:
					return "1";
				case 2:
					return "2";
				case 3:
					return "3";
				case 4:
					return "4";
				case 5:
					return "5";
				case 6:
					return "6";
				case 7:
					return "7";
				case 8:
					return "8";
				case 9:
					return "9";
				case 10:
					return "A";
				case 11:
					return "B";
				case 12:
					return "C";
				case 13:
					return "D";
				case 14:
					return "E";
				case 15:
					return "F";
				default:
					return "NULL";

			}

		}


		public void RefreshPathA()
		{
			lb00.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[0];
			lb01.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[1];
			lb02.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[2];
			lb03.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[3];
			lb04.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[4];
			lb05.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[5];
			lb06.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[6];
			lb07.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[7];
			lb08.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[8];
			lb09.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[9];
			lb10.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[10];
			lb11.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[11];
			lb12.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[12];
			lb13.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[13];
			lb14.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[14];
			lb15.SelectedIndex = tcf.CollisionPath1[curColisionMask].Collision[15];

			cb00.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[0];
			cb01.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[1];
			cb02.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[2];
			cb03.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[3];
			cb04.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[4];
			cb05.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[5];
			cb06.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[6];
			cb07.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[7];
			cb08.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[8];
			cb09.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[9];
			cb10.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[10];
			cb11.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[11];
			cb12.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[12];
			cb13.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[13];
			cb14.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[14];
			cb15.IsChecked = tcf.CollisionPath1[curColisionMask].HasCollision[15];

			if (tcf.CollisionPath1[curColisionMask].HasCollision[0])
			{ Viewer1.Source = ColImges[lb00.SelectedIndex]; }
			else { Viewer1.Source = ColImgesNoCol[lb00.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[1])
			{ Viewer2.Source = ColImges[lb01.SelectedIndex]; }
			else { Viewer2.Source = ColImgesNoCol[lb01.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[2])
			{ Viewer3.Source = ColImges[lb02.SelectedIndex]; }
			else { Viewer3.Source = ColImgesNoCol[lb02.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[3])
			{ Viewer4.Source = ColImges[lb03.SelectedIndex]; }
			else { Viewer4.Source = ColImgesNoCol[lb03.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[4])
			{ Viewer5.Source = ColImges[lb04.SelectedIndex]; }
			else { Viewer5.Source = ColImgesNoCol[lb04.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[5])
			{ Viewer6.Source = ColImges[lb05.SelectedIndex]; }
			else { Viewer6.Source = ColImgesNoCol[lb05.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[6])
			{ Viewer7.Source = ColImges[lb06.SelectedIndex]; }
			else { Viewer7.Source = ColImgesNoCol[lb06.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[7])
			{ Viewer8.Source = ColImges[lb07.SelectedIndex]; }
			else { Viewer8.Source = ColImgesNoCol[lb07.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[8])
			{ Viewer9.Source = ColImges[lb08.SelectedIndex]; }
			else { Viewer9.Source = ColImgesNoCol[lb08.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[9])
			{ Viewer10.Source = ColImges[lb09.SelectedIndex]; }
			else { Viewer10.Source = ColImgesNoCol[lb09.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[10])
			{ Viewer11.Source = ColImges[lb10.SelectedIndex]; }
			else { Viewer11.Source = ColImgesNoCol[lb10.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[11])
			{ Viewer12.Source = ColImges[lb11.SelectedIndex]; }
			else { Viewer12.Source = ColImgesNoCol[lb11.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[12])
			{ Viewer13.Source = ColImges[lb12.SelectedIndex]; }
			else { Viewer13.Source = ColImgesNoCol[lb12.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[13])
			{ Viewer14.Source = ColImges[lb13.SelectedIndex]; }
			else { Viewer14.Source = ColImgesNoCol[lb13.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[14])
			{ Viewer15.Source = ColImges[lb14.SelectedIndex]; }
			else { Viewer15.Source = ColImgesNoCol[lb14.SelectedIndex]; }

			if (tcf.CollisionPath1[curColisionMask].HasCollision[15])
			{ Viewer16.Source = ColImges[lb15.SelectedIndex]; }
			else { Viewer16.Source = ColImgesNoCol[lb15.SelectedIndex]; }




			if (cb00.IsChecked.Value) { RGBox0.Source = ColActivatedImges[1]; }
			else { RGBox0.Source = ColActivatedImges[0]; }

			if (cb01.IsChecked.Value) { RGBox1.Source = ColActivatedImges[1]; }
			else { RGBox1.Source = ColActivatedImges[0]; }

			if (cb02.IsChecked.Value) { RGBox2.Source = ColActivatedImges[1]; }
			else { RGBox2.Source = ColActivatedImges[0]; }

			if (cb03.IsChecked.Value) { RGBox3.Source = ColActivatedImges[1]; }
			else { RGBox3.Source = ColActivatedImges[0]; }

			if (cb04.IsChecked.Value) { RGBox4.Source = ColActivatedImges[1]; }
			else { RGBox4.Source = ColActivatedImges[0]; }

			if (cb05.IsChecked.Value) { RGBox5.Source = ColActivatedImges[1]; }
			else { RGBox5.Source = ColActivatedImges[0]; }

			if (cb06.IsChecked.Value) { RGBox6.Source = ColActivatedImges[1]; }
			else { RGBox6.Source = ColActivatedImges[0]; }

			if (cb07.IsChecked.Value) { RGBox7.Source = ColActivatedImges[1]; }
			else { RGBox7.Source = ColActivatedImges[0]; }

			if (cb08.IsChecked.Value) { RGBox8.Source = ColActivatedImges[1]; }
			else { RGBox8.Source = ColActivatedImges[0]; }

			if (cb09.IsChecked.Value) { RGBox9.Source = ColActivatedImges[1]; }
			else { RGBox9.Source = ColActivatedImges[0]; }

			if (cb10.IsChecked.Value) { RGBoxA.Source = ColActivatedImges[1]; }
			else { RGBoxA.Source = ColActivatedImges[0]; }

			if (cb11.IsChecked.Value) { RGBoxB.Source = ColActivatedImges[1]; }
			else { RGBoxB.Source = ColActivatedImges[0]; }

			if (cb12.IsChecked.Value) { RGBoxC.Source = ColActivatedImges[1]; }
			else { RGBoxC.Source = ColActivatedImges[0]; }

			if (cb13.IsChecked.Value) { RGBoxD.Source = ColActivatedImges[1]; }
			else { RGBoxD.Source = ColActivatedImges[0]; }

			if (cb14.IsChecked.Value) { RGBoxE.Source = ColActivatedImges[1]; }
			else { RGBoxE.Source = ColActivatedImges[0]; }

			if (cb15.IsChecked.Value) { RGBoxF.Source = ColActivatedImges[1]; }
			else { RGBoxF.Source = ColActivatedImges[0]; }
		}

		public void RefreshPathB()
		{
			lb00.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[0];
			lb01.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[1];
			lb02.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[2];
			lb03.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[3];
			lb04.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[4];
			lb05.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[5];
			lb06.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[6];
			lb07.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[7];
			lb08.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[8];
			lb09.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[9];
			lb10.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[10];
			lb11.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[11];
			lb12.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[12];
			lb13.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[13];
			lb14.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[14];
			lb15.SelectedIndex = tcf.CollisionPath2[curColisionMask].Collision[15];

			cb00.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[0];
			cb01.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[1];
			cb02.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[2];
			cb03.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[3];
			cb04.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[4];
			cb05.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[5];
			cb06.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[6];
			cb07.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[7];
			cb08.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[8];
			cb09.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[9];
			cb10.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[10];
			cb11.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[11];
			cb12.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[12];
			cb13.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[13];
			cb14.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[14];
			cb15.IsChecked = tcf.CollisionPath2[curColisionMask].HasCollision[15];


			if (tcf.CollisionPath2[curColisionMask].HasCollision[0])
			{ Viewer1.Source = ColImges[lb00.SelectedIndex]; }
			else { Viewer1.Source = ColImgesNoCol[lb00.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[1])
			{ Viewer2.Source = ColImges[lb01.SelectedIndex]; }
			else { Viewer2.Source = ColImgesNoCol[lb01.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[2])
			{ Viewer3.Source = ColImges[lb02.SelectedIndex]; }
			else { Viewer3.Source = ColImgesNoCol[lb02.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[3])
			{ Viewer4.Source = ColImges[lb03.SelectedIndex]; }
			else { Viewer4.Source = ColImgesNoCol[lb03.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[4])
			{ Viewer5.Source = ColImges[lb04.SelectedIndex]; }
			else { Viewer5.Source = ColImgesNoCol[lb04.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[5])
			{ Viewer6.Source = ColImges[lb05.SelectedIndex]; }
			else { Viewer6.Source = ColImgesNoCol[lb05.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[6])
			{ Viewer7.Source = ColImges[lb06.SelectedIndex]; }
			else { Viewer7.Source = ColImgesNoCol[lb06.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[7])
			{ Viewer8.Source = ColImges[lb07.SelectedIndex]; }
			else { Viewer8.Source = ColImgesNoCol[lb07.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[8])
			{ Viewer9.Source = ColImges[lb08.SelectedIndex]; }
			else { Viewer9.Source = ColImgesNoCol[lb08.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[9])
			{ Viewer10.Source = ColImges[lb09.SelectedIndex]; }
			else { Viewer10.Source = ColImgesNoCol[lb09.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[10])
			{ Viewer11.Source = ColImges[lb10.SelectedIndex]; }
			else { Viewer11.Source = ColImgesNoCol[lb10.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[11])
			{ Viewer12.Source = ColImges[lb11.SelectedIndex]; }
			else { Viewer12.Source = ColImgesNoCol[lb11.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[12])
			{ Viewer13.Source = ColImges[lb12.SelectedIndex]; }
			else { Viewer13.Source = ColImgesNoCol[lb12.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[13])
			{ Viewer14.Source = ColImges[lb13.SelectedIndex]; }
			else { Viewer14.Source = ColImgesNoCol[lb13.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[14])
			{ Viewer15.Source = ColImges[lb14.SelectedIndex]; }
			else { Viewer15.Source = ColImgesNoCol[lb14.SelectedIndex]; }

			if (tcf.CollisionPath2[curColisionMask].HasCollision[15])
			{ Viewer16.Source = ColImges[lb15.SelectedIndex]; }
			else { Viewer16.Source = ColImgesNoCol[lb15.SelectedIndex]; }




			if (cb00.IsChecked.Value) { RGBox0.Source = ColActivatedImges[1]; }
			else { RGBox0.Source = ColActivatedImges[0]; }

			if (cb01.IsChecked.Value) { RGBox1.Source = ColActivatedImges[1]; }
			else { RGBox1.Source = ColActivatedImges[0]; }

			if (cb02.IsChecked.Value) { RGBox2.Source = ColActivatedImges[1]; }
			else { RGBox2.Source = ColActivatedImges[0]; }

			if (cb03.IsChecked.Value) { RGBox3.Source = ColActivatedImges[1]; }
			else { RGBox3.Source = ColActivatedImges[0]; }

			if (cb04.IsChecked.Value) { RGBox4.Source = ColActivatedImges[1]; }
			else { RGBox4.Source = ColActivatedImges[0]; }

			if (cb05.IsChecked.Value) { RGBox5.Source = ColActivatedImges[1]; }
			else { RGBox5.Source = ColActivatedImges[0]; }

			if (cb06.IsChecked.Value) { RGBox6.Source = ColActivatedImges[1]; }
			else { RGBox6.Source = ColActivatedImges[0]; }

			if (cb07.IsChecked.Value) { RGBox7.Source = ColActivatedImges[1]; }
			else { RGBox7.Source = ColActivatedImges[0]; }

			if (cb08.IsChecked.Value) { RGBox8.Source = ColActivatedImges[1]; }
			else { RGBox8.Source = ColActivatedImges[0]; }

			if (cb09.IsChecked.Value) { RGBox9.Source = ColActivatedImges[1]; }
			else { RGBox9.Source = ColActivatedImges[0]; }

			if (cb10.IsChecked.Value) { RGBoxA.Source = ColActivatedImges[1]; }
			else { RGBoxA.Source = ColActivatedImges[0]; }

			if (cb11.IsChecked.Value) { RGBoxB.Source = ColActivatedImges[1]; }
			else { RGBoxB.Source = ColActivatedImges[0]; }

			if (cb12.IsChecked.Value) { RGBoxC.Source = ColActivatedImges[1]; }
			else { RGBoxC.Source = ColActivatedImges[0]; }

			if (cb13.IsChecked.Value) { RGBoxD.Source = ColActivatedImges[1]; }
			else { RGBoxD.Source = ColActivatedImges[0]; }

			if (cb14.IsChecked.Value) { RGBoxE.Source = ColActivatedImges[1]; }
			else { RGBoxE.Source = ColActivatedImges[0]; }

			if (cb15.IsChecked.Value) { RGBoxF.Source = ColActivatedImges[1]; }
			else { RGBoxF.Source = ColActivatedImges[0]; }
		}

		public void GetRawSlopeNUD()
		{
			if (tcf != null)
			{
				if (!showPathB)
				{
					SlopeNUD.Value = tcf.CollisionPath1[curColisionMask].FloorAngle;
					int calculationSlopeA = (int)((decimal)tcf.CollisionPath1[curColisionMask].FloorAngle / 256 * 360);
					//degreeLabel.Text = "Degree of Slope: " + calculationSlopeA.ToString();
					RawSlopeNUD.Value = calculationSlopeA;

				}
				else
				{
					SlopeNUD.Value = tcf.CollisionPath2[curColisionMask].FloorAngle;
					int calculationSlopeB = (int)((decimal)tcf.CollisionPath2[curColisionMask].FloorAngle / 256 * 360);
					//degreeLabel.Text = "Degree of Slope: " + calculationSlopeB.ToString();
					RawSlopeNUD.Value = calculationSlopeB;

				}
			}
		}

		public void RefreshCollisionList()
		{
			CollisionList.Images.Clear();

			if (!showPathB)
			{
				for (int i = 0; i < 1024; i++)
				{
					if (listSetting == 0)
					{
						CollisionList.Images.Add(CollisionListImgA[i]);
					}
					else
					{
						CollisionList.Images.Add(Tiles[i]);
					}
				}
			}
			else if (showPathB)
			{
				for (int i = 0; i < 1024; i++)
				{
					if (listSetting == 0)
					{
						CollisionList.Images.Add(CollisionListImgB[i]);
					}
					else
					{
						CollisionList.Images.Add(Tiles[i]);
					}
				}
			}
			CollisionList.SelectedIndex = curColisionMask;
			CollisionList.Refresh();
		}

		public void showPathBToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//Do we want to show Path B's Collision Masks instead of Path A's ones?
			if (!showPathB)
			{
				showPathB = showPathBToolStripMenuItem.IsChecked = true;
				VPLabel.Content = "Currently Viewing: Path B";
				RefreshUI();
			}
			else if (showPathB)
			{
				showPathB = showPathBToolStripMenuItem.IsChecked = false;
				VPLabel.Content = "Currently Viewing: Path A";
				RefreshUI();
			}
		}

		public void aboutToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow frm = new AboutWindow();
			frm.ShowDialog();
		}

		private void GotoNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (tcf != null)
			{
				curColisionMask = (int)GotoNUD.Value;

			}

			if (e.Source == GotoNUD) RefreshUI();
		}


		private void SlopeNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (tcf != null)
			{
				if (mirrorMode)
				{

					if (SlopeNUD.Value <= 255)
					{
						tcf.CollisionPath2[curColisionMask].FloorAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path B
						tcf.CollisionPath1[curColisionMask].FloorAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path A
					}
					else
					{
						SlopeNUD.Value = 255;
					}

				}
				else
				{
					if (!showPathB)
					{
						if (SlopeNUD.Value <= 255)
						{
							tcf.CollisionPath1[curColisionMask].FloorAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path A
						}
						else
						{
							SlopeNUD.Value = 255;
						}


					}
					if (showPathB)
					{
						if (SlopeNUD.Value <= 255)
						{
							tcf.CollisionPath2[curColisionMask].FloorAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path B
						}
						else
						{
							SlopeNUD.Value = 255;
						}
					}
				}

				GetRawSlopeNUD();

				//RefreshUI();
			}
		}


		private void PhysicsNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (tcf != null)
			{
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].CeilingAngle = (byte)PhysicsNUD.Value; //Set the Physics for Path A
					tcf.CollisionPath2[curColisionMask].CeilingAngle = (byte)PhysicsNUD.Value; //Set the Physics for Path B
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].CeilingAngle = (byte)PhysicsNUD.Value; //Set the Physics for Path A
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].CeilingAngle = (byte)PhysicsNUD.Value; //Set the Physics for Path B
					}
				}

				//RefreshUI();
			}
		}

		private void MomentumNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (tcf != null)
			{
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].LWallAngle = (byte)MomentumNUD.Value; //Set the Momentum value for Path A
					tcf.CollisionPath2[curColisionMask].LWallAngle = (byte)MomentumNUD.Value; //Set the Momentum value for Path B
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].LWallAngle = (byte)MomentumNUD.Value; //Set the Momentum value for Path A
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].LWallAngle = (byte)MomentumNUD.Value; //Set the Momentum value for Path B
					}
				}

				//RefreshUI();
			}
		}

		private void UnknownNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (tcf != null)
			{
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].RWallAngle = (byte)UnknownNUD.Value; //Set the RWallAngle value for Path A
					tcf.CollisionPath2[curColisionMask].RWallAngle = (byte)UnknownNUD.Value; //Set the RWallAngle value for Path B
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].RWallAngle = (byte)UnknownNUD.Value; //Set the RWallAngle value for Path A
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].RWallAngle = (byte)UnknownNUD.Value; //Set the RWallAngle value for Path B
					}
				}

				//RefreshUI();
			}
		}

		private void SpecialNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (tcf != null)
			{
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].Behaviour = (byte)SpecialNUD.Value; //Set the "Special" value for Path A
					tcf.CollisionPath2[curColisionMask].Behaviour = (byte)SpecialNUD.Value; //Set the "Special" value for Path B
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].Behaviour = (byte)SpecialNUD.Value; //Set the "Special" value for Path A
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].Behaviour = (byte)SpecialNUD.Value; //Set the "Special" value for Path B
					}
				}

				//RefreshUI();
			}
		}

		private void IsCeilingButton_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (tcf != null)
			{
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path A
					tcf.CollisionPath2[curColisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path B
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path A
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path B
					}
				}

				RefreshUI();
			}
		}

		private void CollisionList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CollisionList.SelectedIndex >= 0)
			{
				curColisionMask = CollisionList.SelectedIndex;
				GotoNUD.Value = CollisionList.SelectedIndex;
			}
			RefreshUI();
		}

		public void copyToOtherPathToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!showPathB)
			{
				tcf.CollisionPath2[curColisionMask].Collision = (byte[])tcf.CollisionPath1[curColisionMask].Collision.Clone();
				tcf.CollisionPath2[curColisionMask].HasCollision = (bool[])tcf.CollisionPath1[curColisionMask].HasCollision.Clone();
				tcf.CollisionPath2[curColisionMask].IsCeiling = tcf.CollisionPath1[curColisionMask].IsCeiling;
				tcf.CollisionPath2[curColisionMask].FloorAngle = tcf.CollisionPath1[curColisionMask].FloorAngle;
				tcf.CollisionPath2[curColisionMask].LWallAngle = tcf.CollisionPath1[curColisionMask].LWallAngle;
				tcf.CollisionPath2[curColisionMask].CeilingAngle = tcf.CollisionPath1[curColisionMask].CeilingAngle;
				tcf.CollisionPath2[curColisionMask].Behaviour = tcf.CollisionPath1[curColisionMask].Behaviour;

				CollisionListImgB[curColisionMask] = CollisionListImgA[curColisionMask];
				RefreshUI();
			}
			else if (showPathB)
			{
				tcf.CollisionPath1[curColisionMask].Collision = (byte[])tcf.CollisionPath2[curColisionMask].Collision.Clone();
				tcf.CollisionPath1[curColisionMask].HasCollision = (bool[])tcf.CollisionPath2[curColisionMask].HasCollision.Clone();
				tcf.CollisionPath1[curColisionMask].IsCeiling = tcf.CollisionPath2[curColisionMask].IsCeiling;
				tcf.CollisionPath1[curColisionMask].FloorAngle = tcf.CollisionPath2[curColisionMask].FloorAngle;
				tcf.CollisionPath1[curColisionMask].LWallAngle = tcf.CollisionPath2[curColisionMask].LWallAngle;
				tcf.CollisionPath1[curColisionMask].CeilingAngle = tcf.CollisionPath2[curColisionMask].CeilingAngle;
				tcf.CollisionPath1[curColisionMask].Behaviour = tcf.CollisionPath2[curColisionMask].Behaviour;

				CollisionListImgA[curColisionMask] = CollisionListImgB[curColisionMask];
				RefreshUI();
			}
		}

		public void mirrorPathsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (mirrorPathsToolStripMenuItem1.IsChecked)
			{
				mirrorMode = true;
			}
			else if (!mirrorPathsToolStripMenuItem1.IsChecked)
			{
				mirrorMode = false;
			}
			UpdateMirrorModeStatusLabel();
		}

		#region Collision Mask Methods

		private void lb_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			int row = GetLBSender(sender);
			if (tcf != null && row != -1)
			{
				System.Windows.Controls.ListBox lb = (System.Windows.Controls.ListBox)sender;
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
					CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					tcf.CollisionPath2[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
					CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					CollisionList.Refresh();
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
						CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
						CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
				}
				RefreshUI();
			}
		}

		private int GetLBSender(object sender)
		{
			if (sender.Equals(lb00))
			{
				return 0;
			}
			else if (sender.Equals(lb01))
			{
				return 1;
			}
			else if (sender.Equals(lb02))
			{
				return 2;
			}
			else if (sender.Equals(lb03))
			{
				return 3;
			}
			else if (sender.Equals(lb04))
			{
				return 4;
			}
			else if (sender.Equals(lb05))
			{
				return 5;
			}
			else if (sender.Equals(lb06))
			{
				return 6;
			}
			else if (sender.Equals(lb07))
			{
				return 7;
			}
			else if (sender.Equals(lb08))
			{
				return 8;
			}
			else if (sender.Equals(lb09))
			{
				return 9;
			}
			else if (sender.Equals(lb10))
			{
				return 10;
			}
			else if (sender.Equals(lb11))
			{
				return 11;
			}
			else if (sender.Equals(lb12))
			{
				return 12;
			}
			else if (sender.Equals(lb13))
			{
				return 13;
			}
			else if (sender.Equals(lb14))
			{
				return 14;
			}
			else if (sender.Equals(lb15))
			{
				return 15;
			}
			else
			{
				return -1;
			}
		}

		private int GetCBSender(object sender)
		{
			if (sender.Equals(cb00))
			{
				return 0;
			}
			else if (sender.Equals(cb01))
			{
				return 1;
			}
			else if (sender.Equals(cb02))
			{
				return 2;
			}
			else if (sender.Equals(cb03))
			{
				return 3;
			}
			else if (sender.Equals(cb04))
			{
				return 4;
			}
			else if (sender.Equals(cb05))
			{
				return 5;
			}
			else if (sender.Equals(cb06))
			{
				return 6;
			}
			else if (sender.Equals(cb07))
			{
				return 7;
			}
			else if (sender.Equals(cb08))
			{
				return 8;
			}
			else if (sender.Equals(cb09))
			{
				return 9;
			}
			else if (sender.Equals(cb10))
			{
				return 10;
			}
			else if (sender.Equals(cb11))
			{
				return 11;
			}
			else if (sender.Equals(cb12))
			{
				return 12;
			}
			else if (sender.Equals(cb13))
			{
				return 13;
			}
			else if (sender.Equals(cb14))
			{
				return 14;
			}
			else if (sender.Equals(cb15))
			{
				return 15;
			}
			else
			{
				return -1;
			}
		}

		private void cb_CheckedChanged(object sender, RoutedEventArgs e)
		{
			int box = GetCBSender(sender);
			if (tcf != null && box != -1)
			{
				System.Windows.Controls.CheckBox cb = (System.Windows.Controls.CheckBox)sender;
				if (mirrorMode)
				{
					tcf.CollisionPath1[curColisionMask].HasCollision[box] = cb.IsChecked.Value;
					CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					tcf.CollisionPath2[curColisionMask].HasCollision[box] = cb.IsChecked.Value;
					CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					CollisionList.Refresh();
				}
				else
				{
					if (!showPathB)
					{
						tcf.CollisionPath1[curColisionMask].HasCollision[box] = cb.IsChecked.Value;
						CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
					if (showPathB)
					{
						tcf.CollisionPath2[curColisionMask].HasCollision[box] = cb.IsChecked.Value;
						CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
				}


				RefreshUI();
			}
		}

		public void lb_scrolling(object sender, MouseEventArgs e)
		{
			int lb = GetLBSender(sender);
			if (lb != -1)
			{
				System.Windows.Controls.ListBox list = (System.Windows.Controls.ListBox)sender;
				if (e.Delta <= -1)
				{
					if (list.SelectedIndex > 0)
					{
						list.SelectedIndex--;
					}
				}
				else
				{
					if (list.SelectedIndex < 15)
					{
						list.SelectedIndex++;
					}
				}
			}


		}
		#endregion

		public void saveUncompressedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            SetGridActivation(false);
            if (filepath != null) //Did we open a file?
			{
				tcf.WriteUnc(filepath);
			}
			else //if not then use Save As instead
			{
				saveAsUncompressedToolStripMenuItem_Click(null, e);
			}
            SetGridActivation(true);
        }

		public void saveAsUncompressedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            SetGridActivation(false);
            SaveFileDialog dlg = new SaveFileDialog
			{
				Title = "Save Uncompressed As...",
				DefaultExt = ".bin",
				Filter = "RSDKv5 Tileconfig Files (*.bin)|*.bin"
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tcf.WriteUnc(dlg.FileName); //Write Uncompressed
			}
            SetGridActivation(true);
        }

		private void radioButton1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!changingModes)
			{
				unCheckModes();
				viewerSetting = 0;
				colllisionViewButton.IsChecked = true;
				CollisionPicBox.Visible = true;
				PicBoxHost3Collision.Visibility = Visibility.Visible;
				Properties.Settings.Default.Save();
				changingModes = false;
				RefreshUI();

			}
		}

		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!changingModes)
			{
				unCheckModes();
				tileViewButton.IsChecked = true;
				viewerSetting = 1;
				Properties.Settings.Default.Save();
				PicBoxHost2Tile.Visibility = Visibility.Visible;
				TilePicBox.Visible = true;
				changingModes = false;
				RefreshUI();

			}
		}

		private void radioButton3_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!changingModes)
			{
				unCheckModes();
				viewerSetting = 2;
				Properties.Settings.Default.Save();
				overlayViewButton.IsChecked = true;
				PicBoxHost1Overlay.Visibility = Visibility.Visible;
				overlayPicBox.Visible = true;
				changingModes = false;
				RefreshUI();

			}
		}

		void unCheckModes()
		{
			changingModes = true;
			colllisionViewButton.IsChecked = false;
			tileViewButton.IsChecked = false;
			overlayViewButton.IsChecked = false;
			TilePicBox.Visible = false;
			CollisionPicBox.Visible = false;
			overlayPicBox.Visible = false;
			PicBoxHost1Overlay.Visibility = Visibility.Hidden;
			PicBoxHost2Tile.Visibility = Visibility.Hidden;
			PicBoxHost3Collision.Visibility = Visibility.Hidden;

		}

		private void tileViewRadioButton_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (lockRadioButtons == false)
			{
				uncheckListViews();
				listSetting = 1;
				Properties.Settings.Default.Save();
				tileViewRadioButton.IsChecked = true;
				lockRadioButtons = false;
				refreshCollision();
                RefreshUI();
			}
		}

		private void collisionViewRadioButton_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (lockRadioButtons == false)
			{
				uncheckListViews();
				listSetting = 0;
				Properties.Settings.Default.Save();
				collisionViewRadioButton.IsChecked = true;
				lockRadioButtons = false;
				refreshCollision();
                RefreshUI();
            }
        }

		void uncheckListViews()
		{
			lockRadioButtons = true;
			collisionViewRadioButton.IsChecked = false;
			tileViewRadioButton.IsChecked = false;
		}

		public void copyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!showPathB)
			{
				TileClipboard = tcf.CollisionPath1[curColisionMask];
				Clipboard.SetData("TileManiacCollision", tcf.CollisionPath1[curColisionMask]);
				RefreshUI();
			}
			else if (showPathB)
			{
				TileClipboard = tcf.CollisionPath2[curColisionMask];
				Clipboard.SetData("TileManiacCollision", tcf.CollisionPath2[curColisionMask]);
				RefreshUI();
			}
		}

		public void pasteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!showPathB)
			{
				if (Clipboard.ContainsData("TileManiacCollision") && Properties.Defaults.Default.TileManiacEnableWindowsClipboard)
				{
					var copyData = Clipboard.GetData("TileManiacCollision") as TileConfig.CollisionMask;
					if (copyData != null)
					{
						tcf.CollisionPath1[curColisionMask] = copyData;
					}

				}
				else if (TileClipboard != null)
				{
					tcf.CollisionPath1[curColisionMask] = TileClipboard;
				}

				RefreshUI();
			}
			else if (showPathB)
			{
				if (Clipboard.ContainsData("TileManiacCollision") && Properties.Defaults.Default.TileManiacEnableWindowsClipboard)
				{
					var copyData = Clipboard.GetData("TileManiacCollision") as TileConfig.CollisionMask;
					if (copyData != null)
					{
						tcf.CollisionPath2[curColisionMask] = copyData;
					}
				}
				else if (TileClipboard != null)
				{
					tcf.CollisionPath2[curColisionMask] = TileClipboard;
				}
				RefreshUI();
			}
		}

		void refreshCollision()
		{

			if (filepath != null)
			{
				CollisionList.Images.Clear();
				CollisionListImgA.Clear();
				CollisionListImgB.Clear();

				for (int i = 0; i < 1024; i++)
				{
					if (listSetting == 0)
					{
						CollisionListImgA.Add(tcf.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
						CollisionList.Images.Add(CollisionListImgA[i]);
					}
					else
					{
						CollisionListImgA.Add(Tiles[i]);
						CollisionList.Images.Add(Tiles[i]);
					}

				}

				for (int i = 0; i < 1024; i++)
				{
					if (listSetting == 0)
					{
						CollisionListImgB.Add(tcf.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
						CollisionList.Images.Add(CollisionListImgB[i]);
					}
					else
					{
						CollisionListImgB.Add(Tiles[i]);
						CollisionList.Images.Add(Tiles[i]);
					}
				}
				CollisionList.Refresh();

				RefreshUI(); //update the UI

			}
		}

		public void openSingleCollisionMaskToolStripMenuItem_Click_1(object sender, RoutedEventArgs e)
		{
            SetGridActivation(false);
            OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Import CollisionMask...";
			dlg.DefaultExt = ".rcm";
			dlg.Filter = "Singular RSDKv5 CollisionMask (*.rcm)|*.rcm";

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				RSDKv5.Reader Reader1 = new RSDKv5.Reader(dlg.FileName);
				RSDKv5.Reader Reader2 = new RSDKv5.Reader(dlg.FileName);
				tcf.CollisionPath1[curColisionMask] = new RSDKv5.Tileconfig.CollisionMask(Reader1);
				Reader1.Close();
				tcf.CollisionPath2[curColisionMask] = new RSDKv5.Tileconfig.CollisionMask(Reader2);
				Reader2.Close();
			}
			RefreshUI();
			//RefreshCollisionList(true);


		}

		public void exportCurrentCollisionMaskAsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Export As...";
			dlg.DefaultExt = ".rcm";
			dlg.Filter = "Singular RSDKv5 CollisionMask (*.rcm)|*.rcm";

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				BinaryWriter Writer1 = new BinaryWriter(File.Create(dlg.FileName));
				BinaryWriter Writer2 = new BinaryWriter(File.Create(dlg.FileName));
				tcf.CollisionPath1[curColisionMask].WriteUnc(Writer1);
				tcf.CollisionPath2[curColisionMask].WriteUnc(Writer2);
				Writer1.Close();
				Writer2.Close();
				RefreshUI();
			}
		}

		public void importFromOlderRSDKVersionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            SetGridActivation(false);
            OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Open Compressed";
			dlg.DefaultExt = ".bin";
			dlg.Filter = "RSDK ColllisionMask Files (CollisionMasks.bin)|CollisionMasks.bin";

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				indexedImagedLoaded = false;
				curColisionMask = 0; //Set the current collision mask to zero (avoids rare errors)
				filepath = dlg.FileName;
				tcf = new RSDKv5.Tileconfig();
				tcfBak = new RSDKv5.Tileconfig();
				RSDKvB.Tileconfig tcfOLD = new RSDKvB.Tileconfig(dlg.FileName);
				string tileBitmapPath = filepath.Replace("CollisionMasks.bin", "16x16tiles.gif"); //get the path to the stage's tileset
				LoadTileSet(new Bitmap(tileBitmapPath)); //load each 16x16 tile into the list
				bitmappath = tileBitmapPath;


				CollisionListImgA.Clear();
				CollisionListImgB.Clear();
				CollisionList.Images.Clear();

				for (int i = 0; i < 1024; i++)
				{
					CollisionListImgA.Add(tcfOLD.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
					CollisionListImgB.Add(tcfOLD.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));

					CollisionList.Images.Add(CollisionListImgA[i]);
					CollisionList.Images.Add(CollisionListImgB[i]);

					tcf.CollisionPath1[i].Collision = tcfOLD.CollisionPath1[i].Collision;
					tcf.CollisionPath1[i].HasCollision = tcfOLD.CollisionPath1[i].HasCollision;
					tcf.CollisionPath1[i].IsCeiling = tcfOLD.CollisionPath1[i].isCeiling;
					tcf.CollisionPath1[i].LWallAngle = tcfOLD.CollisionPath1[i].LWallAngle;
					tcf.CollisionPath1[i].CeilingAngle = tcfOLD.CollisionPath1[i].CeilingAngle;
					tcf.CollisionPath1[i].FloorAngle = tcfOLD.CollisionPath1[i].FloorAngle;
					tcf.CollisionPath1[i].Behaviour = 0;
					tcf.CollisionPath1[i].RWallAngle = tcfOLD.CollisionPath1[i].RWallAngle;

					tcf.CollisionPath2[i].Collision = tcfOLD.CollisionPath2[i].Collision;
					tcf.CollisionPath2[i].HasCollision = tcfOLD.CollisionPath2[i].HasCollision;
					tcf.CollisionPath2[i].IsCeiling = tcfOLD.CollisionPath2[i].isCeiling;
					tcf.CollisionPath2[i].LWallAngle = tcfOLD.CollisionPath2[i].LWallAngle;
					tcf.CollisionPath2[i].CeilingAngle = tcfOLD.CollisionPath2[i].CeilingAngle;
					tcf.CollisionPath2[i].FloorAngle = tcfOLD.CollisionPath2[i].FloorAngle;
					tcf.CollisionPath2[i].Behaviour = 0;
					tcf.CollisionPath2[i].RWallAngle = tcfOLD.CollisionPath2[i].RWallAngle;
				}
				CollisionList.SelectedIndex = curColisionMask - 1;
				CollisionList.Refresh();

				RefreshUI(); //update the UI
			}
            SetGridActivation(true);
        }

		private void gridPicBox_Paint(object sender, PaintEventArgs e)
		{
			if (showGrid)
			{
				Graphics g = e.Graphics;
				g.PixelOffsetMode = PixelOffsetMode.None;
				int numOfCells = 16;
				int cellSize = 6;
				System.Drawing.Pen p = new System.Drawing.Pen(Color.Black);
				p.DashOffset = -1;

				for (int i = 0; i < numOfCells; i++)
				{
					// Vertical
					g.DrawLine(p, i * cellSize, 0, i * cellSize, numOfCells * cellSize);
					// Horizontal
					g.DrawLine(p, 0, i * cellSize, numOfCells * cellSize, i * cellSize);
				}
			}

		}

		private void developerInterfaceToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//DeveloperTerminal developerTerminal = new DeveloperTerminal();
			//developerTerminal.Show();
		}

		public void showGridToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (showGridToolStripMenuItem.IsChecked)
			{
				showGrid = true;
				RefreshUI();
			}
			else
			{
				showGrid = false;
				RefreshUI();
			}
		}

		private void groupBox1_Enter(object sender, RoutedEventArgs e)
		{

		}

		private void Mainform_Load(object sender, RoutedEventArgs e)
		{

		}

		private void CollisionViewer_Click(object sender, RoutedEventArgs e)
		{
            UpdateCollisionalGrid(sender, e);
        }



		private void CollisionViewer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			UpdateCollisionalGrid(sender, e);
		}

		private void UpdateCollisionalGrid(object sender, RoutedEventArgs e)
		{
            if (!CollisionViewer.IsEnabled) return;
            if (freezeGrid) return;
			Extensions.GetRowColIndex(CollisionViewer, System.Windows.Input.Mouse.GetPosition(CollisionViewer), out int row, out int col);
			System.Windows.Point cellPos = new System.Windows.Point(col, row);
			if (cellPos.Y >= 16) return;
			switch (cellPos.X)
			{
				case 0:
					lb00.SelectedIndex = (int)cellPos.Y;
					break;
				case 1:
					lb01.SelectedIndex = (int)cellPos.Y;
					break;
				case 2:
					lb02.SelectedIndex = (int)cellPos.Y;
					break;
				case 3:
					lb03.SelectedIndex = (int)cellPos.Y;
					break;
				case 4:
					lb04.SelectedIndex = (int)cellPos.Y;
					break;
				case 5:
					lb05.SelectedIndex = (int)cellPos.Y;
					break;
				case 6:
					lb06.SelectedIndex = (int)cellPos.Y;
					break;
				case 7:
					lb07.SelectedIndex = (int)cellPos.Y;
					break;
				case 8:
					lb08.SelectedIndex = (int)cellPos.Y;
					break;
				case 9:
					lb09.SelectedIndex = (int)cellPos.Y;
					break;
				case 10:
					lb10.SelectedIndex = (int)cellPos.Y;
					break;
				case 11:
					lb11.SelectedIndex = (int)cellPos.Y;
					break;
				case 12:
					lb12.SelectedIndex = (int)cellPos.Y;
					break;
				case 13:
					lb13.SelectedIndex = (int)cellPos.Y;
					break;
				case 14:
					lb14.SelectedIndex = (int)cellPos.Y;
					break;
				case 15:
					lb15.SelectedIndex = (int)cellPos.Y;
					break;
			}
		}

		private void CollisionViewer_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
		{
			RefreshUI();
        }

		private void CollisionViewer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				UpdateCollisionalGrid(null, null);
			}

		}

		private void CollisionViewer_MouseHover(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				UpdateCollisionalGrid(null, null);
			}
		}

		private void cb_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				if (mouseIsDownL)
				{
					checkUncheckBox(true);
				}
				else if (mouseIsDownR)
				{
					checkUncheckBox(false);
				}

			}
		}

		private void cb_MouseHover(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				if (mouseIsDownL)
				{
					checkUncheckBox(true);
				}
				else if (mouseIsDownR)
				{
					checkUncheckBox(false);
				}

			}
		}

		private void cb_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			RefreshUI();
		}

		private void cb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				if (mouseIsDownL)
				{
					checkUncheckBox(true);
				}
				else if (mouseIsDownR)
				{
					checkUncheckBox(false);
				}

			}

		}

		private void checkUncheckBox(bool state = false)
		{
			Extensions.GetRowColIndex(CollisionViewer, System.Windows.Input.Mouse.GetPosition(CollisionViewer), out int row, out int col);
			System.Windows.Point cellPos = new System.Windows.Point(col, row);
			switch (cellPos.X)
			{
				case 0:
					cb00.IsChecked = state;
					cb00.ReleaseMouseCapture();
					break;
				case 1:
					cb01.IsChecked = state;
					cb01.ReleaseMouseCapture();
					break;
				case 2:
					cb02.IsChecked = state;
					cb02.ReleaseMouseCapture();
					break;
				case 3:
					cb03.IsChecked = state;
					cb03.ReleaseMouseCapture();
					break;
				case 4:
					cb04.IsChecked = state;
					cb04.ReleaseMouseCapture();
					break;
				case 5:
					cb05.IsChecked = state;
					cb05.ReleaseMouseCapture();
					break;
				case 6:
					cb06.IsChecked = state;
					cb06.ReleaseMouseCapture();
					break;
				case 7:
					cb07.IsChecked = state;
					cb07.ReleaseMouseCapture();
					break;
				case 8:
					cb08.IsChecked = state;
					cb08.ReleaseMouseCapture();
					break;
				case 9:
					cb09.IsChecked = state;
					cb09.ReleaseMouseCapture();
					break;
				case 10:
					cb10.IsChecked = state;
					cb10.ReleaseMouseCapture();
					break;
				case 11:
					cb11.IsChecked = state;
					cb11.ReleaseMouseCapture();
					break;
				case 12:
					cb12.IsChecked = state;
					cb12.ReleaseMouseCapture();
					break;
				case 13:
					cb13.IsChecked = state;
					cb13.ReleaseMouseCapture();
					break;
				case 14:
					cb14.IsChecked = state;
					cb14.ReleaseMouseCapture();
					break;
				case 15:
					cb15.IsChecked = state;
					cb15.ReleaseMouseCapture();
					break;
			}
		}

		public void classicViewModeToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (classicViewModeToolStripMenuItem.IsChecked)
			{
				classicMode = true;
			}
			else
			{
				classicMode = false;
			}
			RefreshUI();
		}

		public void overlayToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			viewAppearanceMode = 0;
			UpdateViewApperancePlusButtons();

		}

		public void collisionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			viewAppearanceMode = 1;
			UpdateViewApperancePlusButtons();
		}

		private void UpdateViewApperancePlusButtons()
		{
			switch (viewAppearanceMode)
			{
				case 0:
					overlayToolStripMenuItem.IsChecked = true;
					collisionToolStripMenuItem.IsChecked = false;
					break;
				case 1:
					overlayToolStripMenuItem.IsChecked = false;
					collisionToolStripMenuItem.IsChecked = true;
					break;
			}
			RefreshUI();
		}

		public void UpdateMirrorModeStatusLabel()
		{
			if (mirrorMode)
			{
				mirrorModeStatusLabel.Content = "Mirror Mode: ON";
			}
			else
			{
				mirrorModeStatusLabel.Content = "Mirror Mode: OFF";
			}
		}

		public void pathAToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				tcf.CollisionPath1[curColisionMask].Collision = (byte[])tcfBak.CollisionPath1[curColisionMask].Collision.Clone();
				tcf.CollisionPath1[curColisionMask].HasCollision = (bool[])tcfBak.CollisionPath1[curColisionMask].HasCollision.Clone();
				tcf.CollisionPath1[curColisionMask].IsCeiling = tcfBak.CollisionPath1[curColisionMask].IsCeiling;
				tcf.CollisionPath1[curColisionMask].FloorAngle = tcfBak.CollisionPath1[curColisionMask].FloorAngle;
				tcf.CollisionPath1[curColisionMask].LWallAngle = tcfBak.CollisionPath1[curColisionMask].LWallAngle;
				tcf.CollisionPath1[curColisionMask].CeilingAngle = tcfBak.CollisionPath1[curColisionMask].CeilingAngle;
				tcf.CollisionPath1[curColisionMask].Behaviour = tcfBak.CollisionPath1[curColisionMask].Behaviour;
				RefreshUI();
			}
		}

		public void pathBToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				tcf.CollisionPath2[curColisionMask].Collision = (byte[])tcfBak.CollisionPath2[curColisionMask].Collision.Clone();
				tcf.CollisionPath2[curColisionMask].HasCollision = (bool[])tcfBak.CollisionPath2[curColisionMask].HasCollision.Clone();
				tcf.CollisionPath2[curColisionMask].IsCeiling = tcfBak.CollisionPath2[curColisionMask].IsCeiling;
				tcf.CollisionPath2[curColisionMask].FloorAngle = tcfBak.CollisionPath2[curColisionMask].FloorAngle;
				tcf.CollisionPath2[curColisionMask].LWallAngle = tcfBak.CollisionPath2[curColisionMask].LWallAngle;
				tcf.CollisionPath2[curColisionMask].CeilingAngle = tcfBak.CollisionPath2[curColisionMask].CeilingAngle;
				tcf.CollisionPath2[curColisionMask].Behaviour = tcfBak.CollisionPath2[curColisionMask].Behaviour;
				RefreshUI();
			}
		}

		public void bothToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				tcf.CollisionPath1[curColisionMask].Collision = (byte[])tcfBak.CollisionPath1[curColisionMask].Collision.Clone();
				tcf.CollisionPath1[curColisionMask].HasCollision = (bool[])tcfBak.CollisionPath1[curColisionMask].HasCollision.Clone();
				tcf.CollisionPath1[curColisionMask].IsCeiling = tcfBak.CollisionPath1[curColisionMask].IsCeiling;
				tcf.CollisionPath1[curColisionMask].FloorAngle = tcfBak.CollisionPath1[curColisionMask].FloorAngle;
				tcf.CollisionPath1[curColisionMask].CeilingAngle = tcfBak.CollisionPath1[curColisionMask].CeilingAngle;
				tcf.CollisionPath1[curColisionMask].LWallAngle = tcfBak.CollisionPath1[curColisionMask].LWallAngle;
				tcf.CollisionPath1[curColisionMask].RWallAngle = tcfBak.CollisionPath1[curColisionMask].RWallAngle;
				tcf.CollisionPath1[curColisionMask].Behaviour = tcfBak.CollisionPath1[curColisionMask].Behaviour;


				tcf.CollisionPath2[curColisionMask].Collision = (byte[])tcfBak.CollisionPath2[curColisionMask].Collision.Clone();
				tcf.CollisionPath2[curColisionMask].HasCollision = (bool[])tcfBak.CollisionPath2[curColisionMask].HasCollision.Clone();
				tcf.CollisionPath2[curColisionMask].IsCeiling = tcfBak.CollisionPath2[curColisionMask].IsCeiling;
				tcf.CollisionPath2[curColisionMask].FloorAngle = tcfBak.CollisionPath2[curColisionMask].FloorAngle;
				tcf.CollisionPath2[curColisionMask].CeilingAngle = tcfBak.CollisionPath2[curColisionMask].CeilingAngle;
				tcf.CollisionPath2[curColisionMask].LWallAngle = tcfBak.CollisionPath2[curColisionMask].LWallAngle;
				tcf.CollisionPath2[curColisionMask].RWallAngle = tcfBak.CollisionPath2[curColisionMask].RWallAngle;
				tcf.CollisionPath2[curColisionMask].Behaviour = tcfBak.CollisionPath2[curColisionMask].Behaviour;

				RefreshUI();
			}
		}

		public void newInstanceToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var mainWindow = new MainWindow();
			mainWindow.Show();
		}

		public void flipTileHorizontallyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (AllowFlipPrompt())
			{
				Bitmap tile = Tiles[curColisionMask];
				tile.RotateFlip(RotateFlipType.RotateNoneFlipX);
				tile = Tiles[curColisionMask];
				imageIsModified = true;

				Bitmap indexedTile = IndexedTiles[curColisionMask];
				indexedTile.RotateFlip(RotateFlipType.RotateNoneFlipX);
				indexedTile = IndexedTiles[curColisionMask];

				RefreshUI();
			}

		}

		public void flipTileVerticallyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (AllowFlipPrompt())
			{
				Bitmap tile = Tiles[curColisionMask];
				tile.RotateFlip(RotateFlipType.RotateNoneFlipY);
				tile = Tiles[curColisionMask];
				imageIsModified = true;

				Bitmap indexedTile = IndexedTiles[curColisionMask];
				indexedTile.RotateFlip(RotateFlipType.RotateNoneFlipY);
				indexedTile = IndexedTiles[curColisionMask];

				RefreshUI();
			}
		}

		private bool AllowFlipPrompt()
		{
			if (!indexedImagedLoaded)
			{
				MessageBoxResult result = MessageBox.Show("To flip the tile, we have to load an indexed version of the image first. This may take some time. Would you like to continue?", "Create Indexed Image", MessageBoxButton.YesNo, MessageBoxImage.Information);
				if (result == MessageBoxResult.Yes)
				{
					LoadTileSet(new Bitmap(bitmappath), true);
					indexedImagedLoaded = true;
					return true;
				}
				else return false;
			}
			else return true;

		}

		public void settingsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            OptionsMenu options = new OptionsMenu();
            options.Owner = this;
            options.ShowDialog();
		}

		public void x16TilesgifToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Save16x16Tiles(true);
		}

		public void tileConfigbinToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			BackupCollisionData();
		}

		public void openCollisionHomeFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (filepath != null)
			{
				Process.Start("explorer.exe", "/select, " + filepath);
			}
			else
			{
				MessageBox.Show("No File Opened Yet!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void tableLayoutPanel2_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				if (mouseIsDownL)
				{
					checkUncheckBox(true);
				}
				else if (mouseIsDownR)
				{
					checkUncheckBox(false);
				}

			}
		}

		private void cb00_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				if (mouseIsDownL)
				{
					checkUncheckBox(true);
				}
				else if (mouseIsDownR)
				{
					checkUncheckBox(false);
				}

			}
		}

		public System.Windows.Media.Color ColorConvertToMedia(System.Drawing.Color input)
		{
			return System.Windows.Media.Color.FromArgb(input.A, input.R, input.G, input.B);
		}

		public System.Drawing.Color ColorConvertToDrawing(System.Windows.Media.Color input)
		{
			return System.Drawing.Color.FromArgb(input.A, input.R, input.G, input.B);
		}

		private void CollisionViewer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (mouseIsDownL || mouseIsDownR)
			{
				RefreshUI();
			}

		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				Properties.Settings.Default.Save();
			}
			catch (Exception ex)
			{
				Debug.Write("Failed to write settings: " + ex);
			}
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			EditorControls.TileManiac_OnKeyDown(sender, KeyEventExts.ToWinforms(e));
		}

		private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			EditorControls.TileManiac_OnKeyUp(sender, KeyEventExts.ToWinforms(e));
		}

		public void WindowAlwaysOnTop_Click(object sender, RoutedEventArgs e)
		{
			if (windowAlwaysOnTop.IsChecked)
			{
				this.Topmost = true;
			}
			else
			{
				this.Topmost = false;
			}
		}
    }
}
