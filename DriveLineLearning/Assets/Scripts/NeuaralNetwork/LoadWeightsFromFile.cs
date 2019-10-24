using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadWeightsFromFile : MonoBehaviour
{
    public TextAsset File;
    
    // Start is called before the first frame update
    void Start()
    {
        SetCarsNN_WeightsToThoseReadInFromFile();

        this.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //this.gameObject.GetComponent<CarSideEvolutionaryBehaviour>().isDriving = true;
        this.gameObject.GetComponent<NeuralNetwork>().WakeUp();
        this.gameObject.GetComponent<Timer>().ResetTimer();
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.GetComponent<CarSideEvolutionaryBehaviour>().isDriving = true;
        this.gameObject.GetComponent<NeuralNetwork>().WakeUp();
    }

    private void SetCarsNN_WeightsToThoseReadInFromFile() 
    {
        NeuralNetwork NN = this.gameObject.GetComponent<NeuralNetwork>();
        string path = "Assets/Best weights Logs/";
        path = path + File.name + ".txt";

        StreamReader sr = new StreamReader(path);
        sr.ReadLine();//WriteLine("/////////////////// START ///////////////////");
        sr.ReadLine();//WriteLine("Hidden layers: ", NN.hiddenLayers);
        sr.ReadLine();//WriteLine("Hidden layer size: ", NN.hLayer_size);
        sr.ReadLine();//WriteLine("Hidden layers: ", NN.hiddenLayers);

        sr.ReadLine();//WriteLine("Fitness: " + GlobalBestNN_Fitness);
        foreach (float[][] cur in NN.weights)
        {
            for (int x = 0, length = cur.Length; x < length; x++)
            {
                for (int y = 0, innerLength = cur[x].Length; y < innerLength; y++)
                {   
                    float[] vals = Array.ConvertAll(sr.ReadLine().Split(':'), float.Parse);
                    NN.weights[(int)vals[0]][(int)vals[1]][(int)vals[2]] = vals[3];//sr.WriteLine("{0}{1}{2}:{3}",layer,x,y,cur[x][y]);
                }
            }
        }
        //sr.WriteLine("///////////////////  END  ///////////////////");
        sr.Close();
    }

}
