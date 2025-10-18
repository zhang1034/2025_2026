using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AA0000
{
    public class TestingScript : MonoBehaviour
    {
        public Vector3 rotationPoint = Vector3.zero;
        public Vector3 axis = Vector3.up;
        public float rotationSpeed = 1.0f;


        // Start is called before the first frame update
        void Start()
        {
            rotationPoint = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(rotationPoint,axis,rotationSpeed);
        }
    } 
}
