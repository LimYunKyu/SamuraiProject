using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    //PlayerState
    enum PLAYER_STATE
    { 
        IDLE,
        ATTACK_IDLE,
        EQUIP,
        UNEQUIP,
        WALK,
        RUN,
        ATTACK,
        HIT
    }


    //Player Moving
    Vector3 moveDir = Vector3.zero;

    [SerializeField]
    float speed = 5f;

    float angle = 0f;

    //Player Attack

    int currentAttackNum = 0;
    [SerializeField]
    float attackDelay = 1f;
    [SerializeField]
    float comboDelay = 0.2f;
    bool isAttacking = false;

    Transform attackEffect;
    float effectStartFrame = 0;
    float effectEndFrame = 0;

    //Animation
    Animator animator;


    //ChildObjcet
    Transform modelChild;
    Transform cameraHolder;


    //STATE

    PLAYER_STATE playerState = PLAYER_STATE.IDLE;


    //Coroutine

    Coroutine coCheckingComboDelay;


    //Key

    bool keyW = false;
    bool keyA = false;
    bool keyS = false;
    bool keyD = false;
    bool keyLShift = false;

    bool keyLButton = false;

   

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        modelChild = animator.transform;
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
            case PLAYER_STATE.EQUIP:
                break;
            case PLAYER_STATE.UNEQUIP:
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

        SyncAnimationTransform();


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

    }


    void UpdateMove()
    {
        if (isAttacking)
        {
            SyncAnimationTransform();
            return;
        }

        if (keyLButton)
        {
            SetState(PLAYER_STATE.ATTACK);
            return;
        
        }

        currentAttackNum = 0;

        moveDir = Vector3.zero;

        if (keyW)
        {
            moveDir += transform.forward;
        }
        if (keyA)
        {
            moveDir -= transform.right;
        }
        if (keyD)
        {
            moveDir += transform.right;
        }
        if (keyS)
        {
            moveDir -= transform.forward;
        }

        moveDir.Normalize();

        transform.position += moveDir * speed * Time.deltaTime;

        if (moveDir == Vector3.zero)
            SetState(PLAYER_STATE.ATTACK_IDLE);
        else
            SyncCameraHolderRotation();

    }
    

    void UpdateRotation()
    {
        if (isAttacking)
            return;


        if (keyW && keyD)
        {
            angle = 45;
        }
        else if (keyS && keyD)
        {
            angle = 135;
        }
        else if (keyS && keyA)
        {
            angle = 225;
        }
        else if (keyW && keyA)
        {
            angle = 315;
        }
        else if (keyW)
        {
            angle = 0;
        }
        else if (keyD)
        {
            angle = 90;
        }
        else if (keyS)
        {
            angle = 180;
        }
        else if (keyA)
        {
            angle = 270;
        }
        else
        {
            return;        
        }

        Vector3 _angle = new Vector3(0, angle, 0);

        modelChild.localRotation = Quaternion.Slerp(modelChild.transform.localRotation, Quaternion.Euler(_angle), 40 * Time.deltaTime);

    }
    void Attack()
    {

        if (!isAttacking)
        {

            if(coCheckingComboDelay != null)
                StopCoroutine(coCheckingComboDelay);
            StartCoroutine(ComboAttack());
            coCheckingComboDelay = StartCoroutine(CheckingCurrentComboDelay());
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
        SetState(PLAYER_STATE.ATTACK_IDLE); 
        isAttacking = false;
        
        
    }

    IEnumerator CheckingCurrentComboDelay()
    { 

        yield return new WaitForSeconds(comboDelay + attackDelay + 0.1f);

        currentAttackNum = 0;
      

    }
    void SyncAnimationTransform()
    {

        //애니메이션 모델이랑 동기화

        Vector3 posOffset = animator.transform.localPosition;
        transform.position += posOffset;
        animator.transform.localPosition = Vector3.zero;

        //Quaternion rotOffset = animator.transform.localRotation;
    }

    void SyncCameraHolderRotation()
    {

        Vector3 _dir = new Vector3(cameraHolder.transform.forward.x, 0, cameraHolder.transform.forward.z);
        _dir.Normalize();
        transform.rotation = Quaternion.LookRotation(_dir);



    }

    void SetState(PLAYER_STATE state)
    { 
        switch(state) 
        {

            case PLAYER_STATE.IDLE:
                playerState = PLAYER_STATE.IDLE;
                animator.CrossFade("IDLE", 0.2f);
                break;
            case PLAYER_STATE.ATTACK_IDLE:
                playerState = PLAYER_STATE.ATTACK_IDLE;
                animator.CrossFade("ATTACK_IDLE0", 0.2f);
                break;
            case PLAYER_STATE.EQUIP:
                playerState = PLAYER_STATE.EQUIP;
                animator.CrossFade("EQUIP", 0.2f);
                break;
            case PLAYER_STATE.UNEQUIP:
                playerState = PLAYER_STATE.UNEQUIP;
                animator.CrossFade("UNEQUIP", 0.2f);
                break;
            case PLAYER_STATE.WALK:
                playerState = PLAYER_STATE.WALK;
                animator.CrossFade("WALK", 0.2f);
                break;
            case PLAYER_STATE.RUN:
                playerState = PLAYER_STATE.RUN;
                animator.CrossFade("RUN", 0.2f);
                break;
            case PLAYER_STATE.ATTACK:
                playerState = PLAYER_STATE.ATTACK; 
                if (coCheckingComboDelay != null)
                    StopCoroutine(coCheckingComboDelay);
                StartCoroutine(ComboAttack());
                coCheckingComboDelay = StartCoroutine(CheckingCurrentComboDelay());
                break;
            case PLAYER_STATE.HIT:
                playerState = PLAYER_STATE.HIT;
                break;
        }

    }

  
}
