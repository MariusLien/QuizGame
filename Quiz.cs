using System;
using System.Collections.Generic;
using System.Xml;

namespace QuizServer
{
    public class Quiz
    {

        public String title;
        public String filepath;
        public int amount_questions;
        public List<QuestionSet> questionsets;


        public Quiz(String name)
        {

            questionsets = new List<QuestionSet>();

            String[] args = name.Split('\\');

            String title = System.IO.Path.GetFileName(name);
            title = title.Replace("-", " ");
            title = title.Replace(".xml", "");
            title = title.Replace(".quiz", "");

            this.title = title;
            this.filepath = name;

        }

       

        public bool loadQuestions()
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            XmlDocument document = new XmlDocument();
            Console.WriteLine(filepath);
            document.Load(filepath);
            XmlNodeList nodes = document.GetElementsByTagName("question");
            if (nodes == null) return false;

            foreach(XmlNode node in nodes)
            {
                String quest = node.FirstChild.InnerText;
                List<Answer> answers = new List<Answer>();



                XmlNodeList options = node.SelectNodes("./options/*");
                    if (options == null) return false;
                int index = 0;
                foreach(XmlNode option in options)
                {
                    char id = alpha[index];
                    Console.WriteLine(id);
                    answers.Add(new Answer(option.InnerText, Boolean.Parse(option.Attributes["correct"].Value), id));
                    index++;
                }

                this.questionsets.Add(new QuestionSet(quest, answers));
            }

            
            this.amount_questions = this.questionsets.Count;
            return true;
        }
    }
}