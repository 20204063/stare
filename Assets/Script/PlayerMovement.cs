using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float horizontalInput;
    public float verticalInput;
    public float turnSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // read values from keyboard
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // move the object
        transform.Translate(UnityEngine.Vector3.forward * Time.deltaTime * verticalInput);
        //transform.Translate(UnityEngine.Vector3.right * Time.deltaTime * horizontalInput);

        transform.Rotate(UnityEngine.Vector3.up * horizontalInput * turnSpeed * Time.deltaTime);
         
    }
}
