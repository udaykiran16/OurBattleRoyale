﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;
    public GameObject battleButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connects to master photon server
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.SetActive(true);

    }

    public void OnBattleButtonClicked()
    {
        Debug.Log("BattleButtonClicked");
        battleButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Tried to join a random game but failed. There must be no open games available");
        CreateRoom();
    }

    void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers =(byte) MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to a room but failed. There must already be a room with the same name");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
