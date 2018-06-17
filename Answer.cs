using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizServer
{
    public class Answer
    {

        public bool correct;
        public String answer;
        public char id;

        public Answer(String answer, bool correct, char id)
        {
            this.answer = answer;
            this.correct = correct;
            this.id = id;
        }


    }
}
