using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu: MonoBehaviour
{

    public SelectmodePanel selectmodePannel;
    public SetPanel setPanel;
    public void StartGameOnClick()
    {
        selectmodePannel.Show();
    }

    public void ModifyOnClick()
    {
        setPanel.Show();
    }

    public void ExitOnClick()
    {
        Application.Quit();
    }

}
