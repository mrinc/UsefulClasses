using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Useful_Classes.TV
{
    public class SamsungTV
    {
        private IPAddress MyIP;
        private NetworkInterface MyMAC;
        private IPAddress TVIP;
        private string TVName;
        private string TVProduct;
        private const string RemoteName = "Useful Classes Remote";
        private const string AppName = "iphone.iapp.samsung";

        private Socket SystemConnection;

        public SamsungTV(IPAddress tVIP4, string tVProductModel)
        {
            MyIP = MyComputer.GetComputerIPs(MyComputer.IPAddressSelectType.IPv4)[0];
            MyMAC = MyComputer.GetMacAddresses()[0];
            TVIP = tVIP4;
            TVProduct = tVProductModel;
            TVName = AppName + "." + TVProduct;
        }

        private void CreateConnection()
        {
            SystemConnection = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Ping p = new Ping();
            var pingResponce = p.Send(TVIP, 1000);
            if (pingResponce.Status != IPStatus.Success)
                throw new PingException("IP " + TVIP.MapToIPv4().ToString() + " is unreachble!", new Exception(pingResponce.Status.ToString()));

            SystemConnection.Connect(TVIP, 55000);

            if (!SystemConnection.Connected)
                throw new Exception("IP " + TVIP.MapToIPv4().ToString() + " is unreachble!", new Exception(pingResponce.Status.ToString()));
        }
        
        private void Authenticate()
        {
            var base64Src = StringManipulation.Base64Encode(TVIP.ToString());
            var base64Mac = StringManipulation.Base64Encode(MyMAC.ToString());
            var base64RemoteName = StringManipulation.Base64Encode(RemoteName);

            List<byte> msgToSend = new List<byte>();
            msgToSend.AddRange(new byte[] {
                             0x00,
                             Convert.ToByte(base64Src.Length),
                             0x00});
            msgToSend.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64Src));
            msgToSend.AddRange(new byte[] { 0x00, Convert.ToByte(base64Mac.Length), 0x00 });
            msgToSend.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64Mac));
            msgToSend.AddRange(new byte[] { 0x00, Convert.ToByte(base64RemoteName.Length), 0x00 });
            msgToSend.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64RemoteName));
            msgToSend.Add(0x00);

            List<byte> actualPktMsgBytesToSend = new List<byte>();
            int cc = msgToSend.Count;
            while (cc > 255)
            {
                actualPktMsgBytesToSend.Add(255);
                cc--;
            }
            actualPktMsgBytesToSend.Add(Convert.ToByte(cc));

            List<byte> actualPktsToSend = new List<byte>();
            actualPktsToSend.AddRange(new byte[] {
                             0x00,
                             Convert.ToByte(AppName.Length),
                             0x00 });
            actualPktsToSend.AddRange(Useful_Classes.StringManipulation.StringToByteArray(AppName));
            actualPktsToSend.AddRange(actualPktMsgBytesToSend);
            actualPktsToSend.AddRange(new byte[] { 0x00 });
            actualPktsToSend.AddRange(msgToSend);

            SystemConnection.Send(actualPktsToSend.ToArray());

            Thread.Sleep(100);
        }

        //public void SendMsg(string key)
        //{
        //    Console.WriteLine("Connecting to TV " + TVIP.ToString());
        //    CreateConnection();
        //    Console.WriteLine("Connected to TV " + TVIP.ToString());

        //    Authenticate();

        //    Console.WriteLine("Authenticated to TV " + TVIP.ToString());

        //    var base64key = StringManipulation.Base64Encode(key);
        //    List<byte> msgToSend = new List<byte>();
        //    msgToSend.AddRange(new byte[] {
        //                     0x00,
        //                     0x00,
        //                     0x00,
        //                     Convert.ToByte(base64key.Length) });
        //    msgToSend.AddRange(new byte[] { 0x00 });
        //    msgToSend.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64key));

        //    List<byte> actualPktMsgBytesToSend = new List<byte>();
        //    int cc = msgToSend.Count;
        //    while (cc > 255)
        //    {
        //        actualPktMsgBytesToSend.Add(255);
        //        cc--;
        //    }
        //    actualPktMsgBytesToSend.Add(Convert.ToByte(cc));

        //    List<byte> actualPktsToSend = new List<byte>();
        //    actualPktsToSend.AddRange(new byte[] {
        //                     0x00,
        //                     Convert.ToByte(TVName.Length) });
        //    actualPktsToSend.AddRange(Useful_Classes.StringManipulation.StringToByteArray(TVName));
        //    actualPktsToSend.AddRange(actualPktMsgBytesToSend);
        //    actualPktsToSend.AddRange(new byte[] { 0x00 });
        //    actualPktsToSend.AddRange(msgToSend);

        //    SystemConnection.Send(actualPktsToSend.ToArray());

        //    Thread.Sleep(100);

        //    Console.WriteLine("Completed " + key + " to TV " + TVIP.ToString());

        //    SystemConnection.Disconnect(true);

        //    Console.WriteLine("Disconnected from TV " + TVIP.ToString());
        //}

        public void SendMsg(string key)
        {
            Console.WriteLine("Connecting to TV " + TVIP.ToString());
            CreateConnection();
            Console.WriteLine("Connected to TV " + TVIP.ToString());
            byte[] payLoadStart = {
                0x64, 0x00
            };

            byte[] packetStart = {
                0x00, 0x13, 0x00, 0x69, 0x70, 0x68, 0x6f, 0x6e, 0x65, 0x2e, 0x69, 0x61, 0x70, 0x70, 0x2e, 0x73,
                0x61, 0x6d, 0x73, 0x75, 0x6e, 0x67
            };

            List<byte> payLoad = new List<byte>();
            payLoad.AddRange(payLoadStart);

            var base64RemoteIP = StringManipulation.Base64Encode(MyIP.ToString());
            payLoad.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64RemoteIP));

            var base64Mac = StringManipulation.Base64Encode(MyMAC.ToString());
            payLoad.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64Mac));

            var base64RemoteName = StringManipulation.Base64Encode(RemoteName);
            payLoad.AddRange(Useful_Classes.StringManipulation.StringToByteArray(base64RemoteName));

            Int32 payloadSize = payLoad.Count;

            SystemConnection.Send(packetStart);
            SystemConnection.Send(new byte[] { Convert.ToByte(payloadSize) });
            SystemConnection.Send(payLoad.ToArray());
            
            Thread.Sleep(100);

            Console.WriteLine("Completed " + key + " to TV " + TVIP.ToString());

            SystemConnection.Disconnect(true);

            Console.WriteLine("Disconnected from TV " + TVIP.ToString());
        }

        public void PerformKeyPress(object keyToPress)
        {
            SendMsg(keyToPress.ToString());
        }
    }
}
