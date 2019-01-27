using RSDKv5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor.Interfaces
{
    public partial class GoToPlayerBoxForm : Form
    {
        int playerObjectCount = 0;
        public int selectedPlayer = 0;
        public Editor EditorInstance;
        public GoToPlayerBoxForm(Editor instance)
        {
            InitializeComponent();
            EditorInstance = instance;
            playerObjectCount = EditorInstance.playerObjectPosition.Count;
            for (int i = 0; i < playerObjectCount-1; i++)
            {
                Position pos = EditorInstance.playerObjectPosition[i].Position;
                String id = EditorInstance.playerObjectPosition[i].SlotID.ToString();
                String posText = "X: " + pos.X.High + " Y: " + pos.Y.High;
                comboBox1.Items.Add("[" + id + "] " + posText);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditorInstance.selectPlayerObject_GoTo = comboBox1.SelectedIndex;
            this.Close();
        }

        public int GetSelectedPlayer()
        {
            Debug.Print(selectedPlayer.ToString());
            return selectedPlayer;
        }
    }
}
