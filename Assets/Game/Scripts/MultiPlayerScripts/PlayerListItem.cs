using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public Text playerUsername;
    public Text teamText;
    Player player;
    int team;
    public void SetUp(Player _player, int _team)
    {
        player = _player;
        team = _team;
        playerUsername.text=_player.NickName;
        teamText.text="Team" +_team;

        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["Team"]= _team;
        _player.SetCustomProperties(customProps);


    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }

    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Destroy(gameObject);
    }

}
