using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameMonobehavior : MonoBehaviour
{
    private AudioController _audioController;
    private UIManager _uiManager;
  



    public AudioController Ac
    {
        get
        {
            if (_audioController == null)
            {
                _audioController = AudioController.Instance;
            }
            return _audioController;
        }
    }

    public UIManager Um
    {
        get
        {
            if (_uiManager == null)
            {
                _uiManager = UIManager.Instance;
            }
            return _uiManager;
        }
    }







}
