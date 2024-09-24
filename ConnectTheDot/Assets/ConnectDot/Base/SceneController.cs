using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    //public CanvasGroup transition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        StartCoroutine(DeActive());
    }

    private void OnDisable()
    {
        StopCoroutine(DeActive());
    }

    IEnumerator DeActive()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    /*public void TransitionLoad()
    {
        transition.gameObject.SetActive(true);
        transition.DOFade(0, 0f);
        transition.DOFade(1, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("MainMenu");
            

        });
    }

    public void RTransitionLoad()
    {
        transition.gameObject.SetActive(true);
        transition.DOFade(1, 0.5f);
        transition.DOFade(0, 0.5f).OnComplete(() =>
        {
            transition.gameObject.SetActive(false);
        });
    }*/



}
