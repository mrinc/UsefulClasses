using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/*
namespace Useful_Classes
{
    public class Communication
    {
        public class Host
        {
            private bool Listens = false;
            /// <summary>
            /// Raised everytime data received.
            /// </summary>
            /// <param name="Data">Set of bytes received.</param>
            /// <param name="ID">The sender ID.</param>
            /// <remarks></remarks>
            public event DataReceivedEventHandler DataReceived;
            public delegate void DataReceivedEventHandler(string ID, byte[] Data);
            /// <summary>
            /// Raised everytime a client sent data to another client.
            /// </summary>
            /// <param name="Sender">The sender client ID.</param>
            /// <param name="Recipient">The target client ID.</param>
            /// <param name="Data">The data was sent.</param>
            /// <remarks></remarks>
            public event DataTransferredEventHandler DataTransferred;
            public delegate void DataTransferredEventHandler(string Sender, string Recipient, byte[] Data);
            /// <summary>
            /// Raised everytime an error had occured.
            /// </summary>
            /// <param name="ex">The exception occured.</param>
            /// <remarks></remarks>
            public event errEncounterEventHandler errEncounter;
            public delegate void errEncounterEventHandler(Exception ex);
            /// <summary>
            /// Raised everytime a client connected.
            /// </summary>
            /// <param name="id">The client connected ID.</param>
            /// <remarks></remarks>
            public event onConnectionEventHandler onConnection;
            public delegate void onConnectionEventHandler(string id);
            /// <summary>
            /// Raised everytime a client disconnected.
            /// </summary>
            /// <param name="id">The client disconnected ID.</param>
            /// <remarks></remarks>
            public event lostConnectionEventHandler lostConnection;
            public delegate void lostConnectionEventHandler(string id);
            /// <summary>
            /// Raised everytime the connection was closed.
            /// </summary>
            /// <remarks></remarks>
            public event ConnectionClosedEventHandler ConnectionClosed;
            public delegate void ConnectionClosedEventHandler();
            private List<TcpClientHandler> Clients = new List<TcpClientHandler>();
            private NetworkStream netStream;
            private SynchronizationContext Context;
            private Thread T;
            private TcpListener serverSocket;
            private TcpClient clientSocket;
            private struct DataPacket
            {
                public string Sender;
                public string Recipient;
                public byte[] Data;
                public DataPacket(string SenderID, string RecipientID, byte[] DataBytes)
                {
                    Sender = SenderID;
                    Recipient = RecipientID;
                    Data = DataBytes;
                }
            }
            private enum EventPointer
            {
                DataReceived = 0,
                errEncounter = 1,
                onConnection = 2,
                lostConnection = 3,
                ConnectionClosed = 4,
                DataTransferred = 5
            }
            private void EventHandler(EventArgs Args)
            {
                switch (Args.EventP)
                {
                    case EventPointer.DataReceived:
                        if (DataReceived != null)
                        {
                            DataReceived(Args.Arg.ID, Args.Arg.Data);
                        }

                        break;
                    case EventPointer.DataTransferred:
                        if (DataTransferred != null)
                        {
                            DataTransferred(Args.Arg.Sender, Args.Arg.Recipient, Args.Arg.Data);
                        }

                        break;
                    case EventPointer.errEncounter:
                        if (errEncounter != null)
                        {
                            errEncounter(Args.Arg);
                        }

                        break;
                    case EventPointer.onConnection:
                        if (onConnection != null)
                        {
                            onConnection(Args.Arg);
                        }

                        break;
                    case EventPointer.lostConnection:
                        if (lostConnection != null)
                        {
                            lostConnection(Args.Arg);
                        }

                        break;
                    case EventPointer.ConnectionClosed:
                        if (ConnectionClosed != null)
                        {
                            ConnectionClosed();
                        }

                        break;
                }
            }
            private void EventRaise(EventPointer EventPoint, object Arg = null)
            {
                if ((Context != null))
                {
                    Context.Post(EventHandler, new EventArgs(EventPoint, Arg));
                }
                else
                {
                    EventHandler(new EventArgs(EventPoint, Arg));
                }
            }
            /// <summary>
            /// Initializing a new server with a port mentioned. To start the server use the StartConnection method.
            /// </summary>
            /// <param name="Port">The port the server will listen to.</param>
            /// <remarks></remarks>
            public Host(int Port)
            {
                serverSocket = new TcpListener(System.Net.IPAddress.Any, Port);
                Context = SynchronizationContext.Current();
            }
            /// <summary>
            /// The number of bytes to send.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public int SendBufferSize
            {
                get { return serverSocket.Server.SendBufferSize; }
                set { serverSocket.Server.SendBufferSize = value; }
            }
            /// <summary>
            /// The number of bytes to receive.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public int ReceiveBufferSize
            {
                get { return serverSocket.Server.ReceiveBufferSize; }
                set { serverSocket.Server.ReceiveBufferSize = value; }
            }
            /// <summary>
            /// Determines if the server should wait an amount of time so more data will be added to the send data buffer, if set to true, the server will send the data immediatly.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool NoDelay
            {
                get { return serverSocket.Server.NoDelay; }
                set { serverSocket.Server.NoDelay = value; }
            }
            /// <summary>
            /// Starts listening to clients.
            /// </summary>
            /// <remarks></remarks>
            public void StartConnection()
            {
                try
                {
                    Listens = true;
                    T = new Thread(Main);
                    T.Start();
                }
                catch (Exception ex)
                {
                    EventRaise(EventPointer.errEncounter, ex);
                }
            }
            private void DataReceivedHandler(Useful_Classes.Communication.Functions.ClientMsg Msg)
            {
                DecryptBytes(Msg);
            }
            private void DecryptBytes(Useful_Classes.Communication.Functions.ClientMsg Message)
            {
                bool Disconnect = true;
                for (b = 0; b <= Message.Data.Length - 1; b++)
                {
                    ClientMsg Msg = ClientMsg.FromBytes(Message.Data, b);
                    if ((Msg.Data != null))
                    {
                        if (Msg.ID == null)
                        {
                            EventRaise(EventPointer.DataReceived, new ClientMsg(Message.ID, Msg.Data));
                        }
                        else
                        {
                            TransferData(Msg, Message.ID);
                        }
                        Disconnect = false;
                    }
                    if (b >= Message.Data.Length - 1)
                        break; // TODO: might not be correct. Was : Exit For
                }
                if (Disconnect)
                    DisconnectUser(Message.ID);
            }
            private void errEncounterHandler(Exception ex)
            {
                EventRaise(EventPointer.errEncounter, ex);
            }
            private void lostConnectionHandler(string ID)
            {
                EventRaise(EventPointer.lostConnection, ID);
                TcpClientHandler Handler = GetClientHandlerByID(ID);
                Handler.DataReceived -= DataReceivedHandler;
                Handler.errEncounter -= errEncounterHandler;
                Clients.Remove(Handler);
                Handler.lostConnection -= lostConnectionHandler;
            }
            private TcpClientHandler GetClientHandlerByID(string ID)
            {
                foreach (TcpClientHandler c in Clients)
                {
                    if (c.ID == ID)
                        return c;
                }
                return null;
            }
            /// <summary>
            /// Sends a byte array to a specific client.
            /// </summary>
            /// <param name="ID">Target client ID.</param>
            /// <param name="Data">A set of bytes to send.</param>
            /// <remarks></remarks>
            public void SendData(string ID, byte[] Data)
            {
                try
                {
                    Data = ClientMsg.GetBytes(new ClientMsg(null, Data));
                    TcpClientHandler ClientHandler = GetClientHandlerByID(ID);
                    if ((ClientHandler != null))
                        ClientHandler.SendData(Data);
                }
                catch (Exception ex)
                {
                    EventRaise(EventPointer.errEncounter, ex);
                }
            }
            private void TransferData(Useful_Classes.Communication.Functions.ClientMsg TargetClient, string Sender)
            {
                TcpClientHandler ClientHandler = GetClientHandlerByID(TargetClient.ID);
                if ((ClientHandler != null))
                    ClientHandler.SendData(Useful_Classes.Communication.Functions.ClientMsg.GetBytes(new Useful_Classes.Communication.Functions.ClientMsg(Sender, TargetClient.Data)));
                EventRaise(EventPointer.DataTransferred, new DataPacket(Sender, TargetClient.ID, TargetClient.Data));
            }
            /// <summary>
            /// Sends data to all of the connected clients.
            /// </summary>
            /// <param name="Data">The array of bytes to send.</param>
            /// <remarks></remarks>
            public void Brodcast(byte[] Data)
            {
                try
                {
                    Data = ClientMsg.GetBytes(new ClientMsg(null, Data));
                    foreach (TcpClientHandler c in Clients)
                    {
                        c.SendData(Data);
                    }
                }
                catch (Exception ex)
                {
                    EventRaise(EventPointer.errEncounter, ex);
                }
            }
            /// <summary>
            /// Disconnect a specific user.
            /// </summary>
            /// <param name="ID">The client ID to disconnect.</param>
            /// <remarks></remarks>
            public void DisconnectUser(string ID)
            {
                try
                {
                    GetClientHandlerByID(ID).Disconnect();
                }
                catch (Exception ex)
                {
                    EventRaise(EventPointer.errEncounter, ex);
                }
            }
            private void Main()
            {
                serverSocket.Start();
                do
                {
                    try
                    {
                        clientSocket = serverSocket.AcceptTcpClient();
                        netStream = clientSocket.GetStream();
                        byte[] bytes = new byte[Convert.ToInt32(clientSocket.ReceiveBufferSize) + 1];
                        netStream.Read(bytes, 0, Convert.ToInt32(clientSocket.ReceiveBufferSize));
                        string ID2String = ConvertFromAscii(bytes);
                        if ((GetClientHandlerByID(ID2String) != null))
                        {
                            string OriginID = ID2String;
                            int cnt = 1;
                            ID2String = OriginID + cnt;
                            while ((GetClientHandlerByID(ID2String) != null))
                            {
                                cnt += 1;
                                ID2String = OriginID + cnt;
                            }
                        }
                        TcpClientHandler TcpClientHandle = new TcpClientHandler(clientSocket, ID2String, Context);
                        Clients.Add(TcpClientHandle);
                        TcpClientHandle.DataReceived += DataReceivedHandler;
                        //Handle all of the data received in all clients
                        TcpClientHandle.errEncounter += errEncounterHandler;
                        //Handle all clients errors
                        TcpClientHandle.lostConnection += lostConnectionHandler;
                        //Handle all clients lost connections
                        EventRaise(EventPointer.onConnection, ID2String);
                    }
                    catch (Exception ex)
                    {
                        EventRaise(EventPointer.errEncounter, ex);
                    }
                } while (!(Listens == false));

                if ((Context != null))
                {
                    Context.Post(CloseConnection, null);
                }
                else
                {
                    CloseConnection();
                }
            }
            /// <summary>
            /// Stops listening to connections and closes the server.
            /// </summary>
            /// <remarks></remarks>
            public void CloseConnection()
            {
                // ERROR: Not supported in C#: OnErrorStatement

                Listens = false;
                if ((clientSocket != null))
                    clientSocket.Close();
                foreach (TcpClientHandler c in Clients)
                {
                    c.Disconnect();
                }
                if ((netStream != null))
                    netStream.Close();
                serverSocket.Stop();
                if (ConnectionClosed != null)
                {
                    ConnectionClosed();
                }
            }
            /// <summary>
            /// Returns the current listen state (true if server is listening)
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool Listening
            {
                get { return Listens; }
            }
            /// <summary>
            /// Returns a list of all of the connected users.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public List<string> Users
            {
                get
                {
                    List<string> functionReturnValue = null;
                    functionReturnValue = new List<string>();
                    foreach (TcpClientHandler c in Clients)
                    {
                        functionReturnValue.Add(c.ID);
                    }
                    return functionReturnValue;
                }
            }
            /// <summary>
            /// Converts a set of bytes to a string.
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static string ConvertFromAscii(byte[] bytes)
            {
                string str = ASCIIEncoding.GetEncoding("windows-1255").GetString(bytes);
                int findnull = Strings.InStr(str, Strings.Chr(0));
                if (findnull > 0)
                    str = Strings.Mid(str, 1, findnull - 1);
                return str;
            }
            /// <summary>
            /// Converts a string to a set of bytes.
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static byte[] Convert2Ascii(string str)
            {
                return ASCIIEncoding.GetEncoding("windows-1255").GetBytes(str);
            }
            private class TcpClientHandler
            {
                private TcpClient clientSocket;
                private NetworkStream netStream;
                public string ID;
                private SynchronizationContext Context;
                private Thread T;
                private bool Connected = false;
                public event DataReceivedEventHandler DataReceived;
                public delegate void DataReceivedEventHandler(Useful_Classes.Communication.Functions.ClientMsg Msg);
                public event errEncounterEventHandler errEncounter;
                public delegate void errEncounterEventHandler(Exception ex);
                public event lostConnectionEventHandler lostConnection;
                public delegate void lostConnectionEventHandler(string ID);
                public object isConnected
                {
                    get { return Connected; }
                }
                public TcpClientHandler(TcpClient cSocket, string ClientID, SynchronizationContext SyncContext)
                {
                    clientSocket = cSocket;
                    ID = ClientID;
                    Context = SyncContext;
                    Connected = true;
                    T = new Thread(Main);
                    T.Start();
                }
                private void EventHandler(EventArgs Args)
                {
                    switch (Args.EventP)
                    {
                        case EventPointer.DataReceived:
                            if (DataReceived != null)
                            {
                                DataReceived(new ClientMsg(ID, Args.Arg));
                            }

                            break;
                        case EventPointer.errEncounter:
                            if (errEncounter != null)
                            {
                                errEncounter(Args.Arg);
                            }

                            break;
                        case EventPointer.lostConnection:
                            if (lostConnection != null)
                            {
                                lostConnection(ID);
                            }

                            break;
                    }
                }
                private void EventRaise(EventPointer EventPoint, object Arg = null)
                {
                    if ((Context != null))
                    {
                        Context.Post(EventHandler, new EventArgs(EventPoint, Arg));
                    }
                    else
                    {
                        EventHandler(new EventArgs(EventPoint, Arg));
                    }
                }
                private enum EventPointer
                {
                    DataReceived = 0,
                    errEncounter = 1,
                    lostConnection = 2
                }
                private void Main()
                {
                    netStream = clientSocket.GetStream();
                    while (clientSocket.Connected & Connected)
                    {
                        try
                        {
                            byte[] GetBytes = new byte[Convert.ToInt32(clientSocket.ReceiveBufferSize) + 1];
                            netStream.Read(GetBytes, 0, Convert.ToInt32(clientSocket.ReceiveBufferSize));
                            EventRaise(EventPointer.DataReceived, GetBytes);
                        }
                        catch (Exception ex)
                        {
                            break; // TODO: might not be correct. Was : Exit Do
                        }
                    }
                    if ((Context != null))
                    {
                        Context.Post(Disconnect, null);
                    }
                    else
                    {
                        Disconnect();
                    }
                }
                public void SendData(byte[] Data)
                {
                    try
                    {
                        netStream.Write(Data, 0, Data.Length);
                        netStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        EventRaise(EventPointer.errEncounter, ex);
                    }
                }
                public void Disconnect()
                {
                    // ERROR: Not supported in C#: OnErrorStatement

                    Connected = false;
                    if ((netStream != null))
                        netStream.Close();
                    if ((clientSocket != null))
                        clientSocket.Close();
                    if (lostConnection != null)
                    {
                        lostConnection(ID);
                    }
                }
            }
        }
        public class Client
        {
            private string IP;
            private int Port;
            private string ID;
            private bool ConnectedHost;
            private SynchronizationContext Context;
            private TcpClient clientSocket;
            private NetworkStream netStream;
            private Thread T;
            /// <summary>
            /// Raised everytime data received.
            /// </summary>
            /// <param name="Data">Set of bytes received.</param>
            /// <param name="ID">The sender ID.</param>
            /// <remarks></remarks>
            public event DataReceivedEventHandler DataReceived;
            public delegate void DataReceivedEventHandler(byte[] Data, string ID);
            /// <summary>
            /// Raised everytime an error had occured.
            /// </summary>
            /// <param name="ex">The exception occured.</param>
            /// <remarks></remarks>
            public event errEncounterEventHandler errEncounter;
            public delegate void errEncounterEventHandler(Exception ex);
            /// <summary>
            /// Raised when the client connected.
            /// </summary>
            /// <remarks></remarks>
            public event ConnectedEventHandler Connected;
            public delegate void ConnectedEventHandler();
            /// <summary>
            /// Raiseed when the client disconnected.
            /// </summary>
            /// <remarks></remarks>
            public event DisconnectedEventHandler Disconnected;
            public delegate void DisconnectedEventHandler();
            /// <summary>
            /// Initalizing a new client
            /// </summary>
            /// <remarks></remarks>
            public Client()
            {
                Context = SynchronizationContext.Current;
                clientSocket = new TcpClient();
            }
            /// <summary>
            /// The number of bytes to send.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public int SendBufferSize
            {
                get { return clientSocket.SendBufferSize; }
                set { clientSocket.SendBufferSize = value; }
            }
            /// <summary>
            /// The number of bytes to receive.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public int ReceiveBufferSize
            {
                get { return clientSocket.ReceiveBufferSize; }
                set { clientSocket.ReceiveBufferSize = value; }
            }
            /// <summary>
            /// Determines if the server should wait an amount of time so more data will be added to the send data buffer, if set to true, the server will send the data immediatly.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool NoDelay
            {
                get { return clientSocket.NoDelay; }
                set { clientSocket.NoDelay = value; }
            }
            private void EventHandler(Useful_Classes.Communication.Functions.EventArgs Args)
            {
                switch (Args.EventP)
                {
                    case EventPointer.Connected:
                        if (Connected != null)
                        {
                            Connected();
                        }

                        break;
                    case EventPointer.Disconnected:
                        if (Disconnected != null)
                        {
                            Disconnected();
                        }

                        break;
                    case EventPointer.DataReceived:
                        if (DataReceived != null)
                        {
                            DataReceived(Args.Arg.Data, Args.Arg.ID);
                        }

                        break;
                    case EventPointer.errEncounter:
                        if (errEncounter != null)
                        {
                            errEncounter(Args.Arg);
                        }

                        break;
                }
            }
            private void EventRaise(EventPointer EventPoint, object Arg = null)
            {
                if ((Context != null))
                {
                    EventHandler(Arg);
                }
                else
                {
                    //FIX
                    EventHandler(new EventArgs() {  } );
                }
            }
            private enum EventPointer
            {
                DataReceived = 0,
                errEncounter = 1,
                Connected = 2,
                Disconnected = 3
            }
            /// <summary>
            /// Connects to a remote NetComm host with a specific client ID.
            /// </summary>
            /// <param name="HostIP">The remote host ip address.</param>
            /// <param name="HostPort">The remote host port.</param>
            /// <param name="ClientID">The client ID to be entered with.</param>
            /// <remarks></remarks>
            public void Connect(string HostIP, int HostPort, string ClientID)
            {
                IP = HostIP;
                Port = HostPort;
                ID = ClientID;
                try
                {
                    T = new Thread(Main);
                    T.Start();
                }
                catch (Exception ex)
                {
                    EventRaise(EventPointer.errEncounter, ex);
                }
            }
            /// <summary>
            /// Sends data to a client connected to the server.
            /// </summary>
            /// <param name="Data">An array of bytes to send.</param>
            /// <param name="ID">The target client to send the data to, set null to send the data to the host.</param>
            /// <remarks></remarks>
            public void SendData(byte[] Data, string ID = null)
            {
                try
                {
                    Data = Useful_Classes.Communication.Functions.ClientMsg.GetBytes(new Useful_Classes.Communication.Functions.ClientMsg(ID, Data));
                    netStream.Write(Data, 0, Data.Length);
                    netStream.Flush();
                }
                catch (Exception ex)
                {
                    EventRaise(EventPointer.errEncounter, ex);
                }
            }
            private void Main()
            {
                try
                {
                    clientSocket.Connect(IP, Port);
                    if (clientSocket.Connected)
                    {
                        //Send the client ID data
                        netStream = clientSocket.GetStream();
                        netStream.Write(Convert2Ascii(ID), 0, ID.Length);
                        netStream.Flush();
                        //Sending ends
                        ConnectedHost = true;
                        EventRaise(EventPointer.Connected);
                    }

                }
                catch (Exception ex)
                {
                }
                while (clientSocket.Connected & ConnectedHost)
                {
                    try
                    {
                        byte[] bytes = new byte[Convert.ToInt32(clientSocket.ReceiveBufferSize) + 1];
                        netStream.Read(bytes, 0, Convert.ToInt32(clientSocket.ReceiveBufferSize));
                        DecryptBytes(bytes);
                    }
                    catch (Exception ex)
                    {
                        break; // TODO: might not be correct. Was : Exit While
                    }
                }
                if ((Context != null))
                {
                    Disconnect();
                }
                else
                {
                    Disconnect();
                }
            }
            private void DecryptBytes(byte[] Message)
            {
                bool Disconnected = true;
                for (int b = 0; b <= Message.Length - 1; b++)
                {
                    Useful_Classes.Communication.Functions.ClientMsg Msg = Useful_Classes.Communication.Functions.ClientMsg.FromBytes(Message, b);
                    if ((Msg.Data != null))
                    {
                        EventRaise(EventPointer.DataReceived, Msg);
                        Disconnected = false;
                    }
                }
                if (Disconnected)
                    ConnectedHost = false;
            }
            /// <summary>
            /// Disconnects from the server.
            /// </summary>
            /// <remarks></remarks>
            public void Disconnect()
            {
                // ERROR: Not supported in C#: OnErrorStatement

                ConnectedHost = false;
                //Save the current client socket variable so we can redeclare it (Because it will be disposed)
                int SBufferSize = 0;
                int RBufferSize = 0;
                SBufferSize = clientSocket.SendBufferSize;
                RBufferSize = clientSocket.ReceiveBufferSize;
                bool NDelay = clientSocket.NoDelay;
                if ((clientSocket != null))
                    clientSocket.Close();
                clientSocket = new TcpClient();
                clientSocket.SendBufferSize = SBufferSize;
                clientSocket.ReceiveBufferSize = RBufferSize;
                clientSocket.NoDelay = NDelay;
                if (Disconnected != null)
                {
                    Disconnected();
                }
            }
            /// <summary>
            /// Returns the current connection state (True if connected)
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool isConnected
            {
                get { return ConnectedHost; }
            }
            /// <summary>
            /// Converts a set of bytes to a string.
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static string ConvertFromAscii(byte[] bytes)
            {
                System.Text.Encoding encoding = new System.Text.ASCIIEncoding();

                return encoding.GetString(bytes);
            }
            /// <summary>
            /// Converts a string to a set of bytes.
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static byte[] Convert2Ascii(string str)
            {
                return ASCIIEncoding.GetEncoding("windows-1255").GetBytes(str);
            }
        }

        static class Functions
        {
            private static Encoding MainEncoding = ASCIIEncoding.GetEncoding("windows-1255");
            public struct ClientMsg
            {
                public string ID;
                public byte[] Data;
                public ClientMsg(string ClientID, byte[] DataByte)
                {
                    ID = ClientID;
                    Data = DataByte;
                }
                public static byte[] GetBytes(Useful_Classes.Communication.Functions.ClientMsg c)
                {
                    if (c.ID == null)
                        c.ID = "";
                    string EncryptedString = "!" + c.ID + "!" + c.Data.Length + "!";
                    return Functions.JoinBytes(Functions.MainEncoding.GetBytes(EncryptedString), c.Data);
                }
                public static ClientMsg FromBytes(byte[] bytes, int Start = 0)
                {
                    try
                    {
                        ClientMsg Msg = default(Useful_Classes.Communication.Functions.ClientMsg);
                        int Length_Start = Start;
                        //Defines from where our ID starts (skip from useless '0')
                        while (!(bytes[Length_Start] == 33))
                        {
                            Length_Start += 1;
                            if (Length_Start >= bytes.Length - 1)
                                return new ClientMsg(null, null);
                        }
                        int Length_End = Length_Start + 1;
                        //Defines from where our ID ends
                        while (!(bytes[Length_End] == 33))
                        {
                            Length_End += 1;
                            if (Length_End >= bytes.Length - 1)
                                return new ClientMsg(null, null);
                        }
                        Msg.ID = (Length_End - Length_Start > 1 ? Functions.MainEncoding.GetString(Functions.ChopBytes(bytes, Length_Start + 1, Length_End - Length_Start - 1)) : null);
                        //Gets the ID within the "!" characters - if the length is 1 it means that the ID is nothing.
                        Length_Start = Length_End;
                        //The first "!" character is already the previous one
                        Length_End = Length_Start + 1;
                        //Gets the data length within the "!" characters
                        while (!(bytes[Length_End] == 33))
                        {
                            Length_End += 1;
                            if (Length_End >= bytes.Length - 1)
                                return new ClientMsg(null, null);
                        }
                        int DataLength = 0;
                        DataLength = Functions.ChopBytes(bytes, Length_Start + 1, Length_End - Length_Start - 1).Length;
                        Msg.Data = Functions.ChopBytes(bytes, Length_End + 1, DataLength);
                        Start = Length_End + DataLength;
                        return Msg;
                    }
                    catch (Exception ex)
                    {
                        return new ClientMsg(null, null);
                    }
                }
            }
            public static bool CheckPacket(byte[] bytes)
            {
                foreach (byte b in bytes)
                {
                    if (b != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            public struct EventArgs
            {
                public int EventP;
                public object Arg;
                public EventArgs(int e, object Argument)
                {
                    EventP = e;
                    Arg = Argument;
                }
            }
            public static byte[] JoinBytes(byte[] Original, byte[] JoinPart)
            {
                byte[] JoinedBytes = new byte[Original.Length + JoinPart.Length];
                int cnt = 0;
                foreach (byte b in Original)
                {
                    JoinedBytes[cnt] = b;
                    cnt += 1;
                }
                foreach (byte b in JoinPart)
                {
                    JoinedBytes[cnt] = b;
                    cnt += 1;
                }
                return JoinedBytes;
            }
            public static byte[] ChopBytes(byte[] Original, int Start, int Length = 0)
            {
                if (Length == null | Length < 1)
                {
                    Length = Original.Length - Start;
                }
                byte[] ChoppedBytes = new byte[Length];
                int cnt = 0;
                for (int by = Start; by <= Start + Length - 1; by++)
                {
                    ChoppedBytes[cnt] = Original[by];
                    cnt += 1;
                }
                return ChoppedBytes;
            }
        }
    }
}
*/