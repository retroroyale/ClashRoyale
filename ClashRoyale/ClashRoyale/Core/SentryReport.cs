using System;
using SharpRaven;
using SharpRaven.Data;

namespace ClashRoyale.Core
{
    public class SentryReport
    {
        public SentryReport()
        {
            Client = new RavenClient("") // Instert your own api url here
            {
                Logger = "ClashRoyale",
                IgnoreBreadcrumbs = true
            };

            Client.Tags.Add("contentVersion", Resources.Fingerprint.GetVersion);
        }

        public RavenClient Client { get; set; }

        public async void Report(string message, Type type, ErrorLevel level)
        {
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