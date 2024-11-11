using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SmoothLookAt : MonoBehaviour
{
    public Transform Target; // 목표로 삼을 대상
    public float Speed = 1f; // 회전 속도

    // 매 프레임마다 키 입력을 감지하여 회전
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) // "1" 키를 누르고 있는 동안
        {
            RotateTowardsTarget();
        }
    }

    // 목표를 향해 부드럽게 회전하는 함수 (Y축 고정)
    private void RotateTowardsTarget()
    {
        // 목표 방향 계산
        Vector3 direction = Target.position - transform.position;

        // 목표 방향으로의 회전을 계산한 후, Y축은 고정하고 X, Z 회전만 설정
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // 현재 회전에서 목표 회전으로 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);
    }
}