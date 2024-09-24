using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Connect.Core;

public class PausePopUp : GameMonobehavior
{
    public void BackMenu()
    {

        Ac.PlaySound(Ac.click);
        /*Um.ToggleImg(false);*/
        GameManager.Instance.GoToMainMenu();
        

        //GameplayManager.Instance.isPause = false;
        //gameObject.SetActive(false);
        
       
    }
    


}
