using Connect.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : GameMonobehavior
{
    //popup
    
    [SerializeField] private GameObject _winPopup;

   

    [SerializeField] private GameObject _NoIneternetPopup;

  

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        
    }

    
    public IEnumerator ShowLosePopup()
    {
        yield return new WaitForSeconds(1f);
        
        Ac.StopPlayMusic();
       
    }

    public IEnumerator ShowWinPopup()
    {

        yield return new WaitForSeconds(1f);
        Ac.PlaySound(Ac.win);
        _winPopup.SetActive(true);
       
    }

    public void ShowNoInternetPopUp()
    {

        
        _NoIneternetPopup.SetActive(true);
        

    }

    /*public void ToggleImg(bool isOn)
    {
        if (isOn)
        {
            
            _pauseImg.gameObject.SetActive(false);
            _playImg.gameObject.SetActive(true);

        }
        else
        {
            _pauseImg.gameObject.SetActive(true);
            _playImg.gameObject.SetActive(false);
        }
    }*/

    /*public void ShowPausePopup()
    {
        //_pauseImg.gameObject.SetActive(false);
        //ToggleImg(true);
        //GameplayManager.Instance.isPause = true;
        Ac.PlaySound(Ac.openPopup);
        _pausePopup.SetActive(true);
    }*/

}
