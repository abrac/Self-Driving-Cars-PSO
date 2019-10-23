using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //public Text TimerText;

    public float timeElapsedInSec;
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeElapsedInSec = Time.time - startTime;
        string minutes = ((int)timeElapsedInSec / 60).ToString();
        string seconds = (timeElapsedInSec % 60).ToString("f2");

        //TimerText.text = minutes + ":" + seconds;
    }

    public void ResetTimer() 
    {
        startTime = Time.time;
    }
}

