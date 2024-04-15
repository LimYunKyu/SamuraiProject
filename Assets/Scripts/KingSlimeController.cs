using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class KingSlimeController : MonoBehaviour
{

    public enum KINGSLIME_STATE
    {
        IDLE,
        MOVE,
        ATTACK,
        HIT,
        JUMP,

    }


    bool isTracking = false;
    [SerializeField]
    float attackDistance = 1f;

    SphereCollider sphereCollider = null;

    NavMeshAgent nav;


    Animator anim;
    [SerializeField]
    Transform target;

    
    [SerializeField]
    public KINGSLIME_STATE slimeState = KINGSLIME_STATE.IDLE;
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {

       

        switch (slimeState)
        {
            case KINGSLIME_STATE.IDLE:
                UpdateIdle();
                break;
            case KINGSLIME_STATE.MOVE:
                UpdateMove();
                break;
            case KINGSLIME_STATE.ATTACK:
                UpdateAttack();
                break;
            case KINGSLIME_STATE.HIT:
                UpdateHit();
                break;
            case KINGSLIME_STATE.JUMP:
                UpdateJump();
                break;




        }

    }
    void UpdateIdle()
    {

        float dist = Vector3.Distance(transform.position, target.position);
        

        if (dist < attackDistance)
        {

            SetState(KINGSLIME_STATE.JUMP);

        }
        else if (dist > attackDistance)
        {
            Vector3 targetPos = target.position;

            if (nav.enabled)
            {

                nav.SetDestination(targetPos);
                nav.isStopped = false;

            }

        }

    }

    void UpdateMove()
    {
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < attackDistance)
        {

            SetState(KINGSLIME_STATE.ATTACK);
            isTracking = false;
            nav.isStopped = true;
        }

        Vector3 targetPos = target.position;

        if (nav.enabled)
        {

            nav.SetDestination(targetPos);
            nav.isStopped = false;

        }



    }

    void UpdateAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            SetState(KINGSLIME_STATE.IDLE);

        }
    }

    void UpdateHit()
    {

    }

    void UpdateJump()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            SetState(KINGSLIME_STATE.IDLE);

        }
    }




    void SetState(KINGSLIME_STATE state)
    {
        switch (state)
        {
            case KINGSLIME_STATE.IDLE:

                if (slimeState != KINGSLIME_STATE.IDLE)
                {
                    slimeState = KINGSLIME_STATE.IDLE;
                    anim.Play("Idle");
                }
                break;
            case KINGSLIME_STATE.MOVE:
                if (slimeState != KINGSLIME_STATE.MOVE)
                {
                    slimeState = KINGSLIME_STATE.MOVE;
                    anim.Play("Walk");

                }
                break;
            case KINGSLIME_STATE.ATTACK:
                if (slimeState != KINGSLIME_STATE.ATTACK)
                {
                    slimeState = KINGSLIME_STATE.ATTACK;
                    anim.Play("Attack");

                }
                break;
            case KINGSLIME_STATE.HIT:
                if (slimeState != KINGSLIME_STATE.HIT)
                {
                    slimeState = KINGSLIME_STATE.HIT;
                    anim.Play("Hit");
                }
                break;
            case KINGSLIME_STATE.JUMP:
                if (slimeState != KINGSLIME_STATE.JUMP)
                {
                    slimeState = KINGSLIME_STATE.JUMP;
                    anim.Play("Jump");

                }
                break;




        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            isTracking = true;
            SetState(KINGSLIME_STATE.MOVE);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {

            isTracking = false;
            SetState(KINGSLIME_STATE.IDLE);

        }
    }

   

}
