using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SmoothLookAt : MonoBehaviour
{
    public Transform Target; // ��ǥ�� ���� ���
    public float Speed = 1f; // ȸ�� �ӵ�

    // �� �����Ӹ��� Ű �Է��� �����Ͽ� ȸ��
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) // "1" Ű�� ������ �ִ� ����
        {
            RotateTowardsTarget();
        }
    }

    // ��ǥ�� ���� �ε巴�� ȸ���ϴ� �Լ� (Y�� ����)
    private void RotateTowardsTarget()
    {
        // ��ǥ ���� ���
        Vector3 direction = Target.position - transform.position;

        // ��ǥ ���������� ȸ���� ����� ��, Y���� �����ϰ� X, Z ȸ���� ����
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);
    }
}