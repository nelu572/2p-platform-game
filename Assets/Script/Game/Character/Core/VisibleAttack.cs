using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class VisibleAttack : MonoBehaviour
{
    [SerializeField] private string _sortingLayerName = "Player";
    [SerializeField] private int _sortingOrder = -10;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private LineRenderer _lineRenderer;
    private Material _fillMaterial;
    private Material _lineMaterial;
    private Mesh _mesh;
    private Coroutine _hideRoutine;
    private readonly Vector3[] _cachedBoxVertices = new Vector3[4];
    private readonly List<Vector3> _cachedLocalVertices = new List<Vector3>();

    private void Awake()
    {
    }

    private void OnDisable()
    {
        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }
    }

    private void OnDestroy()
    {
        DestroyRuntimeMaterial(_fillMaterial);
        DestroyRuntimeMaterial(_lineMaterial);
        DestroyRuntimeMesh(_mesh);
    }

    public void Hide()
    {
        EnsureRenderers();
        _meshRenderer.enabled = false;
        _lineRenderer.enabled = false;
    }

    protected void ShowShape(Vector2 position, float angle, Vector3[] vertices, Color fillColor, Color outlineColor)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        ShowShape(vertices, fillColor, outlineColor);
    }

    protected void ShowWorldShape(Vector3[] worldVertices, Color fillColor, Color outlineColor)
    {
        ShowShape(ToLocalVertices(worldVertices), fillColor, outlineColor);
    }

    protected void ShowShape(Vector3[] vertices, Color fillColor, Color outlineColor)
    {
        EnsureRenderers();

        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }

        UpdateMesh(vertices);
        _fillMaterial.color = fillColor;
        _lineRenderer.positionCount = vertices.Length;
        _lineRenderer.SetPositions(vertices);
        _lineRenderer.startColor = outlineColor;
        _lineRenderer.endColor = outlineColor;
        _lineRenderer.loop = true;

        _meshRenderer.enabled = true;
        _lineRenderer.enabled = true;
    }

    protected void ShowFill(Vector2 position, float angle, Vector3[] vertices, Color fillColor)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        ShowFill(vertices, fillColor);
    }

    protected void ShowWorldFill(Vector3[] worldVertices, Color fillColor)
    {
        ShowFill(ToLocalVertices(worldVertices), fillColor);
    }

    protected void ShowWorldFill(List<Vector3> worldVertices, Color fillColor)
    {
        ShowFill(ToLocalVertices(worldVertices), fillColor);
    }

    protected void ShowFill(Vector3[] vertices, Color fillColor)
    {
        EnsureRenderers();

        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }

        UpdateMesh(vertices);
        _fillMaterial.color = fillColor;
        _meshRenderer.enabled = true;
        _lineRenderer.enabled = false;
    }

    protected void ShowFill(List<Vector3> vertices, Color fillColor)
    {
        EnsureRenderers();

        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }

        UpdateMesh(vertices);
        _fillMaterial.color = fillColor;
        _meshRenderer.enabled = true;
        _lineRenderer.enabled = false;
    }

    protected void HideAfter(float duration)
    {
        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideAfterDelay(Mathf.Max(0f, duration)));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _hideRoutine = null;
        Hide();
    }


    // 각종 메쉬나 렌더러 검사용도
    private void EnsureRenderers()
    {
        if (_meshFilter == null)
            _meshFilter = GetOrAdd<MeshFilter>();

        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.name = "VisibleAttackMesh";
            _mesh.hideFlags = HideFlags.DontSave;
            _meshFilter.sharedMesh = _mesh;
        }

        if (_meshRenderer == null)
        {
            _meshRenderer = GetOrAdd<MeshRenderer>();
            _meshRenderer.sortingLayerName = _sortingLayerName;
            _meshRenderer.sortingOrder = _sortingOrder;
        }

        if (_lineRenderer == null)
        {
            _lineRenderer = GetOrAdd<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.widthMultiplier = 0.05f;
            _lineRenderer.numCapVertices = 0;
            _lineRenderer.numCornerVertices = 0;
            _lineRenderer.sortingLayerName = _sortingLayerName;
            _lineRenderer.sortingOrder = _sortingOrder + 1;
        }

        if (_fillMaterial == null)
        {
            _fillMaterial = CreateRuntimeMaterial();
            _meshRenderer.material = _fillMaterial;
        }

        if (_lineMaterial == null)
        {
            _lineMaterial = CreateRuntimeMaterial();
            _lineRenderer.material = _lineMaterial;
        }
    }

    private void UpdateMesh(Vector3[] vertices)
    {
        _mesh.Clear();
        _mesh.vertices = vertices;
        UpdateTriangles(vertices.Length);
        _mesh.RecalculateBounds();
    }

    private void UpdateMesh(List<Vector3> vertices)
    {
        _mesh.Clear();
        _mesh.SetVertices(vertices);
        UpdateTriangles(vertices.Count);
        _mesh.RecalculateBounds();
    }

    private void UpdateTriangles(int vertexCount)
    {
        if (vertexCount == 3)
        {
            _mesh.triangles = new[] { 0, 1, 2 };
        }
        else if (vertexCount == 4)
        {
            _mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        }
        else if (vertexCount % 4 == 0)
        {
            int quadCount = vertexCount / 4;
            int[] triangles = new int[quadCount * 6];
            for (int i = 0; i < quadCount; i++)
            {
                int vertexIndex = i * 4;
                int triangleIndex = i * 6;
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + 2;
                triangles[triangleIndex + 3] = vertexIndex;
                triangles[triangleIndex + 4] = vertexIndex + 2;
                triangles[triangleIndex + 5] = vertexIndex + 3;
            }

            _mesh.triangles = triangles;
        }
        else
        {
            _mesh.triangles = System.Array.Empty<int>();
        }
    }

    private T GetOrAdd<T>() where T : Component
    {
        if (TryGetComponent(out T component))
            return component;

        return gameObject.AddComponent<T>();
    }

    private Vector3[] ToLocalVertices(Vector3[] worldVertices)
    {
        Vector3[] localVertices = new Vector3[worldVertices.Length];
        for (int i = 0; i < worldVertices.Length; i++)
            localVertices[i] = transform.InverseTransformPoint(worldVertices[i]);

        return localVertices;
    }

    private List<Vector3> ToLocalVertices(List<Vector3> worldVertices)
    {
        _cachedLocalVertices.Clear();
        if (_cachedLocalVertices.Capacity < worldVertices.Count)
            _cachedLocalVertices.Capacity = worldVertices.Count;

        for (int i = 0; i < worldVertices.Count; i++)
            _cachedLocalVertices.Add(transform.InverseTransformPoint(worldVertices[i]));

        return _cachedLocalVertices;
    }

    private Material CreateRuntimeMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");

        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }

    protected static Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    protected Vector3[] GetBoxVertices(Vector2 center, Vector2 size)
    {
        Vector2 halfSize = new Vector2(Mathf.Max(0f, size.x), Mathf.Max(0f, size.y)) * 0.5f;
        _cachedBoxVertices[0] = new Vector3(center.x - halfSize.x, center.y - halfSize.y, 0f);
        _cachedBoxVertices[1] = new Vector3(center.x + halfSize.x, center.y - halfSize.y, 0f);
        _cachedBoxVertices[2] = new Vector3(center.x + halfSize.x, center.y + halfSize.y, 0f);
        _cachedBoxVertices[3] = new Vector3(center.x - halfSize.x, center.y + halfSize.y, 0f);
        return _cachedBoxVertices;
    }

    private void DestroyRuntimeMaterial(Material material)
    {
        if (material == null)
            return;

        if (Application.isPlaying)
            Destroy(material);
        else
            DestroyImmediate(material);
    }

    private void DestroyRuntimeMesh(Mesh mesh)
    {
        if (mesh == null)
            return;

        if (Application.isPlaying)
            Destroy(mesh);
        else
            DestroyImmediate(mesh);
    }
}
