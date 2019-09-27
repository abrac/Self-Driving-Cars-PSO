using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Feelers_RayGenerator : MonoBehaviour
{
    public float dist60;
    public float dist30;
    public float dist0;
    public float distNeg30;
    public float distNeg60;
    private float feelerLength = 8;

    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        dist0 = getDistInDir(0);
        dist30 = getDistInDir(30);
        dist60 = getDistInDir(60);
        distNeg30 = getDistInDir(-30);
        distNeg60 = getDistInDir(-60);
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
