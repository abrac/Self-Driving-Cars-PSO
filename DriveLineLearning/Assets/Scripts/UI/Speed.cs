using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour
{
    public Text SpeedText;
    public float speed2;
    public GameObject car;

    // Start is called before the first frame update
    void Start()
    {
        //lastposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        speed2 = car.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>().CurrentSpeed;
        string SpeedToShow = Mathf.Abs(((int)speed2)).ToString();

        SpeedText.text = SpeedToShow + "km/h";
    }
}
