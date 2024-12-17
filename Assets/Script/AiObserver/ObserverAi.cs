using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObserverAi : MonoBehaviour
{
    GameObject player;

    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer; // NavMeshSurface가 속한 레이어
    [SerializeField] float range;

    // Patrol
    Vector3 desPoint;
    bool walkPointSet;

    // Player detection on NavMeshSurface
    bool playerOnGroundLayer; // 플레이어가 GroundLayer 위에 있는지 여부

    // Runaway Script Reference
    Runaway runawayScript; // 같은 GameObject에 붙어 있는 Runaway 스크립트

    // SmoothLookAt References
    [SerializeField] List<SmoothLookAt> lookAtScripts; // 모든 NPC의 SmoothLookAt 스크립트를 참조
    private bool hasTriggered = false; // runaway가 한 번만 실행하도록 제어


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 360; // 회전 속도를 높게 설정
        agent.updateRotation = true; // 자동 회전 활성화
        player = GameObject.Find("FirstPersonPlayer");

        // 같은 GameObject에 있는 Runaway 스크립트를 가져옴
        runawayScript = GetComponent<Runaway>();
        if (runawayScript != null)
        {
            runawayScript.enabled = false; // 초기 상태 비활성화
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerOnGroundLayer(); // 플레이어가 GroundLayer 위에 있는지 확인

        if (playerOnGroundLayer)
        {
            StopMovement(); // GroundLayer 위에 있으면 AI 멈춤
            ActivateRunaway(); // 같은 GameObject의 Runaway 스크립트 활성화


            if (!hasTriggered)
            {
                RotateAllTowardsPlayerOnce(); // 모든 NPC가 플레이어를 바라보게 설정
                hasTriggered = true; // 한 번만 실행되도록 설정
            }

            // 모든 NPC가 회전 완료했는지 확인
            else if (CheckAllRotationsComplete())
            {
                StopAllForceLookAt(); // 강제 회전 해제
            }
        }
        else
        {
            Patrol(); // GroundLayer 밖에 있으면 순찰
            DeactivateRunaway(); // Runaway 스크립트 비활성화
            hasTriggered = false; // 다시 초기화
        }
    }


    void Patrol()
    {
        if (!walkPointSet) SearchForDest();
        if (walkPointSet)
        {
            agent.SetDestination(desPoint);

            // AI가 목적지를 바라보도록 강제로 회전 처리
            RotateTowardsDestination(desPoint);
        }

        if (Vector3.Distance(transform.position, desPoint) < agent.stoppingDistance + 0.5f)
        {
            walkPointSet = false;
        }
    }

    void SearchForDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        desPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Vector3.Distance(transform.position, desPoint) > 5f) // 최소 거리 조건 추가
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(desPoint, out hit, range, NavMesh.AllAreas))
            {
                desPoint = hit.position;
                walkPointSet = true;
            }
        }
    }

    void CheckPlayerOnGroundLayer()
    {
        // 플레이어가 GroundLayer 위에 있는지 확인
        Ray ray = new Ray(player.transform.position, Vector3.down); // 플레이어 아래로 레이캐스트
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            playerOnGroundLayer = true; // GroundLayer에 부딪혔으면 true
        }
        else
        {
            playerOnGroundLayer = false; // 그렇지 않으면 false
        }

        
    }

    void StopMovement()
    {
        agent.SetDestination(transform.position); // 현재 위치를 목적지로 설정해 멈추게 함
    }

    void RotateTowardsDestination(Vector3 target)
    {
        // 목표 방향 계산
        Vector3 direction = (target - transform.position).normalized;

        // 회전 처리
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed / 100f);
        }
    }

    void ActivateRunaway()
    {
        if (runawayScript != null && !runawayScript.enabled)
        {
            runawayScript.enabled = true; // Runaway 스크립트 활성화
        }
    }

    void DeactivateRunaway()
    {
        if (runawayScript != null && runawayScript.enabled)
        {
            runawayScript.enabled = false; // Runaway 스크립트 비활성화
        }
    }

    void RotateAllTowardsPlayerOnce()
    {
        foreach (SmoothLookAt lookAt in lookAtScripts)
        {
            if (lookAt != null && !lookAt.IsRotationComplete()) // 회전이 완료되지 않은 NPC만 처리
            {
                lookAt.Target = player.transform; // 플레이어를 목표로 설정
                lookAt.ForceLookAt(); // 강제로 회전
            }
        }
    }
    bool CheckAllRotationsComplete()
    {
        foreach (SmoothLookAt lookAt in lookAtScripts)
        {
            if (lookAt != null && !lookAt.IsRotationComplete())
            {
                return false; // 하나라도 회전이 완료되지 않았다면 false 반환
            }
        }
        return true; // 모두 회전 완료
    }

    void StopAllForceLookAt()
    {
        foreach (SmoothLookAt lookAt in lookAtScripts)
        {
            if (lookAt != null)
            {
                lookAt.StopForceLookAt(); // 강제 회전 해제
            }
        }
    }
}