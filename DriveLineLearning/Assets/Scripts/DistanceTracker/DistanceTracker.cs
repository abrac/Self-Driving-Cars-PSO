using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    public float distanceTravelled = 0;
    Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        distanceTravelled = 0;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += Vector3.Distance(transform.position,lastPosition);
        lastPosition = transform.position;
    }
}
