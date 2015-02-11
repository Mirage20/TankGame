using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using lk.ac.mrt.cse.pc11.bean;

namespace lk.ac.mrt.cse.pc11.util
{
    /// <summary>
    /// Is responsible for communication handling
    /// </summary>
    class Communicator
    {
        #region "Variables"
        private NetworkStream clientStream; //Stream - outgoing
        private TcpClient client; //To talk back to the client
        private BinaryWriter writer; //To write to the clients

        private NetworkStream serverStream; //Stream - incoming        
        private TcpListener listener; //To listen to the clinets        
        public string reply = ""; //The message to be written
        
        private static Communicator comm = new Communicator();
        #endregion

        private Communicator()
        {
        }

        public static Communicator GetInstance()
        {
            return comm;
        }

        public void ReceiveData()
        {
            bool errorOcurred = false;
            Socket connection = null; //The socket that is listened to       
            try
            {
                //Creating listening Socket
                this.listener = new TcpListener(IPAddress.Parse(Constant.SERVER_IP), Constant.SERVER_PORT);
                //Starts listening
                this.listener.Start();
                //Establish connection upon client request
                DataObject dataObj;
                while (true)
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

                        int asw = 0;
                        while (asw != -1)
                        {
                            asw = this.serverStream.ReadByte();
                            inputStr.Add((Byte)asw);
                        }

                        reply = Encoding.UTF8.GetString(inputStr.ToArray());
                        this.serverStream.Close();
                        string ip = s.Substring(0, s.IndexOf(":"));
                        int port = Constant.CLIENT_PORT;
                        try
                        {
                            string ss = reply.Substring(0, reply.IndexOf(";"));                            
                            port = Convert.ToInt32(ss);                            
                        }
                        catch (Exception)
                        {                            
                           port= Constant.CLIENT_PORT;
                        }
                        Console.WriteLine(ip + ": " + reply.Substring(0, reply.Length - 1));
                        dataObj = new DataObject(reply.Substring(0, reply.Length - 1), ip, port);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(GameEngine.Resolve), (object)dataObj);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication (RECEIVING) Failed! \n " + e.Message);
                errorOcurred = true;
            }
            finally
            {
                if(connection != null)
                    if(connection.Connected)
                        connection.Close();
                if (errorOcurred)
                    this.ReceiveData();
            }
        }

        public void SendData(object stateInfo)
        {
            DataObject dataObj = (DataObject)stateInfo;
            //Opening the connection
            this.client = new TcpClient();

            try
            {
                if (dataObj.ClientPort == 7000)
                {
                    
                    this.client.Connect(dataObj.ClientMachine, dataObj.ClientPort);

                    if (this.client.Connected)
                    {
                        //To write to the socket
                        this.clientStream = client.GetStream();

                        //Create objects for writing across stream
                        this.writer = new BinaryWriter(clientStream);
                        Byte[] tempStr = Encoding.ASCII.GetBytes(dataObj.MSG);

                        //writing to the port                
                        this.writer.Write(tempStr);
                        Console.WriteLine("\t Data: " + dataObj.MSG + " is written to " + dataObj.ClientMachine + " on " + dataObj.ClientPort);
                        this.writer.Close();
                        this.clientStream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication (WRITING) to " + dataObj.ClientMachine + " on " + dataObj.ClientPort + "Failed! \n " + e.Message);
            }
            finally
            {
                this.client.Close();
            }
        }

        public void BroadCast(object stateInfo)
        {
            DataObject dataObj = (DataObject)stateInfo;
            //Opening the connection
         //   this.client = new TcpClient(); //before even number bug fix
            int i = 0;
            try
            {
                for (i = dataObj.ConsiderFrom; i < dataObj.PlayerIPList.Count; i++)
                {
                    this.client = new TcpClient(); //the even number bux fix

                    if (dataObj.PlayerPortList[i] == 7000)
                    {
                        
                        this.client.Connect(dataObj.PlayerIPList[i], dataObj.PlayerPortList[i]);

                        if (this.client.Connected)
                        {
                            //To write to the socket
                            this.clientStream = client.GetStream();

                            //Create objects for writing across stream
                            this.writer = new BinaryWriter(clientStream);
                            Byte[] tempStr = Encoding.ASCII.GetBytes(dataObj.MSG);

                            //writing to the port                
                            this.writer.Write(tempStr);
                            this.writer.Close();
                            this.clientStream.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication (BROADCASTING) to " +
                    dataObj.PlayerIPList[i] + " on " + dataObj.PlayerPortList[i] + " Failed! \n " + e.Message);
                if (i != dataObj.PlayerIPList.Count - 1) //Some other clients should get this message.
                {
                    dataObj.ConsiderFrom = i + 1;
                    this.BroadCast(dataObj);
                }
            }
            finally
            {   
                this.client.Close();
            }
        }

    }
}
