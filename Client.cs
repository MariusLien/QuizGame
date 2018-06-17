using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuizServer
{
    public class Client
    {

        public int points;
        private TcpClient tcpClient;
        public string name;
        public ClientObject cobject;
        public bool instaniated;
        public Answer answer;

        public Client(TcpClient tcpClient, string name)
        {
            this.tcpClient = tcpClient;
            this.name = name;
            this.answer = null;
            this.instaniated = false;
            this.points = 0;
            this.listenForData();

            Console.WriteLine("[" + name + "] I'm all set up: ");
        }



        async private void listenForData()
        {
            await Task.Run(() =>
            {
                while (true && tcpClient.Connected)
                {
                    NetworkStream networkStream = tcpClient.GetStream();

                    byte[] data = new Byte[256];


                    String responseData = String.Empty;


                    Int32 bytes = networkStream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    String[] args = responseData.Split(':');

                    /* Parse the data */

                    if(QuizHandler.server.gamestarted)
                    {
                        if(args[0] == "0x01")
                        {


                            char option = 'A';
                            Console.WriteLine(option);

                            this.answer = getAnswer(option);
                        }
                    }

                }
            });
        }

        private Answer getAnswer(char option)
        {
            foreach(Answer ans in QuizHandler.server.currentquestion.answers)
            {
                if(ans.id == option)
                {
                    return ans;
                }
            }
            return null;
        }
        
        public void sendData(String data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            tcpClient.Client.Send(bytes);
        }

        public void sendData(byte[] bytes)
        {
            tcpClient.Client.Send(bytes);
        }

    }
}