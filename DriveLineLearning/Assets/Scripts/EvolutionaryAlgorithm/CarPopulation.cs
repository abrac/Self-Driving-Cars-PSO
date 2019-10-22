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
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
