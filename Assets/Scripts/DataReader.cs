using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataReader
{
    public List<Dictionary<string, float>> gatherInput()
    {
        List<Dictionary<string, float>> data = new List<Dictionary<string, float>>();
        try
        {   // Open the text file using a stream reader.
            //using (StreamReader sr = new StreamReader(dataFile))
            //{
            //String text = sr.ReadToEnd();
            TextAsset SourceFile = (TextAsset)Resources.Load("TestFile", typeof(TextAsset));
            String text = SourceFile.text;
            Debug.Log(text);

            String[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                data.Add(new Dictionary<string, float>());

                if (!String.IsNullOrEmpty(lines[i]))
                {
                    String[] values = lines[i].Split(new[] { ";" }, StringSplitOptions.None);

                    for (int j = 0; j < values.Length; j++)
                    {
                        String[] pair = values[j].Split(new[] { "," }, StringSplitOptions.None);
                        data[i].Add(pair[0], float.Parse(pair[1]));
                    }
                }
                else
                {
                    data[i].Add("epsilon", 0);
                    Console.WriteLine("Epsilon output");
                }
            }
            //}
        }
        catch (Exception e)
        {
            Debug.LogError("The file could not be read:");
            Debug.LogError(e.Message);
        }

        return data;


        for (int j = 0; j < data.Count; j++)
        {
            foreach (KeyValuePair<string, float> pair in data[j])
            {
                Console.WriteLine("Key = {0}, Value = {1}", pair.Key, pair.Value);
            }
        }
    }
}
