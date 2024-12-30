using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControllerManager : MonoBehaviourPunCallbacks
{
    private PhotonView view;
    private GameObject controller;


    public int playerTeam;
    private Dictionary<int, int> playerTeams = new Dictionary<int, int>();
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }


    private void Start()
    {
        if (view.IsMine)
        {
            CreateController();
        }
    }


    private void CreateController()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log("Player's Team: " + playerTeam);
        }


        AssignPlayerToSpawnArea(playerTeam);
    }


    private void AssignPlayerToSpawnArea(int team)
    {
        GameObject spawnArea1 = GameObject.Find("SpawnArea1");
        GameObject spawnArea2 = GameObject.Find("SpawnArea2");


        if (spawnArea1 == null || spawnArea2 == null)
        {
            Debug.LogError("Spawn area not found");
            return;
        }


        Transform spawnPoint = null;
        if (team == 1)
        {
            spawnPoint = spawnArea1.transform.GetChild(Random.Range(0, spawnArea1.transform.childCount));
        }
        if (team == 2)
        {
            spawnPoint = spawnArea2.transform.GetChild(Random.Range(0, spawnArea2.transform.childCount));
        }


        if (spawnPoint != null)
        {
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PLayer"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { view.ViewID });
            Debug.Log("instatiate plater at spawn point");
        }
        else
        {
            Debug.Log("no available spawn points for team: " + team);
        }


    }


    void AssignTeamsToAllPlayer()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team"))
            {
                int team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber] = team;
                Debug.Log(player.NickName + " 's Team: " + team);

                AssignPlayerToSpawnArea(team);
            }
        }
    }


    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AssignTeamsToAllPlayer();
    }


}




