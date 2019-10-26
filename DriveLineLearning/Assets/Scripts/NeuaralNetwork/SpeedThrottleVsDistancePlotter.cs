using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpeedThrottleVsDistancePlotter : MonoBehaviour
{
    // Variables For Outputting Data
    public bool logOutputs  = false;
    public string folderToSaveTo = "Throt&Velo&Dist";

    public string customLabel;
    private StreamWriter outputStream;

    private float startTime;
    private int lastPrintedDist;

    // Start is called before the first frame update
    void Start()
    {
        // Open logging files
        if (logOutputs)
        {
            lastPrintedDist = -1;
            startTime = Time.time;
            Feelers_RayGenerator feelerSettings = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Feelers_RayGenerator>();
            NeuralNetwork NN = this.gameObject.GetComponent<NeuralNetwork>();
            // Initialize naming conventions (open/create all the files) add a time stamp line to each
            if (folderToSaveTo.Length != 0) 
                folderToSaveTo = folderToSaveTo + "/";
            string filepath = String.Format("{0}{5}-Log(Feelr#={1}len{2};#recur={3};#hid={4})"
                                        ,folderToSaveTo, feelerSettings.feelerDists.Length, feelerSettings.feelerLength, NN.outputs-2, NN.hLayer_size, customLabel);

            outputStream = new StreamWriter("Assets/logs/" + filepath + ".txt", true);
            outputStream.WriteLine("/////////////////// START ///////////////////");
            outputStream.WriteLine("///// " + DateTime.Now);
            outputStream.WriteLine("///// " + filepath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (logOutputs)
        {
            float distTravelled = this.gameObject.GetComponent<CarSideEvolutionaryBehaviour>().distanceTravelled;

            //float t = Time.time - startTime;
            //print every second if (t > 1)
            int curDist = (int)distTravelled; 
            if (curDist != lastPrintedDist)
            {
                float time = this.gameObject.GetComponent<Timer>().timeElapsedInSec;
                
                float throttleSetting = this.gameObject.GetComponent<NeuralNetwork>().throttle;
                float speed = this.gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>().CurrentSpeed;;
                string SpeedToShow = Mathf.Abs(((int)speed)).ToString();
                string lineToPrint = String.Format("{0};{1};{2};{3}",time,(int)distTravelled,throttleSetting,SpeedToShow);
                outputStream.WriteLine(lineToPrint.Replace(',','.'));
                Debug.Log(lineToPrint);
                startTime = Time.time;
                lastPrintedDist = curDist;
            }
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (logOutputs) 
        {
            float time = this.gameObject.GetComponent<Timer>().timeElapsedInSec;
            if (time > 10)
            {
                outputStream.WriteLine("//////// Lap Time: {0}",time);
                outputStream.WriteLine("///////////////////  END  ///////////////////");
                outputStream.Close();
                logOutputs = false;
            } 
        }  
    }

    private void OnDestroy() {
         
    }
}
