using UnityEngine;

public class ExplodeCircle : MonoBehaviour
{
    [SerializeField] private Cube _cube;
    [SerializeField] private SpriteRenderer _circle;

    private float _seconds = 2f;

    private void OnEnable()
    {
        _cube.OnRadiusChanged += Initialize;
    }

    private void OnDisable()
    {
        _cube.OnRadiusChanged -= Initialize;
    }

    private void Initialize(float radius)
    {
        _circle = Instantiate(_circle, transform.position, _circle.transform.rotation);
        _circle.transform.localScale = Vector3.one * radius * 2;

        Destroy(_circle.gameObject, _seconds);
    }
}
