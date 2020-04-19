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
using ManiacEditor.Controls;
using MessageBox = System.Windows.MessageBox;
using TileConfig = RSDKv5.Tileconfig;
using ManiacEditor.Controls.Global.Controls;
using ManiacEditor.Extensions;

using ManiacEditor.Controls.Misc;
using ManiacEditor.Controls.Misc.Configuration;
using ManiacEditor.Controls.Options;


using GenerationsLib.WPF;


namespace ManiacEditor.Controls.TileManiac
{
	public partial class CollisionEditor : Window
	{
		#region Definitions

		#region Bitmap Lists
		List<Bitmap> CollisionListImgA { get; set; } = new List<Bitmap>();
		List<Bitmap> CollisionListImgB { get; set; } = new List<Bitmap>();
		List<Bitmap> Tiles { get; set; } = new List<Bitmap>(); //List of all the 16x16 Stage Tiles
		List<Bitmap> IndexedTiles { get; set; } = new List<Bitmap>(); //List of all the 16x16 Stage Tiles (Preserving Color Pallete)

		#endregion

		#region Winform Controls

		public PictureBoxNearestNeighbor OverlayPicBox { get; set; } = new PictureBoxNearestNeighbor();
		public PictureBoxNearestNeighbor TilePicBox { get; set; } = new PictureBoxNearestNeighbor();
		public PictureBoxNearestNeighbor CollisionPicBox { get; set; } = new PictureBoxNearestNeighbor();
		public ManiacEditor.Controls.Global.Controls.RetroEDTileList CollisionList { get; set; } = new ManiacEditor.Controls.Global.Controls.RetroEDTileList();
		public DevicePanel GraphicPanel { get; set; }
		public IDrawPanel HostPanel { get; set; }

		public class IDrawPanel : System.Windows.Forms.Panel, IDrawArea
		{
			public SFML.System.Vector2i GetPosition()
			{
				return new SFML.System.Vector2i(0, 0);
			}

			public Rectangle GetScreen()
			{
				return new Rectangle(0, 0, this.Width, this.Height);
			}
			public float GetZoom()
			{
				return 1;
			}
		}

		#endregion

		#region Status Variables

		bool IsMouseLDown { get => System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed; }
		bool IsMouseRDown { get => System.Windows.Input.Mouse.RightButton == System.Windows.Input.MouseButtonState.Pressed; }

		public string TC_FilePath { get; set; }  //Where is the file located?
		public string TC_FolderPath { get; set; }  //Where is the folder located?
		public string TC_BitmapPath { get; set; }  //Where is the image located?


		public bool HasConfigBeenModified { get; set; } = false; //For intergrating tools to know that we have saved/made edits to this config.
		private bool IsChangingViewerMode { get; set; } = false; //To prevent updating the radio buttons until after we change the viewer mode.
		private bool LockRadioButtons { get; set; } = false; //for locking radio button updates when switching single select options.
		private bool HasImageBeenModified { get; set; } = false;
		private bool IsIndexedImageLoaded { get; set; } = false;
		public bool ShouldFreezeGrid { get; set; } = true;


		public int CurrentCollisionMask { get; set; } //What Collision Mask are we editing?
		private bool ShowPathB { get; set; } = false; //should we show Path A or Path B?
		public int CurrentViewerSetting { get; set; } = 0;
		public bool ShowGrid { get; set; } = false;
		public bool MirrorMode { get; set; } = false;
		public int TileListSetting { get; set; } = 0;
		public bool IsEditorClosed { get; private set; }



		#endregion

		#region Current Tile Config + Clipboard

		private RSDKv5.Tileconfig.CollisionMask TileClipboard { get; set; }
		public RSDKv5.Tileconfig TileConfig { get; set; } //The ColllisionMask Data
		public RSDKv5.Tileconfig OriginalTileConfig { get; set; } //Backup ColllisionMask Data

		#endregion

		#region Colors
		private System.Windows.Media.Brush NormalText
		{
			get
			{
				return Methods.Internal.Theming.NormalText;
			}
		}
		private Color CollisionColor
		{
			get
			{
				return Methods.Internal.Theming.TileManiac_CollisionColor;
			}
		}
		private Color AntiCollisionColor
		{
			get
			{
				return Methods.Internal.Theming.TileManiac_AntiCollisionColor;
			}
		}

		private System.Windows.Media.Brush CollisionColorBrush
		{
			get
			{
				return new SolidColorBrush(ColorExt.ToSWMColor(Methods.Internal.Theming.TileManiac_CollisionColor));
			}
		}
		private System.Windows.Media.Brush AntiCollisionColorBrush
		{
			get
			{
				return new SolidColorBrush(ColorExt.ToSWMColor(Methods.Internal.Theming.TileManiac_AntiCollisionColor));
			}
		}


		#endregion

		#region Instance + Misc

		public static CollisionEditor Instance { get; set; }

		#endregion

		#endregion

		#region Init
		public CollisionEditor()
		{
			InitializeComponent();
			InitalizeViewer();
			InitalizeDevicePanel();
			UpdateThemeColors();
			Instance = this;
			LoadSettings();
		}
		void LoadSettings()
		{
			if (Properties.Settings.MyDefaults.TileManiacListSetting == 0)
			{
				UncheckListViews();
				collisionViewRadioButton.IsChecked = true;
				LockRadioButtons = false;
				TileListSetting = 0;
			}
			else if (Properties.Settings.MyDefaults.TileManiacListSetting == 1)
			{
				UncheckListViews();
				tileViewRadioButton.IsChecked = true;
				LockRadioButtons = false;
				TileListSetting = 1;
			}

			if (Properties.Settings.MyDefaults.TileManiacRenderViewerSetting == 0)
			{
				UnCheckModes();
				tileViewButton.IsChecked = true;
				TilePicBox.Visible = true;
				PicBoxHost2Tile.Visibility = Visibility.Visible;
				IsChangingViewerMode = false;
				CurrentViewerSetting = 0;
			}
			else if (Properties.Settings.MyDefaults.TileManiacRenderViewerSetting == 1)
			{
				UnCheckModes();
				colllisionViewButton.IsChecked = true;
				CollisionPicBox.Visible = true;
				PicBoxHost3Collision.Visibility = Visibility.Visible;
				IsChangingViewerMode = false;
				CurrentViewerSetting = 1;
			}
			else if (Properties.Settings.MyDefaults.TileManiacRenderViewerSetting == 2)
			{
				UnCheckModes();
				overlayViewButton.IsChecked = true;
				OverlayPicBox.Visible = true;
				PicBoxHost1Overlay.Visibility = Visibility.Visible;
				IsChangingViewerMode = false;
				CurrentViewerSetting = 2;
			}
			if (Properties.Settings.MyDefaults.TileManiacShowGrid)
			{
				showGridToolStripMenuItem.IsChecked = true;
				ShowGrid = true;
			}
			if (Properties.Settings.MyDefaults.TileManiacMirrorMode)
			{
				mirrorPathsToolStripMenuItem1.IsChecked = true;
				MirrorMode = true;
				UpdateMirrorModeStatusLabel();

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
			this.OverlayPicBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.OverlayPicBox.BackColor = System.Drawing.Color.Transparent;
			this.OverlayPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.OverlayPicBox.InitialImage = null;
			this.OverlayPicBox.Dock = DockStyle.Fill;
			this.OverlayPicBox.Location = new System.Drawing.Point(0, 0);
			this.OverlayPicBox.Margin = new System.Windows.Forms.Padding(2);
			this.OverlayPicBox.Name = "overlayPicBox";
			this.OverlayPicBox.Size = new System.Drawing.Size(96, 96);
			this.OverlayPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.OverlayPicBox.TabIndex = 71;
			this.OverlayPicBox.TabStop = false;
			this.OverlayPicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPicBox_Paint);
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
			this.TilePicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPicBox_Paint);
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
			this.CollisionPicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPicBox_Paint);


			PicBoxHost1Overlay.Child = OverlayPicBox;
			PicBoxHost2Tile.Child = TilePicBox;
			PicBoxHost3Collision.Child = CollisionPicBox;
			TilesListHost.Child = CollisionList;


