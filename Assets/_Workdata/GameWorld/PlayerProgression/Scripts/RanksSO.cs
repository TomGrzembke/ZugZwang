using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RanksSO", menuName = "Scriptable Objects/RanksSO")]
public class RanksSO : ScriptableObject
{
    public List<Rank> ranks;
    
    [Serializable]
    public struct Rank
    {
        public string name;
        public int levelToReach;
        public int xpPerLevel;
    }
}
