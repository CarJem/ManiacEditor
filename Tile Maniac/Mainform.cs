using System;
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
using RSDKv4;
using RSDKv5;
using System.Drawing.Imaging;
using System.Media;
using Color = System.Drawing.Color;
using System.Runtime.InteropServices;

namespace TileManiac
{
    public partial class Mainform : Form
    {
        bool lockRadioButtons = false; //for locking radio button updates when switching single select options

        RSDKv5.TilesConfig.CollisionMask TileClipboard;

        List<Bitmap> ColImges = new List<Bitmap>(); //List of images, saves memory
        List<Bitmap> ColImgesNoCol = new List<Bitmap>(); //List of images, saves memory
        List<Bitmap> ColActivatedImges = new List<Bitmap>(); //List of images, saves memory

        List<Bitmap> CollisionListImgA = new List<Bitmap>();
        List<Bitmap> CollisionListImgB = new List<Bitmap>();

        public int curColisionMask; //What Collision Mask are we editing?

        public string filepath; //Where is the file located?
        public string folderpath; //Where is the folder located?

        bool showPathB = false; //should we show Path A or Path B?

        public bool hasModified = false; //For intergrating tools to know that we have saved/made edits to this config.
        bool imageIsModified = false;

        bool mouseHeldDown = false;
        MouseButtons mouseButtonHeld = MouseButtons.None;

        bool changingModes = false; //To prevent updating the radio buttons until after we change the viewer mode

        public RSDKv5.TilesConfig tcf; //The ColllisionMask Data
        public RSDKv5.TilesConfig tcfBak; //Backup ColllisionMask Data

        List<Bitmap> Tiles = new List<Bitmap>(); //List of all the 16x16 Stage Tiles
        List<Bitmap> IndexedTiles = new List<Bitmap>(); //List of all the 16x16 Stage Tiles (Preserving Color Pallete)
        int gotoVal; //What collision mask we goto when "GO!" is pressed

