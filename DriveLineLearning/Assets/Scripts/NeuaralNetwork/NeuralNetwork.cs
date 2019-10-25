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
    public int outputs = 4;
    public int inputs = 0;
    public float maxValue = 1f;
    public bool sleep = false;
    public float[] outInput;
 
    // List of neuron outputs and weights
    public List<List<float>> neurons;
    public List<float[][]> weights { get; set; }

    private int layers;
    int size = 0;
    void Awake()
    {
        m_Car = GetComponent<CarController>();
        Feelers_RayGenerator feelerNum = this.GetComponentInChildren<Feelers_RayGenerator>();
        size = feelerNum.feelerDists.GetLength(0);
        inputs = size +3 +(outputs-2);  //bias, carspeed and angle included in input layer, and the inputs for the recurrentNN from output
        layers = hiddenLayers + 2; // total layers including input and output layers
        weights = new List<float[][]>(); //weight initialisation
        neurons = new List<List<float>>();
        outInput = new float[outputs-2];

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

 
    private void FixedUpdate()
    {
        if (!sleep)
        {
            Feelers_RayGenerator feelerNum = this.GetComponentInChildren<Feelers_RayGenerator>();
            float[] inputs = new float[size + 3 + (outputs - 2)]; // initialised size of inputs as the num of feelers + 2 vars(speed and angle),inputs from Output and bias
            int j = 0;
            for (int i = 0; i < feelerNum.feelerDists.Length; i++)
            {
                inputs[i] = feelerNum.feelerDists[i];
                j++;
            }
            // j is to keep track of the counter used to add to the inputs list 
            int io = 0; // to iterate through the outputs, with the first 2 items passed as input to the car
            for (int i = j; i < j + outInput.Length; i++)
            {
                inputs[i] = outInput[io];
                ++io;
            }
            inputs[inputs.GetLength(0) - 3] = m_Car.CurrentSpeed;
            inputs[inputs.GetLength(0) - 2] = m_Car.CurrentSteerAngle;
            inputs[inputs.GetLength(0) - 1] = -1;//bias value 


            Feedforward(inputs);
            //Get outputs to be used for recurrent NN
            for (int i = 2; i < outputs; i++)
            {
                outInput[i-2] = getOutputs()[i];
            }
            // pass the input to the car!
            float h = getOutputs()[0];
            float v = getOutputs()[1];
            m_Car.Move(h, v, v, 0f);
        }
        else
        {
            m_Car.Move(0, 0, 0, 0/*h, v, v, 0f*/);
            m_Car.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            m_Car.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_Car.Move(0, 0, 0, 0/*h, v, v, 0f*/);
        }
    }

    public void WakeUp()
    {
        sleep = false;
    }

    public void Sleep()
    {
        sleep = true;
    }

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
            size = hLayer_size;

        return size;

    }

}
