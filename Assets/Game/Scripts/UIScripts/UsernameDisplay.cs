using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UsernameDisplay : MonoBehaviour
{
    public Text usernameText;

    public PhotonView view;

    private void Start()
    {
        if (!view.IsMine)
        {
            //don't want to display username
            gameObject.SetActive(false);
        }
        usernameText.text= view.Owner.NickName;

        //show tem number
    }
}

