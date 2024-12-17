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

    [SerializeField] private float speed = 5f;

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    { 
        if (npc == null)
            if (!TryGetComponent(out npc))
                Debug.LogWarning(name + "need a navemesh agent");
        // �̵� �ӵ��� 2��� ����
        npc.speed *= speed;
        npc.angularSpeed *= speed; // ȸ�� �ӵ� ����

        // �ﰢ ������ ���� ����
        npc.acceleration = 100f;       // ���� �ʱ� ����
        npc.autoBraking = false;       // �극��ŷ ��Ȱ��ȭ�� �ﰢ �̵�
        npc.isStopped = false;         // �ʱ� �̵� �غ� �Ϸ�

        // ȸ�� �ڵ� ������Ʈ ��Ȱ��ȭ
        npc.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {



        if (player == null || npc == null) return;

        // �÷��̾�� NPC �� �Ÿ� ���
        float distance = Vector3.Distance(player.position, transform.position);

        // �۵� ����: �÷��̾ Ư�� �Ÿ�(triggerDistance) �̳��� ���� ���� ����
        if (distance > triggerDistance) 
        {
           

            return;
        }

        // NPC�� �̹� ����� �ָ� �ִٸ� �̵����� ����
        if (distance >= displacementDist) return;

        // �÷��̾��� �ݴ� �������� �̵�
        Vector3 normDir = (player.position - transform.position).normalized;
        MoveToPos(transform.position - (normDir * displacementDist));
        animator.SetBool("isAnimation", true);
        
    }

    void MoveToPos(Vector3 pos)
    {
        if (npc == null) return;

        npc.SetDestination(pos);
        npc.isStopped = false;


    }
}
