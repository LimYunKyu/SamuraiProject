using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{


    //PlayerState
    public enum PLAYER_STATE
    { 
        IDLE,
        ATTACK_IDLE,     
        WALK,
        RUN,
        ATTACK,
        HIT
    }


    //Player Moving
    Vector3 moveDir = Vector3.zero;

    [SerializeField]
    float speed = 2f;

    float angle = 0f;

    float height = 0f;

    [SerializeField]
    float downDistance = 0.5f;

    [SerializeField]
    float forwardDistance = 0.5f;

    [SerializeField]
    float rayBoxSize = 1f;

    bool isMoving = true;

    float currentYangle = 0f;
    //Player Attack

    int currentAttackNum = 0;
    [SerializeField]
    float attackDelay = 1f;
    [SerializeField]
    float comboDelay = 0.2f;
    bool isAttacking = false;


    Transform attackTarget = null;

    //Animation
    Animator animator;


    //ChildObjcet
    //Transform modelChild;
    Transform cameraHolder;

    //Collider
    SphereCollider sphereCollider;


    //STATE

    public PLAYER_STATE playerState = PLAYER_STATE.IDLE;


    //Coroutine

    Coroutine coCheckingComboDelay;
    Coroutine coBattleTimeCheck;


    //Key Press

    bool keyW = false;
    bool keyA = false;
    bool keyS = false;
    bool keyD = false;
    bool keyLShift = false;

    bool keyLButton = false;


    //Key Down
    bool dKeyW = false;
    // Battle
    float battleTime = 3f;
    bool isBattle = false;
    void Start()
    {
        animator = GetComponent<Animator>();
       sphereCollider = GetComponent<SphereCollider>();
        cameraHolder = GetComponentInChildren<CameraHolderController>().transform;


    }


    void Update()
    {

        InputChecking();


        switch (playerState)
        {
            case PLAYER_STATE.IDLE:
                UpdateIdle();
                break;
            case PLAYER_STATE.ATTACK_IDLE:
                UpdateAttackIdle();
                break;
            case PLAYER_STATE.WALK:
            case PLAYER_STATE.RUN:
                UpdateMove();
                UpdateRotation();
                break;
            case PLAYER_STATE.ATTACK:
                Attack();
                break;
            case PLAYER_STATE.HIT:
                break;

        }

        Vector3 pos;
        if (BlockDownChecking(out pos))
        {
            //transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
            //Debug.Log(pos);

        }

         transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }


    void LateUpdate()
    {
        Vector3 fowardPos;
        if (!BlockForwardChecking(out fowardPos))
        {

            isMoving = true;
        }
        else
        {

            isMoving = false;
        }
       

        Vector3 pos;
        if (BlockDownChecking(out pos))
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }

       
    }
    void InputChecking()
    { 
        keyW = Input.GetKey(KeyCode.W);
        keyA = Input.GetKey(KeyCode.A);
        keyS = Input.GetKey(KeyCode.S);
        keyD = Input.GetKey(KeyCode.D);
        keyLShift = Input.GetKey(KeyCode.LeftShift);

        keyLButton = Input.GetMouseButtonDown(0);
    }
    void UpdateIdle()
    {

        if (keyW || keyA || keyS || keyD)
        {
            if (keyLShift)
            {
                SetState(PLAYER_STATE.RUN);
                
            }
            else
            {
                SetState(PLAYER_STATE.WALK);
              
            }
        }

        if (keyLButton)
        {

            SetState(PLAYER_STATE.ATTACK);
        }


    }
 
    void UpdateAttackIdle()
    {
        if (keyW || keyA || keyS || keyD)
        {
            if (keyLShift)
            {
                
                SetState(PLAYER_STATE.RUN);
            }
            else
            {
                
                SetState(PLAYER_STATE.WALK);
            }
        }
        if (keyLButton)
        {

            SetState(PLAYER_STATE.ATTACK);
        }

        if (!isBattle)
        {

            SetState(PLAYER_STATE.IDLE);


        }

    }


    void UpdateMove()
    {


        moveDir = Vector3.zero;
        if (keyA || keyS || keyD || keyW)
        {
            SyncCameraHolderRotation();
            if (keyLShift && playerState != PLAYER_STATE.RUN)
            {
                
                SetState(PLAYER_STATE.RUN);
            }
            else if (!keyLShift && playerState != PLAYER_STATE.WALK)
            {
               
                SetState(PLAYER_STATE.WALK);
            }

            moveDir = transform.forward;
        }

        if (keyLButton)
        {
            SetState(PLAYER_STATE.ATTACK);
            return;
        
        }

        currentAttackNum = 0;

        moveDir.Normalize();

        
        if(isMoving)
        transform.position += moveDir * speed * Time.deltaTime;

        if (moveDir == Vector3.zero)
        {
            if(!isBattle)
                SetState(PLAYER_STATE.IDLE);
            else
                SetState(PLAYER_STATE.ATTACK_IDLE);
        }
      
          
    }
    

    void UpdateRotation()
    {
        if (isAttacking)
            return;


        if (keyW && keyD)
        {
            angle = currentYangle + 45;
        }
        else if (keyS && keyD)
        {
            angle = currentYangle + 135;
        }
        else if (keyS && keyA)
        {
            angle = currentYangle + 225;
        }
        else if (keyW && keyA)
        {
            angle = currentYangle + 315;
        }
        else if (keyW)
        {
            angle = currentYangle + 0;
        }
        else if (keyD)
        {
            angle = currentYangle + 90;
        }
        else if (keyS)
        {
            angle = currentYangle + 180;
        }
        else if (keyA)
        {
            angle = currentYangle + 270;
        }
        else
        {
            return;        
        }

        Vector3 _angle = new Vector3(0, angle, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_angle), 40 * Time.deltaTime);

        
    }
    void Attack()
    {

        if (!isAttacking)
        {
            RotationToEnemy();
            if (coCheckingComboDelay != null)
                StopCoroutine(coCheckingComboDelay);
            if (coBattleTimeCheck != null)
                StopCoroutine(coBattleTimeCheck);


            StartCoroutine(ComboAttack());

            coCheckingComboDelay = StartCoroutine(CheckingCurrentComboDelay());


            
        }

        
    }


    void SyncCameraHolderRotation()
    {

        Vector3 _dir = new Vector3(cameraHolder.transform.forward.x, 0, cameraHolder.transform.forward.z);
        _dir.Normalize();
        //transform.rotation = Quaternion.LookRotation(_dir);
        currentYangle = cameraHolder.rotation.eulerAngles.y;
        //cameraHolder.GetComponent<CameraHolderController>()._angle.y = 0;


    }

    void RotationToEnemy()
    {
        if (attackTarget) {

            Vector3 targetDirection = attackTarget.position - transform.position;
            targetDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(targetDirection);
        
        }


    }


    private void OnTriggerStay(Collider other)
    {

        float distance = 0;
        if (other.tag == "Monster")
        { 
            distance = Vector3.Distance(other.transform.position, transform.position);

            if (attackTarget)
            {
                float currentDist = Vector3.Distance(attackTarget.transform.position, transform.position);
                if (distance < currentDist)
                {
                    attackTarget = other.transform;
                }
            }
            else
            {

                attackTarget = other.transform;
            }
        
        
        }
    }

    void SetState(PLAYER_STATE state)
    {
        int a;
        switch (state) 
        {

            case PLAYER_STATE.IDLE:
                playerState = PLAYER_STATE.IDLE;
                animator.CrossFade("IDLE", 0.2f);
                break;
            case PLAYER_STATE.ATTACK_IDLE:
                playerState = PLAYER_STATE.ATTACK_IDLE;
                animator.CrossFade("ATTACK_IDLE0", 0.2f);
                coBattleTimeCheck = StartCoroutine(CheckBattleTime());
                break;
            case PLAYER_STATE.WALK:
                speed = 2f;
                playerState = PLAYER_STATE.WALK;
                animator.CrossFade("WALK", 0.1f);
                break;
            case PLAYER_STATE.RUN:
                speed = 7f;
                playerState = PLAYER_STATE.RUN;
                animator.CrossFade("RUN", 0.1f);
                break;
            case PLAYER_STATE.ATTACK:
                playerState = PLAYER_STATE.ATTACK;

                //if (coCheckingComboDelay != null)
                //    StopCoroutine(coCheckingComboDelay);
                //StartCoroutine(ComboAttack());
                //coCheckingComboDelay = StartCoroutine(CheckingCurrentComboDelay());

                //if(coBattleTimeCheck != null)
                //StopCoroutine(coBattleTimeCheck);
                Attack();
                break;
            case PLAYER_STATE.HIT:
                playerState = PLAYER_STATE.HIT;
                break;
        }

    }

    bool BlockDownChecking(out Vector3 hitPos)
    {
      
        Vector3 rayPos = transform.position;
        rayPos.y += 0.5f;

        LayerMask mask = 1 << LayerMask.NameToLayer("Floor");
        RaycastHit hit;
        Debug.DrawRay(rayPos, Vector3.down, Color.red);
        if (Physics.Raycast(rayPos,Vector3.down,out hit, downDistance, mask))
        {

            hitPos = hit.point;
            //Debug.Log(hit.transform.name);
            height = hit.point.y;
            return true;
        
        }
        hitPos = transform.position;
        return false;
    }


    bool BlockForwardChecking(out Vector3 hitPos)
    {
        
        Vector3 rayPos = transform.position;
        rayPos.y += 0.5f;

        LayerMask mask = 1 << LayerMask.NameToLayer("Floor");
        RaycastHit hit;
        Vector3 size = new Vector3(rayBoxSize, rayBoxSize,rayBoxSize); // 박스의 크기
        Debug.DrawRay(rayPos, transform.forward, Color.red);
        if (Physics.BoxCast(rayPos, size/2f,transform.forward, out hit, Quaternion.identity,forwardDistance,  mask))
        {
            Debug.Log("앞부디침");
            hitPos = hit.point;
            return true;
            
        }
        else
        {
            Debug.Log("앞 안부디침");
        
        }

        hitPos = hit.point;
        return false; 
    }
    ///코루틴
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
          
        }
        currentAttackNum++;
        currentAttackNum = currentAttackNum % 3;


        yield return new WaitForSeconds(attackDelay);

        animator.CrossFade("ATTACK_IDLE0", 0.1f);
        yield return new WaitForSeconds(0.1f);
        SetState(PLAYER_STATE.ATTACK_IDLE);
        isAttacking = false;


    }
    IEnumerator CheckingCurrentComboDelay()
    {

        yield return new WaitForSeconds(comboDelay + attackDelay + 0.1f);

        currentAttackNum = 0;


    }

    IEnumerator CheckBattleTime()
    {
        isBattle = true;
        yield return new WaitForSeconds(battleTime);
        isBattle = false;

    }

   
}
