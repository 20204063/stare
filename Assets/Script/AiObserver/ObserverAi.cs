using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 120; // 회전 속도를 낮게 설정 (기본값은 300)
        player = GameObject.Find("FirstPersonPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerOnGroundLayer(); // 플레이어가 GroundLayer 위에 있는지 확인

        if (playerOnGroundLayer)
        {
            StopMovement(); // GroundLayer 위에 있으면 멈춤
        }
        else
        {
            Patrol(); // GroundLayer 밖에 있으면 순찰
        }
    }

    void Patrol()
    {
        if (!walkPointSet) SearchForDest();
        if (walkPointSet) agent.SetDestination(desPoint);

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

        // 디버깅 출력
        Debug.Log($"PlayerOnGroundLayer: {playerOnGroundLayer}");
    }

    void StopMovement()
    {
        agent.SetDestination(transform.position); // 현재 위치를 목적지로 설정해 멈추게 함
    }
}
