using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feelers_RayGenerator : MonoBehaviour
{
    public float Dist0;
    public float Dist45;
    public float DistNeg45;
    private float feelerLength = 8;

    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //0 degrees
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, feelerLength))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Dist0 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * feelerLength, Color.blue);
            Dist0 = -1;
        }

        //45 degrees
        Vector3 directionVector45 = transform.localPosition + new Vector3((float)(1 / System.Math.Sqrt(2)), 0, (float)(1 / System.Math.Sqrt(2)));
        directionVector45 = transform.TransformVector(directionVector45);
        if (Physics.Raycast(transform.position, directionVector45, out hit, feelerLength))
        {
            Debug.DrawRay(transform.position, directionVector45 * hit.distance, Color.yellow);
            Dist45 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVector45 * feelerLength, Color.blue);
            Dist45 = -1;
        }

        //-45 degrees
        Vector3 directionVectorNeg45 = transform.localPosition + new Vector3((float)(-1 / System.Math.Sqrt(2)), 0, (float)(1 / System.Math.Sqrt(2)));
        directionVectorNeg45 = transform.TransformVector(directionVectorNeg45);
        if (Physics.Raycast(transform.position, directionVectorNeg45, out hit, feelerLength))
        {
            Debug.DrawRay(transform.position, directionVectorNeg45 * hit.distance, Color.yellow);
            DistNeg45 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVectorNeg45 * feelerLength, Color.blue);
            DistNeg45 = -1;
        }
    }

}
