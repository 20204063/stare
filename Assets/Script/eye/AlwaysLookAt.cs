using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookAt : MonoBehaviour
{
    public Transform Target;
    private Transform loclaTrans;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(new Vector3(Target.position.x, Target.position.y , Target.position.z));
        //transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.y));

        LimitRot();
    }

    private void LimitRot()
    {
        Vector3 EyesEulerAngles = transform.rotation.eulerAngles;

        EyesEulerAngles.y = (EyesEulerAngles.y > 180) ? EyesEulerAngles.y - 360 : EyesEulerAngles.y;
        EyesEulerAngles.y = Mathf.Clamp(EyesEulerAngles.y, 40, 160);

        transform.rotation = Quaternion.Euler(EyesEulerAngles);

    }
}
