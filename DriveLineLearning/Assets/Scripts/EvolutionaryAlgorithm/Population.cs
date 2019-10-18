using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    // Parameters
    public int POPULATION_SIZE = 50;
    public int INITIAL_WEIGHTS_UPPER_BOUND = 100;

    // Car objects - Population
    public List<GameObjects> CarPopulation = new List<GameObjects>();
    
    // Individuals of the population's fitness
    private List<float> Fitness = new List<float>();

    // Stored velocity of each individual 
    List<List<float[]>> velocityVectors = new List<List<float[]>>();

    // Stored Personal Best of each individual 
    List<List<float[]>> personalBestDNA = new List<List<float[]>>();

    // Stored Global Best DNA and fitness achieved
    List<float[]> GlobalBestDNA = new List<float[]>();
    float GlobalBestFitness;

    // Links to used Game Objects
    public GameObject NeuralNetworkControlledCar;
    public GameObject StartingBlocks;

    // DNA (vector dimensions) based on car's NN structure
    private List<int> DNA_template_dimensions;

    // 
    private Random random;

    // Create genes(weights) property and instantiate and position cars
    void Start()
    {
        random = new Random();

        // Get "chromosome structure" from NeuralNetwork structure
        List<float[]> DNA_Template = NeuralNetworkControlledCar.GetComponent<NeuralNetwork>().weights;
        foreach(float[] cur in DNA_Template)
        {
            DNA_Template.Add(cur.Length);
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

    // Generate Randomly Initialised DNA
    private List<float[]> GenerateRandomlyInitializedDNA()
    {

        //initialization call
        //float weight = random.NextInt(INITIAL_WEIGHTS_UPPER_BOUND) + ranndom.NextDouble();
    }

    // Calculate fitness for a given car
    private float CalculateFitnessForCarIndex(int index)
    {
        // Get Distance car travelled
        // using: float dist = car.GetComponent<DistanceTracker>().distanceTravelled;

        // Get time elapsed



        // some funtion to combine them
        return fitness;

    }




}
