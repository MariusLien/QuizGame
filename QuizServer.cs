using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuizServer
{
    public partial class QuizServer : Form
    {

        public bool timeOut;
        public int timer;
        public static QuizServer main;
        public TcpListener server;
        public UdpClient udpserver;
        public Quiz quiz;
        public List<Client> clients;
        public bool listening;
        public QuestionSet currentquestion;
        private bool running;
        public bool allAnswered;
        public bool gamestarted;


        public QuizServer(Quiz quiz)
        {
            InitializeComponent();
            this.quiz = quiz;
            clients = new List<Client>();
            listening = false;
            main = this;
            gamestarted = false;
            this.running = false;
            timeOut = false;
            this.timer = 15;
            this.currentquestion = null;
      
             

        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        async private void listenForClients()
        {
            listening = true;
            Console.WriteLine("Listening");
            await Task.Run(async () =>
            {
                while (listening && running)
                {

                    Console.WriteLine("Running");
                    TcpClient client = await server.AcceptTcpClientAsync();
                    
                    instaniateClient(client);
                    //ThreadPool.QueueUserWorkItem(instaniateClient, client);
                }

            });
        }

        private void openServer()
        {
            timer1.Start();

            server = new TcpListener(IPAddress.Any, 8001);
            server.Start();
            Console.WriteLine("Server is now running...");

            listenForClients();
            startLANlistener();

        }


        private void instaniateClient(Object client)
        {


            TcpClient tcpClient = (TcpClient)client;
            Console.WriteLine("TCP client connected");
            bool foundName = false;

            while (foundName == false && running)
            {
                NetworkStream networkStream = tcpClient.GetStream();

                byte[] data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = networkStream.Read(data, 0, data.Length); //(**This receives the data using the byte method**)
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes); //(**This converts it to string**)
                Console.WriteLine(responseData);
                String[] args = responseData.Split(':');
                
                String packet = "0x00";

                if (args[0] == packet)
                {

                    String name = args[1];

                    if(NameIsTaken(name))
                    {
                        tcpClient.Client.Send(Encoding.ASCII.GetBytes("1x01:" + name));
                        continue;
                    }

                    Console.WriteLine("Client added: " + name);
                    foundName = true;
                    this.clients.Add(new Client(tcpClient, name));
                    break;
                }
            }
        }

        public bool NameIsTaken(string name)
        {
            bool taken = false;
            foreach(Client client in clients)
            {
                if (client.name.ToLower() == name.ToLower()) taken = true;
            }


            return taken;
        }

        async private void startLANlistener()
        {


            udpserver = new UdpClient(9991);
            await Task.Run(() =>
            {
                while (true && running)
                {
                    IPEndPoint point = new IPEndPoint(IPAddress.Any, 9991);

                    byte[] recieved_bytes = udpserver.Receive(ref point);

                    String line = Encoding.ASCII.GetString(recieved_bytes);
                    if (line == "0x02") {
                        byte[] packet = Encoding.ASCII.GetBytes("0x03");
                        Console.WriteLine(line);
                        udpserver.Send(packet, packet.Length, point);
                    }
                }
                udpserver.Close();
            });

        }


        private void QuizServer_Load(object sender, EventArgs e)
        {
            panel1.MouseDown += new MouseEventHandler(Panel1_MouseDown);

            this.label5.Text = "Players connected: " + clients.Count;
            this.label6.Text = "Amount of questions: " + quiz.amount_questions;
            this.label4.Text = "Current selected quiz: " + quiz.title;

            running = true;

            this.openServer();

        }

        private void closeServer()
        {
            this.server.Server.Close();
            this.udpserver.Close();
            this.running = false;
            this.listening = false;
            server.Stop();
            this.Dispose();
            QuizHandler.server = null;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            closeServer();

            this.Visible = false;
            Form1.main.Visible = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            label5.Text = "Players connected: " + clients.Count;

            foreach(Client client in clients)
            {
                if(!client.instaniated)
                {
                    client.instaniated = true;
                    client.cobject = new ClientObject(client.name, client);
                    client.cobject.nameL.Text = client.name;
                    this.flowLayoutPanel1.Controls.Add(client.cobject);
                }
            }

        }

        private void broadcast(String data)
        {
            foreach(Client client in clients)
            {
                client.sendData(data);
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        async private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(clients.Count <= 0)
            {
                MessageBox.Show("Der skal minimum være 1 spiller forbundet.");
                return;
            }

            gamestarted = true;
            pictureBox1.Enabled = false;
            pictureBox1.Cursor = Cursors.No;




            foreach(QuestionSet question in quiz.questionsets)
            {
              
                

            }


        }

        private void timeInterval_Tick(object sender, EventArgs e)
        {
            timer -= 1;
            label10.Text = "Timer: " + timer;
            broadcast("0x06:" + timer);
            if(timer <= 0)
            {
                timeInterval.Stop();
                broadcast("1x03");
                timeOut = true;
                return;
            }

            int answered = 0;

            foreach(Client client in clients)
            {
                if(client.answer != null)
                {
                    answered += 1;
                    label11.Text = "Answered: " + answered;
                }
            }

            if(answered == clients.Count)
            {
                timeInterval.Stop();
                allAnswered = true;
                return;
            }

        }
    }
}






