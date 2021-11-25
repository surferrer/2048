using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//格子实体类
public class Grid
{
    public GameObject grid { get; set; }
    public Num Num { get; set; }

    int row;
    int column;

    public int Row { 
        get
        {
            return row;
        }
    }
    public int Column { 
        get
        {
            return column;
        }
    }

    public Grid(GameObject grid, int row, int column)
    {
        this.row = row;
        this.column = column;
        this.grid = grid;
    }

    public void SetChildNum(Num num)
    {
        Num = num;
    }

    public void destoryNum()
    {
        Num.destoryNum();
        Num = null;
    }

}
