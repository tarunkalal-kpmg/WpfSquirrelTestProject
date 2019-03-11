using Squirrel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSquirrelTestProject
{
    public class AutoUpdateManager : IDisposable
    {
        private const string AppName = "WpfSquirrelTestProject";
        private IUpdateManager  _updateManager;
        private readonly string updatePath;

        public AutoUpdateManager(string updateUrl)
        {
            updatePath = updateUrl;
        }

        public void Dispose()
        {
            _updateManager?.Dispose();
        }
        public async Task InvokeUpdate()
        {
            try
            {
                _updateManager = await UpdateManager.GitHubUpdateManager(updatePath, AppName);
                var updates = await _updateManager.CheckForUpdate();
                var lastVersion = updates?.ReleasesToApply?.OrderBy(x => x.Version).LastOrDefault();
                if (lastVersion == null) return;

                if (MessageBox.Show($"An update to version {lastVersion.Version} is available. Do you want to update?",
                        "Update available", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }

                await _updateManager.DownloadReleases(new[] { lastVersion });
                await _updateManager.ApplyReleases(updates);
                await _updateManager.UpdateApp();
                MessageBox.Show("The application has been updated - please close and restart.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Update check failed with an exception: " + e.Message);
            }
        }
    }
}
