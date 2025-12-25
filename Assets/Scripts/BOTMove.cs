using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class BOTMove : LivingEntity
{

    [SerializeField] protected float walkSpeed = 2f;  // 걷기 속력
    [SerializeField] protected float runSpeed = 5f;  // 달리기 속력
    [SerializeField] protected float turningSpeed = 10f;  // 회전 속력
    private float applySpeed;

    private Vector3 direction;  // 방향
    private Vector3 destination;

    // 상태 변수
    private bool isAction;  // 행동 중인지 아닌지 판별
    private bool isWalking; // 걷는지, 안 걷는지 판별
    private bool isRunning; // 달리는지 판별
    private bool isWaiting = false;

    private float Timer = 0f;
    private float walkTime = 4f;  // 걷기 시간
    private float waitTime = 5f;  // 대기 시간
    private float runTime = 2f;  // 뛰기 시간
    private float currentTime;
    private NavMeshAgent nav;
    public Transform Target;

    // 필요한 컴포넌트
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected CapsuleCollider CapCol;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        nav.isStopped = true;

        currentTime = 1f;   // 대기 시작
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        if (!dead)
        {

            StartCoroutine(Move());
            //Rotation();
            ElapseTime();
            SetAnimation();

        }
        
    }

    private bool hasTarget
    {
        get {
            if (Target != null)
            {
                return true;
            }
            return false;

        }
    }

    private IEnumerator Move()
    {
        if (!isWaiting)
        {
            if(hasTarget)
            {
                SetSpeed();
                nav.isStopped = false;

                nav.SetDestination(Target.transform.position);

            }
            else
            {
                nav.isStopped = true;

            }
            //nav.SetDestination(transform.position + destination * walkSpeed * Time.deltaTime);
            //rigid.MovePosition(transform.position + transform.forward * applySpeed * Time.deltaTime);
        }
        else
        {
            nav.isStopped = true;
        }
        yield return new WaitForSeconds(0.25f);
    }    

    private void ElapseTime()
    { 
        Timer += Time.deltaTime;
        if (Timer >= currentTime)
        {
            Timer = 0;
            RandomAction();

        }
    }

    private void SetSpeed()
    {
        if(isWalking)
        {
            nav.speed = walkSpeed;
        }
        else if(isRunning)
        {
            nav.speed = runSpeed;
        }
        else if(isWaiting)
        {
            nav.speed = 0;
            nav.velocity = Vector3.zero;
        }
    }

    private void RandomAction()
    {
        int setRandom = Random.Range(0, 3);

        if (setRandom == 0)
            Wait();
        else if (setRandom == 1)
            TryWalk();
        else if (setRandom == 2)
            TryRun();
    }

    private void TryWalk()  // 걷기
    {
        currentTime = walkTime;

        isWalking = true;
        isRunning = false;
        isWaiting = false;

        //Debug.Log("걷기");
    }

    private void TryRun()
    {
        currentTime = runTime;

        isWalking = false;
        isRunning = true;
        isWaiting = false;


        //Debug.Log("뛰기");
    }

    private void Wait()
    {
        currentTime = waitTime;

        isWalking = false;
        isRunning = false;
        isWaiting = true;

       //Debug.Log("대기");
    }

    private void SetAnimation()
    {
        anim.SetBool("Walk", isWalking);
        anim.SetBool("Run", isRunning);
    }


}