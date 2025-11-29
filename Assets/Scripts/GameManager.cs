using Manager;
using System.Collections.Generic;
using System.Data;
using System.Net;
using UnityEngine;

namespace Manager
{
    public class PlayerEntry
    {
        public int id;
        public IPEndPoint ipEndPoint;
        public GameObject gameObject;

        public PlayerEntry(int id, GameObject gameObject, IPEndPoint ipEndPoint)
        {
            this.id = id;
            this.ipEndPoint = ipEndPoint;
            this.gameObject = gameObject;
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
        if(Instance == null)
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
        _players[playerID] = new PlayerEntry(playerID, CreatePlayerPrefab(isMine), ipEndPoint);
    }

    public List<PlayerEntry> GetPlayers()
    {
        return new List<PlayerEntry>(_players.Values);
    }

    public bool IsExistPlayer(int playerID)
    {
        return _players.ContainsKey(playerID);
    }

    public int GetNextPlayerID() { return _nextPlayerID++; }


    private GameObject CreatePlayerPrefab(bool isMine)
    {
        GameObject gameObject = Instantiate(prefab, new Vector3( -4 + _next++ * 2, 1, 0), Quaternion.identity);
        gameObject.GetComponent<PlayerInput>().isMine = isMine;

        return gameObject;
    }
}
