using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UnityEngine.UI.Text를 사용하기 위해 명시
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Diagnostics;

public class UIManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }

    private static UIManager m_instance;

    public UnityEngine.UI.Text[] scoreText; // 명시적으로 UnityEngine.UI.Text 사용
    public UnityEngine.UI.Text[] PlayerNameText; // 명시적으로 UnityEngine.UI.Text 사용

    public UnityEngine.UI.Text TimerText; // 명시적으로 UnityEngine.UI.Text 사용

    // 플레이어 상태 관리
    private float[] PlayerHealthSet = new float[6];
    private bool[] isPlayerDeath = new bool[6];
    public UnityEngine.UI.Text WinnerTextUI; // 명시적으로 UnityEngine.UI.Text 사용
    public GameObject WinnerTextObject;
    public GameObject DefeatTextObject;

    private float Sec = 0;

    public UnityEngine.UI.Text OnlineText; // 명시적으로 UnityEngine.UI.Text 사용
    public UnityEngine.UI.Text SurvText; // 명시적으로 UnityEngine.UI.Text 사용
    public GameObject gameoverUI;
    public bool LastPlayerWin;

    public int PlayerCount = 0;

    private int[] score = new int[6];
    private int increase_score_value = 10;

    public void Start()
    {
        LastPlayerWin = false;
        PV = GetComponent<PhotonView>();
        for (int i = 0; i < 6; i++)
        {
            isPlayerDeath[i] = false;
        }

        // 플레이어 관련 변수를 초기화
        PlayerCount = PhotonNetwork.PlayerList.Length; // 플레이어 수를 초기화
        PlayerHealth.playerdeadcount = 0; // 사망한 플레이어 수 초기화

        SurvText.text = "생존: " + (PlayerCount - PlayerHealth.playerdeadcount);
    }

    public void AddMyScore(PlayerHealth ph, int Actor_Number)
    {
        ph.Myscore += increase_score_value;
        photonView.RPC("UpdateScore", RpcTarget.All, ph.Myscore, Actor_Number);
    }

    [PunRPC]
    public void UpdateScore(int newScore, int Player_Number)
    {
        score[Player_Number - 1] = newScore;
    }

    public void PlayerHit2(float NowHealth, int ActorNumber)
    {
        PV.RPC("PlayerHit", RpcTarget.All, NowHealth, ActorNumber);
    }

    // 플레이어 체력 관리
    [PunRPC]
    public void PlayerHit(float NowHealth, int Actor_Number)
    {
        if (isPlayerDeath[Actor_Number - 1] == false)
        {
            UnityEngine.Debug.Log(Actor_Number - 1); 
            PlayerHealthSet[Actor_Number - 1] = NowHealth;

            if (PlayerHealthSet[Actor_Number - 1] <= 0)
            {
                isPlayerDeath[Actor_Number - 1] = true;
                PlayerCount--;

                if (PhotonNetwork.IsMasterClient)
                {
                    OnPlayerDeath();
                }
                else
                {
                    SurvText.text = "생존: " + (PlayerCount - PlayerHealth.playerdeadcount);

                    if (PlayerCount - PlayerHealth.playerdeadcount < 0)
                    {
                        SurvText.text = "생존: 0";
                    }
                }

                if (photonView != null)
                {
                    photonView.RPC("DeadCheck", RpcTarget.All);
                }
                else
                {
                    UnityEngine.Debug.LogError("PhotonView is missing on the object attempting to call DeadCheck RPC");
                }
            }
        }
    }

    [PunRPC]
    public void DeadCheck()
    {
        if (PlayerCount <= 1)
        {
            // 승리자를 찾아서 다른 모든 클라이언트에 알림
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (isPlayerDeath[i] == false)
                {
                    // 승리자를 모든 클라이언트에서 처리하도록 RPC 호출
                    PV.RPC("ShowWinner", RpcTarget.All, PhotonNetwork.PlayerList[i].NickName);
                    break; // 승리자를 찾으면 루프를 종료
                }
            }
        }
    }

    [PunRPC]
    public void UpdateSurvivalText()
    {
        // 모든 플레이어의 생존 상태를 업데이트
        int aliveCount = PhotonNetwork.PlayerList.Length - PlayerHealth.playerdeadcount;
        SurvText.text = "생존: " + aliveCount;

        // 생존자가 0명 미만으로 표시되지 않도록 처리
        if (aliveCount < 0)
        {
            SurvText.text = "생존: 0";
        }

    }

    // 플레이어가 사망할 때마다 호출할 메서드
    [PunRPC]
    public void UpdatePlayerCount()
    {
        UpdateSurvivalText();
    }

    public void OnPlayerDeath()
    {
        // 플레이어 사망 시 생존자 수 업데이트를 모든 클라이언트에 반영
        photonView.RPC("UpdateSurvivalText", RpcTarget.MasterClient);
    }


    [PunRPC]
    public void ShowWinner(string winnerName)
    {
        UnityEngine.Debug.Log("Game Over");
        WinnerTextUI.text = winnerName + " Win!";
        DefeatTextObject.SetActive(false);
        WinnerTextObject.SetActive(true);
    }

    // 점수와 플레이어 이름 표시
    private void ManageScoreText()
    {
        for (int i = 0; i < 6; i++)
        {
            scoreText[i].text = score[i].ToString();
        }
    }

    public void SetActivateGameoverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void Update()
    {
        Timer();

        // 플레이어 수에 맞춰서 반복
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!PlayerNameText[i].text.Equals("---"))
            {
                if (isPlayerDeath[i] == false) // 플레이어가 살아있는 경우 이름을 업데이트
                {
                    PlayerNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
                }
                else if (isPlayerDeath[i] == true)  // 플레이어가 죽었을 때 텍스트 색상 변경
                {
                    PlayerNameText[i].text = "<color=#8B0000>" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                }
            }
            
        }
    }

 

    public void OnButtonClick()
    {
        PlayerCount = PhotonNetwork.PlayerList.Length;
        WinnerTextObject.SetActive(false);
    }

    private void Timer()
    {
        Sec += Time.deltaTime;
        TimerText.text = "" + (int)Sec;
    }

    public PhotonView PV;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UnityEngine.Debug.Log(newPlayer.ActorNumber - 1); // 명시적으로 UnityEngine.Debug 사용
        PlayerNameText[newPlayer.ActorNumber - 1].text = newPlayer.NickName;
        PlayerHealthSet[newPlayer.ActorNumber - 1] = 2;
        isPlayerDeath[newPlayer.ActorNumber - 1] = false;
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        PlayerNameText[other.ActorNumber - 1].text = "---";
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PlayerCount);
            for (int i = 0; i < 6; i++)
            {
                stream.SendNext(isPlayerDeath[i]);
            }
        }
        else
        {
            PlayerCount = (int)stream.ReceiveNext();
            for (int i = 0; i < 6; i++)
            {
                isPlayerDeath[i] = (bool)stream.ReceiveNext();
            }
        }
    }
}
