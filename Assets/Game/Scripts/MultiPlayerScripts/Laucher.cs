using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;
public class Laucher : MonoBehaviourPunCallbacks
{
    public static Laucher instance;
    [SerializeField] InputField roomNameInputField;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    public GameObject startButton;

    int nextTeamNumber = 1;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Debug.Log("Connect to Master...");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
    
    
        Debug.Log("Connected to  Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        MenuManager.instance.OpenMenu("UserNameMenu");
        Debug.Log("Joined Lobby");
     
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("LoadingMenu");

    }   
    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text=PhotonNetwork.CurrentRoom.Name;

        Player[]players=PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++) {
            int teamNumber = GetNextTeamNumber();
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i], teamNumber);  
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string errorMessage)
    {
        errorText.text="Room Genaration Unsuccesfull"+errorMessage;
        MenuManager.instance.OpenMenu("ErrorMenu");

    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }


    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); 
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("LoadingMenu");
    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("TitleMenu");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomlist)
    {
        foreach (Transform trans in roomListContent) {
            Destroy(trans.gameObject);
        
        }
       // Debug.Log("Total Room: "+ roomlist.Count);
        for(int i=0; i < roomlist.Count; i++)
        {

            if (roomlist[i].RemovedFromList) {
                continue;    
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomlist[i]);

        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
        int teamNumber = GetNextTeamNumber();
        GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);

        playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer, teamNumber);
    }

    private int GetNextTeamNumber()
    {
        int teamNumber = nextTeamNumber;
        nextTeamNumber = 3-nextTeamNumber;
        return teamNumber;
    }

}
