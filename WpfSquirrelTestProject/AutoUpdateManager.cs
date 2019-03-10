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
        private const string ExeName = AppName + ".exe";

        private readonly IUpdateManager  _updateManager;

        public AutoUpdateManager(string updateUrl)
        {
            _updateManager = UpdateManager.GitHubUpdateManager(updateUrl).Result;
        }
       
        public async Task CheckForUpdate()
        {
           await InvokeUpdate();
        }

        public void Dispose()
        {
            _updateManager.Dispose();
        }

        private async Task InvokeUpdate()
        {
            var updateInfo = await _updateManager.CheckForUpdate();
            var lastVersion = updateInfo?.ReleasesToApply?.OrderBy(x => x.Version).LastOrDefault();
            if (lastVersion == null) return;
            if (MessageBox.Show($"An update to version {lastVersion.Version} is available. Do you want to update?",
                           "Update available", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }
            await _updateManager.DownloadReleases(new[] { lastVersion });
            await _updateManager.ApplyReleases(updateInfo);
            await _updateManager.UpdateApp();
        }
    }
}
