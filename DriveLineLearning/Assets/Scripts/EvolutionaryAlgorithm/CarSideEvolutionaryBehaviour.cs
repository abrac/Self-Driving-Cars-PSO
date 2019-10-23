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
    public bool isDriving;

    // Start is called before the first frame update
    void Start()
    {
        isDriving = true;
        distanceTravelled = 0;
        lastPosition = transform.position;

        StationaryPos = transform.position;
        startTime = Time.time;

    }

    // Update is called once per frame
    void FixedUpdate()
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
                    ResetAndLogCarTermination();
                }
            }
            StationaryPos = transform.position;
            startTime = Time.time;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        ResetAndLogCarTermination();
    }

    private void ResetAndLogCarTermination()
    {
        GameObject car = this.gameObject;
        PopulationManager popMan = evolutionManager.GetComponent<PopulationManager>();
        car.GetComponent<NeuralNetwork>().Sleep();
        if (distanceTravelled > 1000)
        {
            popMan.CallInAsTravelledFar();
        }
        //int gen = popMan.curGeneration;
        float time = evolutionManager.GetComponent<Timer>().timeElapsedInSec; // Using a master timer
        //float time = car.GetComponent<Timer>().timeElapsedInSec; // used if each car has a timer
        if (popMan.useTimeInFitness)
        {
            fitness = Mathf.Pow(distanceTravelled,2)/time;
        }
        else
        {
            fitness = Mathf.Pow(distanceTravelled,2);
        }
        distanceTravelled = 0;
        popMan.PositionCarAtStartLine(car);
        isDriving = false;
    }
}
