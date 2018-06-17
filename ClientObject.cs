using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuizServer
{
    public partial class ClientObject : UserControl
    {

        public String name;
        public Label nameL;
        public Client client;

        public ClientObject(String name, Client client)
        {
            InitializeComponent();
            this.name = name;
            this.client = client;
            this.nameL = this.label1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            client.sendData("1x02");
            QuizServer.main.flowLayoutPanel1.Controls.Remove(client.cobject);
            QuizServer.main.clients.Remove(client);
        }



 


    }
}
