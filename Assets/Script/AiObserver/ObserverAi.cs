using System.Collections;
using System.Collections.Generic;
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

    // Runaway Script Reference
    Runaway runawayScript; // ���� GameObject�� �پ� �ִ� Runaway ��ũ��Ʈ

    // SmoothLookAt References
    [SerializeField] List<SmoothLookAt> lookAtScripts; // ��� NPC�� SmoothLookAt ��ũ��Ʈ�� ����
    private bool hasTriggered = false; // runaway�� �� ���� �����ϵ��� ����


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 360; // ȸ�� �ӵ��� ���� ����
        agent.updateRotation = true; // �ڵ� ȸ�� Ȱ��ȭ
        player = GameObject.Find("FirstPersonPlayer");

        // ���� GameObject�� �ִ� Runaway ��ũ��Ʈ�� ������
        runawayScript = GetComponent<Runaway>();
        if (runawayScript != null)
        {
            runawayScript.enabled = false; // �ʱ� ���� ��Ȱ��ȭ
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerOnGroundLayer(); // �÷��̾ GroundLayer ���� �ִ��� Ȯ��

        if (playerOnGroundLayer)
        {
            StopMovement(); // GroundLayer ���� ������ AI ����
            ActivateRunaway(); // ���� GameObject�� Runaway ��ũ��Ʈ Ȱ��ȭ


            if (!hasTriggered)
            {
                RotateAllTowardsPlayerOnce(); // ��� NPC�� �÷��̾ �ٶ󺸰� ����
                hasTriggered = true; // �� ���� ����ǵ��� ����
            }

            // ��� NPC�� ȸ�� �Ϸ��ߴ��� Ȯ��
            else if (CheckAllRotationsComplete())
            {
                StopAllForceLookAt(); // ���� ȸ�� ����
            }
        }
        else
        {
            Patrol(); // GroundLayer �ۿ� ������ ����
            DeactivateRunaway(); // Runaway ��ũ��Ʈ ��Ȱ��ȭ
            hasTriggered = false; // �ٽ� �ʱ�ȭ
        }
    }


    void Patrol()
    {
        if (!walkPointSet) SearchForDest();
        if (walkPointSet)
        {
            agent.SetDestination(desPoint);

            // AI�� �������� �ٶ󺸵��� ������ ȸ�� ó��
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

        
    }

    void StopMovement()
    {
        agent.SetDestination(transform.position); // ���� ��ġ�� �������� ������ ���߰� ��
    }

    void RotateTowardsDestination(Vector3 target)
    {
        // ��ǥ ���� ���
        Vector3 direction = (target - transform.position).normalized;

        // ȸ�� ó��
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
            runawayScript.enabled = true; // Runaway ��ũ��Ʈ Ȱ��ȭ
        }
    }

    void DeactivateRunaway()
    {
        if (runawayScript != null && runawayScript.enabled)
        {
            runawayScript.enabled = false; // Runaway ��ũ��Ʈ ��Ȱ��ȭ
        }
    }

    void RotateAllTowardsPlayerOnce()
    {
        foreach (SmoothLookAt lookAt in lookAtScripts)
        {
            if (lookAt != null && !lookAt.IsRotationComplete()) // ȸ���� �Ϸ���� ���� NPC�� ó��
            {
                lookAt.Target = player.transform; // �÷��̾ ��ǥ�� ����
                lookAt.ForceLookAt(); // ������ ȸ��
            }
        }
    }
    bool CheckAllRotationsComplete()
    {
        foreach (SmoothLookAt lookAt in lookAtScripts)
        {
            if (lookAt != null && !lookAt.IsRotationComplete())
            {
                return false; // �ϳ��� ȸ���� �Ϸ���� �ʾҴٸ� false ��ȯ
            }
        }
        return true; // ��� ȸ�� �Ϸ�
    }

    void StopAllForceLookAt()
    {
        foreach (SmoothLookAt lookAt in lookAtScripts)
        {
            if (lookAt != null)
            {
                lookAt.StopForceLookAt(); // ���� ȸ�� ����
            }
        }
    }
}