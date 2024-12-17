using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SmoothLookAt : MonoBehaviour
{
    public Transform Target; // ��ǥ�� ���� ���
    public float Speed = 3f; // ȸ�� �ӵ�
    [SerializeField] private float triggerDistance = 10f;

    private bool forceLookAt = false; // ������ ȸ����Ű�� �÷���

    private float angleThreshold = 10f; // ȸ�� �Ϸ�� ������ ���� �Ӱ谪

    // �� �����Ӹ��� Ű �Է��� �����Ͽ� ȸ��
    void Update()
    {

        if (Target == null) return;

        // �÷��̾�� NPC �� �Ÿ� ���
        float distance = Vector3.Distance(Target.position, transform.position);

        // ���� ȸ�� ���¿��� ȸ�� �Ϸ� �˻�
        if (forceLookAt)
        {
            RotateTowardsTarget();
        }
        else if (distance <= triggerDistance)
        {
            RotateTowardsTarget(); // ���� triggerDistance ����
        }
    }


    //if (Input.GetKey(KeyCode.Alpha1)) // "1" Ű�� ������ �ִ� ����
    //{
    //    RotateTowardsTarget();
    //}


    // ��ǥ�� ���� �ε巴�� ȸ���ϴ� �Լ� (Y�� ����)
    public void RotateTowardsTarget()
    {

        if (Target == null) return; // Target�� �������� �ʾҴٸ� �������� ����

        // ��ǥ ���� ���
        Vector3 direction = Target.position - transform.position;

        // ��ǥ ���������� ȸ���� ����� ��, Y���� �����ϰ� X, Z ȸ���� ����
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);

    }

    // ������ ȸ���ϰ� ����
    public void ForceLookAt()
    {
        forceLookAt = true;
    }

    // ���� ȸ���� ����
    public void StopForceLookAt()
    {
        forceLookAt = false;
    }

    // ȸ�� �Ϸ� ���� Ȯ��
    public bool IsRotationComplete()
    {
        if (Target == null) return false;

        Vector3 directionToTarget = Target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        bool isComplete = angle <= angleThreshold;

        Debug.Log($"{name}: IsRotationComplete={angle <= angleThreshold}, Angle={angle}, Threshold={angleThreshold}");
        return angle <= angleThreshold; // ���� ���̰� �Ӱ谪 ���϶�� true
    }
}



