using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feelers_RayGenerator : MonoBehaviour
{
    public float dist0;
    public float dist45;
    public float distNeg45;
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
            dist0 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * feelerLength, Color.blue);
            dist0 = -1;
        }

        //45 degrees
        Vector3 directionVector45 = transform.localPosition + new Vector3((float)(1 / System.Math.Sqrt(2)), 0, (float)(1 / System.Math.Sqrt(2)));
        directionVector45 = transform.TransformVector(directionVector45);
        if (Physics.Raycast(transform.position, directionVector45, out hit, feelerLength))
        {
            Debug.DrawRay(transform.position, directionVector45 * hit.distance, Color.yellow);
            dist45 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVector45 * feelerLength, Color.blue);
            dist45 = -1;
        }

        //-45 degrees
        Vector3 directionVectorNeg45 = transform.localPosition + new Vector3((float)(-1 / System.Math.Sqrt(2)), 0, (float)(1 / System.Math.Sqrt(2)));
        directionVectorNeg45 = transform.TransformVector(directionVectorNeg45);
        if (Physics.Raycast(transform.position, directionVectorNeg45, out hit, feelerLength))
        {
            Debug.DrawRay(transform.position, directionVectorNeg45 * hit.distance, Color.yellow);
            distNeg45 = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, directionVectorNeg45 * feelerLength, Color.blue);
            distNeg45 = -1;
        }
    }

}
