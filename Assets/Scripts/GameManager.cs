using Manager;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Manager
{
    public class PlayerEntry
    {
        public readonly GameObject gameObject;
        public readonly PlayerModel playerModel;
        public readonly IPEndPoint ipEndPoint;

        public PlayerEntry(GameObject gameObject, PlayerModel playerModel, IPEndPoint ipEndPoint)
        {
            this.gameObject = gameObject;
            this.playerModel = playerModel;
            this.ipEndPoint = ipEndPoint;
        }

        public void SetModel(PlayerModel playerModel)
        {
            this.playerModel.SetPlayerTransform(playerModel.PlayerPosition, playerModel.PlayerRotation);
            this.playerModel.SetPlayerState(playerModel.PlayerPostureState);
            this.playerModel.SetPlayerState(playerModel.PlayerMovementType);
        }
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject prefab;

    private Dictionary<int, PlayerEntry> _players;
    private int _nextPlayerID = 1;
    private int _next = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        _players = new Dictionary<int, PlayerEntry>();
    }

    public void AddPlayer(PlayerModel playerModel, IPEndPoint ipEndPoint = null)
    {
        _players[playerModel.PlayerID] = new PlayerEntry(CreatePlayerPrefab(), playerModel, ipEndPoint);
        Vector3 initPosition = new Vector3(-4 + _next++ * 2, 1, 0);
        playerModel.SetPlayerTransform(initPosition, Vector3.zero);
        _players[playerModel.PlayerID].gameObject.GetComponent<PlayerRoot>().Initiate(playerModel);
    }

    public int GetNextPlayerID() { return _nextPlayerID++; }


    public List<PlayerEntry> GetPlayers()
    {
        return new List<PlayerEntry>(_players.Values);
    }

    public bool SetPlayers(PlayerModel playerModel)
    {
        if (!_players.ContainsKey(playerModel.PlayerID)) return false;
        _players[playerModel.PlayerID].SetModel(playerModel);
        return true;
    }

    public bool IsExistPlayer(int playerID)
    {
        return _players.ContainsKey(playerID);
    }

    private GameObject CreatePlayerPrefab()
    {
        return Instantiate(prefab);
    }
}