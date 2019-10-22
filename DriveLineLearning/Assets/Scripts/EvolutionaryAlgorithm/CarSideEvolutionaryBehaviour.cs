using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSideEvolutionaryBehaviour : MonoBehaviour
{
    public GameObject evolutionManager;
    public float distanceTravelled = 0;
    Vector3 lastPosition;
    public float fitness;

    // Start is called before the first frame update
    void Start()
    {
        distanceTravelled = 0;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += Vector3.Distance(transform.position,lastPosition);
        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision other) {
        GameObject car = this.gameObject;
        car.GetComponent<NeuralNetwork>().Sleep();
        evolutionManager.GetComponent<PopulationManager>().CallInAsCrashed();
        fitness = distanceTravelled/evolutionManager.GetComponent<Timer>().timeElapsedInSec;
        distanceTravelled = 0;
        evolutionManager.GetComponent<PopulationManager>().PositionCarAtStartLine(car);
    }
}
