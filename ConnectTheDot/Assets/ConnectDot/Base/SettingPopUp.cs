using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingPopUp : GameMonobehavior
{
    [Header("Space between menu item")]
    [SerializeField] Vector2 _spacing;

    [Space]
    [Header("Main button rotation")]
    [SerializeField] float _rotationDuration;
    [SerializeField] Ease _rotationEase;

    [Space]
    [Header("Animation")]
    [SerializeField] float _expandDuration;
    [SerializeField] float _collapseDuration;

    [SerializeField] Ease _expandEase;
    [SerializeField] Ease _collapseEase;

    [Space]
    [Header("Fade")]
    [SerializeField] float _expandFadeDuration;
    [SerializeField] float _collapseFadeDuration;

    [SerializeField] private GameObject uiMuteMusic;
    [SerializeField] private GameObject uiMuteSFX;

    [SerializeField] private GameObject MusicBtn;
    [SerializeField] private GameObject SfxBtn;


    bool _cantClick = false;

    Button _mainButton;
    SettingPopUpItem[] menuItem;
    bool _isExpanded = false;

    Vector2 _mainButtonPosition;
    int _itemCount;




    private void Awake()
    {
        LoadButtonStates();
        
    }

    private void Start()
    {
        _itemCount = transform.childCount - 1;
        menuItem = new SettingPopUpItem[_itemCount];
        for (int i = 0; i < _itemCount; i++)
        {
            menuItem[i] = transform.GetChild(i + 1).GetComponent<SettingPopUpItem>();
        }
        _mainButton = transform.GetChild(0).GetComponent<Button>();

        _mainButton.onClick.AddListener(ToggleMenu);


        _mainButton.transform.SetAsLastSibling();

        _mainButtonPosition = _mainButton.transform.position;

        ResetPosition();
    }



    void ResetPosition()
    {
        for (int i = 0; i < _itemCount; i++)
        {
            menuItem[i].trans.position = _mainButtonPosition;
        }
    }

    void ToggleMenu()
    {
        if (_cantClick) return;
        _cantClick = true; 

        _isExpanded = !_isExpanded;
        if (_isExpanded)
        {
            
            MusicBtn.SetActive(true);
            SfxBtn.SetActive(true);
            for (int i = 0; i < _itemCount; i++)
            {

                menuItem[i].trans.DOMove(_mainButtonPosition + _spacing * (i + 1), _expandDuration).SetEase(_expandEase);
                menuItem[i].img.DOFade(1f, _expandFadeDuration).From(0f);



            }
            DOVirtual.DelayedCall(_expandDuration, () => _cantClick = false);
        }
        else
        {
            
            for (int i = 0; i < _itemCount; i++)
            {
                int index = i;
                menuItem[i].trans.DOMove(_mainButtonPosition, _collapseDuration).SetEase(_collapseEase);
                menuItem[i].img.DOFade(0f, _collapseFadeDuration).OnComplete(() =>
                {
                    if (index == _itemCount - 1)
                    {
                        MusicBtn.SetActive(false);
                        SfxBtn.SetActive(false);
                        _cantClick = false;
                    }
                       


                });

            }
        }

        _mainButton.transform.DORotate(Vector3.forward * 180f, _rotationDuration).From(Vector3.zero).SetEase(_rotationEase);
    }



    public void OnItemClick(int index)
    {
        switch (index)
        {
            case 0:
                uiMuteMusic.gameObject.SetActive(!uiMuteMusic.activeSelf);
                Ac.ToggleMusic();
                SaveButtonStates();
                break;
            case 1:
                uiMuteSFX.gameObject.SetActive(!uiMuteSFX.activeSelf);
                Ac.ToggleSFX();
                SaveButtonStates();
                break;
        }
    }


    private void SaveButtonStates()
    {
        PlayerPrefs.SetInt("MusicOnBtnActive", uiMuteMusic.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt("MusicOffBtnActive", uiMuteMusic.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt("SfxOnBtnActive", uiMuteSFX.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt("SfxOffBtnActive", uiMuteSFX.activeSelf ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadButtonStates()
    {
        uiMuteMusic.SetActive(PlayerPrefs.GetInt("MusicOnBtnActive", 1) == 1);
        uiMuteMusic.SetActive(PlayerPrefs.GetInt("MusicOffBtnActive", 0) == 1);

        uiMuteSFX.SetActive(PlayerPrefs.GetInt("SfxOnBtnActive", 1) == 1);
        uiMuteSFX.SetActive(PlayerPrefs.GetInt("SfxOffBtnActive", 0) == 1);

    }

    private void OnDestroy()
    {
        _mainButton.onClick.RemoveListener(ToggleMenu);

    }
}
