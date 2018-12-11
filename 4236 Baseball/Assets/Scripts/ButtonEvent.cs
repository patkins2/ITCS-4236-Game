using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public void OnClick(int num)
    {
       
        if (num == 1)
        {
            SceneManager.LoadScene("Game");
        }
        
        else if (num == 3)
        {
            Application.Quit();
        }
    }
}