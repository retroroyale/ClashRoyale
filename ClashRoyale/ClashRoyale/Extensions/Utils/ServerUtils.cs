using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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
    }
}