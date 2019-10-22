using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSideEvolutionaryBehaviour : MonoBehaviour
{
    public GameObject evolutionManager;
    public float distanceTravelled = 0;
    Vector3 lastPosition;
    Vector3 StationaryPos;
    public float fitness;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        distanceTravelled = 0;
        lastPosition = transform.position;

        StationaryPos = transform.position;
        startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += Vector3.Distance(transform.position,lastPosition);
        lastPosition = transform.position;
        //Stationary?
        float t = Time.time - startTime;
        if (t > 5)
        {
            if (Vector3.Distance(StationaryPos, transform.position) < 5)
            {
                GameObject car = this.gameObject;
                if (car.GetComponent<NeuralNetwork>().sleep == false)
                {
                    IsMoving();
                }
            }
            StationaryPos = transform.position;
            startTime = Time.time;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        IsMoving();
    }

    private void IsMoving()
    {
        GameObject car = this.gameObject;
        car.GetComponent<NeuralNetwork>().Sleep();
        evolutionManager.GetComponent<PopulationManager>().CallInAsCrashed();
        int gen = evolutionManager.GetComponent<PopulationManager>().curGeneration;
        float time = evolutionManager.GetComponent<Timer>().timeElapsedInSec;
        fitness = Mathf.Pow(distanceTravelled,2)/*/time*/;
        distanceTravelled = 0;
        evolutionManager.GetComponent<PopulationManager>().PositionCarAtStartLine(car);
    }
}
