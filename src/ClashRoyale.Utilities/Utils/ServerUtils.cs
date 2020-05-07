using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ClashRoyale.Utilities.Utils
{
    public class ServerUtils
    {
        public static string GetOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "MacOS";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";

            return "Unknown-" + Environment.OSVersion;
        }

        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static string GetChecksum(string text)
        {
            using var hasher = new SHA1CryptoServiceProvider();
            return hasher.ComputeHash(Encoding.UTF8.GetBytes(text)).Aggregate(string.Empty,
                (current, num) => current + num.ToString("x2"));
        }

        public static string GetChecksum(byte[] data)
        {
            using var hasher = new SHA1CryptoServiceProvider();
            return hasher.ComputeHash(data).Aggregate(string.Empty,
                (current, num) => current + num.ToString("x2"));
        }
    }
}