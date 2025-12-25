using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class AITarget : MonoBehaviourPun
{
    public Transform random;
    public GameObject Target_Object;
    public GameObject save_Target;
    public PhotonView PV;

    public float timeBet = 10f;
    private float lastTime;
    private float respwanTime = 5f;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            random.position = new Vector3(Random.Range(60f, -60f), 9f, Random.Range(-60f, 60f));
            save_Target = Instantiate(Target_Object, random.position, random.rotation);
        }
    }
    
    void Update()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time >= lastTime + respwanTime)
            {
                lastTime = Time.time;

                random.transform.position = new Vector3(Random.Range(60f, -60f), 9f, Random.Range(-60f, 60f));
                save_Target = Instantiate(Target_Object, random.position, random.rotation);
            }
        }


    }

}
