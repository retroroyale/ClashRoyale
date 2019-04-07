using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using SevenZip.Sdk;
using Encoder = SevenZip.Sdk.Compression.Lzma.Encoder;

namespace ClashRoyale.Extensions.Utils
{
    public class ServerUtils
    {
        public static bool IsLinux
        {
            get
            {
                var p = (int) Environment.OSVersion.Platform;
                return p == 4 || p == 6 || p == 128;
            }
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
            var encoder = new Encoder();

            using (var uncompressed = new MemoryStream(input))
            {
                using (var compressed = new MemoryStream())
                {
                    encoder.SetCoderProperties(new[]
                    {
                        CoderPropId.DictionarySize,
                        CoderPropId.PosStateBits,
                        CoderPropId.LitContextBits,
                        CoderPropId.LitPosBits,
                        CoderPropId.Algorithm,
                        CoderPropId.NumFastBytes,
                        CoderPropId.MatchFinder,
                        CoderPropId.EndMarker
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