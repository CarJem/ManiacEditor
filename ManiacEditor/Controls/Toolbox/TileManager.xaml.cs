using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;

namespace ManiacEditor.Controls.Toolbox
{
    public partial class TileManager : Window
    {
        #region Definitions

        List<BitmapSource> TileList { get; set; } = new List<BitmapSource>();

        #endregion

        #region Init
        public TileManager()
        {
            InitializeComponent();
            TilesList.SelectedIndexChanged += TilesList_SelectedIndexChanged;
            TilesList.TileList.SelectionMode = SelectionMode.Extended;
        }
        #endregion

        #region Methods

        private void RemoveDuplicateTiles()
        {
            List<Bitmap> Entries = TileList.ConvertAll(x => Extensions.ImageExtensions.ToWinFormsBitmap(x)).ToList();
            List<int> SelectedIndexes = new List<int>();
            List<BitmapSource> SelectedItems = new List<BitmapSource>();
            for (int i = 0; i < Entries.Count; i++)
            {
                int Count = 0;
                List<int> Indexes = new List<int>();
                for (int j = 0; j < Entries.Count; j++)
                {
                    if (Extensions.ImageExtensions.DrawingBitmapEquals(Entries[i], Entries[j]))
                    {
                        Count = Count + 1;
                        Indexes.Add(j);
                    }
                }

                if (Count > 1)
                {
                    int first = Indexes.First();
                    Indexes.Remove(first);
                    SelectedIndexes.AddRange(Indexes);
                }
            }
            foreach (int i in SelectedIndexes)
            {
                SelectedItems.Add(TileList[i]);
            }
            TileList.RemoveAll(x => SelectedItems.Contains(x));
            UpdateList();
        }
        private void RemoveSelectedTiles()
        {
            List<int> SelectedIndexes = new List<int>();
            List<BitmapSource> SelectedItems = new List<BitmapSource>();
            foreach (var entry in TilesList.TileList.SelectedItems)
            {
                SelectedIndexes.Add(TileList.IndexOf(entry as BitmapSource));
            }
            foreach (int i in SelectedIndexes)
            {
                SelectedItems.Add(TileList[i]);
            }
            TileList.RemoveAll(x => SelectedItems.Contains(x));
            UpdateList();
        }
        private void UpdatePreview()
        {
            if (TileList == null || TileList.Count - 1 < TilesList.SelectedIndex || TilesList.SelectedIndex < 0) return;
            TileView.Stretch = Stretch.Uniform;
            TileView.Source = TileList[TilesList.SelectedIndex];
        }
        public void LoadStageTiles()
        {
            TilesReload(Methods.Solution.CurrentSolution.CurrentTiles?.Image);
        }
        public void TilesReload(Classes.Rendering.GIF Image)
        {
            TileList.Clear();
            TileList = GetCurrentTileImages(Image);
            UpdateList();
        }
        public void UpdateList()
        {
            TilesList.Images.Clear();
            TilesList.Images = new List<System.Windows.Media.ImageSource>(TileList);

            int indexStorage = TilesList.SelectedIndex;
            TilesList.ImageSize = 32;
            TilesList.SelectedIndex = -1;
            TilesList.SelectedIndex = indexStorage;
            TilesList.Invalidate(true);
            UpdatePreview();
        }
        private List<System.Windows.Media.Imaging.BitmapSource> GetCurrentTileImages(Classes.Rendering.GIF Image)
        {
            List<System.Windows.Media.Imaging.BitmapSource> TilesList = new List<System.Windows.Media.Imaging.BitmapSource>();
            if (Image != null)
            {
                int columns = Image.Width / 16;
                int rows = Image.Height / 16;

                int maxTiles = columns * rows;

                for (int i = 0; i < maxTiles; i++)
                {
                    TilesList.Add(Image.GetImageSource(new System.Drawing.Rectangle(0, 16 * i, 16, 16), false, false));
                }
            }
            return TilesList;
        }
        public void Convert()
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Exported Tiles|*.gif",
                DefaultExt = "gif",
                InitialDirectory = ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory,
                RestoreDirectory = false
            };
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = "Converted Tiles|*.gif",
                    DefaultExt = "gif",
                    InitialDirectory = ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory,
                    RestoreDirectory = false,
                    FileName = "16x16Tiles_Converted"
                };
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Classes.Scene.EditorTiles.ImportIndexed(open.FileName, save.FileName);
                }
            }
        }
        public System.Drawing.Bitmap GenerateBitmap()
        {
            int lastIndex = -1;
            try
            {
                int tilesPerRow = 1;

                int columns = 1;
                int rows = TileList.Count;

                int maxTiles = columns * rows;
                int tileSize = 16;

                int imageWidth = tileSize;
                int imageHeight = (maxTiles / tilesPerRow) * tileSize;

                Bitmap bitmap = new Bitmap(imageWidth, imageHeight);

                int row = 0;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    for (int tileIndex = 0; tileIndex < maxTiles - 1;)
                    {
                        for (int col = 0; col < tilesPerRow;)
                        {
                            if (tileIndex > maxTiles - 1) break;
                            else
                            {
                                g.DrawImage(Extensions.ImageExtensions.ToWinFormsBitmap(TileList[tileIndex]), col * 16, row * 16);
                                tileIndex++;
                                lastIndex = tileIndex;
                            }
                            col++;
                        }
                        row++;
                    }
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("[" + lastIndex + "]" + ex.Message);
                return null;
            }
        }
        public void Save(bool saveAs = false)
        {
            int columns = GenerationsLib.WPF.IntergerPrompt.ShowDialog("Enter Column Amount...", "Enter the Amount of Columns You wish this Exported Sheet to Have", 1);
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Exported Tiles|*.gif",
                DefaultExt = "gif",
                InitialDirectory = ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory,
                RestoreDirectory = false,
                FileName = "16x16Tiles_Exported"
            };
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var bitmap = GenerateBitmap();
                Classes.Rendering.GIF ExportableFile = new Classes.Rendering.GIF(bitmap);
                Classes.Scene.EditorTiles.Export(ExportableFile, save.FileName, columns);
            }
        }
        public void Load()
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "GIF Files|*.gif",
                DefaultExt = "gif",
                InitialDirectory = ManiacEditor.Methods.Solution.SolutionPaths.SceneFile_Source.SourceDirectory,
                RestoreDirectory = false
            };
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TilesReload(Classes.Scene.EditorTiles.Import(open.FileName));
            }
        }

        #endregion

        #region Events
        private void TilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadStageTilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadStageTiles();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Save(true);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedTiles();
        }
        private void ConvertMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        private void RemoveDuplicatesButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveDuplicateTiles();
        }

        #endregion
    }
}
