using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace QuizServer
{
    public partial class Form1 : Form
    {


        public static string BASE_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuizServer");
        public static Form1 main;
        public Form1()
        {
            InitializeComponent();
            main = this;
        }


        /* Adds the ability to move the window from the top panel. */
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



        private void Form1_Load(object sender, EventArgs e)
        {
            this.createBaseFolder();
            QuizHandler.getQuizes();
            panel1.MouseDown += new MouseEventHandler(Panel1_MouseDown);

            flowLayoutPanel1.DragEnter += new DragEventHandler(flowpanel_DragEnter);
            flowLayoutPanel1.DragLeave += new EventHandler(flowLayoutPanel1_DragLeave);
            flowLayoutPanel1.DragDrop += new DragEventHandler(flowLayoutPanel1_DragDrop);

            this.loadQuizes();

        }

        private void flowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            flowLayoutPanel1.BackgroundImage = base.BackgroundImage;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                String ext = System.IO.Path.GetExtension(file);

                if (ext == ".xml" || ext == ".quiz")
                {
                    String filename = System.IO.Path.GetFileName(file);
                    if (QuizHandler.isNameTaken(filename))
                    {
                        MessageBox.Show("The file is named the same as a prevouis quiz. Change the name of the file.");
                        continue;
                    }

                    try
                    {
                        System.IO.File.Copy(file, BASE_FOLDER + "\\quizes\\" + System.IO.Path.GetFileName(filename));
                    }  catch(IOException exception)
                    {
                        Console.WriteLine(exception.Message);
                        MessageBox.Show("An error occured. (Probably a quiz with the same filename)");
                        continue;
                    }
                    QuizHandler.loadQuiz(file);
                }

                Console.WriteLine(file);
            }

        }

        private void flowpanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            flowLayoutPanel1.BackgroundImage = Properties.Resources.dragin;
        }

        private void flowLayoutPanel1_DragLeave(object sender, System.EventArgs e)
        {

            flowLayoutPanel1.BackgroundImage = base.BackgroundImage;
            
        }
        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void loadQuizes()
        {
            foreach(Quiz quiz in QuizHandler.getQuizes())
            {
                QuizObject panelitem = new QuizObject(quiz);
                panelitem.title.Text = quiz.title;
                panelitem.questions.Text = "" + quiz.amount_questions;
                this.flowLayoutPanel1.Controls.Add(panelitem);
            }
        }

        private void createBaseFolder()
        {
            Console.WriteLine("Base folder: " + BASE_FOLDER);
            if(!Directory.Exists(BASE_FOLDER))
            Directory.CreateDirectory(BASE_FOLDER);

            if(!Directory.Exists(BASE_FOLDER + "\\quizes"))
            {
                Directory.CreateDirectory(BASE_FOLDER + "\\quizes");


                WebClient client = new WebClient();
                String downloadedString = client.DownloadString("https://pastebin.com/raw/2SHasB5U");

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(downloadedString);
                xdoc.Save(BASE_FOLDER + "\\quizes\\Test-Quiz.xml");

            }

        }

        
    }
}
