using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    //Player Moving
    Vector3 moveDir = Vector3.zero;

    [SerializeField]
    float speed = 5f;


    //Player Attack

    int currentAttackNum = 0;
    [SerializeField]
    float attackDelay = 1f;
    float comboDelay = 0.2f;
    bool isAttacking = false;


    //Animation
    Animator animator;



    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }


    void Update()
    {

        Move();
        Attack();
    }


    void Move()
    {

        if (isAttacking)
        {
            SyncAnimationTransform();
            return;
        }

        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += Vector3.back;
        }

        moveDir.Normalize();

        transform.position += moveDir * speed * Time.deltaTime;

    }

    void Attack()
    {

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {

            StartCoroutine(ComboAttack());
        }

    }


    IEnumerator ComboAttack()
    {
        isAttacking = true;

        switch (currentAttackNum)
        {
            case 0:
                animator.Play("COMBO_ATTACK_0");
                break;
            case 1:
                animator.Play("COMBO_ATTACK_1");
                break;
            case 2:
                animator.Play("COMBO_ATTACK_2");
                break;
            case 3:
                animator.Play("COMBO_ATTACK_3");
                break;
        }
        currentAttackNum++;
        currentAttackNum = currentAttackNum % 4;


        yield return new WaitForSeconds(attackDelay);

        animator.CrossFade("ATTACK_IDLE0", 0.1f);
        yield return new WaitForSeconds(0.1f);

        isAttacking = false;
    }

    void SyncAnimationTransform()
    {

        //애니메이션 모델이랑 동기화

        Vector3 posOffset = animator.transform.localPosition;
        transform.position += posOffset;
        animator.transform.localPosition = Vector3.zero;

        //Quaternion rotOffset = animator.transform.localRotation;
        //transform.rotation += animator.transform.localRotation;
        //animator.transform.localRotation = Quaternion.identity;


    }
}
