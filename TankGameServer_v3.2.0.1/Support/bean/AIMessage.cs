using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lk.ac.mrt.cse.pc11.bean
{
    public class AIMessage
    {
        private Contestant contestant;
        private String command;

        public Contestant Contestant
        {
            get { return contestant; }
           // set { contestant = value; }
        }
      

        public String Command
        {
            get { return command; }
           // set { command = value; }
        }

        public AIMessage(Contestant con, String com)
        {
            contestant = con;
            command = com;
        }

    }
}
