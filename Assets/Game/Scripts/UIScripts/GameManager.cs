using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public bool IsMenuOpened = false;
    public GameObject menuUI;

    

    public GameObject scoreUi;
    

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) && IsMenuOpened == false)
        {
       
            scoreUi.SetActive(false);
           
            menuUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            IsMenuOpened = true;
            AudioListener.pause=true;
        }
        else if(Input.GetKeyUp(KeyCode.Escape) && IsMenuOpened == true)
        {
            
            scoreUi.SetActive(true);
          
            menuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            IsMenuOpened = false;
            AudioListener.pause = false;
        } 

    }
    public void LeaveGame()
    {
        Debug.Log("GameLeaved");
        Application.Quit();
    }
}
