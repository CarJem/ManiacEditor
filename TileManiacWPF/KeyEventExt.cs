namespace TileManiac
{
	public static class KeyEventExtsTM
	{
		public static System.Windows.Forms.KeyEventArgs ToWinforms(this System.Windows.Input.KeyEventArgs keyEventArgs)
		{
			// So far this ternary remained pointless, might be useful in some very specific cases though
			var wpfKey = keyEventArgs.Key == System.Windows.Input.Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
			var winformModifiers = keyEventArgs.KeyboardDevice.Modifiers.ToWinforms();
			var winformKeys = (System.Windows.Forms.Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(wpfKey);
			return new System.Windows.Forms.KeyEventArgs(winformKeys | winformModifiers);
		}

		public static System.Windows.Forms.Keys ToWinforms(this System.Windows.Input.ModifierKeys modifier)
		{
			var retVal = System.Windows.Forms.Keys.None;
			if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Alt))
			{
				retVal |= System.Windows.Forms.Keys.Alt;
			}
			if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Control))
			{
				retVal |= System.Windows.Forms.Keys.Control;
			}
			if (modifier.HasFlag(System.Windows.Input.ModifierKeys.None))
			{
				// Pointless I know
				retVal |= System.Windows.Forms.Keys.None;
			}
			if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Shift))
			{
				retVal |= System.Windows.Forms.Keys.Shift;
			}
			if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Windows))
			{
				// Not supported lel
			}
			return retVal;
		}
	}
}
