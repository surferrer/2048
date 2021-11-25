using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumChange : MonoBehaviour
{
    private float StartscaleTime;
    // Start is called before the first frame update
    private float MergeTime;
    private float RecoveryTime;
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void StartPlay()
    {
        StartscaleTime = 0;
        MergeTime = 2;
        RecoveryTime = 2;
    }

    public void StartMergePlay()
    {
        MergeTime = 0;
        RecoveryTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //出现
        if (StartscaleTime <= 1)
        {
            StartscaleTime += Time.deltaTime * 5;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, StartscaleTime);
        }
        else if (MergeTime <= 1)
        {
            MergeTime += Time.deltaTime * 5;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, MergeTime);
        }
        else if (RecoveryTime <= 1)
        {
            RecoveryTime += Time.deltaTime * 5;
            transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, RecoveryTime);
        }
    }
}
