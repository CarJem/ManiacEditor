using System;
using System.Windows;
using RSDKv5;
using MessageBox = System.Windows.MessageBox;

namespace ManiacEditor.Controls.Object_Manager
{
    /// <summary>
    /// Interaction logic for AddAttributeWindow.xaml
    /// </summary>
    public partial class AddAttributeWindow : Window
	{
		private SceneObject[] objs;

		public AddAttributeWindow(SceneObject[] objs)
		{
			InitializeComponent();

			this.objs = objs;

			foreach (AttributeTypes attType in Enum.GetValues(typeof(AttributeTypes)))
				typeBox.Items.Add(attType);

			typeBox.SelectedIndex = 0;
		}

		private void addBtn_Click(object sender, RoutedEventArgs e)
		{
			if (nameBox.Text.Length <= 0)
			{
				MessageBox.Show("You must enter an attribute name!",
					"No Name!",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return;
			}

			if (objs.Length == 1 && objs[0].HasAttributeOfName(nameBox.Text))
			{
				MessageBox.Show("There is already an attribute with the name \"" + nameBox.Text + "\"!\nChoose a different name and try again.",
					"Name Conflict!",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return;
			}

			bool defaultFailed = false;
			foreach (SceneObject obj in objs)
			{
				if (!obj.HasAttributeOfName(nameBox.Text))
				{
					obj.AddAttribute(nameBox.Text, (AttributeTypes)typeBox.SelectedItem);

					if (!defaultFailed && defaultBox.Text.Length > 0)
					{
						try
						{
							foreach (SceneEntity entity in obj.Entities)
							{
								AttributeValue attVal = entity.attributesMap[nameBox.Text];

								switch ((AttributeTypes)typeBox.SelectedItem)
								{
									case AttributeTypes.INT8:
										attVal.ValueInt8 = sbyte.Parse(defaultBox.Text);
										break;

									case AttributeTypes.INT16:
										attVal.ValueInt16 = short.Parse(defaultBox.Text);
										break;

									case AttributeTypes.INT32:
										attVal.ValueInt32 = int.Parse(defaultBox.Text);
										break;

									case AttributeTypes.UINT8:
										attVal.ValueUInt8 = byte.Parse(defaultBox.Text);
										break;

									case AttributeTypes.UINT16:
										attVal.ValueUInt16 = ushort.Parse(defaultBox.Text);
										break;

									case AttributeTypes.UINT32:
										attVal.ValueUInt32 = uint.Parse(defaultBox.Text);
										break;

									case AttributeTypes.ENUM:
										attVal.ValueEnum = int.Parse(defaultBox.Text);
										break;

									case AttributeTypes.BOOL:
										attVal.ValueBool = bool.Parse(defaultBox.Text);
										break;

									case AttributeTypes.COLOR:
										attVal.ValueColor = getColor(defaultBox.Text);
										break;

									case AttributeTypes.VECTOR2:
										attVal.ValueVector2 = getPosition(defaultBox.Text);
										break;

									case AttributeTypes.STRING:
										attVal.ValueString = defaultBox.Text;
										break;
								}
							}
						}
						catch (Exception ex)
						{
							defaultFailed = true;
							MessageBox.Show("Setting default value for existing entities failed for the following reason:\n\n" + ex.Message + "\n\nA blank value be used instead.");
						}
					}
				}
			}

			this.DialogResult = true;
		}

		private RSDKv5.Color getColor(string str)
		{
			byte[] vals = new byte[3];
			for (int i = 0; i < 2; i++)
			{
				string sub = str.Substring(0, str.IndexOf(','));
				str = str.Substring(str.IndexOf(',') + 1);
				vals[i] = byte.Parse(sub);
			}
			vals[2] = byte.Parse(str);

			return new RSDKv5.Color(vals[0], vals[1], vals[2], 255);    // alpha is always 255
		}

		private Position getPosition(string str)
		{
			short[] vals = new short[2];
			string sub = str.Substring(0, str.IndexOf(','));
			str = str.Substring(str.IndexOf(',') + 1);
			vals[0] = short.Parse(sub);
			vals[1] = short.Parse(str);

			return new Position(vals[0], vals[1]);
		}

		private void cancelBtn_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
