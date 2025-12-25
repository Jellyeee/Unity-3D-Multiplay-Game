using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager m_instance;
    public PhotonView PV;

    public GameObject playerPrefab;

    public static GameManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    public bool isGameover { get; private set; }


    private void Awake()
    {
        
        if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Screen.SetResolution(1280, 720, false);
        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        randomSpawnPos.y = 0f;
        PhotonNetwork.Instantiate("Player", playerPrefab.transform.position, Quaternion.identity);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

}
