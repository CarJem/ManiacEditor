using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Controls;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Clipboard = System.Windows.Clipboard;
using Color = System.Drawing.Color;
using DataObject = System.Windows.DataObject;
using File = System.IO.File;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using ListBoxItem = System.Windows.Controls.ListBoxItem;
using ManiacEditor.Controls.Global;
using ManiacEditor.Enums;
using ManiacEditor.Events;
using ManiacEditor.Extensions;
using System.Windows.Forms.Integration;

namespace ManiacEditor.Classes.Prefrences
{
    public static class RecentsRefrenceState
    {
        #region Collections
        public static IList<Tuple<MenuItem, ListBoxItem>> RecentSceneItems;
        public static IList<Tuple<MenuItem, ListBoxItem>> RecentDataSourceItems;
        #endregion

        #region Init


        private static Controls.Editor.MainEditor Instance { get; set; }

        public static void UpdateInstance(Controls.Editor.MainEditor _instance)
        {
            Instance = _instance;

            RecentSceneItems = new List<Tuple<MenuItem, ListBoxItem>>();
            RecentDataSourceItems = new List<Tuple<MenuItem, ListBoxItem>>();
        }

        #endregion

        #region Recent Scenes Methods

        public static void RefreshRecentScenes()
        {
            if (Classes.Prefrences.SceneHistoryStorage.Collection.List.Count > 0)
            {

                Instance.MenuBar.NoRecentScenesItem.Visibility = Visibility.Collapsed;
                Instance.StartScreen.NoRecentsLabel1.Visibility = Visibility.Collapsed;
                CleanUpRecentScenesList();

                foreach (var RecentItem in Classes.Prefrences.SceneHistoryStorage.Collection.List)
                {
                    RecentSceneItems.Add(new Tuple<MenuItem, ListBoxItem>(CreateRecentScenesMenuLink(RecentItem.EntryName), CreateRecentScenesItem(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentSceneItems.Reverse())
                {
                    Instance.MenuBar.RecentScenes.Items.Insert(0, menuItem.Item1);
                    Instance.StartScreen.RecentScenesList.Items.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                Instance.MenuBar.NoRecentScenesItem.Visibility = Visibility.Visible;
                Instance.StartScreen.NoRecentsLabel1.Visibility = Visibility.Visible;
            }
        }
        private static MenuItem CreateRecentScenesMenuLink(string target, bool startScreenEntry = false)
        {
            MenuItem newItem = new MenuItem();
            TextBlock label = new TextBlock();

            label.Text = target.Replace("/n/n", Environment.NewLine);
            newItem.Tag = target;
            newItem.Header = label;
            newItem.Click += RecentSceneEntryActivate;
            return newItem;
        }
        private static ListBoxItem CreateRecentScenesItem(string target, bool startScreenEntry = false)
        {
            ListBoxItem newItem = new ListBoxItem();
            TextBlock label = new TextBlock();

            label.Text = target.Replace("/n/n", Environment.NewLine);
            newItem.Tag = target;
            newItem.Content = label;
            newItem.PreviewMouseLeftButtonUp += RecentSceneEntryClicked;
            newItem.KeyUp += RecentSceneEntryPressed;
            return newItem;
        }

        private static void RecentSceneEntryPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RecentSceneEntryActivate(sender, null);
            }
        }
        private static void RecentSceneEntryClicked(object sender, MouseButtonEventArgs e)
        {
            RecentSceneEntryActivate(sender, null);
        }


        private static void RecentSceneEntryActivate(object sender, EventArgs e)
        {
            if (ManiacEditor.Methods.Solution.SolutionLoader.UnloadScene() != true) return;

            string entryName;

            if (sender is MenuItem)
            {
                var menuItem = sender as MenuItem;
                entryName = menuItem.Tag.ToString();
            }
            else
            {
                var menuItem = sender as ListBoxItem;
                entryName = menuItem.Tag.ToString();
            }

            var item = Classes.Prefrences.SceneHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            ManiacEditor.Methods.Solution.SolutionLoader.OpenSceneFromSaveState(item);
            Classes.Prefrences.SceneHistoryStorage.AddRecentFile(item);
        }


        private static void CleanUpRecentScenesList()
        {
            foreach (var menuItem in RecentSceneItems)
            {
                menuItem.Item1.Click -= RecentSceneEntryActivate;
                menuItem.Item2.PreviewMouseLeftButtonUp -= RecentSceneEntryClicked;
                menuItem.Item2.KeyUp -= RecentSceneEntryPressed;
                Instance.MenuBar.RecentScenes.Items.Remove(menuItem.Item1);
                Instance.StartScreen.RecentScenesList.Items.Remove(menuItem.Item2);
            }
            RecentSceneItems.Clear();
        }

        #endregion

        #region Recent Data Sources Methods

        public static void RefreshDataSources()
        {
            if (ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.Collection.List.Count > 0)
            {

                Instance.MenuBar.NoRecentDataSources.Visibility = Visibility.Collapsed;
                Instance.StartScreen.NoRecentsLabel2.Visibility = Visibility.Collapsed;

                CleanUpDataSourcesList();

                foreach (var RecentItem in ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.Collection.List)
                {
                    RecentDataSourceItems.Add(new Tuple<MenuItem, ListBoxItem>(CreateRecentDataSourceMenuLink(RecentItem.EntryName), CreateRecentDataSourceItem(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentDataSourceItems.Reverse())
                {
                    Instance.MenuBar.RecentDataSources.Items.Insert(0, menuItem.Item1);
                    Instance.StartScreen.RecentDataContextList.Items.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                Instance.MenuBar.NoRecentDataSources.Visibility = Visibility.Visible;
                Instance.StartScreen.NoRecentsLabel2.Visibility = Visibility.Visible;
            }
        }
        private static MenuItem CreateRecentDataSourceMenuLink(string target, bool wrapText = false)
        {
            MenuItem newItem = new MenuItem();
            TextBlock label = new TextBlock();
            label.Text = target.Replace("/n/n", Environment.NewLine);
            if (wrapText) label.TextWrapping = TextWrapping.Wrap;
            newItem.Header = label;
            newItem.Tag = target;
            newItem.Click += RecentDataSourceActivate;
            return newItem;
        }
        private static ListBoxItem CreateRecentDataSourceItem(string target, bool wrapText = false)
        {
            ListBoxItem newItem = new ListBoxItem();
            TextBlock label = new TextBlock();
            label.Text = target.Replace("/n/n", Environment.NewLine);
            if (wrapText) label.TextWrapping = TextWrapping.Wrap;
            newItem.Content = label;
            newItem.Tag = target;
            newItem.PreviewMouseLeftButtonUp += RecentDataSourceClicked;
            newItem.KeyUp += RecentDataSourcePressed;
            return newItem;
        }
        public static void RecentDataSourceActivate(object sender, RoutedEventArgs e)
        {
            if (ManiacEditor.Methods.Solution.SolutionLoader.UnloadScene() != true) return;

            string entryName;

            if (sender is MenuItem)
            {
                var menuItem = sender as MenuItem;
                entryName = menuItem.Tag.ToString();
            }
            else
            {
                var menuItem = sender as ListBoxItem;
                entryName = menuItem.Tag.ToString();
            }


            var item = ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            ManiacEditor.Methods.Solution.SolutionLoader.OpenSceneSelectSaveState(item);
            ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.AddRecentFile(item);
        }
        private static void RecentDataSourcePressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RecentSceneEntryActivate(sender, null);
            }
        }
        private static void RecentDataSourceClicked(object sender, MouseButtonEventArgs e)
        {
            RecentSceneEntryActivate(sender, null);
        }
        private static void CleanUpDataSourcesList()
        {
            foreach (var menuItem in RecentDataSourceItems)
            {
                menuItem.Item1.Click -= RecentDataSourceActivate;
                menuItem.Item2.PreviewMouseLeftButtonUp -= RecentDataSourceClicked;
                menuItem.Item2.KeyUp -= RecentDataSourcePressed;
                Instance.MenuBar.RecentDataSources.Items.Remove(menuItem.Item1);
                Instance.StartScreen.RecentDataContextList.Items.Remove(menuItem.Item2);
            }
            RecentDataSourceItems.Clear();

        }


        #endregion
    }
}
