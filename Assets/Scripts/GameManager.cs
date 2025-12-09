using Manager;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Manager
{
    public class PlayerEntry
    {
        public int id;
        public GameObject gameObject;
        public PlayerModel playerModel;
        public IPEndPoint ipEndPoint;

        public PlayerEntry(int id, GameObject gameObject, PlayerModel playerModel, IPEndPoint ipEndPoint)
        {
            this.id = id;
            this.gameObject = gameObject;
            this.playerModel = playerModel;
            this.ipEndPoint = ipEndPoint;
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

    public void AddPlayer(int playerID, bool isMine, IPEndPoint ipEndPoint = null)
    {
        _players[playerID] = new PlayerEntry(playerID, CreatePlayerPrefab(), new PlayerModel(isMine), ipEndPoint);
    }

    public List<PlayerEntry> GetPlayers()
    {
        return new List<PlayerEntry>(_players.Values);
    }

    public PlayerEntry GetPlayer(int playerID)
    {
        return _players[playerID];
    }

    public bool IsExistPlayer(int playerID)
    {
        return _players.ContainsKey(playerID);
    }

    public int GetNextPlayerID() { return _nextPlayerID++; }


    public GameObject CreatePlayerPrefab()
    {
        GameObject gameObject = Instantiate(prefab, new Vector3(-4 + _next++ * 2, 1, 0), Quaternion.identity);
//        gameObject.GetComponent<Player>().isMine = isMine;

        return gameObject;
    }
}