using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankGameClient
{
    public partial class Form1 : Form
    {
        Communicator com;
        public Form1()
        {
            InitializeComponent();
            com = Communicator.GetInstance();
            com.MessageReceived +=new MessageReceivedHandler(com_MessageReceived);
            
        }

        void com_MessageReceived(string message)
        {
            
            object[] p = new object[1]{message};
            BeginInvoke(new MessageReceivedHandler(MessegeReceived), p);
        }
        int i = 0;
        private void MessegeReceived(string message)
        {
           
            textBox1.AppendText(message + "\r\n");
            i++;
            this.Text = i.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            com.SendData(textBox2.Text);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            com.StartListening();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            com.StopListening();
        }

       

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    com.SendData("LEFT#");
                    break;
                case Keys.Right:
                    com.SendData("RIGHT#");
                    break;
                case Keys.Up:
                    com.SendData("UP#");
                    break;
                case Keys.Down:
                    com.SendData("DOWN#");
                    break;
                case Keys.Space:
                    com.SendData("SHOOT#");
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageDecoder.GetInstance().com_MessageReceived("I:P0:9,2;3,2;5,7;1,4;3,6:5,8;2,4;7,6;9,8;1,3;7,2;2,1;4,8;6,3:4,7;6,8;7,4;4,2;8,1;0,3;7,1;4,3;8,6;1,7#");
            MessageDecoder.GetInstance().com_MessageReceived("S:P0;0,0;0:P1;0,9;0#");
            MessageDecoder.GetInstance().com_MessageReceived("S:P0;0,0;0:P1;0,9;0:P2;9,0;0:P3;9,9;0:P4;5,5;0#");
            MessageDecoder.GetInstance().com_MessageReceived("G:P0;0,0;0;0;100;0;0:P1;0,9;0;0;100;0;0:P2;9,0;0;0;100;0;0:P3;9,9;0;0;100;0;0:P4;5,5;0;0;100;0;0:1,3,0;7,1,0;9,2,0;2,6,0;4,8,0;6,2,0;9,7,0#");
            MessageDecoder.GetInstance().com_MessageReceived("G:P0;0,0;0;0;100;0;0:P1;0,9;0;0;100;0;0:9,2,0;3,2,0;5,7,0;1,4,0;3,6,0#");
            MessageDecoder.GetInstance().com_MessageReceived("G:P0;1,7;1;1;100;3567;3727:P1;4,6;3;0;70;47854;47854:7,1,4;4,3,4;3,6,3;0,8,4;2,4,2;7,6,0;9,3,1;1,3,4;7,2,0#");
            MessageDecoder.GetInstance().com_MessageReceived("C:7,3:58679:1071#");
            MessageDecoder.GetInstance().com_MessageReceived("C:1,1:58963:1755#");
            MessageDecoder.GetInstance().com_MessageReceived("L:5,0:2352#");
            MessageDecoder.GetInstance().com_MessageReceived("L:1,5:61410#");

        }
    }
}
