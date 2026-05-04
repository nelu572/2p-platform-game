using System.Collections.Generic;
using UnityEngine;

public class CreateLaser : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private int initialPoolSize = 3;

    private Queue<GameObject> _pool = new Queue<GameObject>();

    private void Awake()
    {
        // 초기 풀 채우기
        for (int i = 0; i < initialPoolSize; i++)
        {
            _pool.Enqueue(CreateNew());
        }
    }

    // 풀에서 꺼내기
    public GameObject GetObject()
    {
        GameObject obj;

        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        else
        {
            // 풀이 비었으면 새로 생성
            obj = CreateNew();
        }

        obj.SetActive(true);
        return obj;
    }

    // 풀에 반납
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }

    private GameObject CreateNew()
    {
        var obj = Instantiate(laserPrefab, transform.position, transform.rotation);
        obj.SetActive(false);
        return obj;
    }
}