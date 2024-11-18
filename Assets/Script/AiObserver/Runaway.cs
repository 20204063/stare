using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class Runaway : MonoBehaviour
{

    [SerializeField] private NavMeshAgent npc = null;

    [SerializeField] private Transform player = null;

    [SerializeField] private float displacementDist = 5f;

    [SerializeField] private float triggerDistance = 20f;

    [SerializeField] private float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        if (npc == null)
            if (!TryGetComponent(out npc))
                Debug.LogWarning(name + "need a navemesh agent");
        // 이동 속도를 2배로 설정
        npc.speed *= speed;

        npc.angularSpeed *= speed; // 회전 속도 증가
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || npc == null) return;

        // 플레이어와 NPC 간 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 작동 조건: 플레이어가 특정 거리(triggerDistance) 이내에 있을 때만 실행
        if (distance > triggerDistance) return;


        // NPC가 이미 충분히 멀리 있다면 이동하지 않음
        if (distance >= displacementDist) return;

        // 플레이어의 반대 방향으로 이동
        Vector3 normDir = (player.position - transform.position).normalized;
        MoveToPos(transform.position - (normDir * displacementDist));
    }

    void MoveToPos(Vector3 pos)
    {
        if (npc == null) return;

        npc.SetDestination(pos);
        npc.isStopped = false;




        // 회전을 해서 뒤돌아보지 않고 이동하게

    }
}
