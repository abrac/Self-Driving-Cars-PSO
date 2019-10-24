using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSideEvolutionaryBehaviour : MonoBehaviour
{
    public GameObject evolutionManager;
    public float distanceTravelled = 0;
    Vector3 lastPosition;
    Vector3 StationaryPos;
    Vector3 StartPos;
    public float fitness;
    private float startTime;
    public bool isDriving;

    public bool isDemo = false;

    static int NumberCarsFinished = 0;

    private float finishingBonus;



    // Start is called before the first frame update
    void Start()
    {
        isDriving = true;
        distanceTravelled = 0;
        lastPosition = transform.position;

        StationaryPos = transform.position;
        StartPos = this.gameObject.transform.position;;
        startTime = Time.time;
        finishingBonus = 1;

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
                if (car.GetComponent<NeuralNetwork>().sleep == false /*&& !isDemo*/)
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
        if (isDriving == true) {
            /*if (isDemo && NumberCarsFinished > 30) 
            {
                evolutionManager.GetComponent<PopulationManager>().GlobalBestNN_Fitness -= 10*NumberCarsFinished;
            }*/
            ResetAndLogCarTermination();
        }
    }

    public void ResetAndLogCarTermination()
    {
        if (!isDemo) 
        {
            GameObject car = this.gameObject;
            PopulationManager popMan = evolutionManager.GetComponent<PopulationManager>();
            car.GetComponent<NeuralNetwork>().Sleep();
            CalculateTheIndividualsFitness();
            popMan.PositionCarAtStartLine(car);
            lastPosition = transform.position;
            isDriving = false;
        }
        else 
        {
            GameObject car = this.gameObject;
            PopulationManager popMan = evolutionManager.GetComponent<PopulationManager>();
            // Stop
            popMan.PositionCarAtStartLine(car);
            lastPosition = transform.position;
            distanceTravelled = 0;
            finishingBonus = 1;
            car.GetComponent<NeuralNetwork>().Sleep();
            isDriving = false;
            
            popMan.ResetDemoCar();
        }        
    }

    private float CalculateTheIndividualsFitness() 
    {
        // Get the Time elapsed
        //float time = evolutionManager.GetComponent<Timer>().timeElapsedInSec; // Using a master timer
        float time = this.gameObject.GetComponent<Timer>().timeElapsedInSec; // used if each car has a timer


        // Base Fitness on time or not
        /*if (NumberCarsFinished > 5) //Then use time in fitness
        {*/
            //fitness = Mathf.Pow(distanceTravelled,2) + Mathf.Pow(distanceTravelled/time*0.0001f,distanceTravelled/10);
        if (distanceTravelled != 0)
            fitness = finishingBonus * Mathf.Pow(distanceTravelled,2)/time; //* distanceTravelled/time;
        /*}
        else // Don't use Time
        {
            //fitness = Mathf.Pow(distanceTravelled,2) + Mathf.Pow(distanceTravelled/time*0.0001f,distanceTravelled/10);
            fitness = finishingBonus + Mathf.Pow(distanceTravelled,2) + distanceTravelled/time;
        }*/
        // If too close to the starting Line make fitness negative
        float distCovered = Vector3.Distance(StartPos, this.gameObject.transform.position);
        if(finishingBonus == 1 && distCovered < 10 && distanceTravelled != 0)
        {
            fitness = - distCovered - 10;
        }
        distanceTravelled = 0;
        finishingBonus = 1;

        return fitness; // fitness is global -- so doesn't necessarily have to return
    }

    private void OnTriggerEnter(Collider other) {
        if (isDriving == true)
        {
            /*if (isDemo && NumberCarsFinished > 30) 
            {
                evolutionManager.GetComponent<PopulationManager>().GlobalBestNN_Fitness += 10*NumberCarsFinished;
            }*/
            NumberCarsFinished++;
            float time = this.gameObject.GetComponent<Timer>().timeElapsedInSec;
            finishingBonus = 1.5f;//1000 * Mathf.Pow(distanceTravelled/time,1);
            ResetAndLogCarTermination();
        }
    }
}
