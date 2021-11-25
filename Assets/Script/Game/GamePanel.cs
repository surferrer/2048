using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{

    public Text score; //分数版
    public Text best; //最佳版

    private int NowScore;
    private int bestScore;

    private Vector3 PointDownPos;
    private Vector3 PointUpPos;

    //x存储行标，y存储列标
    private HashSet<Vector2Int> emptyGrid;
    Dictionary<int, int> gridTrans = new Dictionary<int, int>() { { 4, 215 }, { 5, 180 }, { 6, 140 } };


    public GameObject gridUnitObj;

    public Text numUnit;

    public Transform gameCorePanelTrans;

    private int rowNum;

    private int columnNum;

    private Grid[][] grids;

    int RandomCol;
    int RandomRow;

    private const float POSIBILITY_4 = 0.1f;


    private bool moved = false;

    

    public void onRestartClick()
    {
        //点击重来
    }

    public void onLastStepClick()
    {
        //上一步
    }

    public void onExitClick()
    {
        //退出到上一个界面
        SceneManager.LoadSceneAsync(0);
    }

    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt(ConstValue.bestScore);
        NowScore = 0;
        best.text = bestScore.ToString();
        score.text = NowScore.ToString();
    }



    private void Start()
    {
        InitialEmptyGrid();
        InitGrid();

    }

    //初始化空grid字典
    private void InitialEmptyGrid()
    {
        int gridNum = PlayerPrefs.GetInt(ConstValue.gridNum, 4);
        rowNum = gridNum;
        columnNum = gridNum;
        emptyGrid = new HashSet<Vector2Int>();
        for (int i = 0; i < rowNum; i++)
        {
            for (int j = 0; j < columnNum; j++)
            {
                emptyGrid.Add(new Vector2Int(i, j));
            }
        }

    }

    private void InitGrid()
    {
        //初始化grid
        GridLayoutGroup gridLayoutGroup = gameCorePanelTrans.GetComponent<GridLayoutGroup>();
        int gridNum = rowNum;
        grids = new Grid[gridNum][];
        gridLayoutGroup.constraintCount = gridNum;
        gridLayoutGroup.cellSize = new Vector2(gridTrans[gridNum], gridTrans[gridNum]);
        for (int i = 0; i < gridNum; i++)
        {
            grids[i] = new Grid[gridNum];
            for (int j = 0; j < gridNum; j++)
            {
                grids[i][j] = new Grid(GameObject.Instantiate(gridUnitObj, gameCorePanelTrans), i, j);
            }
        }

        Num newNum = GetNewNum(2);
        SetRandomGrid(newNum);
    }

    private Num GetNewNum(int num)
    {
        return new Num(GameObject.Instantiate(numUnit), num);
    }

    private void SetRandomGrid(Num num)
    {

        /*int RandomCol = Random.Range(0, rowNum - 1);
        int RandomRow = Random.Range(0, rowNum - 1);*/
        RandomCol = Random.Range(0, rowNum - 1);
        RandomRow = Random.Range(0, rowNum - 1);
        SetNum(RandomRow, RandomCol, num);
        num.AppearPlay();
        emptyGrid.Remove(new Vector2Int(RandomRow, RandomCol));
    }

    private void SetNum(int row, int col, Num num)
    {
        Grid grid = grids[row][col];
        grid.Num = num;
        num.SetGrid(grid);
    }

    public void SetPointDownPos()
    {
        PointDownPos = Input.mousePosition;
        Debug.Log("PointDownPos" + PointDownPos);
    }

    public void SetPointUpPos()
    {
        PointUpPos = Input.mousePosition;
        Debug.Log("PointUpPos" + PointUpPos);
        MoveNum(CalMove());
    }


    private void doMoved()
    {

    }


    private void refreshOne()
    {
        if (emptyGrid.Count == 0)
            return;
        int newNum = 2;
        //TODO 刷一个2或4
        float p = Random.Range(0f, 1f);
        if (p < 0.1)
            newNum = 4;
        int gridVectorIndex = Random.Range(0, emptyGrid.Count-1);
        Vector2Int gridPoint = getFromEmptyGridByIndex(gridVectorIndex);
        emptyGrid.Remove(gridPoint);
        Num num = GetNewNum(newNum);
        moveNumToPos(num, gridPoint.x, gridPoint.y);
        num.AppearPlay();
    }

    private Vector2Int getFromEmptyGridByIndex(int index)
    {
        if (index >= 0)
        {
            using (IEnumerator<Vector2Int> enumerator = emptyGrid.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (index == 0)
                    {
                        return enumerator.Current;
                    }
                    index--;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }


    private void MoveNum(MOVEMENT move)
    {
        moved = false;
        switch (move)
        {
            case MOVEMENT.UP:

                for (int row = 1; row < rowNum; row++)
                {
                    for (int col = 0; col < columnNum; col++)
                    {
                        Grid nowGrid = grids[row][col];
                        if (nowGrid.Num == null)
                            continue;
                        int i = row - 1;
                        //向上寻找第一个不为空的格子
                        for (; i >= 0 && grids[i][col].Num == null; i--) ;
                        //找到第一个非空格子
                        if (i >= 0 && grids[i][col].Num != null)
                        {
                            //看看数字是否相同，是则合并
                            if (grids[i][col].Num.NumIsEqualsTo(nowGrid.Num))
                            {
                                grids[i][col].Num.SetNumber(grids[i][col].Num.GetNumber() * 2);
                                grids[i][col].Num.MergePlay();
                                NowScore += grids[i][col].Num.GetNumber();
                                score.text = NowScore.ToString();
                                if (NowScore > bestScore)
                                {
                                    bestScore = NowScore;
                                    best.text = bestScore.ToString();
                                    PlayerPrefs.SetInt(ConstValue.bestScore, bestScore);
                                }
                                emptyGrid.Add(new Vector2Int(nowGrid.Row, nowGrid.Column));
                                nowGrid.destoryNum();
                                moved = true;
                                //合并完之后，继续向上找第一个非空的格子，然后移动，并且不再合并；
                                int j = i - 1;
                                for (; j >= 0 && grids[j][col].Num == null; j--) ;
                                emptyGrid.Add(new Vector2Int(i, col));
                                moveNumToPos(grids[i][col].Num, j + 1, col);
                                emptyGrid.Remove(new Vector2Int(j+1, col));
                                

                            }
                            else
                            {
                                //否则先判断该行的下一行是否就是本行，若是则不移动，若不是则移动并置值
                                if (i + 1 != row)
                                {
                                    //不相等则移动
                                    emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                                    moveNumToPos(nowGrid.Num, i + 1, col);
                                    moved = true;
                                    emptyGrid.Remove(new Vector2Int(i + 1, col));
                                }
                            }

                        }
                        else
                        {
                            emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                            moveNumToPos(nowGrid.Num, i + 1, col);
                            emptyGrid.Remove(new Vector2Int(i + 1, col));
                            moved = true;
                            
                        }
                    }
                }
                break;
            case MOVEMENT.DOWN:

                for (int row = rowNum - 2; row >= 0; row--)
                {
                    for (int col = 0; col < columnNum; col++)
                    {
                        
                        Grid nowGrid = grids[row][col];
                        if (nowGrid.Num == null)
                            continue;
                        int i = row + 1;
                        //向下寻找第一个不为空的格子
                        for (; i <= rowNum - 1 && grids[i][col].Num == null; i++) ;
                        //找到第一个非空格子
                        if (i <= rowNum - 1 && grids[i][col].Num != null)
                        {
                            //看看数字是否相同，是则合并
                            if (grids[i][col].Num.NumIsEqualsTo(nowGrid.Num))
                            {
                                grids[i][col].Num.SetNumber(grids[i][col].Num.GetNumber() * 2);
                                grids[i][col].Num.MergePlay();
                                NowScore += grids[i][col].Num.GetNumber();
                                score.text = NowScore.ToString();
                                if (NowScore > bestScore)
                                {
                                    bestScore = NowScore;
                                    best.text = bestScore.ToString();
                                    PlayerPrefs.SetInt(ConstValue.bestScore, bestScore);
                                }
                                nowGrid.destoryNum();
                                emptyGrid.Add(new Vector2Int(nowGrid.Row, nowGrid.Column));
                                moved = true;
                                //合并完之后，继续向下找第一个非空的格子，然后移动，并且不再合并；
                                int j = i + 1;
                                for (; j <= rowNum - 1 && grids[j][col].Num == null; j++) ;
                                emptyGrid.Add(new Vector2Int(i, col));
                                moveNumToPos(grids[i][col].Num, j - 1, col);
                                emptyGrid.Remove(new Vector2Int(j - 1, col));
                            }
                            else
                            {
                                //否则先判断该行的上一行是否就是本行，若是则不移动，若不是则移动并置值
                                if (i - 1 != row)
                                {
                                    //不相等则移动
                                    emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                                    moveNumToPos(nowGrid.Num, i - 1, col);
                                    moved = true;
                                    emptyGrid.Remove(new Vector2Int(i - 1, col));
                                }
                            }

                        }
                        else
                        {
                            emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                            moveNumToPos(nowGrid.Num, i - 1, col);
                            moved = true;
                            emptyGrid.Remove(new Vector2Int(i - 1, col));
                            moved = true;

                        }
                    }
                }

                break;
            //向左移动
            case MOVEMENT.LEFT:
                for (int row = 0; row < rowNum; row++)
                {
                    for (int col = 1; col < columnNum; col++)
                    {
                        Grid nowGrid = grids[row][col];
                        if (nowGrid.Num == null)
                            continue;
                        int i = col - 1;
                        //向左寻找第一个不为空的格子
                        for (; i >= 0 && grids[row][i].Num == null; i--) ;
                        //找到第一个非空格子
                        if (i >= 0 && grids[row][i].Num != null)
                        {
                            //看看数字是否相同，是则合并
                            if (grids[row][i].Num.NumIsEqualsTo(nowGrid.Num))
                            {
                                grids[row][i].Num.SetNumber(grids[row][i].Num.GetNumber() * 2);
                                grids[row][i].Num.MergePlay();
                                NowScore += grids[row][i].Num.GetNumber();
                                score.text = NowScore.ToString();
                                if (NowScore > bestScore)
                                {
                                    bestScore = NowScore;
                                    best.text = bestScore.ToString();
                                    PlayerPrefs.SetInt(ConstValue.bestScore, bestScore);
                                }
                                nowGrid.destoryNum();
                                emptyGrid.Add(new Vector2Int(nowGrid.Row, nowGrid.Column));
                                moved = true;
                                //合并完之后，继续向左找第一个非空的格子，然后移动，并且不再合并；
                                int j = i - 1;
                                for (; j >= 0 && grids[row][j].Num == null; j--) ;
                                emptyGrid.Add(new Vector2Int(row, i));
                                moveNumToPos(grids[row][i].Num, row, j + 1);
                                emptyGrid.Remove(new Vector2Int(row, j + 1));
                            }
                            else
                            {
                                //否则先判断该行的下一列是否就是本列，若是则不移动，若不是则移动并置值
                                if (i + 1 != col)
                                {
                                    //不相等则移动
                                    emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                                    moveNumToPos(nowGrid.Num, row, i + 1);
                                    moved = true;
                                    emptyGrid.Remove(new Vector2Int(row, i+1));
                                }
                            }

                        }
                        else
                        {
                            emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                            moveNumToPos(nowGrid.Num, row, i + 1);
                            emptyGrid.Remove(new Vector2Int(row, i+1));
                            moved = true;

                        }
                    }
                }
                break;
            //向右移动
            case MOVEMENT.RIGHT:
                for (int row = 0; row < rowNum; row++)
                {
                    for (int col = columnNum - 2; col >= 0; col--)
                    {
                        Grid nowGrid = grids[row][col];
                        if (nowGrid.Num == null)
                            continue;
                        int i = col + 1;
                        //向右寻找第一个不为空的格子
                        for (; i <= columnNum - 1 && grids[row][i].Num == null; i++) ;
                        //找到第一个非空格子
                        if (i <= columnNum - 1 && grids[row][i].Num != null)
                        {
                            //看看数字是否相同，是则合并
                            if (grids[row][i].Num.NumIsEqualsTo(nowGrid.Num))
                            {
                                grids[row][i].Num.SetNumber(grids[row][i].Num.GetNumber() * 2);
                                grids[row][i].Num.MergePlay();
                                NowScore += grids[row][i].Num.GetNumber();
                                score.text = NowScore.ToString();
                                if (NowScore > bestScore)
                                {
                                    bestScore = NowScore;
                                    best.text = bestScore.ToString();
                                    PlayerPrefs.SetInt(ConstValue.bestScore, bestScore);
                                }
                                nowGrid.destoryNum();
                                emptyGrid.Add(new Vector2Int(nowGrid.Row, nowGrid.Column));
                                moved = true;
                                //合并完之后，继续向右找第一个非空的格子，然后移动，并且不再合并；
                                int j = i + 1;
                                for (; j <= columnNum - 1 && grids[row][j].Num == null; j++) ;
                                emptyGrid.Add(new Vector2Int(row, i));
                                moveNumToPos(grids[row][i].Num, row, j - 1);
                                
                                emptyGrid.Remove(new Vector2Int(row, j - 1));
                            }
                            else
                            {
                                //否则先判断该行的上一行是否就是本行，若是则不移动，若不是则移动并置值
                                if (i - 1 != col)
                                {
                                    //不相等则移动
                                    emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                                    moveNumToPos(nowGrid.Num, row, i - 1);
                                    moved = true;
                                    
                                    emptyGrid.Remove(new Vector2Int(row, i-1));
                                }
                            }

                        }
                        else
                        {
                            emptyGrid.Add(new Vector2Int(nowGrid.Num.ParentGrid.Row, nowGrid.Num.ParentGrid.Column));
                            moveNumToPos(nowGrid.Num, row, i - 1);
                            moved = true;
                            emptyGrid.Remove(new Vector2Int(row, i-1));
                        }
                    }
                }
                break;
            case MOVEMENT.STOP:
                break;
            default:
                break;
        }
        if (moved)
        {
            refreshOne();

        }

    }

    

    //将Num移动到指定地点
    private void moveNumToPos(Num num, int row, int col)
    {
        if (num.ParentGrid != null&&num.ParentGrid.Row == row && num.ParentGrid.Column == col)
            return;
        Grid targetGrid = grids[row][col];
        Grid originGrid = null;
        if (num.ParentGrid != null) {
            originGrid = num.ParentGrid;
        }
        num.SetGrid(targetGrid);
        if (originGrid != null)
        {
            originGrid.SetChildNum(null);
        }
    }

    private MOVEMENT CalMove()
    {
        if (Vector3.Magnitude(PointDownPos - PointUpPos) < 100)
            return MOVEMENT.STOP;
        float vertical = PointUpPos.y - PointDownPos.y;
        float horizontal = PointUpPos.x - PointDownPos.x;
        if (Mathf.Abs(vertical) > Mathf.Abs(horizontal))
        {
            if (vertical > 0)
                return MOVEMENT.UP;
            else return MOVEMENT.DOWN;
        }
        else
        {
            if (horizontal > 0)
                return MOVEMENT.RIGHT;
            else return MOVEMENT.LEFT;
        }
    }

}
