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
        //미리 생성할 오브젝트
        foreach (var entry in _entries)
        {
            _pools[entry.key] = new Queue<GameObject>();
            _prefabs[entry.key] = entry.prefab;
            //오브젝트 생성
            for (int i = 0; i < entry.initialSize; i++)
                _pools[entry.key].Enqueue(CreateNew(entry.key));
        }
    }

    public GameObject Get(string key)
    {
        if (!_pools.ContainsKey(key))
        {
            Debug.LogError($"[Pool] 키 없음: {key}");
            return null;
        }

        var obj = _pools[key].Count > 0
            ? _pools[key].Dequeue()
            : CreateNew(key);

        obj.SetActive(true);

        if (obj.TryGetComponent<PooledObject>(out var pooled))
            pooled.Init(key, this);
        
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

    private GameObject CreateNew(string key)
    {
        var obj = Instantiate(_prefabs[key]);
        obj.SetActive(false);
        return obj;
    }
}