using System.Collections.Generic;
using UnityEngine;

public class PlayerProgressionContainer : MonoBehaviour
{
    private PlayerAccountProgression playerAccountProgression;

    [SerializeField, Tooltip("List of rank GameObjects in order from lowest to highest rank.")]
    private List<GameObject> ranks = new();

    private void Awake()
    {
        playerAccountProgression = GetComponentInChildren<PlayerAccountProgression>();

        if (playerAccountProgression == null)
        {
            Debugger.LogError("PlayerAccountProgression not found in children.", this);
            enabled = false;
            return;
        }

        foreach (var rank in ranks)
            rank.SetActive(false);
    }

    private void Start()
    {
        SetActiveRank(playerAccountProgression.GetCurrentRankIndex());
    }

    public void OnRankUp(int rankIndex)
    {
        if (!IsValidRankIndex(rankIndex)) return;
        ranks[rankIndex].SetActive(true);
    }

    private void SetActiveRank(int rankIndex)
    {
        if (!IsValidRankIndex(rankIndex))
        {
            Debugger.LogWarning($"Rank index {rankIndex} out of range. ranks.Count={ranks.Count}", this);
            return;
        }

        for (int i = 0; i < ranks.Count; i++)
            ranks[i].SetActive(i == rankIndex);
    }

    private bool IsValidRankIndex(int index) =>
        index >= 0 && index < ranks.Count;
}