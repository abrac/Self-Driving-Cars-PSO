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
    private bool culled;
    float tempStoreOfDistTravelled;



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
        culled = false;

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
            if (Vector3.Distance(StationaryPos, this.gameObject.transform.position) < 5)
            {
                GameObject car = this.gameObject;
                if (car.GetComponent<NeuralNetwork>().sleep == false && !isDemo)
                {
                    culled = true;
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

    public void ResetAndLogCarTermination(bool usurped = false)
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
            culled = false;
        }
        else 
        {
            GameObject car = this.gameObject;
            PopulationManager popMan = evolutionManager.GetComponent<PopulationManager>();
            // Stop
            
            // Meant to be a check to penalize the known best if it crashes - but not working right
            if (!culled && NumberCarsFinished > 15 && !usurped)
            {
                float distCovered = Vector3.Distance(StartPos, this.gameObject.transform.position);
                if (distanceTravelled != 0)
                    tempStoreOfDistTravelled = distanceTravelled;
                float latestFitness = CalculateTheIndividualsFitness();
                if (latestFitness < 0.9f*popMan.GlobalBestNN_Fitness && tempStoreOfDistTravelled > 100 && isDriving)
                {
                    this.gameObject.transform.position = StartPos;                    
                    // need to get the corresponding Personal-Best and adjust by same amount
                    for (int x = 0; x < popMan.PersonalBestNN_Fitness.Count; x++)
                    {
                        if (popMan.PersonalBestNN_Fitness[x] == popMan.GlobalBestNN_Fitness)
                        {
                            popMan.PersonalBestNN_Fitness[x] = latestFitness;
                            popMan.GlobalBestNN_Fitness = latestFitness;
                        }
                    }
                }
            }
                
            popMan.PositionCarAtStartLine(car);
            lastPosition = transform.position;
            distanceTravelled = 0;
            finishingBonus = 1;
            car.GetComponent<NeuralNetwork>().Sleep();
            isDriving = false;
            culled = false;
            
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