        public Mainform()
        {
            InitializeComponent();

            ToolTip ToolTip = new ToolTip();

            TableLayoutControlCollection controls = tableLayoutPanel1.Controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is Label)
                {
                    controls[i].Click += new System.EventHandler(this.tableLayoutPanel1_Click);
                    controls[i].MouseDown += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel1_MouseDown);
                    controls[i].MouseUp += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel1_MouseUp);
                    controls[i].MouseHover += new System.EventHandler(this.tableLayoutPanel1_MouseHover);
                    controls[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel1_MouseMove);
                }
            }


            ColImges.Add(new Bitmap(Properties.Resources._1));
            ColImges.Add(new Bitmap(Properties.Resources._2));
            ColImges.Add(new Bitmap(Properties.Resources._3));
            ColImges.Add(new Bitmap(Properties.Resources._4));
            ColImges.Add(new Bitmap(Properties.Resources._5));
            ColImges.Add(new Bitmap(Properties.Resources._6));
            ColImges.Add(new Bitmap(Properties.Resources._7));
            ColImges.Add(new Bitmap(Properties.Resources._8));
            ColImges.Add(new Bitmap(Properties.Resources._9));
            ColImges.Add(new Bitmap(Properties.Resources._10));
            ColImges.Add(new Bitmap(Properties.Resources._11));
            ColImges.Add(new Bitmap(Properties.Resources._12));
            ColImges.Add(new Bitmap(Properties.Resources._13));
            ColImges.Add(new Bitmap(Properties.Resources._14));
            ColImges.Add(new Bitmap(Properties.Resources._15));
            ColImges.Add(new Bitmap(Properties.Resources._16));
            ColImges.Add(new Bitmap(Properties.Resources._0));

            ColImgesNoCol.Add(new Bitmap(Properties.Resources._1_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._2_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._3_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._4_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._5_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._6_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._7_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._8_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._9_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._10_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._11_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._12_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._13_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._14_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._15_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._16_NoCol));
            ColImgesNoCol.Add(new Bitmap(Properties.Resources._0_NoCol));


            ColActivatedImges.Add(Properties.Resources.Red);
            ColActivatedImges.Add(Properties.Resources.Green);

            Viewer1.Image = ColImges[16];
            Viewer2.Image = ColImges[16];
            Viewer3.Image = ColImges[16];
            Viewer4.Image = ColImges[16];
            Viewer5.Image = ColImges[16];
            Viewer6.Image = ColImges[16];
            Viewer7.Image = ColImges[16];
            Viewer8.Image = ColImges[16];
            Viewer9.Image = ColImges[16];
            Viewer10.Image = ColImges[16];
            Viewer11.Image = ColImges[16];
            Viewer12.Image = ColImges[16];
            Viewer13.Image = ColImges[16];
            Viewer14.Image = ColImges[16];
            Viewer15.Image = ColImges[16];
            Viewer16.Image = ColImges[16];


            RGBox0.Image = ColActivatedImges[0];
            RGBox1.Image = ColActivatedImges[0];
            RGBox2.Image = ColActivatedImges[0];
            RGBox3.Image = ColActivatedImges[0];
            RGBox4.Image = ColActivatedImges[0];
            RGBox5.Image = ColActivatedImges[0];
            RGBox6.Image = ColActivatedImges[0];
            RGBox7.Image = ColActivatedImges[0];
            RGBox8.Image = ColActivatedImges[0];
            RGBox9.Image = ColActivatedImges[0];
            RGBoxA.Image = ColActivatedImges[0];
            RGBoxB.Image = ColActivatedImges[0];
            RGBoxC.Image = ColActivatedImges[0];
            RGBoxD.Image = ColActivatedImges[0];
            RGBoxE.Image = ColActivatedImges[0];
            RGBoxF.Image = ColActivatedImges[0];

            ToolTip.SetToolTip(SlopeNUD, "Controls the slope angle of the tile in degrees");
            ToolTip.SetToolTip(PhysicsNUD, "Controls the physics of the player interacting with the tile");
            ToolTip.SetToolTip(MomentumNUD, "Controls the momentum the player gets from the tile");
            ToolTip.SetToolTip(UnknownNUD, "Controls the Unknown Value of the tile");
            ToolTip.SetToolTip(SpecialNUD, "Controls the the tile's 'Special' Properties, like whether it's a conveyor belt or not");

            LoadSettings();
        }

        void LoadSettings()
        {
            if (Properties.Settings.Default.ListSetting == 0)
            {
                uncheckListViews();
                collisionViewRadioButton.Checked = true;
                lockRadioButtons = false;
            }
            else if (Properties.Settings.Default.ListSetting == 1)
            {
                uncheckListViews();
                tileViewRadioButton.Checked = true;
                lockRadioButtons = false;
            }

            if (Properties.Settings.Default.ViewerSetting == 0)
            {
                unCheckModes();
                radioButton1.Checked = true;
                TilePicBox.Visible = true;
                changingModes = false;
            }
            else if (Properties.Settings.Default.ViewerSetting == 1)
            {
                unCheckModes();
                radioButton2.Checked = true;
                CollisionPicBox.Visible = true;
                changingModes = false;
            }
            else if (Properties.Settings.Default.ViewerSetting == 2)
            {
                unCheckModes();
                radioButton3.Checked = true;
                overlayPicBox.Visible = true;
                changingModes = false;
            }
            if (Properties.Settings.Default.ShowGrid)
            {
                showGridToolStripMenuItem.Checked = true;
            }
            if (Properties.Settings.Default.ClassicMode)
            {
                tableLayoutPanel1.Enabled = false;
                tableLayoutPanel1.Visible = false;
                viewSettingsToolStripMenuItem.Enabled = false;
            }
            switch (Properties.Settings.Default.ViewAppearanceMode)
            {
                case 0:
                    overlayToolStripMenuItem.Checked = true;
                    collisionToolStripMenuItem.Checked = false;
                    break;
                case 1:
                    collisionToolStripMenuItem.Checked = true;
                    overlayToolStripMenuItem.Checked = false;
                    break;
            }
            if (Properties.Settings.Default.MirrorMode)
            {
                mirrorPathsToolStripMenuItem1.Checked = true;
                UpdateMirrorModeStatusLabel();
            }
        }

        public void LoadTileConfigViaIntergration(TilesConfig tilesConfig, string scenePath, int selectedTile = 0)
        {
                curColisionMask = 0; // Set the current collision mask to zero (avoids rare errors)
                filepath = Path.Combine(scenePath, "TileConfig.bin");
                tcf = tilesConfig;
                tcfBak = tilesConfig;
                string tileBitmapPath = Path.Combine(Path.GetDirectoryName(filepath), "16x16tiles.gif"); // get the path to the stage's tileset
                LoadTileSet(new Bitmap(tileBitmapPath)); // load each 16x16 tile into the list

                CollisionList.Images.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    if (Properties.Settings.Default.ListSetting == 0)
                    {
                        CollisionListImgA.Add(tcf.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
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
                    if (Properties.Settings.Default.ListSetting == 0)
                    {
                        CollisionListImgB.Add(tcf.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open RSDKv5 Tileconfig";
            dlg.DefaultExt = ".bin";
            dlg.Filter = "RSDKv5 Tileconfig Files|Tileconfig*.bin|All Files|*";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                curColisionMask = 0; // Set the current collision mask to zero (avoids rare errors)
                filepath = dlg.FileName;
                tcf = new RSDKv5.TilesConfig(dlg.FileName);
                tcfBak = new RSDKv5.TilesConfig(dlg.FileName);
                string tileBitmapPath = Path.Combine(Path.GetDirectoryName(filepath), "16x16tiles.gif"); // get the path to the stage's tileset
                LoadTileSet(new Bitmap(tileBitmapPath)); // load each 16x16 tile into the list

                CollisionList.Images.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    if (Properties.Settings.Default.ListSetting == 0)
                    {
                        CollisionListImgA.Add(tcf.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
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
                    if (Properties.Settings.Default.ListSetting == 0)
                    {
                        CollisionListImgB.Add(tcf.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
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
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != null) //Did we open a file?
            {
                if (imageIsModified)
                {
                    //SaveTileSet(); //Not Functional Yet
                    imageIsModified = false;
                }
                tcf.Write(filepath);
                hasModified = true;
            }
            else //if not then use "Save As..."
            {
                saveAsToolStripMenuItem_Click(this, e);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save RSDKv5 Tileconfig As...";
            dlg.DefaultExt = ".bin";
            dlg.Filter = "RSDKv5 Tileconfig Files|Tileconfig*.bin";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                tcf.Write(dlg.FileName); //Write the data to a file
            }
        }

        private void openSingleCollisionMaskToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportCollisionMaskAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void splitFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select Folder to Export to...";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
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

        public void LoadTileSet(Bitmap TileSet)
        {
            Tiles.Clear(); // Clear the previous images, since we load the entire file!
            int tsize = TileSet.Height; //Height of the image in pixels
            for (int i = 0; i < (tsize / 16); i++) //We divide by 16 to get the "height" in blocks
            {
                Rectangle CropArea = new Rectangle(0, (i * 16), 16, 16); //we then get tile at Y: i * 16, 
                                                                         //we have to multiply i by 16 to get the "true Tile value" (1* 16 = 16, 2 * 16 = 32, etc.)

                Bitmap CroppedImage = CropImage(TileSet, CropArea); // crop that image
                Tiles.Add(CroppedImage); // add it to the tile list

                //Code Not Ready Yet!
                //Bitmap CroppedImageIndexed = CropImage(TileSet, CropArea, true); // crop that indexed image
                //IndexedTiles.Add(CroppedImageIndexed); // add it to the indexed tile list
            }
        }

        public void SaveTileSet()
        {
            Bitmap bmp = mergeImages(IndexedTiles.ToArray());
            bmp.Save(Path.Combine(Path.GetDirectoryName(filepath), "16x16Tiles_Test.gif"));         
        }

        public Bitmap mergeImages(Bitmap[] images)
        {
            Bitmap mergedImg = new Bitmap(16, 16384, PixelFormat.Format8bppIndexed);
            mergedImg.Palette = IndexedTiles[0].Palette;
            for (int i = 0; i < IndexedTiles.Count; i++)
            {
                var bitmapData = IndexedTiles[i].LockBits(new Rectangle(0, 0, IndexedTiles[i].Width, IndexedTiles[i].Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                for (int h = 0; h < 16; h++)
                {
                    for (int w = 0; w < 16; w++)
                    {
                        int indexColor = GetIndexedPixel(w, h, bitmapData);
                        SetPixel(mergedImg, w, h + (16 * i), indexColor);
                    }
                }
            }

            return mergedImg;
        }

        private int GetIndexedPixel(int x, int y, BitmapData bitmapData)
        {
            //I tried to get this section working, but I didn't run into any luck.
            return 0;
        }

        private static void SetPixel(Bitmap bmp, int x, int y, int paletteEntry)
        {
            BitmapData data = bmp.LockBits(new Rectangle(new Point(x, y), new Size(1, 1)), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
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
                bmp = source.Clone(section, PixelFormat.Format8bppIndexed);
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

        public void RefreshUI()
        {
            if (tcf != null)
            {
                CurMaskLabel.Text = "Collision Mask " + (curColisionMask + 1) + " of " + 1024; //what collision mask are we on?
                TilePicBox.Image = ResizeBitmap(Tiles[curColisionMask],96,96); //update the tile preview 
                Bitmap Overlaypic = new Bitmap(16, 16);
                GetRawSlopeNUD();
                if (!showPathB) //if we are showing Path A then refresh the values accordingly
                {
                    CollisionPicBox.Image = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                    Overlaypic = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0), Tiles[curColisionMask]);
                    PhysicsNUD.Value = tcf.CollisionPath1[curColisionMask].physics;
                    MomentumNUD.Value = tcf.CollisionPath1[curColisionMask].momentum;
                    UnknownNUD.Value = tcf.CollisionPath1[curColisionMask].unknown;
                    SpecialNUD.Value = tcf.CollisionPath1[curColisionMask].special;
                    IsCeilingButton.Checked = tcf.CollisionPath1[curColisionMask].IsCeiling;

                    RefreshPathA();

                }

                if (showPathB) //if we are showing Path B then refresh the values accordingly
                {
                    CollisionPicBox.Image = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 255, 0)); Overlaypic = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 255, 0), Tiles[curColisionMask]);
                    SlopeNUD.Value = tcf.CollisionPath2[curColisionMask].slopeAngle;
                    PhysicsNUD.Value = tcf.CollisionPath2[curColisionMask].physics;
                    MomentumNUD.Value = tcf.CollisionPath2[curColisionMask].momentum;
                    UnknownNUD.Value = tcf.CollisionPath2[curColisionMask].unknown;
                    SpecialNUD.Value = tcf.CollisionPath2[curColisionMask].special;
                    IsCeilingButton.Checked = tcf.CollisionPath2[curColisionMask].IsCeiling;

                    RefreshPathB();

                }

                overlayPicBox.Image = Overlaypic;
                overlayPicBox.Image = ResizeBitmap(Overlaypic, 96, 96);
                CollisionPicBox.Image = ResizeBitmap(new Bitmap(CollisionPicBox.Image), 96, 96);

                if (Properties.Settings.Default.ClassicMode)
                {
                    tableLayoutPanel1.Enabled = false;
                    tableLayoutPanel1.Visible = false;
                }
                else
                {
                    if (!mouseHeldDown)
                    {
                        if (Properties.Settings.Default.ViewAppearanceMode == 0)
                        {
                            tableLayoutPanel1.BackgroundImage = ResizeBitmap(Overlaypic, tableLayoutPanel1.Width, tableLayoutPanel1.Height);
                            tableLayoutPanel1.Enabled = true;
                            tableLayoutPanel1.Visible = true;
                        }
                        else if (Properties.Settings.Default.ViewAppearanceMode == 1)
                        {
                            tableLayoutPanel1.BackgroundImage = ResizeBitmap(new Bitmap(CollisionPicBox.Image), tableLayoutPanel1.Width, tableLayoutPanel1.Height);
                            tableLayoutPanel1.Enabled = true;
                            tableLayoutPanel1.Visible = true;
                        }

                    }

                }


                RefreshCollisionList();
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

            cb00.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[0];
            cb01.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[1];
            cb02.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[2];
            cb03.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[3];
            cb04.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[4];
            cb05.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[5];
            cb06.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[6];
            cb07.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[7];
            cb08.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[8];
            cb09.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[9];
            cb10.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[10];
            cb11.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[11];
            cb12.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[12];
            cb13.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[13];
            cb14.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[14];
            cb15.Checked = tcf.CollisionPath1[curColisionMask].HasCollision[15];

            if (tcf.CollisionPath1[curColisionMask].HasCollision[0])
            { Viewer1.Image = ColImges[lb00.SelectedIndex]; }
            else { Viewer1.Image = ColImgesNoCol[lb00.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[1])
            { Viewer2.Image = ColImges[lb01.SelectedIndex]; }
            else { Viewer2.Image = ColImgesNoCol[lb01.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[2])
            { Viewer3.Image = ColImges[lb02.SelectedIndex]; }
            else { Viewer3.Image = ColImgesNoCol[lb02.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[3])
            { Viewer4.Image = ColImges[lb03.SelectedIndex]; }
            else { Viewer4.Image = ColImgesNoCol[lb03.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[4])
            { Viewer5.Image = ColImges[lb04.SelectedIndex]; }
            else { Viewer5.Image = ColImgesNoCol[lb04.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[5])
            { Viewer6.Image = ColImges[lb05.SelectedIndex]; }
            else { Viewer6.Image = ColImgesNoCol[lb05.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[6])
            { Viewer7.Image = ColImges[lb06.SelectedIndex]; }
            else { Viewer7.Image = ColImgesNoCol[lb06.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[7])
            { Viewer8.Image = ColImges[lb07.SelectedIndex]; }
            else { Viewer8.Image = ColImgesNoCol[lb07.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[8])
            { Viewer9.Image = ColImges[lb08.SelectedIndex]; }
            else { Viewer9.Image = ColImgesNoCol[lb08.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[9])
            { Viewer10.Image = ColImges[lb09.SelectedIndex]; }
            else { Viewer10.Image = ColImgesNoCol[lb09.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[10])
            { Viewer11.Image = ColImges[lb10.SelectedIndex]; }
            else { Viewer11.Image = ColImgesNoCol[lb10.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[11])
            { Viewer12.Image = ColImges[lb11.SelectedIndex]; }
            else { Viewer12.Image = ColImgesNoCol[lb11.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[12])
            { Viewer13.Image = ColImges[lb12.SelectedIndex]; }
            else { Viewer13.Image = ColImgesNoCol[lb12.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[13])
            { Viewer14.Image = ColImges[lb13.SelectedIndex]; }
            else { Viewer14.Image = ColImgesNoCol[lb13.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[14])
            { Viewer15.Image = ColImges[lb14.SelectedIndex]; }
            else { Viewer15.Image = ColImgesNoCol[lb14.SelectedIndex]; }

            if (tcf.CollisionPath1[curColisionMask].HasCollision[15])
            { Viewer16.Image = ColImges[lb15.SelectedIndex]; }
            else { Viewer16.Image = ColImgesNoCol[lb15.SelectedIndex]; }




            if (cb00.Checked) { RGBox0.Image = ColActivatedImges[1]; }
            else { RGBox0.Image = ColActivatedImges[0]; }

            if (cb01.Checked) { RGBox1.Image = ColActivatedImges[1]; }
            else { RGBox1.Image = ColActivatedImges[0]; }

            if (cb02.Checked) { RGBox2.Image = ColActivatedImges[1]; }
            else { RGBox2.Image = ColActivatedImges[0]; }

            if (cb03.Checked) { RGBox3.Image = ColActivatedImges[1]; }
            else { RGBox3.Image = ColActivatedImges[0]; }

            if (cb04.Checked) { RGBox4.Image = ColActivatedImges[1]; }
            else { RGBox4.Image = ColActivatedImges[0]; }

            if (cb05.Checked) { RGBox5.Image = ColActivatedImges[1]; }
            else { RGBox5.Image = ColActivatedImges[0]; }

            if (cb06.Checked) { RGBox6.Image = ColActivatedImges[1]; }
            else { RGBox6.Image = ColActivatedImges[0]; }

            if (cb07.Checked) { RGBox7.Image = ColActivatedImges[1]; }
            else { RGBox7.Image = ColActivatedImges[0]; }

            if (cb08.Checked) { RGBox8.Image = ColActivatedImges[1]; }
            else { RGBox8.Image = ColActivatedImges[0]; }

            if (cb09.Checked) { RGBox9.Image = ColActivatedImges[1]; }
            else { RGBox9.Image = ColActivatedImges[0]; }

            if (cb10.Checked) { RGBoxA.Image = ColActivatedImges[1]; }
            else { RGBoxA.Image = ColActivatedImges[0]; }

            if (cb11.Checked) { RGBoxB.Image = ColActivatedImges[1]; }
            else { RGBoxB.Image = ColActivatedImges[0]; }

            if (cb12.Checked) { RGBoxC.Image = ColActivatedImges[1]; }
            else { RGBoxC.Image = ColActivatedImges[0]; }

            if (cb13.Checked) { RGBoxD.Image = ColActivatedImges[1]; }
            else { RGBoxD.Image = ColActivatedImges[0]; }

            if (cb14.Checked) { RGBoxE.Image = ColActivatedImges[1]; }
            else { RGBoxE.Image = ColActivatedImges[0]; }

            if (cb15.Checked) { RGBoxF.Image = ColActivatedImges[1]; }
            else { RGBoxF.Image = ColActivatedImges[0]; }
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

            cb00.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[0];
            cb01.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[1];
            cb02.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[2];
            cb03.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[3];
            cb04.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[4];
            cb05.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[5];
            cb06.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[6];
            cb07.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[7];
            cb08.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[8];
            cb09.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[9];
            cb10.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[10];
            cb11.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[11];
            cb12.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[12];
            cb13.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[13];
            cb14.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[14];
            cb15.Checked = tcf.CollisionPath2[curColisionMask].HasCollision[15];


            if (tcf.CollisionPath2[curColisionMask].HasCollision[0])
            { Viewer1.Image = ColImges[lb00.SelectedIndex]; }
            else { Viewer1.Image = ColImgesNoCol[lb00.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[1])
            { Viewer2.Image = ColImges[lb01.SelectedIndex]; }
            else { Viewer2.Image = ColImgesNoCol[lb01.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[2])
            { Viewer3.Image = ColImges[lb02.SelectedIndex]; }
            else { Viewer3.Image = ColImgesNoCol[lb02.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[3])
            { Viewer4.Image = ColImges[lb03.SelectedIndex]; }
            else { Viewer4.Image = ColImgesNoCol[lb03.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[4])
            { Viewer5.Image = ColImges[lb04.SelectedIndex]; }
            else { Viewer5.Image = ColImgesNoCol[lb04.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[5])
            { Viewer6.Image = ColImges[lb05.SelectedIndex]; }
            else { Viewer6.Image = ColImgesNoCol[lb05.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[6])
            { Viewer7.Image = ColImges[lb06.SelectedIndex]; }
            else { Viewer7.Image = ColImgesNoCol[lb06.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[7])
            { Viewer8.Image = ColImges[lb07.SelectedIndex]; }
            else { Viewer8.Image = ColImgesNoCol[lb07.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[8])
            { Viewer9.Image = ColImges[lb08.SelectedIndex]; }
            else { Viewer9.Image = ColImgesNoCol[lb08.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[9])
            { Viewer10.Image = ColImges[lb09.SelectedIndex]; }
            else { Viewer10.Image = ColImgesNoCol[lb09.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[10])
            { Viewer11.Image = ColImges[lb10.SelectedIndex]; }
            else { Viewer11.Image = ColImgesNoCol[lb10.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[11])
            { Viewer12.Image = ColImges[lb11.SelectedIndex]; }
            else { Viewer12.Image = ColImgesNoCol[lb11.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[12])
            { Viewer13.Image = ColImges[lb12.SelectedIndex]; }
            else { Viewer13.Image = ColImgesNoCol[lb12.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[13])
            { Viewer14.Image = ColImges[lb13.SelectedIndex]; }
            else { Viewer14.Image = ColImgesNoCol[lb13.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[14])
            { Viewer15.Image = ColImges[lb14.SelectedIndex]; }
            else { Viewer15.Image = ColImgesNoCol[lb14.SelectedIndex]; }

            if (tcf.CollisionPath2[curColisionMask].HasCollision[15])
            { Viewer16.Image = ColImges[lb15.SelectedIndex]; }
            else { Viewer16.Image = ColImgesNoCol[lb15.SelectedIndex]; }




            if (cb00.Checked) { RGBox0.Image = ColActivatedImges[1]; }
            else { RGBox0.Image = ColActivatedImges[0]; }

            if (cb01.Checked) { RGBox1.Image = ColActivatedImges[1]; }
            else { RGBox1.Image = ColActivatedImges[0]; }

            if (cb02.Checked) { RGBox2.Image = ColActivatedImges[1]; }
            else { RGBox2.Image = ColActivatedImges[0]; }

            if (cb03.Checked) { RGBox3.Image = ColActivatedImges[1]; }
            else { RGBox3.Image = ColActivatedImges[0]; }

            if (cb04.Checked) { RGBox4.Image = ColActivatedImges[1]; }
            else { RGBox4.Image = ColActivatedImges[0]; }

            if (cb05.Checked) { RGBox5.Image = ColActivatedImges[1]; }
            else { RGBox5.Image = ColActivatedImges[0]; }

            if (cb06.Checked) { RGBox6.Image = ColActivatedImges[1]; }
            else { RGBox6.Image = ColActivatedImges[0]; }

            if (cb07.Checked) { RGBox7.Image = ColActivatedImges[1]; }
            else { RGBox7.Image = ColActivatedImges[0]; }

            if (cb08.Checked) { RGBox8.Image = ColActivatedImges[1]; }
            else { RGBox8.Image = ColActivatedImges[0]; }

            if (cb09.Checked) { RGBox9.Image = ColActivatedImges[1]; }
            else { RGBox9.Image = ColActivatedImges[0]; }

            if (cb10.Checked) { RGBoxA.Image = ColActivatedImges[1]; }
            else { RGBoxA.Image = ColActivatedImges[0]; }

            if (cb11.Checked) { RGBoxB.Image = ColActivatedImges[1]; }
            else { RGBoxB.Image = ColActivatedImges[0]; }

            if (cb12.Checked) { RGBoxC.Image = ColActivatedImges[1]; }
            else { RGBoxC.Image = ColActivatedImges[0]; }

            if (cb13.Checked) { RGBoxD.Image = ColActivatedImges[1]; }
            else { RGBoxD.Image = ColActivatedImges[0]; }

            if (cb14.Checked) { RGBoxE.Image = ColActivatedImges[1]; }
            else { RGBoxE.Image = ColActivatedImges[0]; }

            if (cb15.Checked) { RGBoxF.Image = ColActivatedImges[1]; }
            else { RGBoxF.Image = ColActivatedImges[0]; }
        }

        public void GetRawSlopeNUD()
        {
            if (tcf != null)
            {
                if (!showPathB)
                {                
                    SlopeNUD.Value = tcf.CollisionPath1[curColisionMask].slopeAngle;
                    int calculationSlopeA = (int)((decimal)tcf.CollisionPath1[curColisionMask].slopeAngle / 256 * 360);
                    //degreeLabel.Text = "Degree of Slope: " + calculationSlopeA.ToString();
                    RawSlopeNUD.Value = calculationSlopeA;

                }
                else
                {
                    SlopeNUD.Value = tcf.CollisionPath2[curColisionMask].slopeAngle;
                    int calculationSlopeB = (int)((decimal)tcf.CollisionPath2[curColisionMask].slopeAngle / 256 * 360);
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
                    if (Properties.Settings.Default.ListSetting == 0)
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
                    if (Properties.Settings.Default.ListSetting == 0)
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

        private void showPathBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Do we want to show Path B's Collision Masks instead of Path A's ones?
            if (!showPathB)
            {
                showPathB = showPathBToolStripMenuItem.Checked = true;
                VPLabel.Text = "Currently Viewing: Path B";
                RefreshUI();
            }
            else if (showPathB)
            {
                showPathB = showPathBToolStripMenuItem.Checked = false;
                VPLabel.Text = "Currently Viewing: Path A";
                RefreshUI();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About_Form frm = new About_Form();
            frm.ShowDialog(this); //Show the About window
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                curColisionMask++;
                if (curColisionMask > 1023) //Don't go above 1024... 1023 + 1 (because it starts at zero) = 1024
                {
                    curColisionMask = 1023;
                }
                RefreshUI();
            }
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                curColisionMask--;
                if (curColisionMask < 0) //Don't go below zero
                {
                    curColisionMask = 0;
                }
                RefreshUI();
            }
        }

        private void GotoNUD_ValueChanged(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                gotoVal = (int)GotoNUD.Value - 1; //Set the goto value, we then take -1 to get the "true value"
            }
        }

        private void GotoButton_Click(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                curColisionMask = gotoVal; //Set the Collision Masl to the desired value
                RefreshUI(); //Show the user the new values
            }
        }

        private void SlopeNUD_ValueChanged(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                if (Properties.Settings.Default.MirrorMode)
                {

                    if (SlopeNUD.Value <= 255)
                    {
                        tcf.CollisionPath2[curColisionMask].slopeAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path B
                        tcf.CollisionPath1[curColisionMask].slopeAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path A
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
                            tcf.CollisionPath1[curColisionMask].slopeAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path A
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
                            tcf.CollisionPath2[curColisionMask].slopeAngle = (byte)SlopeNUD.Value; //Set Slope angle for Path B
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


        private void PhysicsNUD_ValueChanged(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                if (Properties.Settings.Default.MirrorMode)
                {
                    tcf.CollisionPath1[curColisionMask].physics = (byte)PhysicsNUD.Value; //Set the Physics for Path A
                    tcf.CollisionPath2[curColisionMask].physics = (byte)PhysicsNUD.Value; //Set the Physics for Path B
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].physics = (byte)PhysicsNUD.Value; //Set the Physics for Path A
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].physics = (byte)PhysicsNUD.Value; //Set the Physics for Path B
                    }
                }

                //RefreshUI();
            }
        }

        private void MomentumNUD_ValueChanged(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                if (Properties.Settings.Default.MirrorMode)
                {
                    tcf.CollisionPath1[curColisionMask].momentum = (byte)MomentumNUD.Value; //Set the Momentum value for Path A
                    tcf.CollisionPath2[curColisionMask].momentum = (byte)MomentumNUD.Value; //Set the Momentum value for Path B
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].momentum = (byte)MomentumNUD.Value; //Set the Momentum value for Path A
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].momentum = (byte)MomentumNUD.Value; //Set the Momentum value for Path B
                    }
                }

                //RefreshUI();
            }
        }

        private void UnknownNUD_ValueChanged(object sender, EventArgs e)
        {           
            if (tcf != null)
            {
                if (Properties.Settings.Default.MirrorMode)
                {
                    tcf.CollisionPath1[curColisionMask].unknown = (byte)UnknownNUD.Value; //Set the unknown value for Path A
                    tcf.CollisionPath2[curColisionMask].unknown = (byte)UnknownNUD.Value; //Set the unknown value for Path B
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].unknown = (byte)UnknownNUD.Value; //Set the unknown value for Path A
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].unknown = (byte)UnknownNUD.Value; //Set the unknown value for Path B
                    }
                }

                //RefreshUI();
            }
        }

        private void SpecialNUD_ValueChanged(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                if (Properties.Settings.Default.MirrorMode)
                {
                    tcf.CollisionPath1[curColisionMask].special = (byte)SpecialNUD.Value; //Set the "Special" value for Path A
                    tcf.CollisionPath2[curColisionMask].special = (byte)SpecialNUD.Value; //Set the "Special" value for Path B
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].special = (byte)SpecialNUD.Value; //Set the "Special" value for Path A
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].special = (byte)SpecialNUD.Value; //Set the "Special" value for Path B
                    }
                }

                //RefreshUI();
            }
        }

        private void IsCeilingButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tcf != null)
            {
                if (Properties.Settings.Default.MirrorMode)
                {
                    tcf.CollisionPath1[curColisionMask].IsCeiling = IsCeilingButton.Checked; //Set the "IsCeiling" Value for Path A
                    tcf.CollisionPath2[curColisionMask].IsCeiling = IsCeilingButton.Checked; //Set the "IsCeiling" Value for Path B
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].IsCeiling = IsCeilingButton.Checked; //Set the "IsCeiling" Value for Path A
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].IsCeiling = IsCeilingButton.Checked; //Set the "IsCeiling" Value for Path B
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
            }
            RefreshUI();
        }

        private void copyToOtherPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!showPathB)
            {
                tcf.CollisionPath2[curColisionMask] = tcf.CollisionPath1[curColisionMask];
                //tcf.CollisionPath2[curColisionMask] = tc;RSDKv5.TilesConfig.ColllisionMask tc
                CollisionListImgB[curColisionMask] = CollisionListImgA[curColisionMask];
                RefreshUI();
            }
            else if (showPathB)
            {
                tcf.CollisionPath1[curColisionMask] = tcf.CollisionPath2[curColisionMask];
                CollisionListImgA[curColisionMask] = CollisionListImgB[curColisionMask];
                RefreshUI();
            }
        }

        private void mirrorPathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mirrorPathsToolStripMenuItem1.Checked)
            {
                Properties.Settings.Default.MirrorMode = true;
            }
            else if (!mirrorPathsToolStripMenuItem1.Checked)
            {
                Properties.Settings.Default.MirrorMode = false;
            }
            UpdateMirrorModeStatusLabel();
        }

        #region Collision Mask Methods

        private void lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int row = GetLBSender(sender);
            if (tcf != null && row != -1)
            {
                ListBox lb = (ListBox) sender;
                if (Properties.Settings.Default.MirrorMode)
                {
                    tcf.CollisionPath1[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
                    CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                    tcf.CollisionPath2[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
                    CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                    CollisionList.Refresh();
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
                        CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                        CollisionList.Refresh();
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].Collision[row] = (byte)lb.SelectedIndex;
                        CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
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

        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            int box = GetCBSender(sender);
            if (tcf != null && box != -1)
            {
                CheckBox cb = (CheckBox)sender;
                if (Properties.Settings.Default.MirrorMode)
                {
                        tcf.CollisionPath1[curColisionMask].HasCollision[box] = cb.Checked;
                        CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                        tcf.CollisionPath2[curColisionMask].HasCollision[box] = cb.Checked;
                        CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                        CollisionList.Refresh();
                }
                else
                {
                    if (!showPathB)
                    {
                        tcf.CollisionPath1[curColisionMask].HasCollision[box] = cb.Checked;
                        CollisionListImgA[curColisionMask] = tcf.CollisionPath1[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
                        CollisionList.Refresh();
                    }
                    if (showPathB)
                    {
                        tcf.CollisionPath2[curColisionMask].HasCollision[box] = cb.Checked;
                        CollisionListImgB[curColisionMask] = tcf.CollisionPath2[curColisionMask].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0));
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
                ListBox list = (ListBox)sender;
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

        private void saveUncompressedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != null) //Did we open a file?
            {
                tcf.WriteUnc(filepath);
            }
            else //if not then use Save As instead
            {
                saveAsUncompressedToolStripMenuItem_Click(this, e);
            }
        }

        private void saveAsUncompressedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Uncompressed As...";
            dlg.DefaultExt = ".bin";
            dlg.Filter = "RSDKv5 Tileconfig Files (*.bin)|*.bin";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                tcf.WriteUnc(dlg.FileName); //Write Uncompressed
            }
        }
        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void InitializeComponents()
        {
            this.SuspendLayout();
            // 
            // Mainform
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Mainform";
            this.ResumeLayout(false);

        }

        public class PictureBoxNearestNeighbor : PictureBox
        {
            protected override void OnPaint(PaintEventArgs paintEventArgs)
            {
                paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                base.OnPaint(paintEventArgs);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!changingModes)
            {
                unCheckModes();
                Properties.Settings.Default.ViewerSetting = 0;
                radioButton1.Checked = true;
                CollisionPicBox.Visible = true;
                Properties.Settings.Default.Save();
                changingModes = false;
                RefreshUI();

            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!changingModes)
            {
                unCheckModes();
                radioButton2.Checked = true;
                Properties.Settings.Default.ViewerSetting = 1;
                Properties.Settings.Default.Save();
                TilePicBox.Visible = true;
                changingModes = false;
                RefreshUI();

            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (!changingModes)
            {
                unCheckModes();
                Properties.Settings.Default.ViewerSetting = 2;
                Properties.Settings.Default.Save();
                radioButton3.Checked = true;
                overlayPicBox.Visible = true;
                changingModes = false;
                RefreshUI();

            }
        }

        void unCheckModes()
        {
            changingModes = true;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            TilePicBox.Visible = false;
            CollisionPicBox.Visible = false;
            overlayPicBox.Visible = false;

        }

        private void tileViewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (lockRadioButtons == false)
            {
                uncheckListViews();
                Properties.Settings.Default.ListSetting = 1;
                Properties.Settings.Default.Save();
                tileViewRadioButton.Checked = true;
                lockRadioButtons = false;
                refreshCollision();
            }
        }

        private void collisionViewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (lockRadioButtons == false)
            {
                uncheckListViews();
                Properties.Settings.Default.ListSetting = 0;
                Properties.Settings.Default.Save();
                collisionViewRadioButton.Checked = true;
                lockRadioButtons = false;
                refreshCollision();
            }
        }

        void uncheckListViews()
        {
            lockRadioButtons = true;
            collisionViewRadioButton.Checked = false;
            tileViewRadioButton.Checked = false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!showPathB)
            {
                if (Clipboard.ContainsData("TileManiacCollision"))
                {
                    var copyData = Clipboard.GetData("TileManiacCollision") as TilesConfig.CollisionMask;
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
                if (Clipboard.ContainsData("TileManiacCollision"))
                {
                    var copyData = Clipboard.GetData("TileManiacCollision") as TilesConfig.CollisionMask;
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
                    if (Properties.Settings.Default.ListSetting == 0)
                    {
                        CollisionListImgA.Add(tcf.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
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
                    if (Properties.Settings.Default.ListSetting == 0)
                    {
                        CollisionListImgB.Add(tcf.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
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

        private void openSingleCollisionMaskToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Import CollisionMask...";
            dlg.DefaultExt = ".rcm";
            dlg.Filter = "Singular RSDKv5 CollisionMask (*.rcm)|*.rcm";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                RSDKv5.Reader Reader1 = new RSDKv5.Reader(dlg.FileName);
                RSDKv5.Reader Reader2 = new RSDKv5.Reader(dlg.FileName);
                tcf.CollisionPath1[curColisionMask] = new RSDKv5.TilesConfig.CollisionMask(Reader1);
                Reader1.Close();
                tcf.CollisionPath2[curColisionMask] = new RSDKv5.TilesConfig.CollisionMask(Reader2);
                Reader2.Close();
            }
            RefreshUI();
            //RefreshCollisionList(true);
            
            
        }

        private void exportCurrentCollisionMaskAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Export As...";
            dlg.DefaultExt = ".rcm";
            dlg.Filter = "Singular RSDKv5 CollisionMask (*.rcm)|*.rcm";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
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

        private void importFromOlderRSDKVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Compressed";
            dlg.DefaultExt = ".bin";
            dlg.Filter = "RSDK ColllisionMask Files (CollisionMasks.bin)|CollisionMasks.bin";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                curColisionMask = 0; //Set the current collision mask to zero (avoids rare errors)
                filepath = dlg.FileName;
                tcf = new RSDKv5.TilesConfig();
                tcfBak = new RSDKv5.TilesConfig();
                RSDKv4.CollisionMask tcfOLD = new RSDKv4.CollisionMask(dlg.FileName);
                string t = filepath.Replace("CollisionMasks.bin", "16x16tiles.gif"); //get the path to the stage's tileset
                LoadTileSet(new Bitmap(t)); //load each 16x16 tile into the list


                CollisionListImgA.Clear();
                CollisionListImgB.Clear();
                CollisionList.Images.Clear();

                for (int i = 0; i < 1024; i++)
                {
                    CollisionListImgA.Add(tcfOLD.CollisionPath1[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));
                    CollisionListImgB.Add(tcfOLD.CollisionPath2[i].DrawCMask(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0)));

                    CollisionList.Images.Add(CollisionListImgA[i]);
                    CollisionList.Images.Add(CollisionListImgB[i]);

                    tcf.CollisionPath1[i].Collision = tcfOLD.CollisionPath1[i].Collision;
                    tcf.CollisionPath1[i].HasCollision = tcfOLD.CollisionPath1[i].HasCollision;
                    tcf.CollisionPath1[i].IsCeiling = tcfOLD.CollisionPath1[i].isCeiling;
                    tcf.CollisionPath1[i].momentum = tcfOLD.CollisionPath1[i].momentum;
                    tcf.CollisionPath1[i].physics = tcfOLD.CollisionPath1[i].physics;
                    tcf.CollisionPath1[i].slopeAngle = tcfOLD.CollisionPath1[i].slopeAngle;
                    tcf.CollisionPath1[i].special = 0;
                    tcf.CollisionPath1[i].unknown = tcfOLD.CollisionPath1[i].unknown;

                    tcf.CollisionPath2[i].Collision = tcfOLD.CollisionPath2[i].Collision;
                    tcf.CollisionPath2[i].HasCollision = tcfOLD.CollisionPath2[i].HasCollision;
                    tcf.CollisionPath2[i].IsCeiling = tcfOLD.CollisionPath2[i].isCeiling;
                    tcf.CollisionPath2[i].momentum = tcfOLD.CollisionPath2[i].momentum;
                    tcf.CollisionPath2[i].physics = tcfOLD.CollisionPath2[i].physics;
                    tcf.CollisionPath2[i].slopeAngle = tcfOLD.CollisionPath2[i].slopeAngle;
                    tcf.CollisionPath2[i].special = 0;
                    tcf.CollisionPath2[i].unknown = tcfOLD.CollisionPath2[i].unknown;
                }
                CollisionList.SelectedIndex = curColisionMask - 1;
                CollisionList.Refresh();

                RefreshUI(); //update the UI
            }
        }

        private void gridPicBox_Paint(object sender, PaintEventArgs e)
        {  
            if (Properties.Settings.Default.ShowGrid)
            {
                Graphics g = e.Graphics;
                g.PixelOffsetMode = PixelOffsetMode.None;
                int numOfCells = (int)Properties.Settings.Default.DevInt2;
                int cellSize = (int)Properties.Settings.Default.DevInt1;
                Pen p = new Pen(Color.Black);
                p.DashOffset = (float)Properties.Settings.Default.DevInt3;

                for (int i = 0; i < numOfCells; i++)
                {
                    // Vertical
                    g.DrawLine(p, i * cellSize, 0, i * cellSize, numOfCells * cellSize);
                    // Horizontal
                    g.DrawLine(p, 0, i * cellSize, numOfCells * cellSize, i * cellSize);
                }
            }

        }

        private void developerInterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeveloperTerminal developerTerminal = new DeveloperTerminal();
            developerTerminal.Show();
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showGridToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.ShowGrid = true;
                RefreshUI();
            }
            else
            {
                Properties.Settings.Default.ShowGrid = false;
                RefreshUI();
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Mainform_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Click(object sender, EventArgs e)
        {
            UpdateCollisionalGrid(sender, e);
        }

        Point GetRowColIndex(TableLayoutPanel tlp, Point point)
        {
            if (point.X > tlp.Width || point.Y > tlp.Height)
                return new Point(-1, -1);

            int w = tlp.Width;
            int h = tlp.Height;
            int[] widths = tlp.GetColumnWidths();

            int i;
            for (i = widths.Length - 1; i >= 0 && point.X < w; i--)
                w -= widths[i];
            int col = i + 1;

            int[] heights = tlp.GetRowHeights();
            for (i = heights.Length - 1; i >= 0 && point.Y < h; i--)
                h -= heights[i];

            int row = i + 1;

            return new Point(col, row);
        }

        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHeldDown = true;
            UpdateCollisionalGrid(sender,e);
        }

        private void UpdateCollisionalGrid(object sender, EventArgs e)
        {
            var cellPos = GetRowColIndex(tableLayoutPanel1, tableLayoutPanel1.PointToClient(Cursor.Position));
            if (cellPos.Y >= 16) return;
            switch (cellPos.X)
            {
                case 0:
                    lb00.SelectedIndex = cellPos.Y;
                    break;
                case 1:
                    lb01.SelectedIndex = cellPos.Y;
                    break;
                case 2:
                    lb02.SelectedIndex = cellPos.Y;
                    break;
                case 3:
                    lb03.SelectedIndex = cellPos.Y;
                    break;
                case 4:
                    lb04.SelectedIndex = cellPos.Y;
                    break;
                case 5:
                    lb05.SelectedIndex = cellPos.Y;
                    break;
                case 6:
                    lb06.SelectedIndex = cellPos.Y;
                    break;
                case 7:
                    lb07.SelectedIndex = cellPos.Y;
                    break;
                case 8:
                    lb08.SelectedIndex = cellPos.Y;
                    break;
                case 9:
                    lb09.SelectedIndex = cellPos.Y;
                    break;
                case 10:
                    lb10.SelectedIndex = cellPos.Y;
                    break;
                case 11:
                    lb11.SelectedIndex = cellPos.Y;
                    break;
                case 12:
                    lb12.SelectedIndex = cellPos.Y;
                    break;
                case 13:
                    lb13.SelectedIndex = cellPos.Y;
                    break;
                case 14:
                    lb14.SelectedIndex = cellPos.Y;
                    break;
                case 15:
                    lb15.SelectedIndex = cellPos.Y;
                    break;
            }

            //MessageBox.Show("Cell chosen: (" + cellPos.Value + ")");
        }

        private void tableLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseHeldDown = false;
            RefreshUI();
        }

        private void tableLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHeldDown)
            {
                UpdateCollisionalGrid(null, null);
            }
            if (e.Button != MouseButtons.None)
            {
                mouseHeldDown = true;
            }

        }

        private void tableLayoutPanel1_MouseHover(object sender, EventArgs e)
        {
            if (mouseHeldDown)
            {
                UpdateCollisionalGrid(null, null);
            }
        }

        private void cb02_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHeldDown = true;
            mouseButtonHeld = e.Button;
        }

        private void cb02_MouseHover(object sender, EventArgs e)
        {
            if (mouseHeldDown)
            {
                if (mouseButtonHeld == MouseButtons.Left)
                {
                    checkUncheckBox(true);
                }
                else if (mouseButtonHeld == MouseButtons.Right)
                {
                    checkUncheckBox(false);
                }

            }
        }

        private void cb02_MouseUp(object sender, MouseEventArgs e)
        {
            mouseHeldDown = false;
            RefreshUI();
        }

        private void cb02_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHeldDown)
            {
                if (mouseButtonHeld == MouseButtons.Left)
                {
                    checkUncheckBox(true);
                }
                else if (mouseButtonHeld == MouseButtons.Right)
                {
                    checkUncheckBox(false);
                }

            }
            if (e.Button != MouseButtons.None)
            {
                mouseHeldDown = true;
            }
        }

        private void checkUncheckBox(bool state = false)
        {
            var cellPos = GetRowColIndex(tableLayoutPanel2, tableLayoutPanel2.PointToClient(Cursor.Position));
            switch (cellPos.X)
            {
                case 0:
                    cb00.Checked = state;
                    break;
                case 1:
                    cb01.Checked = state;
                    break;
                case 2:
                    cb02.Checked = state;
                    break;
                case 3:
                    cb03.Checked = state;
                    break;
                case 4:
                    cb04.Checked = state;
                    break;
                case 5:
                    cb05.Checked = state;
                    break;
                case 6:
                    cb06.Checked = state;
                    break;
                case 7:
                    cb07.Checked = state;
                    break;
                case 8:
                    cb08.Checked = state;
                    break;
                case 9:
                    cb09.Checked = state;
                    break;
                case 10:
                    cb10.Checked = state;
                    break;
                case 11:
                    cb11.Checked = state;
                    break;
                case 12:
                    cb12.Checked = state;
                    break;
                case 13:
                    cb13.Checked = state;
                    break;
                case 14:
                    cb14.Checked = state;
                    break;
                case 15:
                    cb15.Checked = state;
                    break;
            }
        }

        private void classicViewModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (classicViewModeToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.ClassicMode = true;
            }
            else
            {
                Properties.Settings.Default.ClassicMode = false;
            }
            RefreshUI();
        }

        private void overlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewAppearanceMode = 0;
            UpdateViewApperancePlusButtons();

        }

        private void collisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewAppearanceMode = 1;
            UpdateViewApperancePlusButtons();
        }

        private void UpdateViewApperancePlusButtons()
        {
            switch (Properties.Settings.Default.ViewAppearanceMode)
            {
                case 0:
                    overlayToolStripMenuItem.Checked = true;
                    collisionToolStripMenuItem.Checked = false;
                    break;
                case 1:
                    overlayToolStripMenuItem.Checked = false;
                    collisionToolStripMenuItem.Checked = true;
                    break;
            }
            RefreshUI();
        }

        private void UpdateMirrorModeStatusLabel()
        {
            if (Properties.Settings.Default.MirrorMode)
            {
                mirrorModeStatusLabel.Text = "Mirror Mode: ON";
            }
            else
            {
                mirrorModeStatusLabel.Text = "Mirror Mode: OFF";
            }
        }

        private void pathAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                tcf.CollisionPath1[curColisionMask].Collision = tcfBak.CollisionPath1[curColisionMask].Collision;
                RefreshUI();
            }
        }

        private void pathBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                tcf.CollisionPath2[curColisionMask] = tcfBak.CollisionPath2[curColisionMask];
                RefreshUI();
            }
        }

        private void bothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("All progress for this Mask will be undone! Are you sure?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                tcf.CollisionPath1[curColisionMask] = tcfBak.CollisionPath1[curColisionMask];
                tcf.CollisionPath2[curColisionMask] = tcfBak.CollisionPath2[curColisionMask];
                RefreshUI();
            }
        }

        private void newInstanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mainform = new Mainform();
            mainform.Show();
        }

        private void flipTileHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void flipTileVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
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
}
