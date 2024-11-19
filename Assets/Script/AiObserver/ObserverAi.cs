using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class ObserverAi : MonoBehaviour
{
    GameObject player;

    NavMeshAgent agent;


    [SerializeField] LayerMask groundLayer; // NavMeshSurface�� ���� ���̾�
    [SerializeField] float range;

    // Patrol
    Vector3 desPoint;
    bool walkPointSet;

    // Player detection on NavMeshSurface
    bool playerOnGroundLayer; // �÷��̾ GroundLayer ���� �ִ��� ����

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 120; // ȸ�� �ӵ��� ���� ���� (�⺻���� 300)
        player = GameObject.Find("FirstPersonPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerOnGroundLayer(); // �÷��̾ GroundLayer ���� �ִ��� Ȯ��

        if (playerOnGroundLayer)
        {
            StopMovement(); // GroundLayer ���� ������ ����
        }
        else
        {
            Patrol(); // GroundLayer �ۿ� ������ ����
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

        if (Vector3.Distance(transform.position, desPoint) > 5f) // �ּ� �Ÿ� ���� �߰�
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
        // �÷��̾ GroundLayer ���� �ִ��� Ȯ��
        Ray ray = new Ray(player.transform.position, Vector3.down); // �÷��̾� �Ʒ��� ����ĳ��Ʈ
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            playerOnGroundLayer = true; // GroundLayer�� �ε������� true
        }
        else
        {
            playerOnGroundLayer = false; // �׷��� ������ false
        }

        // ����� ���
        Debug.Log($"PlayerOnGroundLayer: {playerOnGroundLayer}");
    }

    void StopMovement()
    {
        agent.SetDestination(transform.position); // ���� ��ġ�� �������� ������ ���߰� ��
    }
}
