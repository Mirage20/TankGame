using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TankGameXNAClient
{
    
    public delegate void MessageReceivedHandler(string message);

    //Manage the network communications between server and client
    class Communicator
    {
        public event MessageReceivedHandler MessageReceived;
        private Thread receiver;

        private NetworkStream clientStream; //Stream - outgoing
        private TcpClient client; //To talk back to the client
        private BinaryWriter writer; //To write to the clients

        private NetworkStream serverStream; //Stream - incoming        
        private TcpListener listener; //To listen to the clinets        
        

        private static Communicator comm = new Communicator();

        private const int SERVER_PORT = 6000;
        private const string SERVER_IP = "127.0.0.1";
        private const int CLIENT_PORT = 7000;
        private bool isStarted = false;

        private Communicator()
        {
            
        }

        public static Communicator GetInstance()
        {
            return comm;
        }

        // start the listning thread
        public void StartListening()
        {
            if (!isStarted)
            {
                receiver = new Thread(new ThreadStart(ReceiveDataLoop));
                receiver.Start();
                isStarted = true;
            }
        }

        //stop the listning thread
        public void StopListening()
        {
            if (isStarted)
            {
                listener.Stop();
                receiver.Abort();
                receiver.Join();              
                isStarted = false;
            }
        }
        // loop for gather incoming messages
        private void ReceiveDataLoop()
        {

            Socket connection = null; //The socket that is listened to       

            //Creating listening Socket
            this.listener = new TcpListener(IPAddress.Parse(Global.ClientIP), Global.ClientPort);
            //Starts listening
            this.listener.Start();
            //Establish connection upon client request  
            string message = "";
            while (true)
            {
                try
                {


                    //connection is connected socket
                    connection = listener.AcceptSocket();
                    if (connection.Connected)
                    {
                        //To read from socket create NetworkStream object associated with socket
                        this.serverStream = new NetworkStream(connection);

                        SocketAddress sockAdd = connection.RemoteEndPoint.Serialize();
                        string s = connection.RemoteEndPoint.ToString();
                        List<Byte> inputStr = new List<byte>();
                        int i = 0;
                        int receivedByte;
                        do
                        {
                            receivedByte = this.serverStream.ReadByte();
                            inputStr.Add((Byte)receivedByte);
                            
                            i++;
                        } while (serverStream.DataAvailable);

                        message = Encoding.UTF8.GetString(inputStr.ToArray());
                        this.serverStream.Close();
                        connection.Close();
                        MessageReceived(message);
                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }



        }

        // send message to the server
        public void SendData(string message)
        {
            if (message.Length > 0)
            {
                //Opening the connection
                this.client = new TcpClient();

                try
                {

                    this.client.Connect(IPAddress.Parse(Global.ServerIP), Global.ServerPort);

                    if (this.client.Connected)
                    {
                        //To write to the socket
                        this.clientStream = client.GetStream();

                        //Create objects for writing across stream
                        this.writer = new BinaryWriter(clientStream);
                        Byte[] tempStr = Encoding.ASCII.GetBytes(message);

                        //writing to the port                
                        this.writer.Write(tempStr);
                        this.writer.Close();
                        this.clientStream.Close();
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                finally
                {
                    this.client.Close();
                }
            }
        }


    }

}
