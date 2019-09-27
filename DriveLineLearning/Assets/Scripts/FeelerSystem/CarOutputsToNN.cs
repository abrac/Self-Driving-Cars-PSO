using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarOutputsToNN : MonoBehaviour
{
    public GameObject feelerContainer;
    Feelers_RayGenerator feelers;
    
    public float[] feelersDists { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        feelers = feelerContainer.GetComponent<Feelers_RayGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        feelersDists = feelers.feelerDists;

    }
}
