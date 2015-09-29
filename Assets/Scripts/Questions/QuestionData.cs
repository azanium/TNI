using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;

public class QuestionData
{
    public static QuestionData current = null;

    public class answer
    {
        /// <summary>
        /// Correct attribute of the answer
        /// </summary>
        [XmlAttribute("correct")]
        public bool correct = false;

        /// <summary>
        /// Option attribute of the answer
        /// </summary>
        [XmlAttribute("option")]
        public string option;

        /// <summary>
        /// The Answer Text
        /// </summary>
        [XmlText()]
        public string text;
    }

    public class question
    {
        /// <summary>
        /// Question's Color Type
        /// </summary>
        public string type;

        /// <summary>
        /// Question's Section
        /// </summary>
        public string section;

        /// <summary>
        /// The Question
        /// </summary>
        public string content;

        /// <summary>
        /// Answers
        /// </summary>
        public List<answer> answers;

        /// <summary>
        /// Correct Answer
        /// </summary>
        public string correct;

        /// <summary>
        /// Additional Info
        /// </summary>
        public string info;

        public question()
        {
            answers = new List<answer>();
        }

        public GameData.PillarType GetQuestionType()
        {
            GameData.PillarType qtype;
            if (char.IsLower(this.type[0]))
            {
                this.type = char.ToUpper(type[0]) + (this.type.Substring(1, this.type.Length - 1).ToLower());
            }

            qtype = (GameData.PillarType)Enum.Parse(typeof(GameData.PillarType), this.type);

            return qtype;
        }
    }
    public List<question> questions;

    public QuestionData()
    {
        questions = new List<question>();
    }

    public void EmitQuestionsMap(out Dictionary<GameData.PillarType, List<QuestionData.question>> map)
    {
        map = new Dictionary<GameData.PillarType, List<question>>();

        foreach (var q in questions)
        {
            GameData.PillarType qKey = q.GetQuestionType();
            List<question> questionList;
            if (map.ContainsKey(qKey))
            {
                questionList = map[qKey];
            }
            else
            {
                questionList = new List<question>();
                map.Add(qKey, questionList);
            }
            questionList.Add(q);
        }
    }

    static public void Load(string name)
    {
        current = (QuestionData)XmlManager.LoadInstanceAsXml(name, typeof(QuestionData));
    }

    static public QuestionData LoadAsInstance(string name)
    {
        QuestionData qdata = (QuestionData)XmlManager.LoadInstanceAsXml(name, typeof(QuestionData));

        return qdata;
    }

    static public void Save(string name, QuestionData qData)
    {
        XmlManager.SaveInstanceAsXml(name, typeof(QuestionData), qData);
    }
}
