using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopUpItem : MonoBehaviour
{
    [HideInInspector] public Image img;
    [HideInInspector] public Transform trans;
    SettingPopUp _settingsPopUp;
    Button button;
    int _index;
    private void Awake()
    {
        img = GetComponent<Image>();
        trans = transform;

        _settingsPopUp = trans.parent.GetComponent<SettingPopUp>();
        _index = trans.GetSiblingIndex() - 1;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnItemClick);

    }

    void OnItemClick()
    {
        _settingsPopUp.OnItemClick(_index);
        AudioController.Instance.PlaySound(AudioController.Instance.click);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnItemClick);

    }
}
