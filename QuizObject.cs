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
    public partial class QuizObject : UserControl
    {

        public Quiz quiz;
        public Label title;
        public Label questions;

        public QuizObject(Quiz quiz)
        {
            this.quiz = quiz;
            InitializeComponent();
            questions = this.label2;
            title = this.label1;

         

        }

        

        public QuizObject()
        {
            InitializeComponent();
            questions = this.label2;
            title = this.label1;
        }

   
      


        private void label1_Click(object sender, EventArgs e)
        {
            /* Redirecting to the server setup page. */
            QuizHandler.server = new QuizServer(quiz);
            QuizHandler.server.Show();
            Form1.main.Visible = false;
        }
    }
}
