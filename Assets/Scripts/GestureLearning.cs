using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class GestureLearning : MonoBehaviour
{

    public const int pointsCount = 20;
    public const int inputSize = pointsCount * 3;
    public const int gestureCount = 6;
    public const int wordsInLineCount = inputSize + gestureCount;
    char[] learnDataSplitCharacters = new char[] { ' ', '|', '#' };

    NeuralNetwork.NeuralNetwork neuralNetwork;
    string learnDataFile = "learn.txt";
    string networkDataFile = "network.txt";

    List<NeuralNetworkIO> learnData;


    void Awake()
    {
        neuralNetwork = new NeuralNetwork.NeuralNetwork(inputSize, new int[] { 42, 24, 6 });
        LoadLearnData();
        LoadNetwork();
    }

    [ContextMenu("LoadLearnData")]
    public void LoadLearnData()
    {
        try
        {
            learnData = new List<NeuralNetworkIO>();

            if (!File.Exists(learnDataFile))
                throw new Exception("Can't load file");


            TextReader reader = File.OpenText(learnDataFile);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] numbersStr = line.Split(learnDataSplitCharacters);
                if (numbersStr.Length != wordsInLineCount)
                {
                    Debug.LogError("Incorrect number count in line:" + numbersStr.Length);
                    continue;
                }

                NeuralNetworkIO data = new NeuralNetworkIO();
                data.input = new float[inputSize];
                data.output = new float[gestureCount];
                for (int a = 0; a < wordsInLineCount; ++a)
                {
                    if (a < inputSize)
                        data.input[a] = float.Parse(numbersStr[a]);
                    else
                        data.output[a - inputSize] = float.Parse(numbersStr[a]);

                }

                learnData.Add(data);
            }

            reader.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    [ContextMenu("SaveLearnData")]
    public void SaveLearnData()
    {
        if (!File.Exists(learnDataFile))
            File.Delete(learnDataFile);

        StreamWriter writer = File.CreateText(learnDataFile);
        foreach (NeuralNetworkIO data in learnData)
        {
            for (int a = 0; a < wordsInLineCount; ++a)
            {
                if (a < inputSize)
                    writer.Write(data.input[a]);
                else
                    writer.Write(data.output[a - inputSize]);
                if (a == pointsCount - 1 || a == pointsCount * 2 - 1)
                    writer.Write("|");
                else if (a == pointsCount * 3 - 1)
                    writer.Write("#");
                else if (a != wordsInLineCount - 1)
                    writer.Write(" ");
            }
            writer.WriteLine();
        }
        writer.Close();
    }

    [ContextMenu("LoadLearnData")]
    public void LoadNetwork()
    {
        try
        {
            neuralNetwork = NeuralNetwork.NeuralNetwork.load(networkDataFile);
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Cannot open file: " + networkDataFile);
        }
    }

    [ContextMenu("SaveNetwork")]
    public void SaveNetwork()
    {
        neuralNetwork.save(networkDataFile);
    }

    public float[][] GetInputs()
    {
        float[][] inputs = new float[learnData.Count][];
        for (int a = 0; a < learnData.Count; ++a)
        {
            inputs[a] = learnData[a].input;
        }
        return inputs;
    }

    public float[][] GetOutputs()
    {
        float[][] outputs = new float[learnData.Count][];
        for (int a = 0; a < learnData.Count; ++a)
        {
            outputs[a] = learnData[a].output;
        }
        return outputs;
    }

    [ContextMenu("Learn")]
    public void Learn()
    {
        neuralNetwork.LearningAlg.Learn(GetInputs(), GetOutputs());
    }

    public void Add(NeuralNetworkIO data)
    {
        learnData.Add(data);
    }
}
