using System;
using System.Collections.Generic;
using System.Text;

namespace lk.ac.mrt.cse.pc11.bean
{
    class DataObject
    {
        #region "Variables"
        private string msg = "";
        private string clientMachine = "";
        private int clientPort = -1;
        private List<string> playerIPList = new List<string>();
        private List<int> playerPortList = new List<int>();
        private int considerFrom = 0;
        #endregion

        #region "Properties"
        public string MSG
        {
            get { return msg; }
            set { msg = value; }
        }

        public string ClientMachine
        {
            get { return clientMachine; }
        }

        public int ClientPort
        {
            get { return clientPort; }
        }

        public List<string> PlayerIPList
        {
            get { return playerIPList; }
            set { playerIPList = value; }
        }

        public List<int> PlayerPortList
        {
            get { return playerPortList; }
            set { playerPortList = value; }
        }

        public int ConsiderFrom
        {
            get { return considerFrom; }
            set { considerFrom = value; }
        }


        #endregion

        #region "Public Methods"
        public DataObject(string msgP, string clientMachineP, int clientPortP)
        {
            msg = msgP;
            clientMachine = clientMachineP;
            clientPort = clientPortP;
        }

        public DataObject(string msgP, List<string> clientMachinesList, List<int> clientPortP)
        {
            msg = msgP;
            playerIPList = clientMachinesList;
            playerPortList = clientPortP;
        }
        #endregion
    }
}
