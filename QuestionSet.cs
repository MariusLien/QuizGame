using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizServer
{
    public class QuestionSet
    {
        


        public String quest;
        public List<Answer> answers;



        public QuestionSet(String quest, List<Answer> answers)
        {
            this.quest = quest;
            this.answers = answers;
        }

        public List<char> getOptions()
        {
            List<char> options = new List<char>();
            foreach(Answer answer in answers)
            {
                Console.WriteLine(answer.id);
                options.Add(answer.id);
            }

            return options;
        }

        public List<Answer> correctAnswers()
        {
            List<Answer> correctAnswers = new List<Answer>();
            foreach(Answer ans in answers)
            {
                if(ans.correct)
                {
                    correctAnswers.Add(ans);
                }
            }
            return correctAnswers;
        }

    }
}
