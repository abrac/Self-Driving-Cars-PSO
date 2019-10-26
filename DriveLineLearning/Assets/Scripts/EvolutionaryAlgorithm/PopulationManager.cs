using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PopulationManager : MonoBehaviour
{
    // Parameters
    public int POPULATION_SIZE = 10;
    public int INITIAL_WEIGHTS_UPPER_BOUND = 1;
    public int MAX_GENERATIONS = 1500;
    public int BEST_INDIVIDUAL_STREAK = 5000;

    // The car population script Component
    public CarPopulation cars;

    private float sessionStartTime;

    // Individuals of the population's fitness
    private List<float> CurrentNN_Fitness = new List<float>();

    // Stored velocity of each individual particle 
    private List<List<float[][]>> ParticleVelocityVectors = new List<List<float[][]>>();

    // Stored Personal Best of each individual 
    private List<List<float[][]>> PersonalBestNN_Weights = new List<List<float[][]>>();
    public List<float> PersonalBestNN_Fitness = new List<float>();

    // Stored y-Hat of each individual (using Ring topology)
    private List<List<float[][]>> Y_HatNN_Weights = new List<List<float[][]>>();
    private List<float> Y_HatNN_Fitness = new List<float>();

    // Stored Global Best Weights and fitness achieved
    private List<float[][]> GlobalBestWeights = new List<float[][]>();
    public float GlobalBestNN_Fitness;

    // Links to used Game Objects
    public GameObject BestCarDemo;

    // PSO parameters
    public float cognitiveConst = 0.7f;
    public float socialConst = 1.0f;
    public float w = 0.6f;
    public float socialRandom;
    public float cognitiveRandom;

    // Other variables
    public int curGeneration;
    public int leadCounter;
    [SerializeField]
    private int numberCarsDriving = 0;
    [SerializeField]

    // Variables For Outputting Data
    public bool logOutputs  = false;
    public string folderToSaveTo;
    private StreamWriter outputStream;

    // Create genes(weights) property and instantiate and position cars
    void Start()
    {
        // Initialize some variables
        curGeneration = 0;
        leadCounter = 0;

        // Open logging files
        if (logOutputs && sessionStartTime == 0)
        {
            sessionStartTime = Time.time;
            Feelers_RayGenerator feelerSettings = cars.carPopulation[0].transform.GetChild(0).gameObject.GetComponent<Feelers_RayGenerator>();
            NeuralNetwork NN = cars.carPopulation[0].GetComponent<NeuralNetwork>();
            // Initialize naming conventions (open/create all the files) add a time stamp line to each
            if (folderToSaveTo.Length != 0) 
                folderToSaveTo = folderToSaveTo + "/";
            string filepath = String.Format("{0}Log(Feelr#={1}len{2};NN-w={3}socC={4}cognC={5};#recur={6};#hid={7})"
                                        ,folderToSaveTo, feelerSettings.feelerDists.Length, feelerSettings.feelerLength, w, socialConst, cognitiveConst, NN.outputs-2, NN.hLayer_size);

            outputStream = new StreamWriter("Assets/logs/" + filepath + ".txt", true);
            outputStream.WriteLine("/////////////////// START ///////////////////");
            outputStream.WriteLine("///// " + DateTime.Now);
            outputStream.WriteLine("///// " + filepath);
        }


        //Initialize Velocity Vectors to Zero
        ParticleVelocityVectors = new List<List<float[][]>>();
        for (int x = 0; x < POPULATION_SIZE; x ++) {
            ParticleVelocityVectors.Add(InitializeParticleVelocityToZero());
        }

        // Can't evaluate fitness until raced at least once...
        // So can build that in on Update() 
    }

    private void OnDestroy() {
        if (logOutputs)
        {
            outputStream.WriteLine("///////////////////  END  ///////////////////");
            outputStream.Close();
        }   
    }

    void FixedUpdate()
    {
        numberCarsDriving = 0;
        foreach (GameObject cur in cars.carPopulation) 
        {
            if (cur.GetComponent<CarSideEvolutionaryBehaviour>().isDriving) 
            {
                numberCarsDriving++;
            }
        }

        // Still part of setup - run initialised cars once to get initial fitness
        if (curGeneration == 0)
        {
            // If no cars are still driving
            if (numberCarsDriving <= 0)
            {
                // Note: Have fitness calculated by each car as it crashes. Have a method that the car calls to decrease NumberCarsDriving by 1.
                // Initialize Personal bests (weights and fitness)
                PersonalBestNN_Fitness = new List<float>();
                PersonalBestNN_Weights = new List<List<float[][]>>();
                InitializeParticlePersonalBests(); // to the weights currently in the car and the resultant fitness
                
                // Initialize "Global" bests (y-hat) All-best and Ring topology (Based on Personal bests initially - then subsequently on Y-Hats)
                GlobalBestNN_Fitness = 0;
                GlobalBestWeights = new List<float[][]>();
                Y_HatNN_Fitness = new List<float>();
                Y_HatNN_Weights = new List<List<float[][]>>();
                UpdateGlobalAndY_HatRingTopologyBestVectors(true);
            
                // reset test cycle - new generation
                curGeneration++;
                EnableAllTheCrashedCarsNNs();
            }
        }
        // Start of evolutionary cycles
        else if (leadCounter < BEST_INDIVIDUAL_STREAK)
        {

            // have fitness calc in each car as it crashes. have a method that the car calls to decrease NumberCarsDriving by 1.
            // Fetch each car's latest NN fitness
            for (int curIndividual = 0; curIndividual < POPULATION_SIZE; curIndividual++) 
            {
                if (!cars.carPopulation[curIndividual].gameObject.GetComponent<CarSideEvolutionaryBehaviour>().isDriving) 
                {
                    // Update personal best
                    if (GetA_CarsNN_Fitness(curIndividual) > PersonalBestNN_Fitness[curIndividual]) 
                    {
                        PersonalBestNN_Fitness[curIndividual] = GetA_CarsNN_Fitness(curIndividual);
                        PersonalBestNN_Weights[curIndividual] = CloneOfWeights(GetA_CarsNN_Weights(curIndividual));
                    }
                }
            }

            // Recalculate latest ring Y_Hats 
            UpdateGlobalAndY_HatRingTopologyBestVectors(false);

            // Calculate new velocity for each individual
            for (int bob = 0; bob < cars.carPopulation.Count; bob++) 
            {
                if (!cars.carPopulation[bob].gameObject.GetComponent<CarSideEvolutionaryBehaviour>().isDriving) 
                {
                    List<float[][]> bobIndividual = GetA_CarsNN_Weights(bob);
                    List<float[][]> bobVelocity = ParticleVelocityVectors[bob];
                    List<float[][]> bobPersonalBest = PersonalBestNN_Weights[bob];
                    //List<float[][]> bobYHat = Y_HatNN_Weights[bob];
                    // for each dimension of the Vectors
                    for (int layer = 0; layer < bobVelocity.Count; layer++) 
                    {
                        for (int to = 0; to < bobVelocity[layer].Length; to++)
                        {
                            for (int from = 0; from < bobVelocity[layer][to].Length; from++)
                            {
                                socialRandom = UnityEngine.Random.Range(0f,1f);
                                cognitiveRandom = UnityEngine.Random.Range(0f,1f);
                                // Gbest
                                bobVelocity[layer][to][from] = w*bobVelocity[layer][to][from] + cognitiveConst*cognitiveRandom*(bobPersonalBest[layer][to][from] - bobIndividual[layer][to][from]) + socialConst*socialRandom*(GlobalBestWeights[layer][to][from] - bobIndividual[layer][to][from]);                                                                                                                    
                            }
                        } 
                    }
                    // Apply new velocity to Particle
                    for (int layer = 0; layer < bobVelocity.Count; layer++) 
                    {
                        for (int to = 0; to < bobVelocity[layer].Length; to++)
                        {
                            for (int from = 0; from < bobVelocity[layer][to].Length; from++)
                            {
                                bobIndividual[layer][to][from] = bobIndividual[layer][to][from] + bobVelocity[layer][to][from];                                                            
                            }
                        }
                    }
                    // Place new Gen weights back in the car
                    cars.carPopulation[bob].GetComponent<NeuralNetwork>().weights = bobIndividual;
                }
            }

            // reset test cycle - new generation
            //curGeneration++;
            leadCounter++;
            // Set cars to be driving again
            // ...here... Generation 1.. GO!
            EnableAllTheCrashedCarsNNs/*AndResetTimer*/();
        }
        else 
        {
            //Solution is reached..
            Debug.Log("PSO SOLUTION: Final fitness: " + GlobalBestNN_Fitness);
            if (logOutputs)
            {
                //Log final best fitness for graph
                outputStream.WriteLine((Time.time-sessionStartTime + ";" + GlobalBestNN_Fitness).Replace(',','.'));
            }
            // Record the Solution's weights
            WriteBestWeightsToFile();
            Debug.Log("Best Weights saved to file...");
            leadCounter = 0;
            //Save BEST Global Weights to a new text file: or put them in one car and let it race
        }
    }

    public void PositionCarAtStartLine(GameObject car)
    {
        // Set car's velocity to zero
        //car.GetComponent<NeuralNetwork>().Sleep();
        /*Rigidbody carsRigidbody = car.GetComponent<Rigidbody>();
        carsRigidbody.velocity = Vector3.zero;
        carsRigidbody.angularVelocity = Vector3.zero;*/
        car.transform.position = cars.CarStartingPosition;
        car.transform.rotation = cars.CarStartingRotation;  
        //this.gameObject.GetComponent<Timer>().ResetTimer();  
    }

    public void ResetDemoCar() 
    {
        // Update NN with Best Weights
        if (BestCarDemo.GetComponent<CarSideEvolutionaryBehaviour>().isDriving != true)
        {
            if (GlobalBestWeights.Count != 0)
                BestCarDemo.GetComponent<NeuralNetwork>().weights = CloneOfWeights(GlobalBestWeights);
            BestCarDemo.GetComponent<CarSideEvolutionaryBehaviour>().fitness = GlobalBestNN_Fitness;

            // Reset and Start
            BestCarDemo.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            BestCarDemo.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BestCarDemo.GetComponent<CarSideEvolutionaryBehaviour>().isDriving = true;
            BestCarDemo.GetComponent<NeuralNetwork>().WakeUp();
            BestCarDemo.GetComponent<Timer>().ResetTimer();
        }
    }
    
    private List<float[][]> CloneOfWeights(List<float[][]> weightsToClone)
    {
        List<float[][]> newCopy = new List<float[][]>();
        foreach (float[][] cur in weightsToClone)
        {
            float[][] newClone = new float[cur.Length][];
            for (int x = 0, length = cur.Length; x < length; x++)
            {
                newClone[x] = new float[cur[x].Length];
                for (int y = 0, innerLength = cur[x].Length; y < innerLength; y++)
                {   
                    newClone[x][y] = cur[x][y];
                }
            }
            newCopy.Add(newClone);
        }
        return newCopy;
    }

    public void WriteBestWeightsToFile() 
    {
        NeuralNetwork NN = BestCarDemo.GetComponent<NeuralNetwork>();
        string path = String.Format("Assets/Best weights Logs/Best_log_NNFormat({0}-HidUnits,{1}-Inputs).txt", NN.hLayer_size, NN.inputs);

        StreamWriter sr = new StreamWriter(path,true);
        sr.WriteLine("/////////////////// START ///////////////////");
        sr.WriteLine("Hidden layers: {0}", NN.hiddenLayers);
        sr.WriteLine("Hidden layer size: {0}", NN.hLayer_size);
        sr.WriteLine("Hidden layers: {0}", NN.hiddenLayers);

        sr.WriteLine("Fitness: " + GlobalBestNN_Fitness);
        int layer = 0;
        foreach (float[][] cur in GlobalBestWeights)
        {
            for (int x = 0, length = cur.Length; x < length; x++)
            {
                for (int y = 0, innerLength = cur[x].Length; y < innerLength; y++)
                {   
                    sr.WriteLine("{0}:{1}:{2}:{3}",layer,x,y,cur[x][y]);
                }
            }
            layer++;
        }
        sr.WriteLine("///////////////////  END  ///////////////////");
        sr.Close();
    }

    // Generate Randomly Initialised Weights
    /*private List<float[][]> GenerateRandomlyInitializedWeights()
    {
        List<float[][]> newRandomizedWeights = new List<float[][]>();
        foreach (int[] cur in Weights_template_dimensions)
        {
            float[][] newLayerWeights = new float[cur[0]][];
            for (int x = 0; x < cur[0]; x++)
            {
                for (int y = 0; y < cur[1]; y++)
                {   
                    //random weight initialization call
                    newLayerWeights[x][y] = Random.Range(0f,INITIAL_WEIGHTS_UPPER_BOUND);
                }
            }
            newRandomizedWeights.Add(newLayerWeights);
        }
        return newRandomizedWeights;  
    }*/

    // Initialise Particle velocity to zero
    private List<float[][]> InitializeParticleVelocityToZero()
    {
        List<float[][]> magnitudes = CloneOfWeights(cars.carPopulation[0].GetComponent<NeuralNetwork>().weights);
        foreach (float[][] cur in magnitudes)
        {
            for (int x = 0; x < cur.Length; x++)
            {
                for (int y = 0; y < cur[x].Length; y++)
                {   
                    cur[x][y] = 0f;
                }
            }
        }
        return magnitudes;
        /*List<float[][]> magnitudes = new List<float[][]>();
        foreach (int[] cur in Weights_template_dimensions)
        {
            float[][] newLayerMagnitudes = new float[cur[0]][];
            for (int x = 0; x < cur[0]; x++)
            {
                for (int y = 0; y < cur[1]; y++)
                {   
                    newLayerMagnitudes[x][y] = 0;
                }
            }
            magnitudes.Add(newLayerMagnitudes);
        }
        return magnitudes;*/  
    }

    // Initialize Particle personal best weights to the weights currently in the car
    private void InitializeParticlePersonalBests()
    {
        for (int x = 0; x < cars.carPopulation.Count; x++) 
        {
            PersonalBestNN_Weights.Add(GetA_CarsNN_Weights(x));
            PersonalBestNN_Fitness.Add(GetA_CarsNN_Fitness(x));
        } 
    }

    // Switch the cars' NNs back on
    private void EnableAllTheCrashedCarsNNs/*AndResetTimer*/()
    {
        for (int x = 0; x < cars.carPopulation.Count; x++) 
        {
            CarSideEvolutionaryBehaviour cur = cars.carPopulation[x].GetComponent<CarSideEvolutionaryBehaviour>();
            if (!cur.isDriving) 
            {
                cur.isDriving = true;
                cars.carPopulation[x].GetComponent<NeuralNetwork>().WakeUp();
                cars.carPopulation[x].GetComponent<Timer>().ResetTimer();
            }  
        }
    }
    

    // This will move to the CAR object 
    /*// Calculate fitness for a given car
    private float CalculateFitnessTheWeightsAchieved(GameObject car)
    {
        // Get Distance car travelled
        float dist = car.GetComponent<DistanceTracker>().distanceTravelled;

        // Get time elapsed
        float timeElapsed = car.GetComponent<Timer>().timeElapsedInSec/60;

        // some funtion to combine them
        return dist/timeElapsed;
    }*/

    private void UpdateGlobalAndY_HatRingTopologyBestVectors(bool initilizationStep = false) 
    {
        List<List<float[][]>> originWeights = PersonalBestNN_Weights;
        List<float> originFitness = PersonalBestNN_Fitness;
        int indexOfGlobalFittest = GetIndexOfFittest(originFitness);
        if (GlobalBestNN_Fitness < originFitness[indexOfGlobalFittest])
        {
            
            // Update the global best weights
            GlobalBestNN_Fitness = originFitness[indexOfGlobalFittest];
            GlobalBestWeights = CloneOfWeights(originWeights[indexOfGlobalFittest]);
            // Reset Demo car
            BestCarDemo.GetComponent<CarSideEvolutionaryBehaviour>().ResetAndLogCarTermination(true);
            //float prevGlobalBestFitness = GlobalBestNN_Fitness;

            //Print Best Fitness
            Debug.Log("Best fitness:" + GlobalBestNN_Fitness + "; Generation: " + curGeneration + "; First weight: " + GlobalBestWeights[0][0][0]);

            if (logOutputs)
            {
                //Log change in best fitness for graph
                float time = Time.time-sessionStartTime;
                //outputStream.WriteLine(( time + ";" + prevGlobalBestFitness).Replace(',','.'));
                outputStream.WriteLine((time + ";" + GlobalBestNN_Fitness).Replace(',','.'));
            }
        }
        
    }

    private int GetIndexOfFittest(List<float> theFitnessValues, int[] subsetIndices = null) 
    {
        int indexOfFittest = -1;
        if (subsetIndices == null || subsetIndices.Length == 0) 
        {
            //Debug.LogError("Please implement GetIndexOfFittest() for the full population");
            indexOfFittest = 0;
            float fitnessOfCurFittest = theFitnessValues[indexOfFittest];
            for (int x = 1; x < cars.carPopulation.Count; x++)
            {
                float fitnessOfCur = theFitnessValues[x];
                if (fitnessOfCurFittest < fitnessOfCur)
                {
                    indexOfFittest = x;
                    fitnessOfCurFittest = fitnessOfCur;
                }
            }
        }
        else 
        {
            indexOfFittest = subsetIndices[0];
            float fitnessOfCurFittest = theFitnessValues[indexOfFittest];
            for (int x = 1; x < subsetIndices.Length; x++)
            {
                float fitnessOfCur = theFitnessValues[subsetIndices[x]];
                if (fitnessOfCurFittest < fitnessOfCur)
                {
                    indexOfFittest = subsetIndices[x];
                    fitnessOfCurFittest = fitnessOfCur;
                }
            }
        }
        return indexOfFittest;
    }

    private List<float[][]> GetA_CarsNN_Weights(int carIndex) 
    {
        if (carIndex < cars.carPopulation.Count)
        {
            return CloneOfWeights(cars.carPopulation[carIndex].GetComponent<NeuralNetwork>().weights);
        }
        else
        {
            return null;
        }
    }

    private float GetA_CarsNN_Fitness(int carIndex) 
    {
        if (carIndex < cars.carPopulation.Count)
        {
            return cars.carPopulation[carIndex].GetComponent<CarSideEvolutionaryBehaviour>().fitness;
        }
        else
        {
            return 0;
        }
    }
}