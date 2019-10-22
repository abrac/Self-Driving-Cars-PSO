using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPopulation : MonoBehaviour
{
    // Car objects - Population
    public List<GameObject> carPopulation = new List<GameObject>();

    // Links to used Game Objects
    public GameObject NeuralNetworkControlledCar;
    public GameObject StartingBlocks;
    public Vector3 CarStartingPosition;
    public Quaternion CarStartingRotation;

    // Start is called before the first frame update
    void Awake()
    {
        // Find and store reference to Starting Block where to initialize the cars
        Transform startPosition = StartingBlocks.transform.Find("Start Position Solo");
        CarStartingRotation = startPosition.rotation;
        CarStartingPosition = startPosition.position;

        // Instantiate the car population
        int popSize = this.gameObject.GetComponent<PopulationManager>().POPULATION_SIZE;
        for (int x = 0; x < popSize; x++) 
        {
            carPopulation.Add(Instantiate(NeuralNetworkControlledCar, CarStartingPosition, CarStartingRotation) as GameObject);
        }

        /*
        float[] good00 = new float[] {0.1971179f, -0.7873962f, -0.1772171f, 0.3655338f, 0.8897562f};
        float[] good01 = new float[] {-0.9953563f, 0.4975347f, -0.7351885f, -0.3298408f, 0.7022059f};
        float[] good02 = new float[] {0.9550359f, 0.1878072f, 0.2774045f, -0.932277f, 0.002232909f};
        float[] good03 = new float[] {0.4231319f, -0.1798769f, -0.1396383f, -0.6100574f, -0.8416488f};
        float[] good04 = new float[] {0.8465564f, 0.497458f, 0.1564225f, -0.9475875f, -0.9111381f};
        float[] good05 = new float[] {-0.3780454f, -0.2332152f, -0.128101f, -0.1601473f, 0.09461606f};
        float[] good06 = new float[] {-0.1294144f, 0.3694024f, -0.1567475f, 0.7233827f, 0.4618244f};
        float[] good10 = new float[] {-0.2573169f, 0.6115253f};
        float[] good11 = new float[] {-0.4219903f, 0.390186f};
        float[] good12 = new float[] {0.8425839f, 0.2486659f};
        float[] good13 = new float[] {-0.2009448f, -0.2468568f};
        float[] good14 = new float[] {-0.6576905f, -0.36332f};

        float[][] good0 = new float[][] {good00, good01, good02, good03, good04, good05, good06};
        float[][] good1 = new float[][] {good10, good11, good12, good13, good14};

        List<float[][]> good = new List<float[][]>();
        good.Add(good0);
        good.Add(good1);

        carPopulation[0].GetComponent<NeuralNetwork>().weights = good;
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
