using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ManiacEditor.Extensions
{
	public static class ConsoleExtensions
	{
		#region Console Window Extensions

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
		public static void ShowConsoleWindow()
		{
			var handle = GetConsoleWindow();

			if (handle == IntPtr.Zero)
			{
				AllocConsole();
			}
			else
			{
				ShowWindow(handle, SW_SHOW);
			}
		}
		public static void HideConsoleWindow()
		{
			var handle = GetConsoleWindow();

			ShowWindow(handle, SW_HIDE);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		#endregion

		#region Maniac Console Window

		public class ConsoleContent : INotifyPropertyChanged
		{
			string consoleInput = string.Empty;
			ObservableCollection<string> consoleOutput = new ObservableCollection<string>();

			public string ConsoleInput
			{
				get
				{
					return consoleInput;
				}
				set
				{
					consoleInput = value;
					OnPropertyChanged("ConsoleInput");
				}
			}

			public ObservableCollection<string> ConsoleOutput
			{
				get
				{
					return consoleOutput;
				}
				set
				{
					consoleOutput = value;
					OnPropertyChanged("ConsoleOutput");
				}
			}

			public void RunCommand()
			{
				ConsoleOutput.Add(ConsoleInput);
				// do your stuff here.
				ConsoleInput = String.Empty;
			}


			public event PropertyChangedEventHandler PropertyChanged;
			void OnPropertyChanged(string propertyName)
			{
				if (null != PropertyChanged)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private static ConsoleContent ManiacConsoleContext { get; set; } = new ConsoleContent();

		public static Controls.Misc.Dev.ManiacConsole ManiacConsole { get; set; }

		public static void ToggleManiacConsole()
		{
			if (ManiacConsole == null) ShowManiacConsole();
			else CloseManiacConsole();
		}

		public static void ShowManiacConsole(bool WaitForClose = false)
		{
			ManiacConsole = new Controls.Misc.Dev.ManiacConsole(ManiacConsoleContext);
			if (!WaitForClose) ManiacConsole.Show();
			else ManiacConsole.ShowDialog();
		}

		public static void CloseManiacConsole()
		{
			if (ManiacConsole != null) ManiacConsole.Close();
			ManiacConsole = null;
		}

		public static void PrintManiacOutput(string value, bool ShowTimeStamp = true)
		{
			ManiacConsoleContext.ConsoleInput = (ShowTimeStamp ? string.Format("[{0}] {1}", DateTime.Now, value) : value);
			ManiacConsoleContext.RunCommand();

			if (ManiacConsole != null) ManiacConsole.Update();
		}

		#endregion


		#region Output

		public static bool UseDebugOutput { get; set; } = true;
		public static bool UseConsoleOutput { get; set; } = false;
		public static bool UseManiacConsole { get; set; } = true;

		public static void Print() 
		{
			if (UseDebugOutput) System.Diagnostics.Debug.Print(Environment.NewLine);
			if (UseConsoleOutput) Console.WriteLine();
			if (UseManiacConsole) PrintManiacOutput(Environment.NewLine, false);
		}
        public static void Print(float value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(int value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(uint value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(long value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(ulong value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(object value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void PrintWithLog(string value)
		{
			Print(value);
			ManiacEditor.Methods.ProgramBase.Log.InfoFormat(value);
		}
		public static void Print(string value) 
		{
			if (UseDebugOutput) System.Diagnostics.Debug.Print(value);
			if (UseConsoleOutput) Console.WriteLine(value);
			if (UseManiacConsole) PrintManiacOutput(value);
		}
		public static void Print(string format, object arg0) 
		{
			var args = new object[] { arg0 };
			if (UseDebugOutput) System.Diagnostics.Debug.Print(format, args);
			if (UseConsoleOutput) Console.WriteLine(format, arg0);
			if (UseManiacConsole) PrintManiacOutput(string.Format(format, arg0));
		}
		public static void Print(string format, object arg0, object arg1)
		{
			var args = new object[] { arg0, arg1 };
			if (UseDebugOutput) System.Diagnostics.Debug.Print(format, args);
			if (UseConsoleOutput) Console.WriteLine(format, arg0, arg1);
			if (UseManiacConsole) PrintManiacOutput(string.Format(format, arg0, arg1));
		}
		public static void Print(string format, object arg0, object arg1, object arg2) 
		{
			var args = new object[] { arg0, arg1, arg2 };
			if (UseDebugOutput) System.Diagnostics.Debug.Print(format, args);
			if (UseConsoleOutput) Console.WriteLine(format, arg0, arg1, arg2);
			if (UseManiacConsole) PrintManiacOutput(string.Format(format, arg0, arg1, arg2));
		}
        public static void Print(string format, object arg0, object arg1, object arg2, object arg3) 
		{
			var args = new object[] { arg0, arg1, arg2, arg3 };
			if (UseDebugOutput) System.Diagnostics.Debug.Print(format, args);
			if (UseConsoleOutput) Console.WriteLine(format, arg0, arg1, arg2, arg3);
			if (UseManiacConsole) PrintManiacOutput(string.Format(format, arg0, arg1, arg2, arg3));
		}
		public static void Print(string format, params object[] arg)
		{
			if (UseDebugOutput) System.Diagnostics.Debug.Print(format, arg);
			if (UseConsoleOutput) Console.WriteLine(format, arg);
			if (UseManiacConsole) PrintManiacOutput(string.Format(format, arg));
		}
		public static void Print(char[] buffer, int index, int count)
		{
			//if (UseConsoleOutput) Console.WriteLine(buffer, index, count);
		}
		public static void Print(decimal value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(char[] buffer)
		{
			//if (UseConsoleOutput) Console.WriteLine(buffer);
		}
		public static void Print(char value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(bool value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}
		public static void Print(double value)
		{
			//if (UseConsoleOutput) Console.WriteLine(value);
		}

		#endregion

	}
}
