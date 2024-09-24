using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMainMenu : GameMonobehavior
{
    //[SerializeField] private GameObject _settingPopup;
    [SerializeField] private GameObject _NoIneternetPopup;


    public static PopUpMainMenu Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        //_settingPopup.SetActive(false);
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
        Ac.StopPlayMusic();

    }

    public void ShowNoInternetPopUp()
    {


        _NoIneternetPopup.SetActive(true);


    }

    /*public void ShowSettingPopup()
    {
        Ac.PlaySound(Ac.openPopup);

        _settingPopup.SetActive(true);
        

    }*/

    

    

    
}

