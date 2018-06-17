using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuizServer
{
    class QuizHandler
    {

        private static List<Quiz> quizes;
        public static QuizServer server;
        public static List<Quiz> getQuizes()
        {

            if (quizes != null) return quizes;
            quizes = new List<Quiz>();
            foreach(String file in Directory.EnumerateFiles(Form1.BASE_FOLDER + "\\quizes")) 
            {
                Console.WriteLine("File found: " + file);
                Quiz quiz = new Quiz(file);
                Console.WriteLine("Parsing file...");
                if(quiz.loadQuestions())
                {
                    quizes.Add(quiz);
                } else
                {
                    Console.WriteLine("Parse error occured in file:\n " + file);
                }

            }

            return quizes;
        }

        public static bool isNameTaken(String name)
        {
            foreach(Quiz quiz in quizes)
            {
                if(quiz.title.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public static void loadQuiz(String file)
        {
            Quiz quiz = new Quiz(file);

            if(quiz.loadQuestions())
            {
                quizes.Add(quiz);
                QuizObject quizobject = new QuizObject(quiz);
                quizobject.title.Text = quiz.title;
                quizobject.questions.Text = "" + quiz.questionsets.Count;
                Form1.main.flowLayoutPanel1.Controls.Add(quizobject);
            } else
            {
                MessageBox.Show("An error occured while parsing the quiz.");
            }

        }


    }
}
