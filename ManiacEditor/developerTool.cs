using ManiacEditor.Entity_Renders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public partial class DeveloperTerminal : Form
    {
        // For Interger Changer; Change to the Value you want to tweak
        int valueINI = Properties.EditorState.Default.developerInt;

        public DeveloperTerminal()
        {
            InitializeComponent();
        }

        private void DeveloperTerminal_Load(object sender, EventArgs e)
        {

        }
    }
}
