using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using RestSharp;
using System;
using System.Reflection;
using Vatsim.Vatis.Client.Config;

namespace Vatsim.Vatis.Client.Core
{
    internal class VersionCheck : IVersionCheck
    {
        private const string VersionCheckUrl = "https://vatis.clowd.io/api/v4/VersionCheck";
        private readonly IEventBroker mEventBroker;
        private readonly IUserInterface mUserInterface;
        private readonly IAppConfig mAppConfig;

        public VersionCheck(IEventBroker eventBroker, IAppConfig appConfig, IUserInterface userInterface)
        {
            mEventBroker = eventBroker;
            mEventBroker.Register(this);
            mAppConfig = appConfig;
            mUserInterface = userInterface;
        }

        [EventSubscription(EventTopics.PerformVersionCheck, typeof(OnPublisher))]
        public void OnPerformVersionCheck(object sender, EventArgs e)
        {
            PerformVersionCheck();
        }

        private void PerformVersionCheck()
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest(VersionCheckUrl);
                var response = client.Get<VersionCheckResponseDto>(request);

                if (response.IsSuccessful)
                {
                    Version v = new Version(response.Data.LatestVersion);
                    if (v > Assembly.GetExecutingAssembly().GetName().Version)
                    {
                        using var dlg = mUserInterface.CreateVersionCheckForm();
                        dlg.TopMost = mAppConfig.WindowProperties.TopMost;
                        dlg.NewVersion = response.Data.LatestProductVersion;
                        dlg.DownloadUrl = response.Data.LatestVersionUrl;
                        dlg.ShowDialog();
                    }
                }
            }
            catch { }
        }
    }

    [Serializable]
    internal class VersionCheckResponseDto
    {
        public string LatestProductVersion { get; set; }
        public string LatestVersion { get; set; }
        public string LatestVersionUrl { get; set; }
    }
}
