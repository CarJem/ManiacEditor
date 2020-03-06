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
using ManiacEditor.Controls.Global;
using ManiacEditor.Enums;
using ManiacEditor.EventHandlers;
using ManiacEditor.Extensions;
using System.Windows.Forms.Integration;

namespace ManiacEditor.Classes.Prefrences
{
    public static class RecentsRefrenceState
    {
        #region Collections
        public static IList<Tuple<MenuItem, MenuItem>> RecentSceneItems;
        public static IList<Tuple<MenuItem, MenuItem>> RecentDataSourceItems;
        #endregion

        #region Init


        private static Controls.Editor.MainEditor Instance { get; set; }

        public static void UpdateInstance(Controls.Editor.MainEditor _instance)
        {
            Instance = _instance;

            RecentSceneItems = new List<Tuple<MenuItem, MenuItem>>();
            RecentDataSourceItems = new List<Tuple<MenuItem, MenuItem>>();
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
                    RecentSceneItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentScenesMenuLink(RecentItem.EntryName), CreateRecentScenesMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentSceneItems.Reverse())
                {
                    Instance.MenuBar.RecentScenes.Items.Insert(0, menuItem.Item1);
                    Instance.StartScreen.RecentScenesList.Children.Insert(0, menuItem.Item2);
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
            newItem.Click += RecentSceneEntryClicked;
            return newItem;
        }
        public static void RecentSceneEntryClicked(object sender, RoutedEventArgs e)
        {
            if (ManiacEditor.Methods.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            Methods.Editor.Solution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = Classes.Prefrences.SceneHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            ManiacEditor.Methods.Editor.SolutionLoader.OpenSceneFromSaveState(item);
            Classes.Prefrences.SceneHistoryStorage.AddRecentFile(item);
        }
        private static void CleanUpRecentScenesList()
        {
            foreach (var menuItem in RecentSceneItems)
            {
                menuItem.Item1.Click -= RecentSceneEntryClicked;
                menuItem.Item2.Click -= RecentSceneEntryClicked;
                Instance.MenuBar.RecentScenes.Items.Remove(menuItem.Item1);
                Instance.StartScreen.RecentScenesList.Children.Remove(menuItem.Item2);
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
                    RecentDataSourceItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentDataSourceMenuLink(RecentItem.EntryName), CreateRecentDataSourceMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentDataSourceItems.Reverse())
                {
                    Instance.MenuBar.RecentDataSources.Items.Insert(0, menuItem.Item1);
                    Instance.StartScreen.RecentDataContextList.Children.Insert(0, menuItem.Item2);
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
            newItem.Click += RecentDataSourceEntryClicked;
            return newItem;
        }
        public static void RecentDataSourceEntryClicked(object sender, RoutedEventArgs e)
        {
            if (ManiacEditor.Methods.Editor.SolutionLoader.AllowSceneUnloading() != true) return;
            Methods.Editor.Solution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            ManiacEditor.Methods.Editor.SolutionLoader.OpenSceneSelectSaveState(item);
            ManiacEditor.Classes.Prefrences.DataStateHistoryStorage.AddRecentFile(item);
        }
        private static void CleanUpDataSourcesList()
        {
            foreach (var menuItem in RecentDataSourceItems)
            {
                menuItem.Item1.Click -= RecentDataSourceEntryClicked;
                menuItem.Item2.Click -= RecentDataSourceEntryClicked;
                Instance.MenuBar.RecentDataSources.Items.Remove(menuItem.Item1);
                Instance.StartScreen.RecentDataContextList.Children.Remove(menuItem.Item2);
            }
            RecentDataSourceItems.Clear();

        }


        #endregion
    }
}
