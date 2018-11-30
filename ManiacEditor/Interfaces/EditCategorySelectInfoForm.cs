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
    public partial class EditCategorySelectInfoForm : Form
    {
        public RSDKv5.GameConfig.Category Category;
        public List<RSDKv5.GameConfig.SceneInfo> Scenes;

        public EditCategorySelectInfoForm()
        {
            Category = new RSDKv5.GameConfig.Category();
            Scenes = new List<RSDKv5.GameConfig.SceneInfo>();
            InitializeComponent();
        }

        public EditCategorySelectInfoForm(RSDKv5.GameConfig.Category category, List<RSDKv5.GameConfig.SceneInfo> scenes)
        {
            if (category == null)
            {
                Category = new RSDKv5.GameConfig.Category();
                Scenes = new List<RSDKv5.GameConfig.SceneInfo>();
            }
            else
            {
                Category = category;
                Scenes = scenes;
            }

            InitializeComponent();
            textBox1_TextChanged(null, null);
        }

        private void EditSceneSelectInfoForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Category.Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Category.Name = textBox1.Text;
            Category.Scenes = Scenes;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //label4.Text = $"Data\\Stages\\{textBox2.Text}\\Scene{textBox3.Text}.bin";
        }
    }
}
