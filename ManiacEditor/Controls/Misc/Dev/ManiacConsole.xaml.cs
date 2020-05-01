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
using System.ComponentModel;
using System.Collections.ObjectModel;
using ManiacEditor.Extensions;

namespace ManiacEditor.Controls.Misc.Dev
{
    /// <summary>
    /// Interaction logic for ManiacConsole.xaml
    /// </summary>
    public partial class ManiacConsole : Window
    {
        ConsoleExtensions.ConsoleContent ConsoleDataContext;

        public ManiacConsole()
        {
            InitializeComponent();
            ConsoleDataContext = new ConsoleExtensions.ConsoleContent();
            DataContext = ConsoleDataContext;
            Loaded += MainWindow_Loaded;
        }

        public ManiacConsole(ConsoleExtensions.ConsoleContent consoleContent)
        {
            InitializeComponent();
            ConsoleDataContext = consoleContent;
            DataContext = ConsoleDataContext;
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InputBlock.KeyDown += InputBlock_KeyDown;
            InputBlock.Focus();
        }


        public void Update()
        {
            Scroller.ScrollToBottom();
        }

        void InputBlock_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.Key == Key.Enter)
            {
                dc.ConsoleInput = InputBlock.Text;
                dc.RunCommand();
                InputBlock.Focus();
                Scroller.ScrollToBottom();
            }
            */
        }
    }
}
