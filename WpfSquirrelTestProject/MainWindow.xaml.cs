using Squirrel;
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
            UpdateApp().ConfigureAwait(false);

            MessageBox.Show("Finish");
        }

        public async Task UpdateApp()
        {
            try
            {
                var mgr = await UpdateManager.GitHubUpdateManager(updatePath, "WpfSquirrelTestProject");
                var updates = await mgr.CheckForUpdate();
                var lastVersion = updates?.ReleasesToApply?.OrderBy(x => x.Version).LastOrDefault();
                if (lastVersion == null)
                {
                    MessageBox.Show("No Updates are available at this time.");
                    return;
                }

                if (MessageBox.Show($"An update to version {lastVersion.Version} is available. Do you want to update?",
                        "Update available", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
                MessageBox.Show("DEBUG: Don't actually perform the update in debug mode");

                await mgr.DownloadReleases(new[] { lastVersion });
                await mgr.ApplyReleases(updates);
                await mgr.UpdateApp();

                MessageBox.Show("The application has been updated - please close and restart.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Update check failed with an exception: " + e.Message);
            }
        }
    }
}
