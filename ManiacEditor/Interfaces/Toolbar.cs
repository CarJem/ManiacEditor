using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ManiacEditor.Interfaces
{
	public class ToolBar : System.Windows.Controls.ToolBar
	{
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var overflowPanel = base.GetTemplateChild("PART_ToolBarOverflowPanel") as ToolBarOverflowPanel;
			if (overflowPanel != null)
			{
				overflowPanel.Background = OverflowPanelBackground ?? Background;
				overflowPanel.Margin = new System.Windows.Thickness(0);
			}
		}

		public Brush OverflowPanelBackground
		{
			get;
			set;
		}
	}
}