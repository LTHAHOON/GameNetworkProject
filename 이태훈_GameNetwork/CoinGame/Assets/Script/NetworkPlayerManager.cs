using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviour
{
    public static NetworkPlayerManager Instance {get; set;}
    private readonly Dictionary<Guid, int> _dicScoreData = new();

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
    }

    public void SetScoreData(Guid playerId, int curScore)
    {
        if(! _dicScoreData.ContainsKey(playerId))
        {
            _dicScoreData.Add(playerId, curScore);
            return;
        }
        _dicScoreData[playerId] = curScore;
    }
    
    public bool TryGetScoreData(Guid playerId, out int curScore)
    {
        if(_dicScoreData.TryGetValue(playerId, out int score))
        {
            curScore = score;
            return true;
        }
        curScore = 0;
        return false;
    }

}
