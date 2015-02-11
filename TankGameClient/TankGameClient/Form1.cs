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
    }
}
