using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    [System.Serializable]
    public class PoolEntry
    {
        public string key;
        public GameObject prefab;
        public int initialSize = 3;
    }

    //싱글톤 패턴 적용
    public static ObjectPoolManager Instance { get; private set; }

    //미리 생성할 오브젝트들
    [SerializeField] private List<PoolEntry> _entries;

    // 키값으로 Queue를 구별하는 딕셔너리 생성
    private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

    //키값으로 미리 생성한 오브젝트들 구별 & 관리하는 딕셔너리 생성
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (_entries == null)
            return;

        //미리 생성할 오브젝트
        foreach (var entry in _entries)
        {
            Prewarm(entry.key, entry.prefab, entry.initialSize);
        }
    }

    public void Prewarm(string key, GameObject prefab, int count)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning("[Pool] 키가 비어 있어 사전 생성을 건너뜁니다.");
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning($"[Pool] {key} 프리팹이 비어 있어 사전 생성을 건너뜁니다.");
            return;
        }

        if (!_pools.ContainsKey(key))
            _pools[key] = new Queue<GameObject>();

        if (!_prefabs.ContainsKey(key))
            _prefabs[key] = prefab;

        int safeCount = Mathf.Max(0, count);
        int needCount = safeCount - _pools[key].Count;
        for (int i = 0; i < needCount; i++)
            _pools[key].Enqueue(CreateNew(key));
    }

    public GameObject Get(string key)
    {
        GameObject obj = GetInactiveObject(key);
        if (obj == null)
            return null;

        obj.SetActive(true);
        InitPooledObject(key, obj);

        return obj;
    }

    public GameObject Get(string key, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetInactiveObject(key);
        if (obj == null)
            return null;

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        InitPooledObject(key, obj);

        return obj;
    }

    public void Return(string key, GameObject obj)
    {
        if (!_pools.ContainsKey(key))
        {
            Debug.LogWarning($"[Pool] 키 없음: {key}");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        _pools[key].Enqueue(obj);
    }

    private GameObject GetInactiveObject(string key)
    {
        if (!_pools.ContainsKey(key))
        {
            Debug.LogError($"[Pool] 키 없음: {key}");
            return null;
        }

        var obj = _pools[key].Count > 0
            ? _pools[key].Dequeue()
            : CreateNew(key);

        return obj;
    }

    private void InitPooledObject(string key, GameObject obj)
    {
        if (obj.TryGetComponent<PooledObject>(out var pooled))
            pooled.Init(key, this);
    }

    private GameObject CreateNew(string key)
    {
        var obj = Instantiate(_prefabs[key]);
        obj.SetActive(false);
        return obj;
    }
}
