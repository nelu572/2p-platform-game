using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PooledObject))]
public class SelfReturn : MonoBehaviour
{
    private PooledObject _pooledObject;
    private Coroutine _returnRoutine;

    private void Awake()
    {
        _pooledObject = GetComponent<PooledObject>();
    }

    private void OnDisable()
    {
        if (_returnRoutine == null)
            return;

        StopCoroutine(_returnRoutine);
        _returnRoutine = null;
    }

    public void ReturnAfter(float delay)
    {
        if (_returnRoutine != null)
            StopCoroutine(_returnRoutine);

        _returnRoutine = StartCoroutine(ReturnAfterDelay(Mathf.Max(0f, delay)));
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _returnRoutine = null;
        _pooledObject.ReturnToPool();
    }
}
