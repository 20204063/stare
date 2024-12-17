using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SmoothLookAt : MonoBehaviour
{
    public Transform Target; // 목표로 삼을 대상
    public float Speed = 3f; // 회전 속도
    [SerializeField] private float triggerDistance = 10f;

    private bool forceLookAt = false; // 강제로 회전시키는 플래그

    private float angleThreshold = 10f; // 회전 완료로 간주할 각도 임계값

    // 매 프레임마다 키 입력을 감지하여 회전
    void Update()
    {

        if (Target == null) return;

        // 플레이어와 NPC 간 거리 계산
        float distance = Vector3.Distance(Target.position, transform.position);

        // 강제 회전 상태에서 회전 완료 검사
        if (forceLookAt)
        {
            RotateTowardsTarget();
        }
        else if (distance <= triggerDistance)
        {
            RotateTowardsTarget(); // 개별 triggerDistance 조건
        }
    }


    //if (Input.GetKey(KeyCode.Alpha1)) // "1" 키를 누르고 있는 동안
    //{
    //    RotateTowardsTarget();
    //}


    // 목표를 향해 부드럽게 회전하는 함수 (Y축 고정)
    public void RotateTowardsTarget()
    {

        if (Target == null) return; // Target이 설정되지 않았다면 실행하지 않음

        // 목표 방향 계산
        Vector3 direction = Target.position - transform.position;

        // 목표 방향으로의 회전을 계산한 후, Y축은 고정하고 X, Z 회전만 설정
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // 현재 회전에서 목표 회전으로 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);

    }

    // 강제로 회전하게 설정
    public void ForceLookAt()
    {
        forceLookAt = true;
    }

    // 강제 회전을 해제
    public void StopForceLookAt()
    {
        forceLookAt = false;
    }

    // 회전 완료 여부 확인
    public bool IsRotationComplete()
    {
        if (Target == null) return false;

        Vector3 directionToTarget = Target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        bool isComplete = angle <= angleThreshold;

        Debug.Log($"{name}: IsRotationComplete={angle <= angleThreshold}, Angle={angle}, Threshold={angleThreshold}");
        return angle <= angleThreshold; // 각도 차이가 임계값 이하라면 true
    }
}



