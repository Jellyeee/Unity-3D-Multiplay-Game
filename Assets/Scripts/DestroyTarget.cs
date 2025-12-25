using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyTarget : MonoBehaviourPun
{
    public GameObject DTarget;

    public float timeBet = 5f;
    private float lastTime;

    void Update()
    {
        if (Time.time >= lastTime + timeBet)
        {
            lastTime = Time.time;
            //DestoryTarget();
           Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.tag == "AI")
            {
                //  DTarget.transform.position = new Vector3(Random.Range(60f, -60f), 9f, Random.Range(-60f, 60f)); 
                DTarget.SetActive(false);
                //PhotonNetwork.Destroy(DTarget);

                //PhotonNetwork.Destroy(this.DTarget);
            }
        }
    }
}
