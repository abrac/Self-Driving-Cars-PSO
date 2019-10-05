using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Feelers_RayGenerator : MonoBehaviour
{ 
    public float[] feelerDists = new float[5];
    public float fieldOfView = 120;
    private float feelerLength = 8;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < feelerDists.Length; i++)
        {
            feelerDists[i] = getDistInDir(-fieldOfView / 2 + i * fieldOfView / (feelerDists.Length - 1));
        }
    }
    float getDistInDir(float angleDeg)
    {
        float angle = -(float)(angleDeg * Math.PI / 180);
        float dist;
        RaycastHit hit;
        //Getting direction vector, first in local space, then world space
        Vector3 directionVector = transform.localPosition + new Vector3((float)Math.Sin(angle), 0, (float)(Math.Cos(angle)));
        directionVector = transform.TransformVector(directionVector);

        if (Physics.Raycast(transform.position, directionVector, out hit, feelerLength))
        {
            Debug.DrawRay(transform.position, directionVector * hit.distance, Color.yellow);
            dist = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVector * feelerLength, Color.blue);
            dist = -1;
        }
        return dist;
    }
}
