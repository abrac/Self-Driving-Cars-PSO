using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feelers_RayGenerator : MonoBehaviour
{
    public float Dist0 { get; private set; }
    public float Dist45 { get; private set; }
    public float DistNeg45 { get; private set; }

    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //0 degrees
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5.0f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Dist0 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.blue);
            Dist0 = -1;
        }

        //45 degrees
        Vector3 directionVector45 = transform.localPosition + new Vector3((float)(1 / System.Math.Sqrt(2)), 0, (float)(1 / System.Math.Sqrt(2)));
        directionVector45 = transform.TransformVector(directionVector45);
        if (Physics.Raycast(transform.position, directionVector45, out hit, 5.0f))
        {
            Debug.DrawRay(transform.position, directionVector45 * hit.distance, Color.yellow);
            Debug.Log("45deg Hit at " + hit.distance);
            Dist45 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVector45 * 5, Color.blue);
            Dist45 = -1;
        }

        //-45 degrees
        Vector3 directionVectorNeg45 = transform.localPosition + new Vector3((float)(-1 / System.Math.Sqrt(2)), 0, (float)(1 / System.Math.Sqrt(2)));
        directionVectorNeg45 = transform.TransformVector(directionVectorNeg45);
        if (Physics.Raycast(transform.position, directionVectorNeg45, out hit, 5.0f))
        {
            Debug.DrawRay(transform.position, directionVectorNeg45 * hit.distance, Color.yellow);
            DistNeg45 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVectorNeg45 * 5, Color.blue);
            DistNeg45 = -1;
        }
    }

}
