using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;


public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    public Rigidbody rb;
    public Transform tr;
    public AudioSource walksound;
    public BoxCollider BCol;

    private SkinnedMeshRenderer meshRenderer;
    private Color originColor;

    public float m_moveSpeed = 2;
    public float m_turnSpeed = 100;
    public Animator m_animator;

    //공격 딜레이
    bool fDown;
    public float Attack_Delay = 0.4f;
    bool isAttackReady = true;

    //움직임
    private float m_currentV = 0;
    private float m_currnetH = 0;
    private readonly float m_interpolation = 10;
    private readonly float m_backwardRunScale = 0.66f;

    //이동 관련
    bool jDown;
    bool isJump;
    private float Attack_Count = 0;


    private void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1.5f, 0);
        tr = GetComponent<Transform>();
    }
   

    void Attack()
    {
        Attack_Delay += Time.deltaTime;
        isAttackReady = 1.5 < Attack_Delay;
        if (fDown && isAttackReady)
        {
            BCol.enabled = true;
            StartCoroutine("Stop_Turn");

            Attack_Count++;

            if (Attack_Count == 1)
            {
                m_animator.SetTrigger("doSwing1");
            }

            if (Attack_Count == 2)
            {

                m_animator.SetTrigger("doSwing2");
                Attack_Count = 0;
            }

            Attack_Delay = 0;
            StartCoroutine(BoxDelay());
        }
   
    }



    IEnumerator Stop_Turn()
    {
        m_turnSpeed = 0;

        yield return new WaitForSeconds(1f);

        m_turnSpeed = 100;

    }

    IEnumerator BoxDelay()
    {
        yield return new WaitForSeconds(1.5f);
        BCol.enabled = false;

    }

    void Move()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currnetH = Mathf.Lerp(m_currentV, h, Time.deltaTime * m_interpolation);

        tr.Translate(Vector3.forward * v * m_moveSpeed * Time.deltaTime);
        tr.Rotate(Vector3.up * h * m_turnSpeed * Time.deltaTime);

        if (v < 0)
        {
            v *= m_backwardRunScale;
        }



        if (Input.GetKey(KeyCode.LeftShift))
        {
            walksound.Play();
            if (Input.GetKey("w") || Input.GetKey("s"))
            {
                m_animator.SetBool("IsRun", true);
                m_moveSpeed = 5;
                if (!isAttackReady)
                {
                    m_moveSpeed = 0;
                }

            }
            else if(Input.GetKeyUp("w") || Input.GetKeyUp("s"))
            {
                m_animator.SetBool("IsRun", false);
                m_moveSpeed = 0;

            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            walksound.Play();

            m_animator.SetBool("IsRun", false);
            m_moveSpeed = 2;
            if (!isAttackReady)
            {
                m_moveSpeed = 0;
            }

        }
        else
        {
            
            m_animator.SetFloat("Move", v);
            
            m_animator.SetBool("IsRun", false);
            m_moveSpeed = 2;
            if (!isAttackReady)
            {
                m_moveSpeed = 0;
            }

        }




    }

    void Jump()
    {
        if(jDown && !isJump && isAttackReady)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }

    void GetInput()
    {
        fDown = Input.GetButtonUp("Fire1");
        jDown = Input.GetButtonDown("Jump");
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (PV.IsMine)
        {
            GetInput();
            Move();
            Attack();
            Jump();
        }
    }
    //public Material[] playerMt;
    //private int idxMt = -1;

    //[PunRPC]
    //private void SetMt(int idx)
    //{
    //    UIManager.instance.GetScore(PV.ViewID / 1000);
    //    GetComponent<Renderer>().material = playerMt[idx];
    //}


    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if(PV.IsMine && idxMt != -1)
    //    {
    //        PV.RPC(nameof(SetMt), newPlayer, idxMt);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{

    //    //별이랑 충돌시 점수
    //    if (other.tag == "Score")
    //    {
    //        UIManager.instance.GetScore(PV.ViewID / 1000);

    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            PhotonNetwork.Destroy(other.gameObject);
    //        }
    //    }


    //}

}
