using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//数字实体类
public class Num
{
    private Text numObj;

    private NumChange changer;

    public Grid ParentGrid
    {
        get;
        set;
    }

    public Num(Text numObj, int num)
    {
        this.numObj = numObj;
        numObj.text = num.ToString();
        changer = numObj.GetComponent<NumChange>();
    }

    public void SetNumber(int num)
    {
        numObj.text = num.ToString();
    }

    public int GetNumber()
    {
        return int.Parse(numObj.text);
    }

    public void SetGrid(Grid grid)
    {
        Vector2 zero = new Vector2(0, 0);
        ParentGrid = grid;
        numObj.transform.SetParent(grid.grid.transform);
        numObj.rectTransform.offsetMax=zero;
        numObj.rectTransform.offsetMin=zero;
        //numObj.transform.Translate(zero, Space.Self);
        ParentGrid.SetChildNum(this);
    }

    public bool NumIsEqualsTo(Num num)
    {
        int thisNum =
           int.Parse(this.numObj.text);
        int target =
           int.Parse(num.numObj.text);
        return thisNum == target;
    }

    public void destoryNum()
    {
        Text.Destroy(numObj.gameObject);
        numObj = null;
    }

    public void AppearPlay()
    {
        changer.StartPlay();
    }

    public void MergePlay()
    {
        changer.StartMergePlay();
    }
}
