using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookAt : MonoBehaviour
{
    public Transform Target;
    private Transform loclaTrans;

    float fixedYValue = -90f;
    float fixedZValue = -90f;

    Vector3 fixedValue;

    // Start is called before the first frame update
    void Start()
    {
        fixedValue = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        LookAt();
    }

    

    private void LookAt()
    {

        transform.LookAt(new Vector3(Target.position.x, Target.position.y, Target.position.z));


        Vector3 EyesEulerAngles = transform.localRotation.eulerAngles;

        

        EyesEulerAngles.x = (EyesEulerAngles.x > 180) ? EyesEulerAngles.x - 360 : EyesEulerAngles.x;
        EyesEulerAngles.x = Mathf.Clamp(EyesEulerAngles.x, 35, 145);

        transform.localRotation = Quaternion.Euler(EyesEulerAngles);

    }
}
