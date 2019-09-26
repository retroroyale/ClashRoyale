using System;
using SharpRaven;
using SharpRaven.Data;

namespace ClashRoyale.Core
{
    public class SentryReport
    {
        public SentryReport()
        {
            if (string.IsNullOrEmpty(Resources.Configuration.SentryApiUrl)) return;

            Client = new RavenClient(Resources.Configuration.SentryApiUrl)
            {
                Logger = "ClashRoyale",
                IgnoreBreadcrumbs = true,
                ErrorOnCapture = e =>
                {
                    // ignore
                }
            };

            Client.Tags.Add("contentVersion", Resources.Fingerprint.GetVersion);
        }

        private RavenClient Client { get; }

        /// <summary>
        ///     Capture an event triggered by an error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="level"></param>
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