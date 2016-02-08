using UnityEngine;
using System.Collections.Generic;

public class GestureLearning : MonoBehaviour
{

    public const int pointsCount = 20;

    NeuralNetwork.NeuralNetwork neuralNetwork;
    string learnDataFile = "learn.txt";
    string networkDataFile = "network.txt";

    List<NeuralNetworkIO> learnData;


    void Awake()
    {
        neuralNetwork = new NeuralNetwork.NeuralNetwork(pointsCount * 3, new int[] { 42, 24, 6 });
        LoadLearnData();
        LoadNetwork();
    }

    [ContextMenu("LoadLearnData")]
    public void LoadLearnData()
    {

    }

    [ContextMenu("SaveLearnData")]
    public void SaveLearnData()
    {

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

    public void Learn()
    {
        neuralNetwork.LearningAlg.Learn(GetInputs(), GetOutputs());
    }

    public void Add(NeuralNetworkIO data)
    {
        learnData.Add(data);
    }
}
