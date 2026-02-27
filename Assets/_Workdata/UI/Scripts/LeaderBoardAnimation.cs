using System;
using UnityEngine;

public class LeaderBoardAnimation : MonoBehaviour
{
    public void StartProgressBarFilling()
    {
        StartCoroutine(GetComponentInChildren<ProgressBar>().FillOnce());
    }
}
