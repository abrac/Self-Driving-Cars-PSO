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
    public int hiddenLayers = 1;
    public int hLayer_size = 5;
    public int outputs = 2;
    public int inputs = 5;
    public float maxValue = 1f;

    // List of neuron outputs and weights
    public List<List<float>> neurons;
    public List<float[][]> weights { get; set; }

    private int layers;

    void Start()
    {
        layers = hiddenLayers + 2; // total layers including input and output layers
        weights = new List<float[][]>(); //weight initialisation
        neurons = new List<List<float>>();

        // Assign Values to neurons
        for (int i = 0; i < layers; i++)
        {
            float[][] layerWeights;
            List<float> layer = new List<float>();
            int layerSize = getSizeLayer(i);
            if(i != hiddenLayers + 1) // checking that not the last layer (it is + 1, rather than + 2 because i is zero-based)
            {
                layerWeights = new float[layerSize][];
                int nextSize = getSizeLayer(i + 1); // size of the next layer
                for (int j = 0; j < layerSize;  j++)
                {
                    layerWeights[j] = new float[nextSize];
                    for (int k = 0; k < nextSize; k++)
                    {
                        layerWeights[j][k] = getRandom();
                    }
                }
                weights.Add(layerWeights);
            }
            //What is this for? Geoff = Think it stores the Neuron's value (Input/lastestResultantFNet)
            for (int j = 0; j < layerSize; j++)
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
        Feedforward(new float[] { getRandom(), getRandom(), getRandom(), getRandom(), getRandom() });
        // pass the input to the car!
        float h = getOutputs()[0];
        float v = getOutputs()[1];
        m_Car.Move(h, v, v, 0f);
    }

    /*public void ChangeWeights(List<float[][]> weights)
    {
        this.weights = weights;
    }*/

    public void Feedforward(float [] inputs)
    {
        // Create reference to input layer.
        List<float> inputLayer = neurons[0];
        // Replace values in input layer to input argument.
        for (int i = 0; i < inputs.Length; i++)
        {
            inputLayer[i] = inputs[i];
        }

        // Update neuron values in layers 1 to output layer
        for (int j = 0; j <neurons.Count-1; j++)
        {
            // Create reference to weights of layer j
            float[][] weightsLayer = weights[j];
            // value to keep track of the next layer
            int nLayer = j + 1;
            // Create reference to neuron values of layer j
            List<float> neuronLayer = neurons[j];
            // Create reference to neuron values of layer j + 1
            List<float> neuronNLayer = neurons[nLayer];
            // Updating values of all neurons of layer j+1
            // Looping through neurons of layer j+1
            for (int k = 0; k <neuronNLayer.Count; k++)
            {
                float fnet = 0;
                // Multiplying the value of each neuron in layer j by the each weight in layer j
                // Looping through the number of weights in a neuron from layer j
                for (int l = 0; l < neuronLayer.Count; l++)
                {
                    //weight from neuron l of the jth layer to neuron k of the j+1th layer
                    fnet += weightsLayer[l][k] * neuronLayer[l];
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
        float sigmoid = 2 / (float)(1 + Mathf.Exp(-x)) -1;
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

        else if (i == hiddenLayers + 1)
        {
            size = outputs;
        }

        else
            size = hiddenLayers;

        return size;

    }

}
