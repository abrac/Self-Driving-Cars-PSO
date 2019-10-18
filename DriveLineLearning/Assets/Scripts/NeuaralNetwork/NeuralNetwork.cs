using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

public class NeuralNetwork : MonoBehaviour
{
    //[RequireComponent(typeof(CarController))]

    private CarController m_Car; // the car controller we want to use

    // Neural Network parameters
    public int hidddenLayers = 1;
    public int hLayer_size = 5;
    public int outputs = 1;
    public int inputs = 5;
    public float maxValue = 1f;

    // Sigmoid function parameters
    private const float euler = 2.71828f;

    // List of neuron outputs and weights
    public List<List<float>> neurons { get; set; }
    private List<float[][]> weights { get; set; }

    private int layers;

    void Start()
    {
        layers = hidddenLayers + 2; // total layers including input and output layers
        weights = new List<float[][]>(); //weight initialisation
        neurons = new List<List<float>>();

        // Assign Values to neurons
        for (int i = 0; i < layers; i++)
        {
            float[][] layerWeights;
            List<float> layer = new List<float>();
            int size = getSizeLayer(i);
            if(i != hidddenLayers+1)
            {
                layerWeights = new float[size][];
                int nextSize = getSizeLayer(i + 1); // size of the next layer
                for (int j = 0; j < size;  j++)
                {
                    layerWeights[j] = new float[nextSize];
                    for (int k = 0; k < nextSize; k++)
                    {
                        layerWeights[j][k] = getRandom();
                    }
                }
                weights.Add(layerWeights);
            }
            for (int j = 0; j < size; j++)
            {
                layer.Add(0);
            }
            neurons.Add(layer);
        }
    }

    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
        Feedforward(new float[] { 1, 1, 1, 1, 1 });
        // pass the input to the car!
        float h = getOutputs()[0];
        float v = getOutputs()[1];
        m_Car.Move(h, v, v, 0f);
    }

    public void ChangeWeights(List<float[][]> weights)
    {
        this.weights = weights;
    }

    public void Feedforward(float [] inputs)
    {
        List<float> inputLayer = neurons[0];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputLayer[i] = inputs[i];
        }

        //Update neurons in all layers
        for (int j = 0; j <neurons.Count; j++)
        {
            float[][] weightsLayer = weights[j];
            int nLayer = j + 1;// value to keep track of the next layer
            List<float> neuronLayer = neurons[j]; 
            List<float> neuronNLayer = neurons[nLayer]; // next Layer of neurons 
            for (int k = 0; k <neuronNLayer.Count; k++)
            {
                float fnet = 0;
                for (int l = 0; l < neuronLayer.Count; l++)
                {
                    fnet += weightsLayer[j][l] * neuronLayer[j];
                }
                neuronNLayer[k] = Fnet(fnet);
            }
        }
        
    }
    public List<float> getOutputs()
    {
        return neurons[neurons.Count -1 ];
    }
    public float Fnet(float x)
    {
        float sigmoid = 1 / (float)(1 + Mathf.Pow(euler, -x));
        return sigmoid;
    }

    public float getRandom()
    {
        return Random.Range(-maxValue, maxValue);
    }

    public int getSizeLayer( int i)
    {
        int size = 0;
        if (i == 0)
            size = inputs;

        else if (i == hidddenLayers + 1)
        {
            size = outputs;
        }

        else
            size = hidddenLayers;

        return size;

    }

}
