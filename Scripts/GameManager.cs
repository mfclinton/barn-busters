using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameRoundConfig config;
    [SerializeField]
    Trigger spawnAreaTrigger;
    [SerializeField]
    Trigger goalAreaTrigger;
    [SerializeField]
    Trigger deathAreaTrigger;

    int cash;
    int round;
    float roundLength;
    List<Agent> activeAgents;
    List<Agent> wonAgents;

    // Hardcoded / Assumed
    BoxCollider spawnArea;
    [SerializeField] BoxCollider goalArea;

    public delegate void OnCashChangeEventHandler();
    public event OnCashChangeEventHandler OnCashChange;

    public delegate void OnAgentDieEventHandler(Agent a);
    public event OnAgentDieEventHandler OnAgentDie;

    public delegate void OnAgentWinEventHandler(Agent a);
    public event OnAgentWinEventHandler OnAgentWin;

    public delegate void OnStartRoundEventHandler();
    public event OnStartRoundEventHandler OnStartRound;

    public delegate void OnEndRoundEventHandler();
    public event OnEndRoundEventHandler OnEndRound;

    public delegate void OnGameOverEventHandler();
    public event OnGameOverEventHandler OnGameOver;

    public bool gameOver { get; private set; }
    public bool activeRound { get; private set; }
    float timeStarted;

    private void Awake()
    {
        spawnArea = spawnAreaTrigger.GetComponent<BoxCollider>();
        // goalArea = goalAreaTrigger.GetComponent<BoxCollider>();
        InitializeFromConfig();
        
        deathAreaTrigger.OnAgentTrigger += (Agent a) => HandleAgentDie(a);

        goalAreaTrigger.OnAgentTrigger += (Agent a) => HandleAgentWin(a);
    }

    private void Update()
    {
        ProcessRoundTimeElapsed();
    }

    void InitializeFromConfig()
    {
        cash = config.startingCash;
        round = 1;
        roundLength = config.timeLimit;

        activeAgents = new List<Agent>();
        wonAgents = new List<Agent>();
        foreach (AgentRoundInfo ari in config.agentsInfo)
        {
            for (int i = 0; i < ari.quantity; i++)
            {
                Agent newAgent = Instantiate(ari.agent, transform);
                newAgent.agentRoundInfo = ari;
                newAgent.gameObject.SetActive(true);

                activeAgents.Add(newAgent);
                PlaceAgent(newAgent, goalArea.transform);
            }
        }
    }

    public float TotalRoundLength()
    {
        return config.timeLimit;
    }

    public void StartRound()
    {
        if (activeRound || gameOver)
            return;

        timeStarted = Time.time;
        foreach (Agent a in activeAgents)
        {
            a.MoveTo(PathingHelpers.SamplePointFromBox(goalArea));
        }

        activeRound = true;
        OnStartRound?.Invoke();
    }

    // Code for processing the end of a round
    // Stops at the end of the 
    void StopRound()
    {
        ModifyCash(-config.cashToGain);

        while (activeAgents.Count != 0)
        {
            Agent a = activeAgents[0];
            activeAgents.RemoveAt(0);
            HandleLostAgent(a); // Remove agent from game
        }
        activeAgents = wonAgents;
        wonAgents = new List<Agent>();
        foreach (Agent a in activeAgents)
        {
            ResetAgent(a);
        }

        activeRound = false;
        
        if (activeAgents.Count == 0)
        {
            gameOver = true;
            OnGameOver?.Invoke();
        }
        else
        {
            round += 1;
        }
        OnEndRound?.Invoke();
    }

    // Event for falling off
    void HandleAgentDie(Agent a)
    {
        float respawnTime = a.agentRoundInfo != null ? a.agentRoundInfo.respawnTime : 0f;
        int moneyGain = a.agentRoundInfo != null ? a.agentRoundInfo.costFromFall : 0;

        StartCoroutine(DelayedPlaceAgent(a, respawnTime, goalArea.transform));
        if(activeRound)
            ModifyCash(-moneyGain);

        OnAgentDie?.Invoke(a);
    }

    // Event for agent reacing the end
    void HandleAgentWin(Agent a)
    {
        int moneyGain = a.agentRoundInfo != null ? a.agentRoundInfo.costFromWin : 0;
        ModifyCash(-moneyGain);

        bool agentAlreadyWon = !activeAgents.Remove(a);
        if (agentAlreadyWon)
            return;
        
        wonAgents.Add(a);
        OnAgentWin?.Invoke(a);

        if (activeAgents.Count == 0)
            StopRound();
    }

    // Event for agent losing entirely
    void HandleLostAgent(Agent a)
    {
        int moneyGain = a.agentRoundInfo != null ? a.agentRoundInfo.costFromDeath : 0;
        ModifyCash(-moneyGain);

        activeAgents.Remove(a);
        Destroy(a.gameObject);
    }

    // Event for resetting agent (after a round)
    void ResetAgent(Agent a)
    {
        PlaceAgent(a, goalArea.transform, true);
        a.gameObject.SetActive(true);
    }

    void ProcessRoundTimeElapsed()
    {
        // Time Ran Out
        if(activeRound && timeStarted + roundLength < Time.time)
        {
            StopRound();
        }
    }

    IEnumerator DelayedPlaceAgent(Agent a, float timeToWait, Transform lookAt = null)
    {
        yield return new WaitForSeconds(timeToWait);
        PlaceAgent(a, lookAt);
    }

    void PlaceAgent(Agent a, Transform lookAt = null, bool resetPath = false)
    {
        int i = 0;
        Vector3? gridPointResult = null;
        while(!gridPointResult.HasValue)
        {
            Vector3 sampledPoint = PathingHelpers.SamplePointFromBox(spawnArea);
            gridPointResult = PathingHelpers.GetClosestWalkableGridPoint(sampledPoint);
            i++;

            if(4 < i)
            {
                Debug.LogError($"Failed to spawn agent at {sampledPoint}");
                return;
            }
        }

        a.rc?.ResetRagdoll();
        Vector3 spawnPos = gridPointResult.Value + new Vector3(0f, a.height / 2f, 0f);
        a.Teleport(spawnPos, resetPath);
        if (lookAt != null)
            a.transform.LookAt(lookAt);
    }


    public int Cash()
    {
        return cash;
    }

    public int Enemies()
    {
        return activeAgents != null ? activeAgents.Count:0;
    }

    public int Round()
    {
        return round;
    }

    public float TimeElapsed()
    {
        float timeElapsed = (timeStarted + roundLength) - Time.time;
        return timeElapsed;
    }

    public bool ModifyCash(int cost)
    {
        if (!CanBuy(cost))
            return false;

        cash = cash - cost;
        OnCashChange?.Invoke();
        return true;
    }

    public bool CanBuy(int cost)
    {
        return 0 <= cash - cost;
    }
}
