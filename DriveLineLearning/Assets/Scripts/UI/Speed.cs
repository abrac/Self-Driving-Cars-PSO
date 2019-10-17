using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour
{
    public Text SpeedText;
    private Vector3 lastposition;
    // Start is called before the first frame update
    void Start()
    {
        lastposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Vector3.Distance(lastposition, transform.position)/Time.deltaTime;
        lastposition = transform.position;

        string SpeedToShow = Mathf.Abs(((int)speed)).ToString();

        SpeedText.text = SpeedToShow + "km/h";
    }
}
