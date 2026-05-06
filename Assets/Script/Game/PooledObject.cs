using UnityEngine;

public class PooledObject : MonoBehaviour
{//총알이나 투척류같은 오브젝트들이 사용할 스크립트입니다. 레일건의 레이져 같은 경우는 컨트롤러에서 행동하기에 이 스크립트를 사용하지 않습니다.
    public string PoolKey { get; private set; }
    public ObjectPoolManager PoolGroup { get; private set; }

    // 풀에서 꺼낼 때 PoolGroup이 초기화해줌
    public void Init(string key, ObjectPoolManager poolGroup)
    {
        PoolKey = key;
        PoolGroup = poolGroup;
    }

    public void ReturnToPool()
    {
        PoolGroup.Return(PoolKey, gameObject);
    }
}