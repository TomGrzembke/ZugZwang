using System.Collections.Generic;
using UnityEngine;

public static class Prefs
{
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat("musicVolume");
        set
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
        }
    }

    public static float SoundVolume
    {
        get => PlayerPrefs.GetFloat("soundVolume");
        set
        {
            PlayerPrefs.SetFloat("soundVolume", value);
            PlayerPrefs.Save();
        }
    }

    public static int GraphicIndex
    {
        get => PlayerPrefs.GetInt("graphicsIndex", 2);
        set
        {
            PlayerPrefs.SetInt("graphicsIndex", value);
            PlayerPrefs.Save();
        }
    }

    public static bool NewScore
    {
        get => PlayerPrefs.GetInt("newScore", 0) != 0; 
        set
        {
            PlayerPrefs.SetInt("newScore", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    
    public static bool IsFirstGame
    {
        get => PlayerPrefs.GetInt("firstGame", 1) != 0;
        set
        {
            PlayerPrefs.SetInt("firstGame", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int Level
    {
        get => PlayerPrefs.GetInt("level", 0);
        set
        {
            PlayerPrefs.SetInt("level", value);
            PlayerPrefs.Save();
        }
    }

    public static int Xp
    {
        get => PlayerPrefs.GetInt("xp", 0);
        set
        {
            PlayerPrefs.SetInt("xp", value);
            PlayerPrefs.Save();
        }
    }
    
    public static List<int> BestScores
    {
        get
        {
            List<int> scores = new();
            for (int i = 1; i <= 10; i++)
            {
                if (PlayerPrefs.HasKey("highScore" + i)) scores.Add(PlayerPrefs.GetInt("highScore" + i));
            }

            if (scores.Count <= 0) return null;

            scores.Sort();
            scores.Reverse();

            return scores;
        }
        set
        {
            for (int i = 1; i <= 10; i++)
            {
                PlayerPrefs.SetInt("highScore" + i, value[i - 1]);
            }
        }
    }

    public static int GetBestScore()
    {
        List<int> scores = BestScores;
        
        if (scores == null) return -1;
        
        return scores[0];
    }
}