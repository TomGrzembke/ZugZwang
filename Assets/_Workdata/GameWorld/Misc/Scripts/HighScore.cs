using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class HighScore : MonoBehaviour
{
    [Separator("References")] [SerializeField]
    private RoundTimer roundTimer;

    [SerializeField] private DeathZone deathZone;

    [SerializeField] private TextMeshProUGUI[] currentScoreTexts;

    [Space(10)] [Separator("Player Progression Settings")] [SerializeField]
    private List<int> figureScorePerRow;
    
    [Space(10)] [Separator("Highscore Milestones")] [SerializeField]
    private bool useMilestones;

    [SerializeField] [ConditionalField(nameof(useMilestones))]
    private int milestonesValue;

    [SerializeField] [ConditionalField(nameof(useMilestones))]
    public UnityEvent OnScoreMilestoneReached;

    [HideInInspector] public int currentScore;
    private int currentMilestoneScore;


    private void Start()
    {
        currentScore = 0;
        if (useMilestones && milestonesValue > 0) currentMilestoneScore = 0;

        roundTimer.OnTurn += AddHighScore;

        if (currentScoreTexts.Length > 0)
            foreach (var text in currentScoreTexts)
                if (text != null)
                    text.text = "0";

        for (int i = 1; i <= 10; i++)
        {
            if (!PlayerPrefs.HasKey("highScore" + i)) PlayerPrefs.SetInt("highScore" + i, 0);
        }
    }

    private void OnDestroy()
    {
        roundTimer.OnTurn -= AddHighScore;
    }

    private void AddHighScore(CurrentPhaseProperties _phaseSettings)
    {
        var currentPlayerCount = deathZone.currentPlayerCount;
        int scoreToAdd = GetScoreForFigureCount(currentPlayerCount);
        AddScore(scoreToAdd);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreText();

        if (!useMilestones) return;

        currentMilestoneScore += score;
        if (currentMilestoneScore >= milestonesValue)
        {
            OnScoreMilestoneReached?.Invoke();
            currentMilestoneScore = 0;
        }
    }

    private void UpdateScoreText()
    {
        if (currentScoreTexts.Length > 0)
            foreach (TextMeshProUGUI text in currentScoreTexts)
                text.text = currentScore.ToString();
    }

    private int GetScoreForFigureCount(int figureCount)
    {
        if (figureCount > figureScorePerRow.Count && figureScorePerRow.Count == 0)
        {
            Debugger.LogError("The Score Points for each figure count are not set.", this);
            return 0;
        }
        return figureScorePerRow[figureCount - 1];
    }
}