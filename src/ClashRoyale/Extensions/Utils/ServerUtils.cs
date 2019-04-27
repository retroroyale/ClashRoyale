using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using SevenZip;
using LZMAEncoder = SevenZip.Compression.LZMA.Encoder;

namespace ClashRoyale.Extensions.Utils
{
    public class ServerUtils
    {
        public static string GetOSName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "MacOS";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";

            return "Unknown-" + Environment.OSVersion;
        }

        public static string GetIp4Address()
        {
            var ipAddress = string.Empty;
            var nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var nic in nics)
            {
                if (nic.OperationalStatus != OperationalStatus.Up)
                    continue;

                var adapterStat = nic.GetIPv4Statistics();
                var uniCast = nic.GetIPProperties().UnicastAddresses;

                if (uniCast == null) continue;

                if (uniCast.Where(uni => adapterStat.UnicastPacketsReceived > 0
                                         && adapterStat.UnicastPacketsSent > 0
                                         && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Any(uni =>
                    uni.Address.AddressFamily == AddressFamily.InterNetwork))
                    ipAddress = nic.GetIPProperties().UnicastAddresses[0].Address.ToString();
            }

            return ipAddress;
        }

        public static string GetChecksum(string text)
        {
            using (var hasher = new SHA1CryptoServiceProvider())
            {
                return hasher.ComputeHash(Encoding.UTF8.GetBytes(text)).Aggregate(string.Empty,
                    (current, num) => current + num.ToString("x2"));
            }
        }

        public static string GetChecksum(byte[] data)
        {
            using (var hasher = new SHA1CryptoServiceProvider())
            {
                return hasher.ComputeHash(data).Aggregate(string.Empty,
                    (current, num) => current + num.ToString("x2"));
            }
        }

        public static byte[] CompressData(byte[] input)
        {
            var encoder = new LZMAEncoder();

            using (var uncompressed = new MemoryStream(input))
            {
                using (var compressed = new MemoryStream())
                {
                    encoder.SetCoderProperties(new[]
                    {
                        CoderPropID.DictionarySize,
                        CoderPropID.PosStateBits,
                        CoderPropID.LitContextBits,
                        CoderPropID.LitPosBits,
                        CoderPropID.Algorithm,
                        CoderPropID.NumFastBytes,
                        CoderPropID.MatchFinder,
                        CoderPropID.EndMarker
                    }, new object[]
                    {
                        262144,
                        2,
                        3,
                        0,
                        2,
                        32,
                        "bt4",
                        false
                    });

                    encoder.WriteCoderProperties(compressed);

                    compressed.Write(BitConverter.GetBytes(uncompressed.Length), 0, 4);

                    encoder.Code(uncompressed, compressed, uncompressed.Length, -1L,
                        null);

                    return compressed.ToArray();
                }
            }
        }
    }
}