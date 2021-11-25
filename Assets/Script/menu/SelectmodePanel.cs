using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectmodePanel : MonoBehaviour,View
{
    public GameObject gameObject;
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void StartGame(int dimension)
    {
        SceneManager.LoadSceneAsync(1);
        PlayerPrefs.SetInt(ConstValue.gridNum, dimension);
    }
}
