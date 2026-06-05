using System.Collections.Generic;
using UnityEngine;

public class GhostCreateManager : MonoBehaviour
{
    [SerializeField] private ObjectPoolManager _objectPoolManager;

    private Vector3 _p1DeadPoint;
    private Vector3 _p2DeadPoint;

    public void PrewarmSelectedGhosts(PlayerStat player1, PlayerStat player2)
    {
        ObjectPoolManager poolManager = ResolvePoolManager();
        if (poolManager == null)
        {
            Debug.LogWarning("GhostCreateManager: ObjectPoolManager를 찾을 수 없습니다.");
            return;
        }

        Dictionary<string, GhostPrewarmRequest> requests = new Dictionary<string, GhostPrewarmRequest>();
        AddPrewarmRequest(requests, player1);
        AddPrewarmRequest(requests, player2);

        foreach (GhostPrewarmRequest request in requests.Values)
            poolManager.Prewarm(request.PoolKey, request.Prefab, request.Count);
    }

    public void DeadPoint(PlayerStat deadPlayer)
    {
        if (deadPlayer == null)
            return;

        Vector3 deadPosition = deadPlayer.transform.position;

        if (deadPlayer.TeamId == 1)
            _p1DeadPoint = deadPosition;
        else if (deadPlayer.TeamId == 2)
            _p2DeadPoint = deadPosition;

        CreateGhost(deadPlayer, deadPosition);//유령 생성
    }

    private void CreateGhost(PlayerStat deadPlayer, Vector3 spawnPosition)
    {
        ObjectPoolManager poolManager = ResolvePoolManager();

        if (poolManager == null)
        {
            Debug.LogWarning("GhostCreateManager: ObjectPoolManager를 찾을 수 없습니다.");
            return;
        }

        string ghostPoolKey = deadPlayer.GhostPoolKey;
        if (string.IsNullOrWhiteSpace(ghostPoolKey))
        {
            Debug.LogWarning($"player{deadPlayer.TeamId} 유령 풀 키가 비어 있습니다.");
            return;
        }

        GameObject ghost = poolManager.Get(ghostPoolKey, spawnPosition, Quaternion.identity);
        if (ghost == null)
            return;

        Debug.Log($"player{deadPlayer.TeamId} 유령 생성: {spawnPosition}");
    }

    private ObjectPoolManager ResolvePoolManager()
    {
        if (_objectPoolManager == null)
            _objectPoolManager = ObjectPoolManager.Instance;

        return _objectPoolManager;
    }

    private void AddPrewarmRequest(Dictionary<string, GhostPrewarmRequest> requests, PlayerStat playerStat)
    {
        if (playerStat == null)
            return;

        string poolKey = playerStat.GhostPoolKey;
        GameObject prefab = playerStat.GhostPrefab;
        if (string.IsNullOrWhiteSpace(poolKey) || prefab == null)
        {
            Debug.LogWarning($"player{playerStat.TeamId} 유령 풀 정보가 비어 있어 사전 생성을 건너뜁니다.");
            return;
        }

        if (requests.TryGetValue(poolKey, out GhostPrewarmRequest request))
        {
            request.Count += playerStat.GhostPrewarmCount;
            requests[poolKey] = request;
            return;
        }

        requests.Add(poolKey, new GhostPrewarmRequest(poolKey, prefab, playerStat.GhostPrewarmCount));
    }

    private struct GhostPrewarmRequest
    {
        public GhostPrewarmRequest(string poolKey, GameObject prefab, int count)
        {
            PoolKey = poolKey;
            Prefab = prefab;
            Count = count;
        }

        public string PoolKey { get; }
        public GameObject Prefab { get; }
        public int Count { get; set; }
    }
}
