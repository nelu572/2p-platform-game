using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostCreateManager : MonoBehaviour
{
    [SerializeField] private ObjectPoolManager _objectPoolManager;

    private Vector3 _p1DeadPoint;
    private Vector3 _p2DeadPoint;
    private readonly List<ActiveGhost> _activeGhosts = new List<ActiveGhost>();

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

        ConfigureGhostObject(ghost);
        RegisterActiveGhost(ghostPoolKey, ghost);

        Debug.Log($"player{deadPlayer.TeamId} 유령 생성: {spawnPosition}");
    }

    public void ClearGhosts()
    {
        ObjectPoolManager poolManager = ResolvePoolManager();

        for (int i = _activeGhosts.Count - 1; i >= 0; i--)
        {
            ActiveGhost activeGhost = _activeGhosts[i];
            if (activeGhost.Ghost == null)
                continue;

            if (poolManager != null && !string.IsNullOrWhiteSpace(activeGhost.PoolKey))
                poolManager.Return(activeGhost.PoolKey, activeGhost.Ghost);
            else
                activeGhost.Ghost.SetActive(false);
        }

        _activeGhosts.Clear();
    }

    private void RegisterActiveGhost(string poolKey, GameObject ghost)
    {
        for (int i = _activeGhosts.Count - 1; i >= 0; i--)
        {
            GameObject activeGhost = _activeGhosts[i].Ghost;
            if (activeGhost == null || activeGhost == ghost || !activeGhost.activeInHierarchy)
                _activeGhosts.RemoveAt(i);
        }

        _activeGhosts.Add(new ActiveGhost(poolKey, ghost));
    }

    private void ConfigureGhostObject(GameObject ghost)
    {
        DisablePlayerInput(ghost);
        ApplyGhostLayer(ghost);
        IgnorePlayerCollisions(ghost);
    }

    private void DisablePlayerInput(GameObject ghost)
    {
        PlayerInput playerInput = ghost.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        PlayerInputHandler inputHandler = ghost.GetComponent<PlayerInputHandler>();
        if (inputHandler != null)
            inputHandler.enabled = false;
    }

    private void ApplyGhostLayer(GameObject ghost)
    {
        int ghostLayer = LayerMask.NameToLayer("Ghost");
        if (ghostLayer < 0)
            return;

        SetLayerRecursively(ghost.transform, ghostLayer);
    }

    private void SetLayerRecursively(Transform target, int layer)
    {
        target.gameObject.layer = layer;

        for (int i = 0; i < target.childCount; i++)
            SetLayerRecursively(target.GetChild(i), layer);
    }

    private void IgnorePlayerCollisions(GameObject ghost)
    {
        Collider2D[] ghostColliders = ghost.GetComponentsInChildren<Collider2D>();
        PlayerStat[] playerStats = FindObjectsByType<PlayerStat>(FindObjectsSortMode.None);

        foreach (Collider2D ghostCollider in ghostColliders)
        {
            if (ghostCollider == null)
                continue;

            foreach (PlayerStat playerStat in playerStats)
            {
                if (playerStat == null || playerStat.gameObject == ghost)
                    continue;

                Collider2D[] playerColliders = playerStat.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D playerCollider in playerColliders)
                {
                    if (playerCollider != null)
                        Physics2D.IgnoreCollision(ghostCollider, playerCollider, true);
                }
            }
        }
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

    private readonly struct ActiveGhost
    {
        public ActiveGhost(string poolKey, GameObject ghost)
        {
            PoolKey = poolKey;
            Ghost = ghost;
        }

        public string PoolKey { get; }
        public GameObject Ghost { get; }
    }
}
