using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Feelers_RayGenerator : MonoBehaviour
{ 
    public float[] feelerDists = new float[9];
    public float fieldOfView = 120;
    public float feelerLength = 70;
    public float heightOffGround = 0.4f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < feelerDists.Length; i++)
        {
            feelerDists[i] = feelerLength - getDistInDir(-fieldOfView / 2 + i * fieldOfView / (feelerDists.Length - 1));
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

        if (Physics.Raycast(transform.position + new Vector3(0,heightOffGround,0), directionVector, out hit, feelerLength))
        {
            Debug.DrawRay(transform.position + new Vector3(0,heightOffGround,0), directionVector * hit.distance, Color.yellow);
            dist = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position + new Vector3(0,heightOffGround,0), directionVector * feelerLength, Color.blue);
            dist = feelerLength;
        }
        return dist;
    }
}
