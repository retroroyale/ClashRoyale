using System;
using SharpRaven;
using SharpRaven.Data;

namespace ClashRoyale.Core
{
    public class SentryReport
    {
        public SentryReport()
        {
            if (!string.IsNullOrEmpty(Resources.Configuration.SentryApiUrl))
            {
                Client = new RavenClient(Resources.Configuration.SentryApiUrl) 
                {
                    Logger = "ClashRoyale",
                    IgnoreBreadcrumbs = true
                };

                Client.Tags.Add("contentVersion", Resources.Fingerprint.GetVersion);
            }
        }

        public RavenClient Client { get; set; }

        public async void Report(string message, Type type, ErrorLevel level)
        {
            if (Client == null)
                return;

            try
            {
                var sentryEvent = new SentryEvent(message)
                {
                    Level = level
                };

                sentryEvent.Tags.Add("className", type.Name);

                await Client.CaptureAsync(sentryEvent);
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}