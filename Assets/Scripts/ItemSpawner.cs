using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPunCallbacks
{
    public GameObject[] items;
    public Transform playerTransform;

    public float maxDistance = 30f;

    public float timeBetSpawnMax = 7f;
    public float timeBetSpawnMin = 5f;
    private float timeBetSpawn;

    private float lastSpawnTime;


    private void Start()
    {
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time >= lastSpawnTime + timeBetSpawn && playerTransform != null)
            {
                lastSpawnTime = Time.time;
                timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

                Spawn();
            }
        }
    }


    private void Spawn()
    {
        Vector3 spawnPostion = GetRandomPointOnNavMesh(playerTransform.position, maxDistance);

        spawnPostion += Vector3.up * 0.5f;

        GameObject seletedItem = items[Random.Range(0, items.Length)];
        GameObject item = PhotonNetwork.Instantiate(seletedItem.name, spawnPostion, Quaternion.identity);

        DestroyAfter(item, 5);
        //Destroy(item, 20f);
    }

    IEnumerator DestroyAfter(GameObject target,float delay)
    {
        yield return new WaitForSeconds(delay);

        if(target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }

}
