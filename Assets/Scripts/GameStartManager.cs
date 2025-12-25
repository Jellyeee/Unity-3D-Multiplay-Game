using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviourPun
{
    public GameObject UI;
    public GameObject AI;
    public GameObject ItemSpawner;
    public GameObject GameManger;
    public GameObject Waiting;
    public PhotonView PV;
    public Text wailt_text;
    public GameObject Button;

    int count;
    void Start()
    {
       
        UI.SetActive(false);

        AI.SetActive(false);

        ItemSpawner.SetActive(false);

        GameManger.SetActive(false);

        if(PhotonNetwork.IsMasterClient)
        {
            Button.SetActive(true);
        }
    }

    public void OnButtonClick()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PV.RPC("StartGame", RpcTarget.All);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            count = PhotonNetwork.PlayerList.Length;
        }
    }

    [PunRPC]
    public void StartGame()
    {
        Waiting.SetActive(false);

        UI.SetActive(true);

        AI.SetActive(true);

        ItemSpawner.SetActive(true);

        GameManger.SetActive(true);

    }

    void Update()
    {
        wailt_text.text = PhotonNetwork.PlayerList.Length + " / 6";
    }

}
