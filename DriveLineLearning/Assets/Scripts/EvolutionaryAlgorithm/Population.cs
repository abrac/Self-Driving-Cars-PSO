using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    // Parameters
    public int POPULATION_SIZE = 50;
    public int INITIAL_WEIGHTS_UPPER_BOUND = 100;

    // Car objects - Population
    public List<GameObject> CarPopulation = new List<GameObject>();
    
    // Individuals of the population's fitness
    private List<float> Fitness = new List<float>();

    // Stored velocity of each individual 
    List<List<float[]>> velocityVectors = new List<List<float[]>>();

    // Stored Personal Best of each individual 
    List<List<float[]>> personalBestWeights = new List<List<float[]>>();

    // Stored Global Best Weights and fitness achieved
    List<float[]> GlobalBestWeights = new List<float[]>();
    float GlobalBestFitness;

    // Links to used Game Objects
    public GameObject NeuralNetworkControlledCar;
    public GameObject StartingBlocks;

    // Weights (vector dimensions) based on car's NN structure
    private List<int> Weights_template_dimensions;

    // 
    private Random random;

    // Create genes(weights) property and instantiate and position cars
    void Start()
    {
        random = new Random();

        // Get "chromosome structure" from NeuralNetwork structure
        List<float[]> Weights_Template = NeuralNetworkControlledCar.GetComponent<NeuralNetwork>().weights;
        foreach(float[] cur in Weights_Template)
        {
            Weights_Template.Add(cur.Length);
        }




        for (int x = 0; x < POPULATION_SIZE; x++)
        {
            GameObject newCar = GameObject.Create(NeuralNetworkControlledCar);

        }

        //Create genes array representing weights of NN, and initialize with random NN wieghts.
    }

    void Update()
    {

    }

    private void PositionCarAtStartLine(int carIndex)
    {
        StartingBlocks.transform.find
    }
    
    
    private List<float[]> CloneOfWeights(List<float[]> weightsToClone)
    {
        List<float[]> newClone = new List<float[]>();
        foreach (float[] cur in weightsToClone)
        {
            float[] newClone = new float[cur.Length];
            for (int x = 0, length = cur.Length; x < length; x++)
            {
                newClone[x] = cur[x];
            }
            newCopy.Add(newClone);
        }

        return newClone;
    }

    // Generate Randomly Initialised Weights
    private List<float[]> GenerateRandomlyInitializedWeights()
    {

        //initialization call
        //float weight = random.NextInt(INITIAL_WEIGHTS_UPPER_BOUND) + ranndom.NextDouble();
    }

    // Calculate fitness for a given car
    private float CalculateFitness(int carIndex)
    {
        // Get Distance car travelled
        // using: float dist = car.GetComponent<DistanceTracker>().distanceTravelled;

        // Get time elapsed



        // some funtion to combine them
        return fitness;

    }




}
