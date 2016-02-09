using UnityEngine;

public class NeuralNetworkTest : MonoBehaviour
{

    void Start()
    {
        NeuralNetwork.NeuralNetwork nn = new NeuralNetwork.NeuralNetwork(1, new int[] { 5, 5, 2 });
        int numberOfData = 100;
        float[][] input = new float[numberOfData][];
        float[][] output = new float[numberOfData][];
        for (int a = 0; a < numberOfData; ++a)
        {
            float progress = 1f / (numberOfData - 1) * a;
            input[a] = new float[] { progress };
            output[a] = new float[] { progress, 1f - 2 * Mathf.Abs(0.5f - progress) };
        }
        nn.randomizeAll();
        nn.LearningAlg = new NeuralNetwork.GeneticLearningAlgorithm(nn);
        nn.LearningAlg.Learn(input, output);
        for (int a = 0; a <= 10; ++a)
        {
            float progress = 1f / (10) * a;
            float[] result = nn.Output(new float[] { progress });
            Debug.Log("Compute(" + a + "):\t\t" + result[0] + ",\t\t" + result[1] + "\t\t| err: " + Mathf.Abs(progress - result[0]) + ",\t\t" + Mathf.Abs((1f - 2 * Mathf.Abs(0.5f - progress) - result[1])));
        }
    }

}
