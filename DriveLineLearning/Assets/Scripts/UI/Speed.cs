using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour
{
    public Text SpeedText;
    private float lastposition;
    // Start is called before the first frame update
    void Start()
    {
        lastposition = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = (transform.position.magnitude - lastposition)/Time.deltaTime;
        lastposition = transform.position.magnitude;

        string SpeedToShow = Mathf.Abs(((int)speed)).ToString();

        SpeedText.text = SpeedToShow + "km/h";
    }
}
