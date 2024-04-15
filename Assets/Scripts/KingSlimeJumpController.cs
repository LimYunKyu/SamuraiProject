using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeJumpController : MonoBehaviour
{

    [SerializeField]
    Transform jumpEffect;

    KingSlimeController script;
    [SerializeField]
    float jumpDistance = 3f;
    bool isJumping = false;

    LayerMask mask;
    void Start()
    {
        script = transform.root.gameObject.GetComponent<KingSlimeController>();
        mask = 1 << LayerMask.NameToLayer("Floor");
    }

    
    void Update()
    {
        CheckFloor();
    }


    void CheckFloor()
    {
        // ����ĳ��Ʈ�� �߻��� ���� ������ ���� ����
        Vector3 rayOrigin = transform.position; // �߻� ������ ���� ������Ʈ�� ��ġ�� ����
        Vector3 rayDirection = Vector3.down; // �Ʒ� �������� ����ĳ��Ʈ�� �߻�
        
        RaycastHit hit; // �浹 ������ ������ ����ü

        // ����ĳ��Ʈ �߻� �� �浹 ���� Ȯ��
        if (Physics.Raycast(rayOrigin, rayDirection, out hit,jumpDistance, mask))
        {
            //if (hit.collider.gameObject.tag != "Floor")
            //    return;

            if (hit.distance <= 0.2f && isJumping)
            {
                // ���̰� ��ü�� �浹���� �� ������ �ڵ�
                jumpEffect.GetComponent<ParticleSystem>().Play();
                isJumping = false;
            }
        }
        else
        { 
            isJumping = true;
        
        
        }


    }

  
}
