using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace TileManiac
{
	public class PictureBoxNearestNeighbor : PictureBox
	{
		protected override void OnPaint(PaintEventArgs paintEventArgs)
		{
			paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			base.OnPaint(paintEventArgs);
		}
	}
}
