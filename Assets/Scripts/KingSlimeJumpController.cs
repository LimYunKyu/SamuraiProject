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
        // 레이캐스트를 발사할 시작 지점과 방향 설정
        Vector3 rayOrigin = transform.position; // 발사 지점을 현재 오브젝트의 위치로 설정
        Vector3 rayDirection = Vector3.down; // 아래 방향으로 레이캐스트를 발사
        
        RaycastHit hit; // 충돌 정보를 저장할 구조체

        // 레이캐스트 발사 및 충돌 여부 확인
        if (Physics.Raycast(rayOrigin, rayDirection, out hit,jumpDistance, mask))
        {
            //if (hit.collider.gameObject.tag != "Floor")
            //    return;

            if (hit.distance <= 0.2f && isJumping)
            {
                // 레이가 물체와 충돌했을 때 실행할 코드
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
