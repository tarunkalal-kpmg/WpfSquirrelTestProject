﻿using Squirrel;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSquirrelTestProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string updatePath = "https://github.com/tarunkalal-kpmg/WpfSquirrelTestProject";
        public MainWindow()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();
            ExeAssLoc.Text = assembly.Location;
            Vernum.Text = assembly.GetName().Version.ToString(3);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                using (var autoUpdater = new AutoUpdateManager(updatePath))
                {
                    await autoUpdater.InvokeUpdate().ConfigureAwait(false);
                }
            });
        }
    }
}
