using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using System;
using System.Net;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace TFSAdminDashboard
{
    public static class TFSConnect
    {
        /// <summary>
        /// Get Version COntrol Server object
        /// </summary>
        /// <returns></returns>
        public static VersionControlServer GetVersionControlServer()
        {
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                         new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                         TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                         TFSAdminDashboard.Properties.Settings.Default.Password,
                                                         TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                         );
            return tfs.GetService<VersionControlServer>();
        }
        /// <summary>
        /// Get Release Client Object
        /// </summary>
        /// <returns></returns>
        public static ReleaseHttpClient GetReleaseClient()
        {
            ReleaseHttpClient releaseClient = new ReleaseHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                  new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                  TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                  TFSAdminDashboard.Properties.Settings.Default.Password,
                                                  TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                  );
            return releaseClient;
        }
        /// <summary>
        /// Get Build Client object
        /// </summary>
        /// <returns></returns>
        public static BuildHttpClient GetBuildClient()
        {
            var buildClient = new BuildHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                      new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                      TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                      TFSAdminDashboard.Properties.Settings.Default.Password,
                                                      TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                      );
            return buildClient;
        }
    }
}