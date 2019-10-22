using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PopulationManager : MonoBehaviour
{
    // Parameters
    public int POPULATION_SIZE = 10;
    public int INITIAL_WEIGHTS_UPPER_BOUND = 1;
    public int MAX_GENERATIONS = 1000;
    static int BEST_INDIVIDUAL_STREAK = 70;

    // The car population script Component
    public CarPopulation cars;

    // Individuals of the population's fitness
    private List<float> CurrentNN_Fitness = new List<float>();

    // Stored velocity of each individual particle 
    private List<List<float[][]>> ParticleVelocityVectors = new List<List<float[][]>>();

    // Stored Personal Best of each individual 
    private List<List<float[][]>> PersonalBestNN_Weights = new List<List<float[][]>>();
    private List<float> PersonalBestNN_Fitness = new List<float>();

    // Stored y-Hat of each individual (using Ring topology)
    private List<List<float[][]>> Y_HatNN_Weights = new List<List<float[][]>>();
    private List<float> Y_HatNN_Fitness = new List<float>();

    // Stored Global Best Weights and fitness achieved
    private List<float[][]> GlobalBestWeights = new List<float[][]>();
    private float GlobalBestNN_Fitness;

    // Links to used Game Objects
    

    // Weights (vector dimensions) based on car's NN structure in the form: List index = layer; int[2] with { totalfromN,s, totalToN,s };
    //private List<int[]> Weights_template_dimensions; // IGNORE->: + one array of size 1 added (to Store the fitness)

    // PSO parameters
    public float cognitiveConst = 0.7f;
    public float socialConst = 1.0f;
    public float w = 0.6f;
    public float socialRandom;
    public float cognitiveRandom;

    // Other variables
    private int curGeneration;
    private int leadCounter;
    [SerializeField]
    private int numberCarsDriving;


    // Create genes(weights) property and instantiate and position cars
    void Start()
    {
        // Initialize some variables
        curGeneration = 0;
        leadCounter = 0;
        numberCarsDriving = POPULATION_SIZE;

        

        // Get "chromosome structure" from NeuralNetwork structure
        /*List<float[][]> Weights_Template = NeuralNetworkControlledCar.GetComponent<NeuralNetwork>().weights;
        foreach(float[][] cur in Weights_Template)
        {
            Weights_template_dimensions.Add(new int[] {cur.Length, cur[0].Length});
        }
        //Weights_template_dimensions.Add(new int[] {0}); // Stores fitness of this configuration

        // Create genes array representing weights of NN, and initialize with random NN weights.
        for (int x = 0; x < POPULATION_SIZE; x++)
        {
            GameObject newCar = GameObject.Instantiate(NeuralNetworkControlledCar);
            PositionCarAtStartLine(newCar);
            NeuralNetwork carsNN = newCar.GetComponent<NeuralNetwork>();
            carsNN.weights = GenerateRandomlyInitializedWeights();
            // Add the car to the population
            CarPopulation.Add(newCar);
        }*/

        //Initialize Velocity Vectors to Zero
        ParticleVelocityVectors = new List<List<float[][]>>();
        for (int x = 0; x < POPULATION_SIZE; x ++) {
            ParticleVelocityVectors.Add(InitializeParticleVelocityToZero());
        }

        // Can't evaluate fitness until raced at least once...
        // So can build that in on Update() 
    }

    void Update()
    {
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
                curGeneration++;
                numberCarsDriving = POPULATION_SIZE;
                EnableAllTheCarsNNs();
                InitializeParticlePersonalBests(); // to the weights currently in the car and the resultant fitness
                
                // Initialize "Global" bests (y-hat) All-best and Ring topology (Based on Personal bests initially - then subsequently on Y-Hats)
                GlobalBestNN_Fitness = 0;
                GlobalBestWeights = new List<float[][]>();
                Y_HatNN_Fitness = new List<float>();
                Y_HatNN_Weights = new List<List<float[][]>>();
                UpdateGlobalAndY_HatRingTopologyBestVectors(true);
            
                // reset test cycle - new generation
                foreach (GameObject car in cars.carPopulation)
                {
                    PositionCarAtStartLine(car);
                }
                curGeneration++;
                numberCarsDriving = POPULATION_SIZE;
                // Set cars to be driving again
                // ...here... Generation 1.. GO!
                EnableAllTheCarsNNs/*AndResetTimer*/();
            }
        }
        // Start of evolutionary cycles
        else if (curGeneration <= MAX_GENERATIONS && leadCounter < BEST_INDIVIDUAL_STREAK)
        {
            // If no cars are still driving
            if (numberCarsDriving == 0)
            {
                // have fitness calc in each car as it crashes. have a method that the car calls to decrease NumberCarsDriving by 1.
                // Fetch each car's latest NN fitness

                for (int curIndividual = 0; curIndividual < POPULATION_SIZE; curIndividual++) 
                {
                    // Update personal best
                    if (GetA_CarsNN_Fitness(curIndividual) > PersonalBestNN_Fitness[curIndividual]) 
                    {
                        PersonalBestNN_Fitness[curIndividual] = GetA_CarsNN_Fitness(curIndividual);
                        PersonalBestNN_Weights[curIndividual] = CloneOfWeights(GetA_CarsNN_Weights(curIndividual));
                    }
                    // Update Ring Topology Best
                    if (PersonalBestNN_Fitness[curIndividual] > Y_HatNN_Fitness[curIndividual]) 
                    {
                        Y_HatNN_Fitness[curIndividual] = PersonalBestNN_Fitness[curIndividual];
                        Y_HatNN_Weights[curIndividual] = CloneOfWeights(PersonalBestNN_Weights[curIndividual]);
                    }
                    /*// Global Best // NOTE: Performed later in UpdateGlobalAndY_HatRingTopologyBestVectors(false);
                    if (personalBests.get(curIndividual)[4] > globalBest[4]) 
                    {
                        globalBest = cloneArray(personalBests.get(curIndividual));
                        leadCounter = 0;
                    }*/
                }

                //Print Best Fitness
                Debug.Log(GlobalBestNN_Fitness);

                // Recalculate latest ring Y_Hats 
                UpdateGlobalAndY_HatRingTopologyBestVectors(false);

                // Calculate new velocity for each individual
                for (int bob = 0; bob < cars.carPopulation.Count; bob++) 
                {
                    List<float[][]> bobIndividual = GetA_CarsNN_Weights(bob);
                    List<float[][]> bobVelocity = ParticleVelocityVectors[bob];
                    List<float[][]> bobPersonalBest = PersonalBestNN_Weights[bob];
                    List<float[][]> bobYHat = Y_HatNN_Weights[bob];
                    // for each dimension of the Vectors
                    for (int layer = 0; layer < bobVelocity.Count; layer++) 
                    {
                        for (int to = 0; to < bobVelocity[layer].Length; to++)
                        {
                            for (int from = 0; from < bobVelocity[layer][to].Length; from++)
                            {
                                socialRandom = Random.Range(0,1);
                                cognitiveRandom = Random.Range(0,1);
                                bobVelocity[layer][to][from] = w*bobVelocity[layer][to][from] + cognitiveConst*cognitiveRandom*(bobPersonalBest[layer][to][from] - bobIndividual[layer][to][from]) + socialConst*socialRandom*(bobYHat[layer][to][from] - bobIndividual[layer][to][from]);                                                            
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

                // reset test cycle - new generation
                foreach (GameObject car in cars.carPopulation)
                {
                    PositionCarAtStartLine(car);
                }
                curGeneration++;
                leadCounter++;
                numberCarsDriving = POPULATION_SIZE;
                // Set cars to be driving again
                // ...here... Generation 1.. GO!
                EnableAllTheCarsNNs/*AndResetTimer*/();
            }
            Debug.Log("PSO SOLUTION: Final fitness: " + GlobalBestNN_Fitness);
        }
        else 
        {
            //Solution is reached..
            //Save BEST Global Weights to a new text file: or put them in one car and let it race
        }
        
        // Steps:
        //  1. Check for collision triggers.
        //  2. If a collision happened:
        //      2.1. Disable Car. (Geoff: Car or Car's NN? suppose doesnt matter)
        //      2.2. Evaluate fitness.
        //      2.3. Store fitness and weights in personalBestWeights (if applicable)
        //      2.4. Store fitness and weights in globalBestWeights (if applicable)
        // 3. If all cars have collided (when collisionCounter = popSize):
        //      3.1. Update all the particle "positions" (i.e. weights) and particle "velocities"
        //      3.2. start a new generation by moving all cars to the startPosition
        //      3.3. Enable Car.
        // 4. If stopping conditions are met: stop, and save weights and other data to csv file.
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
    
    private List<float[][]> CloneOfWeights(List<float[][]> weightsToClone)
    {
        List<float[][]> newCopy = new List<float[][]>();
        foreach (float[][] cur in weightsToClone)
        {
            float[][] newClone = new float[cur.Length][];
            for (int x = 0, length = cur.Length; x < length; x++)
            {
                for (int y = 0, innerLength = cur[x].Length; y < innerLength; y++)
                {   
                    newClone[x][y] = cur[x][y];
                }
            }
            newCopy.Add(newClone);
        }
        return newCopy;
    }

    // A car calls this to flag it as crashed
    public void CallInAsCrashed() 
    {
        numberCarsDriving = numberCarsDriving - 1;
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
    private void EnableAllTheCarsNNs/*AndResetTimer*/()
    {
        for (int x = 0; x < cars.carPopulation.Count; x++) 
        {
            cars.carPopulation[x].GetComponent<NeuralNetwork>().WakeUp();
        }
        //this.gameObject.GetComponent<Timer>().ResetTimer();
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
        List<List<float[][]>> originWeights = Y_HatNN_Weights;
        List<float> originFitness = Y_HatNN_Fitness;
        // If before first generation, get best of neighbour's personal bests instead
        if (initilizationStep) 
        {
            originWeights = PersonalBestNN_Weights;
            originFitness = PersonalBestNN_Fitness;
        }

        int indexOfGlobalFittest = 0;
        for (int index = 0; index < POPULATION_SIZE; index ++) 
        {
            //get the three linked individuals
            int[] indices = new int[] { ((((index - 1) % POPULATION_SIZE) + POPULATION_SIZE) % POPULATION_SIZE), index, ((((index + 1) % POPULATION_SIZE) + POPULATION_SIZE) % POPULATION_SIZE) };
            
            // get the fittest of the three
            int indexOfRingY_HatFittest = GetIndexOfFittest(originFitness, indices);

            // check if global fitness has been beaten, if so, replace with new best
            if (originFitness[indexOfGlobalFittest] < originFitness[indexOfRingY_HatFittest])
            {
                indexOfGlobalFittest = indexOfRingY_HatFittest;
                // Reset LeadCounter
                leadCounter = 0;
            }

            // Update the Y_Hat values
            if (!initilizationStep) {
                Y_HatNN_Fitness[indexOfRingY_HatFittest] = originFitness[indexOfRingY_HatFittest];
                Y_HatNN_Weights[indexOfRingY_HatFittest] = CloneOfWeights(originWeights[indexOfRingY_HatFittest]);
            }
            else
            {
                Y_HatNN_Fitness.Add(originFitness[indexOfRingY_HatFittest]);
                Y_HatNN_Weights.Add(CloneOfWeights(originWeights[indexOfRingY_HatFittest]));
            }
        }
        // Update the global best weights
        GlobalBestNN_Fitness = originFitness[indexOfGlobalFittest];
        GlobalBestWeights = CloneOfWeights(originWeights[indexOfGlobalFittest]);
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

/* Geoff's JAVA  DE and PSO Code 
// COMMON PARAM
    static List<double[]> population = new ArrayList<>(); //List<double[5]> == 4 alloy amounts & fitness score
    static final int POP_SIZE = 100;
    static final int RANDOM_UP_LIMIT = 100;
    static final double MAX_GENERATIONS = 1500;
    static final int BEST_INDIVIDUAL_STREAK = 70;

    // THE DIFFERENTIAL EVOLUTION ALGORITHM
    private static void PerformDifferentialEvolution() {
        // DE PARAMS
        final double SCALE_FACTOR = 0.7;
        final double CROSSOVER_RATE = 0.3;

        final String DE_TYPE = "DE/rand/1"; // "DE/best/1" "DE/rand-to-best/1 (α = 0.4)" "DE/rand/1"

        List<double[]> MutantVectors = new ArrayList<>();
        List<double[]> TrialVectors = new ArrayList<>();

        double[] bestAlloyCombin = new double[] { 0,0,0,0,0 };

        //Randomly Initialise population
        InitializePopulation();

        // Calc inital fitness
        CalcFitnessValuesForPopulation(population);

        // Start evolution
        int gen = 1;
        int leadCounter = 0;
        System.out.println("Pop size: " + POP_SIZE + "; Crossover rate: " + CROSSOVER_RATE + "; Scale factor: " + SCALE_FACTOR + "; Scheme: " + DE_TYPE);
        while (gen <= MAX_GENERATIONS && leadCounter < BEST_INDIVIDUAL_STREAK) {
            leadCounter++;

            // Get fittest
            List<double[]> sorted = SortOnProfitDesc(population);
            bestAlloyCombin = sorted.get(0);
            System.out.println(bestAlloyCombin[4]);
            //System.out.println("Best Profit after Generation " + gen + ":" + bestAlloyCombin[4]);

            MutantVectors.clear();
            TrialVectors.clear();
            for (int individual = 0; individual < population.size(); individual ++) {
                // Select three other random individuals
                double[][] xr = new double[3][];
                List<Integer> notAllowed = new ArrayList<>();
                notAllowed.add(individual);
                Random random = new Random();
                for (int i = 0; i < 3; i++) {
                    int individualIndex = 0;
                    do {
                        individualIndex = random.nextInt(population.size());
                    } while (notAllowed.contains(individualIndex));
                    notAllowed.add(individualIndex);
                    xr[i] = cloneArray(population.get(individualIndex));
                }

                // Create a new mutant vector
                double[] mutant = new double[5];

                switch (DE_TYPE) {
                    case "DE/rand/1" :
                        for (int x = 0; x < mutant.length; x++) {
                            mutant[x] = Math.abs(xr[0][x] + SCALE_FACTOR * (xr[1][x] - xr[2][x]));
                            //if (mutant[x] < 0) mutant[x] = 0;
                        }
                        break;

                    case "DE/best/1" :
                        xr[0] = cloneArray(bestAlloyCombin);
                        for (int x = 0; x < mutant.length; x++) {
                            mutant[x] = Math.abs(xr[0][x] + SCALE_FACTOR * (xr[1][x] - xr[2][x]));
                        }
                        break;

                    case "DE/rand-to-best/1 (α = 0.4)" :
                        double alpha = 0.4;
                        for (int x = 0; x < mutant.length; x++) {
                            mutant[x] = Math.abs(alpha*bestAlloyCombin[x] + (1-alpha)*xr[0][x] + SCALE_FACTOR * (xr[1][x] - xr[2][x]));
                        }
                        break;
                }

                // Add to global list
                MutantVectors.add(mutant);

                // Create a trial vector
                double[] trial = new double[5];
                for (int x = 0; x < trial.length - 1; x++) {
                    if (RandomlySelect(CROSSOVER_RATE)) {
                        trial[x] = mutant[x];
                    }
                    else
                    {
                        trial[x] = population.get(individual)[x];
                    }
                }

                // Add to global list
                TrialVectors.add(trial);
            }
            // Evaluate Fitness of new Trial Vectors
            CalcFitnessValuesForPopulation(TrialVectors);

            double[] curGenBest = new double[5];
            for (int individual = 0; individual < population.size(); individual ++) {
                // Compare target individual to corresponding in trial population, if trial is better, overwrite target
                if (TrialVectors.get(individual)[4] > population.get(individual)[4]) {
                    population.remove(individual);
                    population.add(individual,cloneArray(TrialVectors.get(individual)));
                }
                if (curGenBest[4] < population.get(individual)[4] || curGenBest[4] == 0) {
                    curGenBest = cloneArray(population.get(individual));
                }

            }
            if (bestAlloyCombin[4] < curGenBest[4]) {
                bestAlloyCombin = cloneArray(curGenBest);
                leadCounter = 0;
            }
            //System.out.println(allBest[8]);
            gen++;
        }
        System.out.println("DE SOLUTION: A: " + bestAlloyCombin[0] + ", U: " + bestAlloyCombin[1] + ", D: " + bestAlloyCombin[2] + ", P: " + bestAlloyCombin[3] + ", Profit: " + bestAlloyCombin[4]);

    }

    // THE PARTICLE SWARM OPTIMIZATION ALGORITHM
    private static void PerformParticleSwarmOptimization() {
        // Generate Population
        InitializePopulation();

        // Determine initial fitness
        CalcFitnessValuesForPopulation(population);

        //Initialize Velocity Vectors
        List<double[]> velocityVectors = new ArrayList<>();
        for (int x = 0; x < POP_SIZE; x ++) {
            double[] newVelocity = new double[] {0,0,0,0};
            velocityVectors.add(newVelocity);
        }

        // Initialize "Global" bests (y-hat) All-best and Ring topology
        double[] globalBest = GetBestGlobalVector();
        List<double[]> yHats = new ArrayList<>();
        CalculateAndSetBestVectorRingTopology(yHats, true);


        // Initialize "personal" bests (y)
        List<double[]> personalBests = new ArrayList<>();
        for (int x = 0; x < POP_SIZE; x ++) {
            personalBests.add(cloneArray(population.get(x)));
        }

        // Start evolution
        int gen = 1;
        int leadCounter = 0;
        while (gen <= MAX_GENERATIONS && leadCounter < BEST_INDIVIDUAL_STREAK) {
            for (int bob = 0; bob < POP_SIZE; bob++) {
                // Update personal best
                if (population.get(bob)[4] > personalBests.get(bob)[4]) {
                    personalBests.remove(bob);
                    personalBests.add(bob,cloneArray(population.get(bob)));
                }
                // Ring topology Best
                if (personalBests.get(bob)[4] > yHats.get(bob)[4]) {
                    yHats.remove(bob);
                    yHats.add(bob,cloneArray(personalBests.get(bob)));
                }
                // Global Best
                if (personalBests.get(bob)[4] > globalBest[4]) {
                    globalBest = cloneArray(personalBests.get(bob));
                    leadCounter = 0;
                }
            }

            //Print Best SSE
            System.out.println(globalBest[4]);


            // Broadcast new ring yHats again
            CalculateAndSetBestVectorRingTopology(yHats,false);

            double cognitiveConst = 0.7;
            double socialConst = 1.0;
            double w = 0.6;
            Random randomness = new Random();

            // Calculate new velocity
            for (int bob = 0; bob < population.size(); bob++) {
                double[] bobIndividual = population.get(bob);
                double[] bobVelocity = velocityVectors.get(bob);
                double[] bobPersonalBest = personalBests.get(bob);
                double[] bobYHat = yHats.get(bob);
                // for each dimension of the Vectors
                for (int dimension = 0; dimension < bobVelocity.length; dimension++) {
                    double socialRandom = randomness.nextDouble();
                    double cognitiveRandom = randomness.nextDouble();
                    bobVelocity[dimension] = w*bobVelocity[dimension] + cognitiveConst*cognitiveRandom*(bobPersonalBest[dimension] - bobIndividual[dimension]) + socialConst*socialRandom*(bobYHat[dimension] - bobIndividual[dimension]);
                }

                // Apply new velocity to Particle
                for (int dimension = 0; dimension < bobVelocity.length; dimension++) {
                    bobIndividual[dimension] = Math.abs(bobIndividual[dimension] + bobVelocity[dimension]);
                }

                List<double[]> toCheckFitness = new ArrayList<>();
                toCheckFitness.add(bobIndividual);
                CalcFitnessValuesForPopulation(toCheckFitness);
            }
            leadCounter++;
            gen++;
        }
        System.out.println("PSO SOLUTION: A: " + globalBest[0] + ", U: " + globalBest[1] + ", D: " + globalBest[2] + ", P: " + globalBest[3] + ", Profit: " + globalBest[4]);
    }

    private static double[] GetBestGlobalVector() {
        return cloneArray(SortOnProfitDesc(population).get(0));
    }

    private static void CalculateAndSetBestVectorRingTopology(List<double[]> yHatVectors, boolean initilizationStep) {
        List<double[]> origin = yHatVectors;
        if (initilizationStep) {
            origin = population;
        }
        for (int index = 0; index < POP_SIZE; index ++) {
            //get the three linked individuals
            int[] indices = new int[] { ((((index-1)%POP_SIZE) + POP_SIZE) %POP_SIZE), index, ((((index+1)%POP_SIZE) + POP_SIZE) %POP_SIZE) };
            List<double[]> threeVectors = new ArrayList<>();
            for (int x = 0; x < 3; x++) {
                threeVectors.add(cloneArray(origin.get(indices[x])));
            }
            threeVectors = SortOnProfitDesc(threeVectors);
            if (!initilizationStep) {
                yHatVectors.remove(index);
                yHatVectors.add(index,cloneArray(threeVectors.get(0)));
            }
            else
            {
                yHatVectors.add(cloneArray(threeVectors.get(0)));
            }
        }
    }


    // Population initialization


    // Fitness function
    

    // cloneArray(double[] array)

    //Sort on fitness descending

    //static private List<double[]> SortOnFitnessDesc(List<double[]> toSort) {
        List<double[]> sorted = new ArrayList<>(toSort);
        sorted.sort((o1,o2) -> ((Double)o2[4]).compareTo(o1[4]));
        //for (double[] d : sorted) {
        //    System.out.println(d[0] + "," + d[1] + "," + d[2] + "," + d[3] + "," + d[4]);
        //}
        return sorted;
    }

    static private boolean RandomlySelect(double probability) {
        Random random = new Random();
        if (random.nextDouble() <= probability)
            return true;
        else
            return false;

    }
}
*/
