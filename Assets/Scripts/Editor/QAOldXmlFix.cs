using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;

public class QAOldXmlFix : EditorWindow
{
    private static QAOldXmlFix window;

    private TextAsset csvTextFile;
    private string targetFileName;
    private string section;

    [MenuItem("Truth N Islam/QA Fix Old Xml")]
    static void ExecuteInteractionItem()
    {
        window = EditorWindow.GetWindow<QAOldXmlFix>("TNI - Suhendra Ahmad");
        window.Initialize();
    }

    void Initialize()
    {
        targetFileName = "QnA.xml";
    }

    void OnGUI()
    {
        GUILayout.Label("Convert", EditorStyles.boldLabel);

        csvTextFile = EditorGUILayout.ObjectField("Xml Text File", csvTextFile, typeof(TextAsset), false) as TextAsset;
        targetFileName = EditorGUILayout.TextField("Xml Target File Name", targetFileName);

        if (GUILayout.Button("Fix"))
        {
            if (string.IsNullOrEmpty(targetFileName))
            {
                EditorUtility.DisplayDialog("Error", "Section must be set", "OK");
                return;
            }

            Read();//(csvTextFile);
        }

        /*if (GUILayout.Button("Read"))
        {
            Read();
        }*/
    }

    void Fix(TextAsset csv)
    {
        StringReader reader = new StringReader(csv.text);

        StringBuilder strings = new StringBuilder();

        StreamWriter writer;
        FileInfo t = new FileInfo(XmlManager.XmlPath + targetFileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            //t.Delete(); 
            writer = t.CreateText();
        }
        

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.ToLower().Contains("<correctcontent>"))
            {
             
                line = line.Replace("</correct>", "</correctcontent>");
                line = line.Replace("& ", "and ");
                Debug.Log(line);
            }
            strings.Append(line);
            writer.WriteLine(line);

        }

        
        writer.Close();

    }

    private QuestionData qdata;
    private QuestionData.question q;
    private QuestionData.answer answer;

    private string xmlValue = "";
   // private string xmlAttribute = "";

    void Read()
    {
        using (XmlReader xml = XmlReader.Create(new StringReader(csvTextFile.text)))
        {
            xml.MoveToContent();

            qdata = new QuestionData();

            while (xml.Read())
            {
                
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element:
                        //Debug.Log("element => " + xml.Name);
                        StartElement(ref qdata, xml);
                        break;

                    case XmlNodeType.Attribute:
                        //Debug.Log(xml.Value);
                        break;

                    case XmlNodeType.Text:
                        xmlValue = xml.Value;
                        break;

                    case XmlNodeType.EndElement:
                        EndElement(ref qdata, xml);
                        break;
                }
            }
        }
    }

    private void StartElement(ref QuestionData qData, XmlReader xml)
    {
        switch (xml.Name)
        {
            case "question":
                q = new QuestionData.question();
                break;

            case "answers":
                break;

            case "answer":
                answer = new QuestionData.answer();
                answer.option = xml.GetAttribute(0);
                break;
        }
    }

    private void EndElement(ref QuestionData qData, XmlReader xml)
    {
        switch (xml.Name)
        {
            case "question":
                qData.questions.Add(this.q);
                break;

            case "type":
                this.q.type = xmlValue;
                break;

            case "section":
                this.q.section = xmlValue;
                FixTypeBasedOnSection(ref this.q);
                break;

            case "content":
                this.q.content = xmlValue;
                break;

            case "info":
                this.q.info = xmlValue;
                break;

            case "answer":
                answer.text = xmlValue;
                this.q.answers.Add(answer);
                break;

            case "correct":
                foreach (var ans in this.q.answers)
                {
                    if (ans.option == xmlValue)
                    {
                        ans.correct = true;
                    }
                }
                break;

            case "questions" :
                Debug.Log("end questions");
                QuestionData.Save(targetFileName, qData);
                break;
        }

        xmlValue = "";
    }

    private void FixTypeBasedOnSection(ref QuestionData.question q)
    {
        /*
         * blue= quran
            green= hadith
            yellow=islamic history
            orange=islamic science
            pink= general knowledge
            purble=prophets*/

        if (q.section.ToLower() == "quran")
        {
            q.type = "blue";
        }

        if (q.section.ToLower() == "hadith")
        {
            q.type = "green";
        }

        if (q.section.ToLower() == "islamic science")
        {
            q.type = "orange";
        }

        if (q.section.ToLower() == "islamic history")
        {
            q.type = "yellow";
        }

        if (q.section.ToLower() == "general knowledge")
        {
            q.type = "red";
        }

        if (q.section.ToLower().Contains("prophets"))
        {
            q.type = "purple";
        }

    }
}
