// Datei: InformixServerScanner.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Informix_Assistenten
{
    public static class InformixServerScanner
    {
        public static async Task<List<string>> ScanAsync(List<int> ports, Action<string> progressCallback = null)
        {
            var foundServers = new List<string>();
            string subnet = GetLocalSubnet();

            if (subnet == null)
                return foundServers;

            List<Task> tasks = new List<Task>();

            for (int i = 1; i < 255; i++)
            {
                string ip = string.Format("{0}.{1}", subnet, i);

                foreach (int port in ports)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        using (var client = new TcpClient())
                        {
                            try
                            {
                                var connectTask = client.ConnectAsync(ip, port);
                                if (await Task.WhenAny(connectTask, Task.Delay(100)) == connectTask && client.Connected)
                                {
                                    lock (foundServers)
                                        foundServers.Add(string.Format("{0}:{1}", ip, port));
                                    if (progressCallback != null)
                                        progressCallback(string.Format("{0}:{1}", ip, port));
                                }
                            }
                            catch { }
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);
            return foundServers;
        }

        private static string GetLocalSubnet()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up || ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string[] parts = ua.Address.ToString().Split('.');
                        if (parts.Length == 4)
                            return string.Format("{0}.{1}.{2}", parts[0], parts[1], parts[2]);
                    }
                }
            }
            return null;
        }
    }

    public static class AppState
    {
        public static List<string> AvailableServers = new List<string>();
    }
}
