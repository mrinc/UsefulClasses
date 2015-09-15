using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Useful_Classes
{
    public static class MyComputer
    {
        public static IPAddress[] GetComputerIPs()
        {
            List<IPAddress> ips = new List<IPAddress>();
            try
            {
                String strHostName = Dns.GetHostName();
                IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    ips.Add(ipaddress);
                }
            }
            catch
            { }
            return ips.ToArray();
        }

        public static IPAddress[] GetComputerIPs(IPAddressSelectType selectionType)
        {
            List<IPAddress> ips = new List<IPAddress>();
            try
            {
                String strHostName = Dns.GetHostName();
                IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    if (selectionType == IPAddressSelectType.ALL)
                        ips.Add(ipaddress);
                    else if (selectionType == IPAddressSelectType.IPv4)
                    {
                        if (ipaddress.ToString().Split('.').Length == 4)
                            ips.Add(ipaddress);
                    }
                    else if (selectionType == IPAddressSelectType.IPv6)
                    {
                        if (ipaddress.ToString().Split('.').Length != 4)
                            ips.Add(ipaddress);
                    }
                }
            }
            catch
            { }
            return ips.ToArray();
        }

        public enum IPAddressSelectType
        {
            ALL,
            IPv6,
            IPv4
        }

        public static FileData GetFileSize(FileData file)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            double len = new FileInfo(file.FileName).Length;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }
            file.Size = len;
            file.Type = (Useful_Classes.MyComputer.FileData.FileSizeType)Enum.Parse(typeof(Useful_Classes.MyComputer.FileData.FileSizeType), sizes[order]);

            return file;
        }

        public class FileData
        {
            public Double Size { get; set; }
            public string FileName { get; set; }
            public FileSizeType Type { get; set; }

            public enum FileSizeType
            {
                B, KB, MB,
                GB, TB, PB,
                EB, ZB, YB
            }
        }

        public static uint GetProcessIDByServiceName(string serviceName)
        {
            uint processId = 0;
            string qry = "SELECT PROCESSID FROM WIN32_SERVICE WHERE NAME = '" + serviceName + "'";
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(qry);
            foreach (System.Management.ManagementObject mngntObj in searcher.Get())
            {
                processId = (uint)mngntObj["PROCESSID"];
            }
            return processId;
        }

        public static uint GetProcessIDByServiceDisplayName(string serviceDisplayName)
        {
            uint processId = 0;
            string qry = "SELECT PROCESSID FROM WIN32_SERVICE WHERE DISPLAYNAME = '" + serviceDisplayName + "'";
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(qry);
            foreach (System.Management.ManagementObject mngntObj in searcher.Get())
            {
                processId = (uint)mngntObj["PROCESSID"];
            }
            return processId;
        }

        public static List<System.ServiceProcess.ServiceController> GetRunningServices()
        {
            System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
            List<System.ServiceProcess.ServiceController> running = new List<System.ServiceProcess.ServiceController>();

            foreach (System.ServiceProcess.ServiceController item in services)
            {
                if (item.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    running.Add(item);
                }
            }
            return running;
        }

        public static string GetValidFilePathInWindows(string path)
        {
            return path.Replace("<", "-LT-")
                                    .Replace(">", "-GT-")
                                    .Replace("|", "-VB-")
                                    .Replace("?", "-QM-")
                                    .Replace("*", "-AS-")
                                    .Replace("@", "-AT-");
        }

        public static bool IsValidFilePathInWindows(string path)
        {
            string newPath = GetValidFilePathInWindows(path);

            return (newPath.Equals(path));
        }

        /// <summary>
        /// Finds the MAC address of the NIC with maximum speed.
        /// </summary>
        /// <returns>The MAC address.</returns>
        /// http://stackoverflow.com/a/1561067
        public static NetworkInterface[] GetMacAddresses()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }
    }
}
