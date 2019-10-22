using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    // Parameters
    public int POPULATION_SIZE = 50;
    public int INITIAL_WEIGHTS_UPPER_BOUND = 100;
    public int MAX_GENERATIONS = 1000;
    private float mutationProbab=0.2f;
    private float maxVariation=1f;
    private float maxMutation=1f;

    // Car objects - Population
    public List<GameObject> CarPopulation = new List<GameObject>();
    
    // Individuals of the population's fitness
    //private List<float> Fitness = new List<float>();

    // Stored velocity of each individual 
    List<float[][]> velocityVectors = new List<float[][]>();

    // Stored Personal Best of each individual 
    List<List<float[]>> personalBestWeights = new List<List<float[]>>();

    // Stored Global Best Weights and fitness achieved
    List<float[]> GlobalBestWeights = new List<float[]>();
    //float GlobalBestFitness;

    // Links to used Game Objects
    public GameObject NeuralNetworkControlledCar;
    public GameObject StartingBlocks;
    private Vector3 CarStartingPosition;
    private Quaternion CarStartingRotation;

    // Weights (vector dimensions) based on car's NN structure in the form: List index = layer; int[2] with { totalfromN,s, totalToN,s };
    private List<int[]> Weights_template_dimensions; // + one array of size 1 added (to Store the fitness)

    // 
    private Random random;
    private int curGeneration;


    // Create genes(weights) property and instantiate and position cars
    void Start()
    {
        random = new Random();
        // Find and store reference to Starting Block where to initialize the cars
        Transform startPosition = StartingBlocks.transform.Find("Start Position Solo");
        CarStartingRotation = startPosition.rotation;
        CarStartingPosition = startPosition.position;

        // Get "chromosome structure" from NeuralNetwork structure
        List<float[][]> Weights_Template = NeuralNetworkControlledCar.GetComponent<NeuralNetwork>().weights;
        foreach(float[][] cur in Weights_Template)
        {
            Weights_template_dimensions.Add(new int[] {cur.Length, cur[0].Length});
        }
        Weights_template_dimensions.Add(new int[] {0}); // Stores fitness of this configuration



        //Create genes array representing weights of NN, and initialize with random NN wieghts.
        for (int x = 0; x < POPULATION_SIZE; x++)
        {
            GameObject newCar = GameObject.Instantiate(NeuralNetworkControlledCar);
            PositionCarAtStartLine(newCar);
            NN carsNN = newCar.GetComponent<NN>();
            carsNN.weights = GenerateRandomlyInitializedWeights();
            // Add the car to the population
            CarPopulation.Add(newCar);
        }

        // Can't evaluate fitness until raced at least once...
        // So can build that in on Update() 
    }

    void Update()
    {
        // Steps:
        //  1. Check for collision triggers.
        //  2. If a collision happened:
        //      2.1. Disable Car.
        //      2.2. Evaluate fitness.
        //      2.3. Store fitness and weights in personalBestWeights (if applicable)
        //      2.4. Store finess and weights in globalBestWeights (if applicable)
        // 3. If all cars have collided (when collisionCounter = popSize):
        //      3.1. Update all the particle "positions" (i.e. weights) and particle "velocities"
        //      3.2. start a new generation by moving all cars to the startPosition
        //      3.3. Enable Car.
        // 4. If stopping conditions are met: stop, and save weights and other data to csv file.
         
      GameObject[] CarPopulation=new GameObject[50];
       POPULATION_SIZE=50;
        for(int i=0;i<POPULATION_SIZE;++i)
           
     {
        CarPopulation[i].NN.CalculateFitnessTheWeightsAchieved();
            CarPopulation.NN.CrossoverPopulation[i+1]

        

   }

    private void PositionCarAtStartLine(GameObject car)
    {
        car.transform.position = CarStartingPosition;
        car.transform.rotation = CarStartingRotation;
        // Set car's velocity to zero
        Rigidbody carsRigidbody = car.GetComponent<Rigidbody>();
        carsRigidbody.velocity = Vector3.zero;
        carsRigidbody.angularVelocity = Vector3.zero;
    }

    public  void  Mutate()// mutation method
    {
         List<float[][]>newGenes= new List<float[][]>();
        for (int i=0; i<POPULATION_SIZE;++i)
        {
            float[][]weightslayer=CarPoulation[i];
            for (int j=0;j<weightslayer[j].Length;j++)
            {
                for(int k=0;k<weightslayer[j].Length;k++)
                {
                    float rand=Random.Range(0f,1f);
                    if(rand<mutationProbab)
                    {
                        weightslayer[j][k] = Random.Range(-maxVariation,maxVariation);


                    }

                }
            }
                       newGenes.Add(weightslayer);
        }
                    return new carPopulation(newGenes);
    }
    public Population crossover(Population otherParent)// crossover method
    {
        List<float[][]> child =new List<float[][]>();
        for (int i=0;i<POPULATION_SIZE;i++)
        {
            float[][]otherParentLayer=otherParent.getGenes()[i]; 
            float[][]parentLayer=genes[i];
            for (int k=0;k<parentLayer.Length; k++)
            {
                float rand=Random.Range(0f,1f);
                if(rand<0.5f)
                {
                   parentLayer[i][k]=otherParentLayer[][];
                }
                else
                {
                    parentLayer[j][k]=parentLayer[j][k];
                }
                child.Add(parentLayer);
            }
        
        return new Population (child);
        }
        
    }
}
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
                for (int y = 0, innerLength = cur[x].Length; y < innerLength; y++)
                {   
                    newClone[x][y] = cur[x][y];
                }
            }
            newCopy.Add(newClone);
        }
        return newCopy;
    }

    // Generate Randomly Initialised Weights
    private List<float[][]> GenerateRandomlyInitializedWeights()
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

        
    }

    // Calculate fitness for a given car
    private float CalculateFitnessTheWeightsAchieved(GameObject car)
    {
        // Get Distance car travelled
        float dist = car.GetComponent<DistanceTracker>().distanceTravelled;

        // Get time elapsed
        float timeElapsed = car.GetComponent<Timer>().timeElapsedInSec/60;

        // some funtion to combine them
        return dist/timeElapsed;
        {
           


         }
    }
}
          
/* Geoff's JAVA PSO Code 
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
