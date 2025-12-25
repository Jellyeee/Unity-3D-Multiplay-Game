using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.AI;

public class AIHealth : LivingEntity
{
    public Slider healthSlider;
    public float damage = 1f;
    private Animator playerAnimator;
    public Animator otherAnimator;
    private BOTMove BOTMove;
    public Rigidbody AIrigid;
    public bool IsHit = false;
    public AudioSource AIHitSound;
    public CapsuleCollider CCol;
    private NavMeshAgent nav;

    private void Awake()
    {
        Random_Spawn();
        AIrigid = GetComponent<Rigidbody>();
        otherAnimator = GetComponent<Animator>();
        playerAnimator = GetComponent<Animator>();
        BOTMove = GetComponent<BOTMove>();
        nav = GetComponent<NavMeshAgent>();

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        healthSlider.maxValue = startingHealth;
        healthSlider.enabled = false;
        BOTMove.enabled = true;
        
    }

    public void AIDie()
    {
        nav.speed = 0;
        nav.velocity = Vector3.zero;

        BOTMove.enabled = false;
        CCol.enabled = false;
        //GetComponent<BOTMove>().enabled = false;


        playerAnimator.SetTrigger("Death");
        AIrigid.constraints = RigidbodyConstraints.FreezeAll;
        photonView.RPC("Die", RpcTarget.Others);

    }

    //public void OnDamaged(float damage)
    //{
    //    //setAnimation();
    //    health -= damage;
    //    //healthSlider.value = health;
    //    if (!dead)
    //    {
    //        playerAnimator.SetTrigger("Hit");
    //    }
    //    //if (health <= 0 && !dead)
    //    //{
    //    //    Die();
    //    //    PhotonNetwork.Destroy(gameObject);
    //    //}
    //    //base.OnDamage(damage);

    //}

    //[PunRPC]
    //public void setAnimation()
    //{
    //    playerAnimator.SetTrigger("Hit");
    //}



    private void OnTriggerEnter(Collider other)
    {
        var animator = other.transform.root.GetComponent<Animator>();

        if (other.tag == "Melee")
        {
           // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            //{
                if (!IsHit)
                {
                    IsHit = true;
                    OnDamage(damage);
                    playerAnimator.SetTrigger("Hit");

                    AIHitSound.Play();
                    if (health <= 0)
                    {
                        
                        //photonView.RPC("AIDie",RpcTarget.All);
                        AIDie();

                    }
                    StartCoroutine(HitDelay());

             //   }
            }
        }
    }

    private IEnumerator HitDelay()
    {
        yield return new WaitForSeconds(1.5f);
        IsHit = false;
    }



    private void Random_Spawn()
    {
        if (photonView.IsMine)
        {
            Vector3 randomSpawnPos = Random.insideUnitSphere * 60f;
            randomSpawnPos.y = 8f;

            transform.position = randomSpawnPos;
        }
    }
}

