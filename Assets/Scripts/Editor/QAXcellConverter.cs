using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Text.RegularExpressions;

public class QAXcellConverter : EditorWindow
{
    private static QAXcellConverter window;

    private string fileName;
    private TextAsset csvTextFile;
    private string targetFileName;
    private string section;
    
    [MenuItem("Truth N Islam/QA Excel Convert")]
    static void ExecuteInteractionItem()
    {
        window = EditorWindow.GetWindow<QAXcellConverter>("TNI - Suhendra Ahmad");
        window.Initialize();
    }

    void Initialize()
    {
        targetFileName = "QuestionsNAnswers.xml";
    }

    void OnGUI()
    {
        GUILayout.Label("Convert", EditorStyles.boldLabel);

        csvTextFile = EditorGUILayout.ObjectField("CSV File", csvTextFile, typeof(TextAsset), false) as TextAsset;
        targetFileName = EditorGUILayout.TextField("Target XML File", targetFileName);
        section = EditorGUILayout.TextField("Section", section);

        if (GUILayout.Button("Convert"))
        {
            if (string.IsNullOrEmpty(section))
            {
                EditorUtility.DisplayDialog("Error", "Section must be set", "OK");
                return;
            }

            Convert(csvTextFile, targetFileName, section);
        }
    }

    string GetColorNameFromCode(string code)
    {
        switch (code)
        {
            case "B" :
                return "blue";
            case "G" :
                return "green";
            case "R" :
                return "red";
            case "Y":
                return "yellow";
            case "O":
                return "orange";
            case "P":
                return "purple";
        }

        return "green";
    }

    void Convert(TextAsset csv, string targetFile, string section)
    {
        QuestionData qData = new QuestionData();
        if (System.IO.File.Exists(XmlManager.XmlPath + targetFile))
        {
            qData = QuestionData.LoadAsInstance(XmlManager.XmlPath + targetFile);
            if (qData == null)
            {
                Debug.Log(string.Format("Target File {0} is exist, but it's content is null", targetFile));
                qData = new QuestionData();
            }
        }

        StringReader reader = new StringReader(csv.text);

        using (CsvReader csvReader = new CsvReader(reader, false))
        {
            int fieldCount = csvReader.FieldCount;
            Debug.Log(fieldCount);
            while (csvReader.ReadNextRecord())
            {
                string colorCode = csvReader[0];

                // Color Code pass check
                if ((colorCode != "B" && colorCode != "G" && colorCode != "R" &&
                    colorCode != "Y" && colorCode != "P" && colorCode != "O") ||
                    string.IsNullOrEmpty(colorCode))
                {
                    continue;
                }

                // Get the question content
                var qcontent = csvReader[2];

                QuestionData.question question = new QuestionData.question();
                question.content = qcontent.Trim();
                question.section = section;
                question.type = GetColorNameFromCode(colorCode);


                qData.questions.Add(question);
            }
        }

        QuestionData.Save(targetFile, qData);

        //Debug.Log("Converted " + count);
    }

    private string RemoveQuote(string source)
    {
        string output = "";
        bool quoteBefore = false;
        for (int i = 0; i < source.Length; i++)
        {
            char current = source[i];
            
            if (current == '"')
            {
                if (!quoteBefore)
                {
                    quoteBefore = true;
                    continue;
                }
                else
                {
                    quoteBefore = false;
                }
            }

            output += current;
        }

        return output;
    }

    private string GetToken(string source, ref int position)
    {
        string output = "";
        int start = position;
        Debug.LogWarning("===============>" + position);
        for (int i = start; i < source.Length; i++)
        {
            position = i;

            char current = source[i];
            if (current != ',')
            {
                output += current;
            }
            else
            {
                break;
            }
        }

        return output;
    }
}
