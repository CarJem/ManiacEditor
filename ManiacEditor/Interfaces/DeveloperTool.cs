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
        public Editor EditorInstance;

        public DeveloperTerminal(Editor instance)
        {
            InitializeComponent();
            EditorInstance = instance;
        }

        private void DeveloperTerminal_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