			OverlayPicBox.Show();
			TilePicBox.Show();
			CollisionPicBox.Show();
			CollisionList.Show();
		}

		public void InitalizeDevicePanel()
		{
			this.GraphicPanel = new ManiacEditor.DevicePanel();
			this.GraphicPanel.AllowDrop = true;
			this.GraphicPanel.AutoSize = false;
			this.GraphicPanel.DeviceBackColor = System.Drawing.Color.White;
			this.GraphicPanel.Location = new System.Drawing.Point(-1, 0);
			this.GraphicPanel.Margin = new System.Windows.Forms.Padding(0);
			this.GraphicPanel.Name = "GraphicPanel";
			this.GraphicPanel.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
			this.GraphicPanel.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
			this.GraphicPanel.TabIndex = 10;

			HostPanel = new IDrawPanel();
			HostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			HostPanel.Controls.Add(GraphicPanel);

			this.HostGrid.Child = HostPanel;

			this.GraphicPanel.OnRender += GraphicPanel_OnRender;

			this.GraphicPanel.Init(HostPanel);
			//this.GraphicPanel.Run();
		}



		#endregion

		#region Load/Save
		public void OpenCollision()
		{
			
			OpenFileDialog dlg = new OpenFileDialog
			{
				Title = "Open RSDKv5 Tileconfig",
				DefaultExt = ".bin",
				Filter = "RSDKv5 Tileconfig Files|Tileconfig*.bin|All Files|*"
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IsIndexedImageLoaded = false;
				CurrentCollisionMask = 0; // Set the current collision mask to zero (avoids rare errors)
				TC_FilePath = dlg.FileName;
				TileConfig = new RSDKv5.Tileconfig(dlg.FileName);
				OriginalTileConfig = new RSDKv5.Tileconfig(dlg.FileName);
				string tileBitmapPath = Path.Combine(Path.GetDirectoryName(TC_FilePath), "16x16tiles.gif"); // get the path to the stage's tileset
				LoadTileSet(new Bitmap(tileBitmapPath)); // load each 16x16 tile into the list
				TC_BitmapPath = tileBitmapPath;

				CollisionList.Images.Clear();

				for (int i = 0; i < 1024; i++)
				{
					if (TileListSetting == 0)
					{
						CollisionListImgA.Add(TileConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
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
					if (TileListSetting == 0)
					{
						CollisionListImgB.Add(TileConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
						CollisionList.Images.Add(CollisionListImgB[i]);
					}
					else
					{
						CollisionListImgB.Add(Tiles[i]);
						CollisionList.Images.Add(Tiles[i]);
					}
				}
				if (CollisionList.SelectedIndex != CurrentCollisionMask - 1) CollisionList.SelectedIndex = CurrentCollisionMask - 1;
				CollisionList.Refresh();

				RefreshUI(); //update the UI
			}
			
		}
		public void LoadTileConfigViaIntergration(TileConfig tilesConfig, string scenePath, int selectedTile = 0)
		{
			LoadSettings();

			IsIndexedImageLoaded = false;
			CurrentCollisionMask = 0;
			TileConfig = new TileConfig(Path.Combine(scenePath));
			OriginalTileConfig = new TileConfig(Path.Combine(scenePath));
			string tileBitmapPath = Path.Combine(Path.GetDirectoryName(scenePath), "16x16tiles.gif"); // get the path to the stage's tileset
			LoadTileSet(new Bitmap(tileBitmapPath)); // load each 16x16 tile into the list
			TC_BitmapPath = tileBitmapPath;

			CollisionList.Images.Clear();

			for (int i = 0; i < 1024; i++)
			{
				if (TileListSetting == 0)
				{
					CollisionListImgA.Add(TileConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
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
				if (TileListSetting == 0)
				{
					CollisionListImgB.Add(TileConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
					CollisionList.Images.Add(CollisionListImgB[i]);
				}
				else
				{
					CollisionListImgB.Add(Tiles[i]);
					CollisionList.Images.Add(Tiles[i]);
				}
			}
			if (CollisionList.SelectedIndex != selectedTile) CollisionList.SelectedIndex = selectedTile;
			CollisionList.Refresh();

			CurrentCollisionMask = selectedTile;
			RefreshUI(); //update the UI

		}
		public void saveToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (TC_FilePath != null) //Did we open a file?
			{
				
				Save16x16Tiles();
				TileConfig.Write(TC_FilePath);
				HasConfigBeenModified = true;
				
			}
			else //if not then use "Save As..."
			{
				saveAsToolStripMenuItem_Click(null, e);
			}
		}
		public void saveAsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			
			SaveFileDialog dlg = new SaveFileDialog
			{
				Title = "Save RSDKv5 Tileconfig As...",
				DefaultExt = ".bin",
				Filter = "RSDKv5 Tileconfig Files|Tileconfig*.bin"
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TileConfig.Write(dlg.FileName); //Write the data to a file
			}
			
		}
		public void OpenToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OpenCollision();
		}
		public void Save16x16Tiles()
		{
			try
			{
				if (HasImageBeenModified && IsIndexedImageLoaded)
				{
					if (!Properties.Settings.MyDefaults.TileManiacAllowDirect16x16TilesGIFEditing)
					{
						if (Properties.Settings.MyDefaults.TileManiacPromptForChoiceOnImageWrite)
						{
							MessageBoxResult result = MessageBox.Show("You have made changes that require the 16x16Tiles.gif to be modifed. While this feature should normally work just fine, it may cause some issues, which is why you may choose if you want to or not. So do you want to save directly to the 16x16Tiles.gif? (Click No will save to 16x16Tiles_Copy.gif, and Cancel with not write this file at all) (You also can change this dialog's visibility in options)", "Saving 16x16Tiles.gif", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
							if (result == MessageBoxResult.Yes)
							{
								SaveTileSet("16x16Tiles.gif");
								HasImageBeenModified = false;
							}
							else if (result == MessageBoxResult.No)
							{
								SaveTileSet("16x16Tiles_Copy.gif");
								HasImageBeenModified = false;
							}
							else if (result == MessageBoxResult.Cancel)
							{
								HasImageBeenModified = true; //We Didn't Change Anything, keep reminding the user
							}
						}
						else
						{
							SaveTileSet("16x16Tiles_Copy.gif");
							HasImageBeenModified = false;
						}

					}
					else
					{
						SaveTileSet("16x16Tiles.gif");
						HasImageBeenModified = false;
					}
				}
			}
			catch (Exception ex)
			{
				ShowError($@"Failed to Save 16x16Tiles Image to file! We will still try to save your collision however" + Environment.NewLine + "Error: " + ex.Message.ToString());
			}

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
					TileConfig.CollisionPath1[i].WriteUnc(Writer1);
					TileConfig.CollisionPath2[i].WriteUnc(Writer2);
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
			bmp.Save(Path.Combine(Path.GetDirectoryName(TC_FilePath), path));
		}
		public void openSingleCollisionMaskToolStripMenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Import CollisionMask...";
			dlg.DefaultExt = ".rcm";
			dlg.Filter = "Singular RSDKv5 CollisionMask (*.rcm)|*.rcm";

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				RSDKv5.Reader Reader1 = new RSDKv5.Reader(dlg.FileName);
				RSDKv5.Reader Reader2 = new RSDKv5.Reader(dlg.FileName);
				TileConfig.CollisionPath1[CurrentCollisionMask] = new RSDKv5.Tileconfig.CollisionMask(Reader1);
				Reader1.Close();
				TileConfig.CollisionPath2[CurrentCollisionMask] = new RSDKv5.Tileconfig.CollisionMask(Reader2);
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
				TileConfig.CollisionPath1[CurrentCollisionMask].WriteUnc(Writer1);
				TileConfig.CollisionPath2[CurrentCollisionMask].WriteUnc(Writer2);
				Writer1.Close();
				Writer2.Close();
				RefreshUI();
			}
		}
		public void importFromOlderRSDKVersionToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Open Compressed";
			dlg.DefaultExt = ".bin";
			dlg.Filter = "RSDK ColllisionMask Files (CollisionMasks.bin)|CollisionMasks.bin";

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IsIndexedImageLoaded = false;
				CurrentCollisionMask = 0; //Set the current collision mask to zero (avoids rare errors)
				TC_FilePath = dlg.FileName;
				TileConfig = new RSDKv5.Tileconfig();
				OriginalTileConfig = new RSDKv5.Tileconfig();
				RSDKvB.Tileconfig tcfOLD = new RSDKvB.Tileconfig(dlg.FileName);
				string tileBitmapPath = TC_FilePath.Replace("CollisionMasks.bin", "16x16tiles.gif"); //get the path to the stage's tileset
				LoadTileSet(new Bitmap(tileBitmapPath)); //load each 16x16 tile into the list
				TC_BitmapPath = tileBitmapPath;


				CollisionListImgA.Clear();
				CollisionListImgB.Clear();
				CollisionList.Images.Clear();

				for (int i = 0; i < 1024; i++)
				{
					CollisionListImgA.Add(tcfOLD.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
					CollisionListImgB.Add(tcfOLD.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));

					CollisionList.Images.Add(CollisionListImgA[i]);
					CollisionList.Images.Add(CollisionListImgB[i]);

					TileConfig.CollisionPath1[i].Collision = tcfOLD.CollisionPath1[i].Collision;
					TileConfig.CollisionPath1[i].HasCollision = tcfOLD.CollisionPath1[i].HasCollision;
					TileConfig.CollisionPath1[i].IsCeiling = tcfOLD.CollisionPath1[i].isCeiling;
					TileConfig.CollisionPath1[i].LWallAngle = tcfOLD.CollisionPath1[i].LWallAngle;
					TileConfig.CollisionPath1[i].CeilingAngle = tcfOLD.CollisionPath1[i].CeilingAngle;
					TileConfig.CollisionPath1[i].FloorAngle = tcfOLD.CollisionPath1[i].FloorAngle;
					TileConfig.CollisionPath1[i].Behaviour = 0;
					TileConfig.CollisionPath1[i].RWallAngle = tcfOLD.CollisionPath1[i].RWallAngle;

					TileConfig.CollisionPath2[i].Collision = tcfOLD.CollisionPath2[i].Collision;
					TileConfig.CollisionPath2[i].HasCollision = tcfOLD.CollisionPath2[i].HasCollision;
					TileConfig.CollisionPath2[i].IsCeiling = tcfOLD.CollisionPath2[i].isCeiling;
					TileConfig.CollisionPath2[i].LWallAngle = tcfOLD.CollisionPath2[i].LWallAngle;
					TileConfig.CollisionPath2[i].CeilingAngle = tcfOLD.CollisionPath2[i].CeilingAngle;
					TileConfig.CollisionPath2[i].FloorAngle = tcfOLD.CollisionPath2[i].FloorAngle;
					TileConfig.CollisionPath2[i].Behaviour = 0;
					TileConfig.CollisionPath2[i].RWallAngle = tcfOLD.CollisionPath2[i].RWallAngle;
				}
				if (CollisionList.SelectedIndex != CurrentCollisionMask - 1) CollisionList.SelectedIndex = CurrentCollisionMask - 1;
				CollisionList.Refresh();

				RefreshUI(); //update the UI
			}
			
		}
		public void saveUncompressedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			
			if (TC_FilePath != null) //Did we open a file?
			{
				TileConfig.WriteUnc(TC_FilePath);
			}
			else //if not then use Save As instead
			{
				saveAsUncompressedToolStripMenuItem_Click(null, e);
			}
			
		}
		public void saveAsUncompressedToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			
			SaveFileDialog dlg = new SaveFileDialog
			{
				Title = "Save Uncompressed As...",
				DefaultExt = ".bin",
				Filter = "RSDKv5 Tileconfig Files (*.bin)|*.bin"
			};

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TileConfig.WriteUnc(dlg.FileName); //Write Uncompressed
			}
			
		}
		private void exportImages_Click(object sender, RoutedEventArgs e)
		{
			Bitmap PathA = MergeImages(CollisionListImgA.ToArray());
			PathA.Save(Path.Combine(Methods.ProgramPaths.GetExecutingDirectoryName(), "PathA.gif"));

			Bitmap PathB = MergeImages(CollisionListImgB.ToArray());
			PathB.Save(Path.Combine(Methods.ProgramPaths.GetExecutingDirectoryName(), "PathB.gif"));
		}

		#endregion

		#region Bitmap Manipulation
		public System.Windows.Media.Color ColorConvertToMedia(System.Drawing.Color input)
		{
			return System.Windows.Media.Color.FromArgb(input.A, input.R, input.G, input.B);
		}

		public System.Drawing.Color ColorConvertToDrawing(System.Windows.Media.Color input)
		{
			return System.Drawing.Color.FromArgb(input.A, input.R, input.G, input.B);
		}
		public Bitmap MergeImages(Bitmap[] images)
		{
			Bitmap mergedImg = new Bitmap(16, 16384, images[0].PixelFormat);
			using (Graphics g = Graphics.FromImage(mergedImg))
			{
				for (int i = 0; i < images.Length; i++)
				{
					g.DrawImage(images[i], new Rectangle(0, 16 * i, images[i].Width, images[i].Height));
				}

			}

			return mergedImg;
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

		#endregion

		#region UI Refresh
		public void UpdateMirrorModeStatusLabel()
		{
			if (MirrorMode)
			{
				mirrorModeStatusLabel.Content = "Mirror Mode: ON";
			}
			else
			{
				mirrorModeStatusLabel.Content = "Mirror Mode: OFF";
			}
		}
		public void SetCollisionIndex(int index)
		{
			if (CollisionList.SelectedIndex != index) CollisionList.SelectedIndex = index;
			CollisionList.Refresh();

			RefreshUI(); //update the UI
		}
		private void RefreshCollision()
		{

			if (TC_FilePath != null)
			{
				CollisionList.Images.Clear();
				CollisionListImgA.Clear();
				CollisionListImgB.Clear();

				for (int i = 0; i < 1024; i++)
				{
					if (TileListSetting == 0)
					{
						CollisionListImgA.Add(TileConfig.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
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
					if (TileListSetting == 0)
					{
						CollisionListImgB.Add(TileConfig.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor));
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
		private void UnCheckModes()
		{
			IsChangingViewerMode = true;
			colllisionViewButton.IsChecked = false;
			tileViewButton.IsChecked = false;
			overlayViewButton.IsChecked = false;
			TilePicBox.Visible = false;
			CollisionPicBox.Visible = false;
			OverlayPicBox.Visible = false;
			PicBoxHost1Overlay.Visibility = Visibility.Hidden;
			PicBoxHost2Tile.Visibility = Visibility.Hidden;
			PicBoxHost3Collision.Visibility = Visibility.Hidden;

		}
		private void UncheckListViews()
		{
			LockRadioButtons = true;
			collisionViewRadioButton.IsChecked = false;
			tileViewRadioButton.IsChecked = false;
		}
		public void RefreshCollisionList()
		{
			this.InvokeIfRequired(() =>
			{
				if (TileConfig != null)
				{
					CollisionList.Images.Clear();

					if (!ShowPathB)
					{
						for (int i = 0; i < 1024; i++)
						{
							if (TileListSetting == 0)
							{
								CollisionList.Images.Add(CollisionListImgA[i]);
							}
							else
							{
								CollisionList.Images.Add(Tiles[i]);
							}
						}
					}
					else if (ShowPathB)
					{
						for (int i = 0; i < 1024; i++)
						{
							if (TileListSetting == 0)
							{
								CollisionList.Images.Add(CollisionListImgB[i]);
							}
							else
							{
								CollisionList.Images.Add(Tiles[i]);
							}
						}
					}
					if (CollisionList.SelectedIndex != CurrentCollisionMask) CollisionList.SelectedIndex = CurrentCollisionMask;
					CollisionList.Refresh();
				}
			}, System.Windows.Threading.DispatcherPriority.Normal);
		}
		public void RefreshPathA()
		{
			this.InvokeIfRequired(() =>
			{
				lb00.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[0];
				lb01.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[1];
				lb02.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[2];
				lb03.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[3];
				lb04.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[4];
				lb05.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[5];
				lb06.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[6];
				lb07.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[7];
				lb08.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[8];
				lb09.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[9];
				lb10.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[10];
				lb11.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[11];
				lb12.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[12];
				lb13.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[13];
				lb14.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[14];
				lb15.SelectedIndex = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[15];

				cb00.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[0];
				cb01.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[1];
				cb02.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[2];
				cb03.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[3];
				cb04.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[4];
				cb05.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[5];
				cb06.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[6];
				cb07.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[7];
				cb08.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[8];
				cb09.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[9];
				cb10.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[10];
				cb11.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[11];
				cb12.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[12];
				cb13.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[13];
				cb14.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[14];
				cb15.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[15];
			}, System.Windows.Threading.DispatcherPriority.Normal);

		}
		public void RefreshPathB()
		{
			this.InvokeIfRequired(() =>
			{
				lb00.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[0];
				lb01.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[1];
				lb02.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[2];
				lb03.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[3];
				lb04.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[4];
				lb05.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[5];
				lb06.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[6];
				lb07.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[7];
				lb08.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[8];
				lb09.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[9];
				lb10.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[10];
				lb11.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[11];
				lb12.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[12];
				lb13.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[13];
				lb14.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[14];
				lb15.SelectedIndex = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[15];

				cb00.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[0];
				cb01.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[1];
				cb02.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[2];
				cb03.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[3];
				cb04.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[4];
				cb05.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[5];
				cb06.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[6];
				cb07.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[7];
				cb08.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[8];
				cb09.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[9];
				cb10.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[10];
				cb11.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[11];
				cb12.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[12];
				cb13.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[13];
				cb14.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[14];
				cb15.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[15];
			}, System.Windows.Threading.DispatcherPriority.Normal);
		}
		public void DummyMethod()
		{
			this.InvokeIfRequired(() =>
			{
			}, System.Windows.Threading.DispatcherPriority.Normal);
		}
		public void SetEditorOnlyButtonsState(bool enabled)
		{
			this.InvokeIfRequired(() =>
			{
				FloorAngleNUD.IsEnabled = enabled;
				CeilingAngleNUD.IsEnabled = enabled;
				LWallAngleNUD.IsEnabled = enabled;
				BehaviourNUD.IsEnabled = enabled;
				RWallAngleNUD.IsEnabled = enabled;
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

				ClassicMode.IsEnabled = enabled;

				if (swapPathButton != null) swapPathButton.IsEnabled = enabled;
				GotoNUD.IsEnabled = enabled;
			}, System.Windows.Threading.DispatcherPriority.Normal);

		}
		public void UpdateThemeColors()
		{
			this.InvokeIfRequired(() =>
			{
				this.CollisionList.Refresh();
				this.CollisionList.vScrollBar1Host.Refresh();
				this.CollisionList.vScrollBar1Host.scroller.Refresh();
				System.Drawing.Color ListBackColor = Methods.Internal.Theming.ThemeBrush1;
				this.CollisionList.BackColor = ListBackColor;
			}, System.Windows.Threading.DispatcherPriority.Normal);

		}
		public void UpdatePaths()
		{
			this.InvokeIfRequired(() =>
			{
				if (TileConfig != null)
				{
					if (!ShowPathB) //if we are showing Path A then refresh the values accordingly
					{
						FloorAngleNUD.Value = TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle;
						CeilingAngleNUD.Value = TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle;
						LWallAngleNUD.Value = TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle;
						RWallAngleNUD.Value = TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle;
						BehaviourNUD.Value = TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour;
						IsCeilingButton.IsChecked = TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling;

						RefreshPathA();
					}
					if (ShowPathB) //if we are showing Path B then refresh the values accordingly
					{
						FloorAngleNUD.Value = TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle;
						CeilingAngleNUD.Value = TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle;
						LWallAngleNUD.Value = TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle;
						RWallAngleNUD.Value = TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle;
						BehaviourNUD.Value = TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour;
						IsCeilingButton.IsChecked = TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling;

						RefreshPathB();
					}
				}
			}, System.Windows.Threading.DispatcherPriority.Normal);
		}
		public void UpdatePreviews()
		{
			this.InvokeIfRequired(() =>
			{
				if (TileConfig != null)
				{
					TilePicBox.Image = ResizeBitmap(Tiles[CurrentCollisionMask], 96, 96); //update the tile preview 
					Bitmap Overlaypic = new Bitmap(16, 16);
					Bitmap Collisionpic = new Bitmap(16, 16);

					if (!ShowPathB)
					{
						Collisionpic = TileConfig.CollisionPath1[CurrentCollisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionColor);
						CollisionPicBox.Image = Collisionpic;
						Overlaypic = TileConfig.CollisionPath1[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor, Tiles[CurrentCollisionMask]);
					}
					else
					{
						Collisionpic = TileConfig.CollisionPath2[CurrentCollisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 255, 0));
						CollisionPicBox.Image = Collisionpic;
						Overlaypic = TileConfig.CollisionPath2[CurrentCollisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionColor, Tiles[CurrentCollisionMask]);
					}

					OverlayPicBox.Image = Overlaypic;
					OverlayPicBox.Image = ResizeBitmap(Overlaypic, 96, 96);
					CollisionPicBox.Image = ResizeBitmap(new Bitmap(CollisionPicBox.Image), 96, 96);
				}
			}, System.Windows.Threading.DispatcherPriority.Normal);
		}

		/*     
		CollisionEditor.Instance.InvokeIfRequired(() =>
        {
        }, System.Windows.Threading.DispatcherPriority.Normal);
		 */


		private async void RunTask(Action action)
		{
			await System.Threading.Tasks.Task.Run(action);
		}

		public void RefreshUI()
		{
			RunTask(() => RefreshListBoxItems());
			RunTask(() => Methods.Internal.UserInterface.TileManiac_UpdateMenuItems());
			RunTask(() => UpdateThemeColors());
			RunTask(() => UpdatePaths());
			RunTask(() => RefreshCollisionList());
			RunTask(() => SetEditorOnlyButtonsState(TileConfig != null));
			RunTask(() => UpdateDegreeValuesDisplay());
			RunTask(() => UpdatePreviews());
		}
		public void UpdateDegreeValuesDisplay()
		{
			this.InvokeIfRequired(() =>
			{
				if (TileConfig != null)
				{
					if (ShowPathB)
					{

						FloorAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle).ToString();
						CeilingAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle).ToString();
						LWallAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle).ToString();
						RWallAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle).ToString();
						BehaviourHexBox.Text = GetHexValueString(TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour).ToString();

						FloorAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle).ToString() + "º";
						CeilingAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle).ToString() + "º";
						LWallAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle).ToString() + "º";
						RWallAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle).ToString() + "º";
						BehaviourDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour).ToString() + "º";
					}
					else
					{
						FloorAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle).ToString();
						CeilingAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle).ToString();
						LWallAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle).ToString();
						RWallAngleHexBox.Text = GetHexValueString(TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle).ToString();
						BehaviourHexBox.Text = GetHexValueString(TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour).ToString();

						FloorAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle).ToString() + "º";
						CeilingAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle).ToString() + "º";
						LWallAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle).ToString() + "º";
						RWallAngleDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle).ToString() + "º";
						BehaviourDegreesNUD.Text = GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour).ToString() + "º";
					}
					if (this.GraphicPanel != null) this.GraphicPanel.Render();
				}
				else
				{
					FloorAngleHexBox.Text = "0xFF";
					CeilingAngleHexBox.Text = "0xFF";
					LWallAngleHexBox.Text = "0xFF";
					RWallAngleHexBox.Text = "0xFF";
					BehaviourHexBox.Text = "0xFF";

					FloorAngleDegreesNUD.Text = "0º";
					CeilingAngleDegreesNUD.Text = "0º";
					LWallAngleDegreesNUD.Text = "0º";
					RWallAngleDegreesNUD.Text = "0º";
					BehaviourDegreesNUD.Text = "0º";
				}
			}, System.Windows.Threading.DispatcherPriority.Normal);
		}

		#endregion

		#region List Box Items Color Refresh 

		private void GetListBoxItemColor(ref ListBoxItem item, int row, int col)
		{
			Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush> Brushes = new Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush>(NormalText, System.Windows.Media.Brushes.Transparent);
			if (TileConfig != null)
			{
				if (!ShowPathB)
				{
					var value = TileConfig.CollisionPath1[CurrentCollisionMask].Collision[row];
					bool hasCollision = TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[row];
					bool isCelling = TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling;
					Brushes =  GetBrush(value, hasCollision, isCelling, row, col, CurrentCollisionMask);
				}
				else
				{
					var value = TileConfig.CollisionPath2[CurrentCollisionMask].Collision[row];
					bool hasCollision = TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[row];
					bool isCelling = TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling;
					Brushes = GetBrush(value, hasCollision, isCelling, row, col, CurrentCollisionMask);
				}
			}

			item.Foreground = Brushes.Item1;
			item.Background = Brushes.Item2;


			Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush> GetBrush(int value, bool hasCollision, bool isCelling, int x, int y, int index)
			{
				if (isCelling)
				{
					if (value <= col && hasCollision) return GetAntiBrush(x, y, index);
					else if (!hasCollision) return GetAntiBrush(x, y, index);
					else return GetPositiveBrush();
				}
				else
				{
					if (value <= col && hasCollision) return GetPositiveBrush();
					else return GetAntiBrush(x, y, index);
				}



			}

			Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush> GetPositiveBrush()
			{
				var background = (CollisionColorBrush as SolidColorBrush).Color;
				var foreground = ColorExt.ToSWMColor(Extensions.Extensions.ContrastColor(ColorExt.ToSDColor(background)));
				return new Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush>(new SolidColorBrush(foreground), new SolidColorBrush(background));
			}

			Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush> GetAntiBrush(int x, int y, int index)
			{
				bool useTileAsBackground = (CurrentViewerSetting == 2);
				var background = (useTileAsBackground ? ColorExt.ToSWMColor(Tiles[index].GetPixel(x, y)) : (AntiCollisionColorBrush as SolidColorBrush).Color);
				var foreground = ColorExt.ToSWMColor(Extensions.Extensions.ContrastColor(ColorExt.ToSDColor(background)));
				return new Tuple<System.Windows.Media.Brush, System.Windows.Media.Brush>(new SolidColorBrush(foreground), new SolidColorBrush(background));
			}
		}
		public void RefreshListBoxItems()
		{
			this.InvokeIfRequired(() =>
			{
				GetListBoxItemColor(ref CB_00, 0, 0);
				GetListBoxItemColor(ref CB_01, 0, 1);
				GetListBoxItemColor(ref CB_02, 0, 2);
				GetListBoxItemColor(ref CB_03, 0, 3);
				GetListBoxItemColor(ref CB_04, 0, 4);
				GetListBoxItemColor(ref CB_05, 0, 5);
				GetListBoxItemColor(ref CB_06, 0, 6);
				GetListBoxItemColor(ref CB_07, 0, 7);
				GetListBoxItemColor(ref CB_08, 0, 8);
				GetListBoxItemColor(ref CB_09, 0, 9);
				GetListBoxItemColor(ref CB_0A, 0, 10);
				GetListBoxItemColor(ref CB_0B, 0, 11);
				GetListBoxItemColor(ref CB_0C, 0, 12);
				GetListBoxItemColor(ref CB_0D, 0, 13);
				GetListBoxItemColor(ref CB_0E, 0, 14);
				GetListBoxItemColor(ref CB_0F, 0, 15);

				GetListBoxItemColor(ref CB_10, 1, 0);
				GetListBoxItemColor(ref CB_11, 1, 1);
				GetListBoxItemColor(ref CB_12, 1, 2);
				GetListBoxItemColor(ref CB_13, 1, 3);
				GetListBoxItemColor(ref CB_14, 1, 4);
				GetListBoxItemColor(ref CB_15, 1, 5);
				GetListBoxItemColor(ref CB_16, 1, 6);
				GetListBoxItemColor(ref CB_17, 1, 7);
				GetListBoxItemColor(ref CB_18, 1, 8);
				GetListBoxItemColor(ref CB_19, 1, 9);
				GetListBoxItemColor(ref CB_1A, 1, 10);
				GetListBoxItemColor(ref CB_1B, 1, 11);
				GetListBoxItemColor(ref CB_1C, 1, 12);
				GetListBoxItemColor(ref CB_1D, 1, 13);
				GetListBoxItemColor(ref CB_1E, 1, 14);
				GetListBoxItemColor(ref CB_1F, 1, 15);

				GetListBoxItemColor(ref CB_20, 2, 0);
				GetListBoxItemColor(ref CB_21, 2, 1);
				GetListBoxItemColor(ref CB_22, 2, 2);
				GetListBoxItemColor(ref CB_23, 2, 3);
				GetListBoxItemColor(ref CB_24, 2, 4);
				GetListBoxItemColor(ref CB_25, 2, 5);
				GetListBoxItemColor(ref CB_26, 2, 6);
				GetListBoxItemColor(ref CB_27, 2, 7);
				GetListBoxItemColor(ref CB_28, 2, 8);
				GetListBoxItemColor(ref CB_29, 2, 9);
				GetListBoxItemColor(ref CB_2A, 2, 10);
				GetListBoxItemColor(ref CB_2B, 2, 11);
				GetListBoxItemColor(ref CB_2C, 2, 12);
				GetListBoxItemColor(ref CB_2D, 2, 13);
				GetListBoxItemColor(ref CB_2E, 2, 14);
				GetListBoxItemColor(ref CB_2F, 2, 15);

				GetListBoxItemColor(ref CB_30, 3, 0);
				GetListBoxItemColor(ref CB_31, 3, 1);
				GetListBoxItemColor(ref CB_32, 3, 2);
				GetListBoxItemColor(ref CB_33, 3, 3);
				GetListBoxItemColor(ref CB_34, 3, 4);
				GetListBoxItemColor(ref CB_35, 3, 5);
				GetListBoxItemColor(ref CB_36, 3, 6);
				GetListBoxItemColor(ref CB_37, 3, 7);
				GetListBoxItemColor(ref CB_38, 3, 8);
				GetListBoxItemColor(ref CB_39, 3, 9);
				GetListBoxItemColor(ref CB_3A, 3, 10);
				GetListBoxItemColor(ref CB_3B, 3, 11);
				GetListBoxItemColor(ref CB_3C, 3, 12);
				GetListBoxItemColor(ref CB_3D, 3, 13);
				GetListBoxItemColor(ref CB_3E, 3, 14);
				GetListBoxItemColor(ref CB_3F, 3, 15);

				GetListBoxItemColor(ref CB_40, 4, 0);
				GetListBoxItemColor(ref CB_41, 4, 1);
				GetListBoxItemColor(ref CB_42, 4, 2);
				GetListBoxItemColor(ref CB_43, 4, 3);
				GetListBoxItemColor(ref CB_44, 4, 4);
				GetListBoxItemColor(ref CB_45, 4, 5);
				GetListBoxItemColor(ref CB_46, 4, 6);
				GetListBoxItemColor(ref CB_47, 4, 7);
				GetListBoxItemColor(ref CB_48, 4, 8);
				GetListBoxItemColor(ref CB_49, 4, 9);
				GetListBoxItemColor(ref CB_4A, 4, 10);
				GetListBoxItemColor(ref CB_4B, 4, 11);
				GetListBoxItemColor(ref CB_4C, 4, 12);
				GetListBoxItemColor(ref CB_4D, 4, 13);
				GetListBoxItemColor(ref CB_4E, 4, 14);
				GetListBoxItemColor(ref CB_4F, 4, 15);

				GetListBoxItemColor(ref CB_50, 5, 0);
				GetListBoxItemColor(ref CB_51, 5, 1);
				GetListBoxItemColor(ref CB_52, 5, 2);
				GetListBoxItemColor(ref CB_53, 5, 3);
				GetListBoxItemColor(ref CB_54, 5, 4);
				GetListBoxItemColor(ref CB_55, 5, 5);
				GetListBoxItemColor(ref CB_56, 5, 6);
				GetListBoxItemColor(ref CB_57, 5, 7);
				GetListBoxItemColor(ref CB_58, 5, 8);
				GetListBoxItemColor(ref CB_59, 5, 9);
				GetListBoxItemColor(ref CB_5A, 5, 10);
				GetListBoxItemColor(ref CB_5B, 5, 11);
				GetListBoxItemColor(ref CB_5C, 5, 12);
				GetListBoxItemColor(ref CB_5D, 5, 13);
				GetListBoxItemColor(ref CB_5E, 5, 14);
				GetListBoxItemColor(ref CB_5F, 5, 15);

				GetListBoxItemColor(ref CB_60, 6, 0);
				GetListBoxItemColor(ref CB_61, 6, 1);
				GetListBoxItemColor(ref CB_62, 6, 2);
				GetListBoxItemColor(ref CB_63, 6, 3);
				GetListBoxItemColor(ref CB_64, 6, 4);
				GetListBoxItemColor(ref CB_65, 6, 5);
				GetListBoxItemColor(ref CB_66, 6, 6);
				GetListBoxItemColor(ref CB_67, 6, 7);
				GetListBoxItemColor(ref CB_68, 6, 8);
				GetListBoxItemColor(ref CB_69, 6, 9);
				GetListBoxItemColor(ref CB_6A, 6, 10);
				GetListBoxItemColor(ref CB_6B, 6, 11);
				GetListBoxItemColor(ref CB_6C, 6, 12);
				GetListBoxItemColor(ref CB_6D, 6, 13);
				GetListBoxItemColor(ref CB_6E, 6, 14);
				GetListBoxItemColor(ref CB_6F, 6, 15);

				GetListBoxItemColor(ref CB_70, 7, 0);
				GetListBoxItemColor(ref CB_71, 7, 1);
				GetListBoxItemColor(ref CB_72, 7, 2);
				GetListBoxItemColor(ref CB_73, 7, 3);
				GetListBoxItemColor(ref CB_74, 7, 4);
				GetListBoxItemColor(ref CB_75, 7, 5);
				GetListBoxItemColor(ref CB_76, 7, 6);
				GetListBoxItemColor(ref CB_77, 7, 7);
				GetListBoxItemColor(ref CB_78, 7, 8);
				GetListBoxItemColor(ref CB_79, 7, 9);
				GetListBoxItemColor(ref CB_7A, 7, 10);
				GetListBoxItemColor(ref CB_7B, 7, 11);
				GetListBoxItemColor(ref CB_7C, 7, 12);
				GetListBoxItemColor(ref CB_7D, 7, 13);
				GetListBoxItemColor(ref CB_7E, 7, 14);
				GetListBoxItemColor(ref CB_7F, 7, 15);

				GetListBoxItemColor(ref CB_80, 8, 0);
				GetListBoxItemColor(ref CB_81, 8, 1);
				GetListBoxItemColor(ref CB_82, 8, 2);
				GetListBoxItemColor(ref CB_83, 8, 3);
				GetListBoxItemColor(ref CB_84, 8, 4);
				GetListBoxItemColor(ref CB_85, 8, 5);
				GetListBoxItemColor(ref CB_86, 8, 6);
				GetListBoxItemColor(ref CB_87, 8, 7);
				GetListBoxItemColor(ref CB_88, 8, 8);
				GetListBoxItemColor(ref CB_89, 8, 9);
				GetListBoxItemColor(ref CB_8A, 8, 10);
				GetListBoxItemColor(ref CB_8B, 8, 11);
				GetListBoxItemColor(ref CB_8C, 8, 12);
				GetListBoxItemColor(ref CB_8D, 8, 13);
				GetListBoxItemColor(ref CB_8E, 8, 14);
				GetListBoxItemColor(ref CB_8F, 8, 15);

				GetListBoxItemColor(ref CB_90, 9, 0);
				GetListBoxItemColor(ref CB_91, 9, 1);
				GetListBoxItemColor(ref CB_92, 9, 2);
				GetListBoxItemColor(ref CB_93, 9, 3);
				GetListBoxItemColor(ref CB_94, 9, 4);
				GetListBoxItemColor(ref CB_95, 9, 5);
				GetListBoxItemColor(ref CB_96, 9, 6);
				GetListBoxItemColor(ref CB_97, 9, 7);
				GetListBoxItemColor(ref CB_98, 9, 8);
				GetListBoxItemColor(ref CB_99, 9, 9);
				GetListBoxItemColor(ref CB_9A, 9, 10);
				GetListBoxItemColor(ref CB_9B, 9, 11);
				GetListBoxItemColor(ref CB_9C, 9, 12);
				GetListBoxItemColor(ref CB_9D, 9, 13);
				GetListBoxItemColor(ref CB_9E, 9, 14);
				GetListBoxItemColor(ref CB_9F, 9, 15);

				GetListBoxItemColor(ref CB_A0, 10, 0);
				GetListBoxItemColor(ref CB_A1, 10, 1);
				GetListBoxItemColor(ref CB_A2, 10, 2);
				GetListBoxItemColor(ref CB_A3, 10, 3);
				GetListBoxItemColor(ref CB_A4, 10, 4);
				GetListBoxItemColor(ref CB_A5, 10, 5);
				GetListBoxItemColor(ref CB_A6, 10, 6);
				GetListBoxItemColor(ref CB_A7, 10, 7);
				GetListBoxItemColor(ref CB_A8, 10, 8);
				GetListBoxItemColor(ref CB_A9, 10, 9);
				GetListBoxItemColor(ref CB_AA, 10, 10);
				GetListBoxItemColor(ref CB_AB, 10, 11);
				GetListBoxItemColor(ref CB_AC, 10, 12);
				GetListBoxItemColor(ref CB_AD, 10, 13);
				GetListBoxItemColor(ref CB_AE, 10, 14);
				GetListBoxItemColor(ref CB_AF, 10, 15);

				GetListBoxItemColor(ref CB_B0, 11, 0);
				GetListBoxItemColor(ref CB_B1, 11, 1);
				GetListBoxItemColor(ref CB_B2, 11, 2);
				GetListBoxItemColor(ref CB_B3, 11, 3);
				GetListBoxItemColor(ref CB_B4, 11, 4);
				GetListBoxItemColor(ref CB_B5, 11, 5);
				GetListBoxItemColor(ref CB_B6, 11, 6);
				GetListBoxItemColor(ref CB_B7, 11, 7);
				GetListBoxItemColor(ref CB_B8, 11, 8);
				GetListBoxItemColor(ref CB_B9, 11, 9);
				GetListBoxItemColor(ref CB_BA, 11, 10);
				GetListBoxItemColor(ref CB_BB, 11, 11);
				GetListBoxItemColor(ref CB_BC, 11, 12);
				GetListBoxItemColor(ref CB_BD, 11, 13);
				GetListBoxItemColor(ref CB_BE, 11, 14);
				GetListBoxItemColor(ref CB_BF, 11, 15);

				GetListBoxItemColor(ref CB_C0, 12, 0);
				GetListBoxItemColor(ref CB_C1, 12, 1);
				GetListBoxItemColor(ref CB_C2, 12, 2);
				GetListBoxItemColor(ref CB_C3, 12, 3);
				GetListBoxItemColor(ref CB_C4, 12, 4);
				GetListBoxItemColor(ref CB_C5, 12, 5);
				GetListBoxItemColor(ref CB_C6, 12, 6);
				GetListBoxItemColor(ref CB_C7, 12, 7);
				GetListBoxItemColor(ref CB_C8, 12, 8);
				GetListBoxItemColor(ref CB_C9, 12, 9);
				GetListBoxItemColor(ref CB_CA, 12, 10);
				GetListBoxItemColor(ref CB_CB, 12, 11);
				GetListBoxItemColor(ref CB_CC, 12, 12);
				GetListBoxItemColor(ref CB_CD, 12, 13);
				GetListBoxItemColor(ref CB_CE, 12, 14);
				GetListBoxItemColor(ref CB_CF, 12, 15);

				GetListBoxItemColor(ref CB_D0, 13, 0);
				GetListBoxItemColor(ref CB_D1, 13, 1);
				GetListBoxItemColor(ref CB_D2, 13, 2);
				GetListBoxItemColor(ref CB_D3, 13, 3);
				GetListBoxItemColor(ref CB_D4, 13, 4);
				GetListBoxItemColor(ref CB_D5, 13, 5);
				GetListBoxItemColor(ref CB_D6, 13, 6);
				GetListBoxItemColor(ref CB_D7, 13, 7);
				GetListBoxItemColor(ref CB_D8, 13, 8);
				GetListBoxItemColor(ref CB_D9, 13, 9);
				GetListBoxItemColor(ref CB_DA, 13, 10);
				GetListBoxItemColor(ref CB_DB, 13, 11);
				GetListBoxItemColor(ref CB_DC, 13, 12);
				GetListBoxItemColor(ref CB_DD, 13, 13);
				GetListBoxItemColor(ref CB_DE, 13, 14);
				GetListBoxItemColor(ref CB_DF, 13, 15);

				GetListBoxItemColor(ref CB_E0, 14, 0);
				GetListBoxItemColor(ref CB_E1, 14, 1);
				GetListBoxItemColor(ref CB_E2, 14, 2);
				GetListBoxItemColor(ref CB_E3, 14, 3);
				GetListBoxItemColor(ref CB_E4, 14, 4);
				GetListBoxItemColor(ref CB_E5, 14, 5);
				GetListBoxItemColor(ref CB_E6, 14, 6);
				GetListBoxItemColor(ref CB_E7, 14, 7);
				GetListBoxItemColor(ref CB_E8, 14, 8);
				GetListBoxItemColor(ref CB_E9, 14, 9);
				GetListBoxItemColor(ref CB_EA, 14, 10);
				GetListBoxItemColor(ref CB_EB, 14, 11);
				GetListBoxItemColor(ref CB_EC, 14, 12);
				GetListBoxItemColor(ref CB_ED, 14, 13);
				GetListBoxItemColor(ref CB_EE, 14, 14);
				GetListBoxItemColor(ref CB_EF, 14, 15);

				GetListBoxItemColor(ref CB_F0, 15, 0);
				GetListBoxItemColor(ref CB_F1, 15, 1);
				GetListBoxItemColor(ref CB_F2, 15, 2);
				GetListBoxItemColor(ref CB_F3, 15, 3);
				GetListBoxItemColor(ref CB_F4, 15, 4);
				GetListBoxItemColor(ref CB_F5, 15, 5);
				GetListBoxItemColor(ref CB_F6, 15, 6);
				GetListBoxItemColor(ref CB_F7, 15, 7);
				GetListBoxItemColor(ref CB_F8, 15, 8);
				GetListBoxItemColor(ref CB_F9, 15, 9);
				GetListBoxItemColor(ref CB_FA, 15, 10);
				GetListBoxItemColor(ref CB_FB, 15, 11);
				GetListBoxItemColor(ref CB_FC, 15, 12);
				GetListBoxItemColor(ref CB_FD, 15, 13);
				GetListBoxItemColor(ref CB_FE, 15, 14);
				GetListBoxItemColor(ref CB_FF, 15, 15);
			}, System.Windows.Threading.DispatcherPriority.Normal);

		}

		#endregion

		#region Current Collision Events

		private void FloorAngleNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (TileConfig != null)
			{
				if (MirrorMode)
				{

					TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle = (byte)FloorAngleNUD.Value; //Set Slope angle for Path B
					TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle = (byte)FloorAngleNUD.Value; //Set Slope angle for Path A
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle = (byte)FloorAngleNUD.Value; //Set Slope angle for Path A

					}
					else
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle = (byte)FloorAngleNUD.Value; //Set Slope angle for Path B
					}
				}

				UpdateDegreeValuesDisplay();
			}
		}
		private void CeilingAngleNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (TileConfig != null)
			{
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle = (byte)CeilingAngleNUD.Value; //Set the Physics for Path A
					TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle = (byte)CeilingAngleNUD.Value; //Set the Physics for Path B
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle = (byte)CeilingAngleNUD.Value; //Set the Physics for Path A
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle = (byte)CeilingAngleNUD.Value; //Set the Physics for Path B
					}
				}

				UpdateDegreeValuesDisplay();
			}
		}
		private void LWallAngleNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (TileConfig != null)
			{
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle = (byte)LWallAngleNUD.Value; //Set the Momentum value for Path A
					TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle = (byte)LWallAngleNUD.Value; //Set the Momentum value for Path B
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle = (byte)LWallAngleNUD.Value; //Set the Momentum value for Path A
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle = (byte)LWallAngleNUD.Value; //Set the Momentum value for Path B
					}
				}

				UpdateDegreeValuesDisplay();
			}
		}
		private void RWallAngleNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (TileConfig != null)
			{
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle = (byte)RWallAngleNUD.Value; //Set the RWallAngle value for Path A
					TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle = (byte)RWallAngleNUD.Value; //Set the RWallAngle value for Path B
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle = (byte)RWallAngleNUD.Value; //Set the RWallAngle value for Path A
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle = (byte)RWallAngleNUD.Value; //Set the RWallAngle value for Path B
					}
				}

				UpdateDegreeValuesDisplay();
			}
		}
		private void BehaviourNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (TileConfig != null)
			{
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour = (byte)BehaviourNUD.Value; //Set the "Special" value for Path A
					TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour = (byte)BehaviourNUD.Value; //Set the "Special" value for Path B
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour = (byte)BehaviourNUD.Value; //Set the "Special" value for Path A
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour = (byte)BehaviourNUD.Value; //Set the "Special" value for Path B
					}
				}

				UpdateDegreeValuesDisplay();
			}
		}
		private void IsCeilingButton_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (TileConfig != null)
			{
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path A
					TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path B
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path A
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling = IsCeilingButton.IsChecked.Value; //Set the "IsCeiling" Value for Path B
					}
				}

				RefreshUI();
			}
		}

		#endregion

		#region Collision Viewer / List Events

		private void GotoNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (TileConfig != null)
			{
				CurrentCollisionMask = (int)GotoNUD.Value;

			}

			if (e.Source == GotoNUD) RefreshUI();
		}
		private void CollisionList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CollisionList.SelectedIndex >= 0)
			{
				CurrentCollisionMask = CollisionList.SelectedIndex;
				GotoNUD.Value = CollisionList.SelectedIndex;
			}
			RefreshUI();
		}
		private void radioButton1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!IsChangingViewerMode)
			{
				UnCheckModes();
				CurrentViewerSetting = 0;
				Properties.Settings.MyDefaults.TileManiacRenderViewerSetting = 0;
				colllisionViewButton.IsChecked = true;
				CollisionPicBox.Visible = true;
				PicBoxHost3Collision.Visibility = Visibility.Visible;
				Classes.Options.DefaultPrefrences.Save();
				IsChangingViewerMode = false;
				RefreshUI();

			}
		}
		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!IsChangingViewerMode)
			{
				UnCheckModes();
				tileViewButton.IsChecked = true;
				CurrentViewerSetting = 1;
				Properties.Settings.MyDefaults.TileManiacRenderViewerSetting = 1;
				Classes.Options.DefaultPrefrences.Save();
				PicBoxHost2Tile.Visibility = Visibility.Visible;
				TilePicBox.Visible = true;
				IsChangingViewerMode = false;
				RefreshUI();

			}
		}
		private void radioButton3_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!IsChangingViewerMode)
			{
				UnCheckModes();
				CurrentViewerSetting = 2;
				Properties.Settings.MyDefaults.TileManiacRenderViewerSetting = 2;
				Classes.Options.DefaultPrefrences.Save();
				overlayViewButton.IsChecked = true;
				PicBoxHost1Overlay.Visibility = Visibility.Visible;
				OverlayPicBox.Visible = true;
				IsChangingViewerMode = false;
				RefreshUI();

			}
		}
		private void tileViewRadioButton_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (LockRadioButtons == false)
			{
				UncheckListViews();
				TileListSetting = 1;
				Properties.Settings.MyDefaults.TileManiacListSetting = 1;
				Classes.Options.DefaultPrefrences.Save();
				tileViewRadioButton.IsChecked = true;
				LockRadioButtons = false;
				RefreshCollision();
				RefreshUI();
			}
		}
		private void collisionViewRadioButton_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (LockRadioButtons == false)
			{
				UncheckListViews();
				TileListSetting = 0;
				Properties.Settings.MyDefaults.TileManiacListSetting = 0;
				Classes.Options.DefaultPrefrences.Save();
				collisionViewRadioButton.IsChecked = true;
				LockRadioButtons = false;
				RefreshCollision();
				RefreshUI();
			}
		}

		#endregion

		#region Tool Toggle Events

		private void SetPathBState()
		{
			//Do we want to show Path B's Collision Masks instead of Path A's ones?
			if (!ShowPathB)
			{
				ShowPathB = showPathBToolStripMenuItem.IsChecked = true;
				VPLabel.Content = "Currently Viewing: Path B";
				RefreshUI();
			}
			else if (ShowPathB)
			{
				ShowPathB = showPathBToolStripMenuItem.IsChecked = false;
				VPLabel.Content = "Currently Viewing: Path A";
				RefreshUI();
			}
		}
		private void SetMirrorPathsState()
		{
			if (mirrorPathsToolStripMenuItem1.IsChecked) MirrorMode = true;
			else MirrorMode = false;

			Properties.Settings.MyDefaults.TileManiacMirrorMode = MirrorMode;
			Classes.Options.DefaultPrefrences.Save();

			UpdateMirrorModeStatusLabel();
		}
		private void SetShowGridState()
		{
			if (showGridToolStripMenuItem.IsChecked) ShowGrid = true;
			else ShowGrid = false;

			Properties.Settings.MyDefaults.TileManiacShowGrid = ShowGrid;
			Classes.Options.DefaultPrefrences.Save();

			RefreshUI();
		}
		private void FlipTileH()
		{
			if (AllowFlipPrompt())
			{
				Bitmap tile = Tiles[CurrentCollisionMask];
				tile.RotateFlip(RotateFlipType.RotateNoneFlipX);
				tile = Tiles[CurrentCollisionMask];
				HasImageBeenModified = true;

				Bitmap indexedTile = IndexedTiles[CurrentCollisionMask];
				indexedTile.RotateFlip(RotateFlipType.RotateNoneFlipX);
				indexedTile = IndexedTiles[CurrentCollisionMask];

				RefreshUI();
			}
		}
		private void FlipTileV()
		{
			if (AllowFlipPrompt())
			{
				Bitmap tile = Tiles[CurrentCollisionMask];
				tile.RotateFlip(RotateFlipType.RotateNoneFlipY);
				tile = Tiles[CurrentCollisionMask];
				HasImageBeenModified = true;

				Bitmap indexedTile = IndexedTiles[CurrentCollisionMask];
				indexedTile.RotateFlip(RotateFlipType.RotateNoneFlipY);
				indexedTile = IndexedTiles[CurrentCollisionMask];

				RefreshUI();
			}
		}

        #endregion

        #region Collision Mask Events

        private void lb_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			int row = GetLBSender(sender);
			if (TileConfig != null && row != -1)
			{
				System.Windows.Controls.ListBox lb = (System.Windows.Controls.ListBox)sender;
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].Collision[row] = (byte)lb.SelectedIndex;
					CollisionListImgA[CurrentCollisionMask] = TileConfig.CollisionPath1[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					TileConfig.CollisionPath2[CurrentCollisionMask].Collision[row] = (byte)lb.SelectedIndex;
					CollisionListImgB[CurrentCollisionMask] = TileConfig.CollisionPath2[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					CollisionList.Refresh();
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].Collision[row] = (byte)lb.SelectedIndex;
						CollisionListImgA[CurrentCollisionMask] = TileConfig.CollisionPath1[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].Collision[row] = (byte)lb.SelectedIndex;
						CollisionListImgB[CurrentCollisionMask] = TileConfig.CollisionPath2[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
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
			if (TileConfig != null && box != -1)
			{
				System.Windows.Controls.CheckBox cb = (System.Windows.Controls.CheckBox)sender;
				if (MirrorMode)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[box] = cb.IsChecked.Value;
					CollisionListImgA[CurrentCollisionMask] = TileConfig.CollisionPath1[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[box] = cb.IsChecked.Value;
					CollisionListImgB[CurrentCollisionMask] = TileConfig.CollisionPath2[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
					CollisionList.Refresh();
				}
				else
				{
					if (!ShowPathB)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision[box] = cb.IsChecked.Value;
						CollisionListImgA[CurrentCollisionMask] = TileConfig.CollisionPath1[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
					if (ShowPathB)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision[box] = cb.IsChecked.Value;
						CollisionListImgB[CurrentCollisionMask] = TileConfig.CollisionPath2[CurrentCollisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), CollisionColor);
						CollisionList.Refresh();
					}
				}


				RefreshUI();
			}
		}
		public void LB_Scrolling(object sender, System.Windows.Input.MouseWheelEventArgs e)
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
		private void CB_MouseInteraction(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (sender != null && sender is ListBoxItem)
			{
				bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed;
				ListBoxItem item = sender as ListBoxItem;
				if (item.IsMouseOver && mouseIsDown)
				{
					if (item.Parent != null && item.Parent is System.Windows.Controls.ListBox)
					{
						System.Windows.Controls.ListBox itemHost = item.Parent as System.Windows.Controls.ListBox;
						int index = itemHost.Items.IndexOf(item);
						itemHost.SelectedIndex = index;
						itemHost.ReleaseMouseCapture();
					}

				}
				item.ReleaseMouseCapture();

			}
			RefreshListBoxItems();
		}
		private void CB_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			CB_MouseInteraction(sender, e);
		}
		private void CB_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			CB_MouseInteraction(sender, e);
		}
		private void cb_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (IsMouseLDown || IsMouseRDown)
			{
				if (IsMouseLDown)
				{
					SetCBCheckbox(sender, true);
				}
				else if (IsMouseRDown)
				{
					SetCBCheckbox(sender, false);
				}

			}
		}
		private void cb_MouseHover(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (IsMouseLDown || IsMouseRDown)
			{
				if (IsMouseLDown)
				{
					SetCBCheckbox(sender, true);
				}
				else if (IsMouseRDown)
				{
					SetCBCheckbox(sender, false);
				}

			}
		}
		private void cb_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			RefreshUI();
		}
		private void cb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (IsMouseLDown || IsMouseRDown)
			{
				if (IsMouseLDown)
				{
					SetCBCheckbox(sender, true);
				}
				else if (IsMouseRDown)
				{
					SetCBCheckbox(sender, false);
				}

			}

		}
		private void SetCBCheckbox(object sender, bool state = false)
		{
			if (sender != null && sender is System.Windows.Controls.CheckBox)
			{
				System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
				cb.IsChecked = state;
				cb.ReleaseMouseCapture();
			}
		}

		#endregion

		#region Misc Methods

		private string GetHexValueString(byte hex_angle)
		{
			string hexValue = String.Format("{0,2:X}", hex_angle);
			return "0x" + hexValue.Replace(" ", "0");
		}
		private double GetAngleValue(byte hex_angle)
		{
			return Math.Round(((256 - hex_angle) * 1.40625), 2);
		}
		private void ShowError(string message, string title = "Error!")
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
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
		private bool AllowFlipPrompt()
		{
			if (!IsIndexedImageLoaded)
			{
				MessageBoxResult result = MessageBox.Show("To flip the tile, we have to load an indexed version of the image first. This may take some time. Would you like to continue?", "Create Indexed Image", MessageBoxButton.YesNo, MessageBoxImage.Information);
				if (result == MessageBoxResult.Yes)
				{
					LoadTileSet(new Bitmap(TC_BitmapPath), true);
					IsIndexedImageLoaded = true;
					return true;
				}
				else return false;
			}
			else return true;

		}


		#endregion

		#region Unsorted Events
		public void copyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!ShowPathB)
			{
				TileClipboard = TileConfig.CollisionPath1[CurrentCollisionMask];
				Clipboard.SetData("TileManiacCollision", TileConfig.CollisionPath1[CurrentCollisionMask]);
				RefreshUI();
			}
			else if (ShowPathB)
			{
				TileClipboard = TileConfig.CollisionPath2[CurrentCollisionMask];
				Clipboard.SetData("TileManiacCollision", TileConfig.CollisionPath2[CurrentCollisionMask]);
				RefreshUI();
			}
		}
		public void pasteToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!ShowPathB)
			{
				if (Clipboard.ContainsData("TileManiacCollision") && Properties.Settings.MyDefaults.TileManiacEnableWindowsClipboard)
				{
					var copyData = Clipboard.GetData("TileManiacCollision") as TileConfig.CollisionMask;
					if (copyData != null)
					{
						TileConfig.CollisionPath1[CurrentCollisionMask] = copyData;
					}

				}
				else if (TileClipboard != null)
				{
					TileConfig.CollisionPath1[CurrentCollisionMask] = TileClipboard;
				}

				RefreshUI();
			}
			else if (ShowPathB)
			{
				if (Clipboard.ContainsData("TileManiacCollision") && Properties.Settings.MyDefaults.TileManiacEnableWindowsClipboard)
				{
					var copyData = Clipboard.GetData("TileManiacCollision") as TileConfig.CollisionMask;
					if (copyData != null)
					{
						TileConfig.CollisionPath2[CurrentCollisionMask] = copyData;
					}
				}
				else if (TileClipboard != null)
				{
					TileConfig.CollisionPath2[CurrentCollisionMask] = TileClipboard;
				}
				RefreshUI();
			}
		}
		public void aboutToolStripMenuItem1_Click(object sender, RoutedEventArgs e)
		{
			About.AboutWindow frm = new About.AboutWindow();
			frm.ShowDialog();
		}
		public void copyToOtherPathToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!ShowPathB)
			{
				TileConfig.CollisionPath2[CurrentCollisionMask].Collision = (byte[])TileConfig.CollisionPath1[CurrentCollisionMask].Collision.Clone();
				TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision = (bool[])TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision.Clone();
				TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling = TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling;
				TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle = TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle = TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle = TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour = TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour;

				CollisionListImgB[CurrentCollisionMask] = CollisionListImgA[CurrentCollisionMask];
				RefreshUI();
			}
			else if (ShowPathB)
			{
				TileConfig.CollisionPath1[CurrentCollisionMask].Collision = (byte[])TileConfig.CollisionPath2[CurrentCollisionMask].Collision.Clone();
				TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision = (bool[])TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision.Clone();
				TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling = TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling;
				TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle = TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle = TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle = TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour = TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour;

				CollisionListImgA[CurrentCollisionMask] = CollisionListImgB[CurrentCollisionMask];
				RefreshUI();
			}
		}

		public void settingsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Controls.Options.OptionsMenu options = new ManiacEditor.Controls.Options.OptionsMenu();
			options.Owner = this;
			options.ShowDialog();
		}

		public void newInstanceToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var mainWindow = new CollisionEditor();
			mainWindow.Show();
		}
		public void openCollisionHomeFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (TC_FilePath != null)
			{
				Process.Start("explorer.exe", "/select, " + TC_FilePath);
			}
			else
			{
				MessageBox.Show("No File Opened Yet!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		#endregion

		#region Reset Path Events
		public void ResetCollisionPathA()
		{
			MessageBoxResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				TileConfig.CollisionPath1[CurrentCollisionMask].Collision = (byte[])OriginalTileConfig.CollisionPath1[CurrentCollisionMask].Collision.Clone();
				TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision = (bool[])OriginalTileConfig.CollisionPath1[CurrentCollisionMask].HasCollision.Clone();
				TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling;
				TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].Behaviour;
				RefreshUI();
			}
		}
		public void ResetCollisionPathB()
		{
			MessageBoxResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				TileConfig.CollisionPath2[CurrentCollisionMask].Collision = (byte[])OriginalTileConfig.CollisionPath2[CurrentCollisionMask].Collision.Clone();
				TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision = (bool[])OriginalTileConfig.CollisionPath2[CurrentCollisionMask].HasCollision.Clone();
				TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling;
				TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].Behaviour;
				RefreshUI();
			}
		}
		public void ResetBothCollisionPaths()
		{
			MessageBoxResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				TileConfig.CollisionPath1[CurrentCollisionMask].Collision = (byte[])OriginalTileConfig.CollisionPath1[CurrentCollisionMask].Collision.Clone();
				TileConfig.CollisionPath1[CurrentCollisionMask].HasCollision = (bool[])OriginalTileConfig.CollisionPath1[CurrentCollisionMask].HasCollision.Clone();
				TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling;
				TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle;
				TileConfig.CollisionPath1[CurrentCollisionMask].Behaviour = OriginalTileConfig.CollisionPath1[CurrentCollisionMask].Behaviour;


				TileConfig.CollisionPath2[CurrentCollisionMask].Collision = (byte[])OriginalTileConfig.CollisionPath2[CurrentCollisionMask].Collision.Clone();
				TileConfig.CollisionPath2[CurrentCollisionMask].HasCollision = (bool[])OriginalTileConfig.CollisionPath2[CurrentCollisionMask].HasCollision.Clone();
				TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling;
				TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle;
				TileConfig.CollisionPath2[CurrentCollisionMask].Behaviour = OriginalTileConfig.CollisionPath2[CurrentCollisionMask].Behaviour;

				RefreshUI();
			}
		}

		#endregion

		#region General Events
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{

		}
		public void pathAToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ResetCollisionPathA();
		}
		public void pathBToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ResetCollisionPathB();
		}
		public void bothToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ResetBothCollisionPaths();
		}
		private void HostGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (GraphicPanel != null) GraphicPanel.Render();
		}
		private void ToolsButton_Click(object sender, RoutedEventArgs e)
		{
			ToolsButton.ContextMenu.IsOpen = true;
		}
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			IsEditorClosed = true;
		}
		public void showPathBToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetPathBState();
		}
		public void mirrorPathsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetMirrorPathsState();
		}
		private void developerInterfaceToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//DeveloperTerminal developerTerminal = new DeveloperTerminal();
			//developerTerminal.Show();
		}
		public void showGridToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetShowGridState();
		}
		public void flipTileHorizontallyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			FlipTileH();
		}
		public void flipTileVerticallyToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			FlipTileV();
		}
		public void x16TilesgifToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.Solution.SolutionLoader.BackupStageTiles();
		}
		public void tileConfigbinToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.Solution.SolutionLoader.BackupTileConfig();
		}
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				Classes.Options.GeneralSettings.Save();
			}
			catch (Exception ex)
			{
				Debug.Write("Failed to write settings: " + ex);
			}
		}
		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			Methods.Internal.Controls.TileManiac_OnKeyDown(sender, KeyEventExts.ToWinforms(e));
		}
		private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			Methods.Internal.Controls.TileManiac_OnKeyUp(sender, KeyEventExts.ToWinforms(e));
		}


		#endregion

		#region Rendering
		private void GridPicBox_Paint(object sender, PaintEventArgs e)
		{
			if (ShowGrid)
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

		private System.Drawing.Point GetAngle(int X, int Y, int Angle, int Length)
		{
			double angleRadians = (Math.PI / 180.0) * Angle;
			double x2 = X + (Math.Cos(angleRadians) * Length);
			double y2 = Y + (Math.Sin(angleRadians) * Length);
			int intX2 = Convert.ToInt32(x2);
			int intY2 = Convert.ToInt32(y2);
			return new System.Drawing.Point(intX2, intY2);
		}

		private System.Drawing.Point DrawAngle(DevicePanel d, int x, int y, int angle, int length, System.Drawing.Color color, int thickness = 1, bool FlipX = false, bool FlipY = false)
		{
			System.Drawing.Point StartPoint = new System.Drawing.Point(x, y);
			System.Drawing.Point EndPoint = GetAngle(x, y, angle, length);
			d.DrawLine(StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y, color, thickness);
			return EndPoint;
		}

		private void GraphicPanel_OnRender(object sender, EventHandlers.DeviceEventArgs e)
		{
			if (TileConfig != null)
			{
				double FloorAngle;
				double CeilingAngle;
				double LWallAngle;
				double RWallAngle;

				bool isCelling = false;

				if (ShowPathB)
				{
					FloorAngle = -GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].FloorAngle);
					CeilingAngle = -GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].CeilingAngle);
					LWallAngle = -GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].LWallAngle);
					RWallAngle = -GetAngleValue(TileConfig.CollisionPath2[CurrentCollisionMask].RWallAngle);
					isCelling = TileConfig.CollisionPath2[CurrentCollisionMask].IsCeiling;
				}
				else
				{
					FloorAngle = -GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].FloorAngle);
					CeilingAngle = -GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].CeilingAngle);
					LWallAngle = -GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].LWallAngle);
					RWallAngle = -GetAngleValue(TileConfig.CollisionPath1[CurrentCollisionMask].RWallAngle);
					isCelling = TileConfig.CollisionPath1[CurrentCollisionMask].IsCeiling;
				}
				

				int width = HostPanel.Width;
				int height = HostPanel.Height;

				int center_x = width / 2;
				int center_y = height / 2;

				int length = 128;
				int offset = length / 2;
				int line_length = 256;

				System.Drawing.Rectangle Rect = new Rectangle(center_x - offset, center_y - offset, length, length);



				System.Drawing.Point LastPoint = new System.Drawing.Point(center_x, center_y);

				int thickness_angles = 2;
				int thickness_box = 2;
				int thickness_axis = 2;

				int axis_offset = 5;

				int bound_x1 = center_x - offset;
				int bound_y1 = center_y - offset;
				int bound_x2 = bound_x1 + length;
				int bound_y2 = bound_y1 + length;

				GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, System.Drawing.Color.FromArgb(128, System.Drawing.Color.Black), System.Drawing.Color.Black, thickness_box);

				System.Drawing.Point rect_left = new System.Drawing.Point(Rect.Left + offset, Rect.Top);
				System.Drawing.Point rect_top = new System.Drawing.Point(Rect.Right, Rect.Top + offset);
				System.Drawing.Point rect_bottom = new System.Drawing.Point(Rect.Right - offset, Rect.Bottom);
				System.Drawing.Point rect_right = new System.Drawing.Point(Rect.Left, Rect.Bottom - offset);

				DrawAngle(GraphicPanel, rect_left.X, rect_left.Y, (int)FloorAngle, line_length, System.Drawing.Color.Red, thickness_angles);
				DrawAngle(GraphicPanel, rect_top.X, rect_top.Y, (int)LWallAngle, line_length, System.Drawing.Color.Green, thickness_angles);
				DrawAngle(GraphicPanel, rect_bottom.X, rect_bottom.Y, (int)CeilingAngle, line_length, System.Drawing.Color.Blue, thickness_angles);
				DrawAngle(GraphicPanel, rect_right.X, rect_right.Y, (int)RWallAngle, line_length, System.Drawing.Color.Gold, thickness_angles);

				DrawAngle(GraphicPanel, rect_left.X, rect_left.Y, (int)FloorAngle, -line_length, System.Drawing.Color.Red, thickness_angles);
				DrawAngle(GraphicPanel, rect_top.X, rect_top.Y, (int)LWallAngle, -line_length, System.Drawing.Color.Green, thickness_angles);
				DrawAngle(GraphicPanel, rect_bottom.X, rect_bottom.Y, (int)CeilingAngle, -line_length, System.Drawing.Color.Blue, thickness_angles);
				DrawAngle(GraphicPanel, rect_right.X, rect_right.Y, (int)RWallAngle, -line_length, System.Drawing.Color.Gold, thickness_angles);

				GraphicPanel.DrawRectangle(rect_left.X - axis_offset, rect_left.Y - axis_offset, rect_left.X + axis_offset, rect_left.Y + axis_offset, System.Drawing.Color.FromArgb(128, System.Drawing.Color.Black), System.Drawing.Color.DarkRed, thickness_axis);
				GraphicPanel.DrawRectangle(rect_top.X - axis_offset, rect_top.Y - axis_offset, rect_top.X + axis_offset, rect_top.Y + axis_offset, System.Drawing.Color.FromArgb(128, System.Drawing.Color.Black), System.Drawing.Color.DarkGreen, thickness_axis);
				GraphicPanel.DrawRectangle(rect_bottom.X - axis_offset, rect_bottom.Y - axis_offset, rect_bottom.X + axis_offset, rect_bottom.Y + axis_offset, System.Drawing.Color.FromArgb(128, System.Drawing.Color.Black), System.Drawing.Color.DarkBlue, thickness_axis);
				GraphicPanel.DrawRectangle(rect_right.X - axis_offset, rect_right.Y - axis_offset, rect_right.X + axis_offset, rect_right.Y + axis_offset, System.Drawing.Color.FromArgb(128, System.Drawing.Color.Black), System.Drawing.Color.DarkGoldenrod, thickness_axis);
			}
		}
		private void RenderButton_Click(object sender, RoutedEventArgs e)
		{
			GraphicPanel.Render();
		}


		#endregion


	}
}
