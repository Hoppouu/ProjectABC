using Network;
using System.Data;
using System;
using UnityEngine;
namespace Player.Model
{
    public enum PlayerPostureState
    {
        Unknown = 0,
        Stand = 1,
        Crawl = 2,
    }

    public enum PlayerMovementType
    {
        Unknown = 0,
        Idle = 1,
        Walk = 2,
        Run = 3,
    }

    public struct PlayerData
    {
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public Vector3 headPosition;
        public Vector3 headRotation;

        public PlayerPostureState playerPostureState;
        public PlayerMovementType playerMovementType;
    }

    public class PlayerModel
    {
        public readonly int playerID;
        public readonly bool isMine;
        public PlayerData Data => _data;
        public float MoveSpeed { get; private set; }
        public bool IsCrawl { get; private set; }

        private PlayerData _data;

        private PlayerModel()
        {
            _data = new PlayerData();
            SetPostureState(PlayerPostureState.Stand);
            SetMovementType(PlayerMovementType.Idle);
        }

        public PlayerModel(int playerID, bool isMine = false):base()
        {
            this.playerID = playerID;
            this.isMine = isMine;
        }

        public void SetPlayerInfo(PlayerData data)
        {
            _data = data;
            ApplyPostureState();
            ApplyMovementType();
        }

        public void SetPlayerTransform(Vector3 playerPosition, Vector3 playerRotation)
        {
            _data.playerPosition = playerPosition;
            _data.playerRotation = playerRotation;
        }
        public void SetPlayerPosition(Vector3 playerPosition)
        {
            _data.playerPosition = playerPosition;
        }
        public void SetPlayerRotation(Vector3 playerRotation)
        {
            _data.playerRotation = playerRotation;
        }

        public void SetPostureState(Network.PlayerPostureState state)
        {
            SetPostureState(MapNetworkToModel(state));
        }

        public void SetMovementType(Network.PlayerMovementType type)
        {
            SetMovementType(MapNetworkToModel(type));
        }

        public void SetPostureState(PlayerPostureState state)
        {
            _data.playerPostureState = state;
            ApplyPostureState();
        }

        public void SetMovementType(PlayerMovementType type)
        {
            _data.playerMovementType = type;
            ApplyMovementType();
        }

        private void ApplyPostureState()
        {
            switch (_data.playerPostureState)
            {
                case PlayerPostureState.Stand:
                    IsCrawl = false; break;
                case PlayerPostureState.Crawl:
                    IsCrawl = true; break;
            }
        }

        private void ApplyMovementType()
        {
            switch (_data.playerMovementType)
            {
                case PlayerMovementType.Idle:
                    MoveSpeed = 0f; break;
                case PlayerMovementType.Walk:
                    MoveSpeed = 0.5f; break;
                case PlayerMovementType.Run:
                    MoveSpeed = 1f; break;
            }
        }

        private PlayerPostureState MapNetworkToModel(Network.PlayerPostureState state)
        {
            return state switch
            {
                Network.PlayerPostureState.Stand => PlayerPostureState.Stand,
                Network.PlayerPostureState.Crawl => PlayerPostureState.Crawl,
                _ => PlayerPostureState.Stand
            };
        }

        private PlayerMovementType MapNetworkToModel(Network.PlayerMovementType type)
        {
            return type switch
            {
                Network.PlayerMovementType.Idle => PlayerMovementType.Idle,
                Network.PlayerMovementType.Walk => PlayerMovementType.Walk,
                Network.PlayerMovementType.Run => PlayerMovementType.Run,
                _ => PlayerMovementType.Idle
            };
        }
    }
}