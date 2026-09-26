using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentRoundInfo
{
    public Agent agent;
    public int quantity;
    public int costFromRagdoll;
    public int costFromFall;
    public int costFromWin;
    public int costFromDeath;
    public float respawnTime;
}

[CreateAssetMenu(fileName = "GameRoundConfig", menuName = "ScriptableObjects/GameRoundConfig", order = 1)]
public class GameRoundConfig : ScriptableObject
{
    public float timeLimit;
    public int startingCash;
    public int cashToGain;
    public AgentRoundInfo[] agentsInfo;
}
