using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Cube : MonoBehaviour
{
    [SerializeField] private float _forceScaler;
    [SerializeField] private float _radiusScaler;

    private int _chanceCount = 1;
    private int _chanceDivisor = 2;
    private int _minCreateValue = 2;
    private int _maxCreateValue = 5;

    private float _newScale = 0.5f;
    private float _explodeRadius = 2f;
    private float _explodeForce = 300f;
    private float _divideForce = 300f;

    private bool _isDivided = false;

    public event Action<float> OnRadiusChanged;

    private void OnValidate()
    {
        float minScale = 1.1f;

        if (_forceScaler < minScale)
        {
            _forceScaler = minScale;
        }

        if (_radiusScaler < minScale)
        {
            _radiusScaler = minScale;
        }
    }

    private void OnMouseDown()
    {
        Collider[] colliders;

        if (UnityEngine.Random.Range(0, _chanceCount) == 0)
        {
            _isDivided = true;

            colliders = GetDividedObjects();
        }
        else
        {
            _isDivided = false;

            colliders = GetNearestObjects();

            OnRadiusChanged?.Invoke(_explodeRadius);
        }

        BlowUp(GetExplodableObjects(colliders));

        Destroy(gameObject);
    }

    private void BlowUp(List<Rigidbody> explodableObjects)
    {
        float newForce = _divideForce;

        foreach (var explodableObject in explodableObjects)
        {
            if (_isDivided == false)
            {
                float distance = (explodableObject.transform.position - transform.position).magnitude;

                newForce = GetNewForce(distance);
            }

            explodableObject.AddExplosionForce(newForce, transform.position, _explodeRadius);
        }
    }

    private List<Rigidbody> GetExplodableObjects(Collider[] colliders)
    {
        List<Rigidbody> hits = new List<Rigidbody>();

        foreach (Collider hit in colliders)
        {
            if (hit.attachedRigidbody != null)
            {
                hits.Add(hit.attachedRigidbody);
            }
        }

        return hits;
    }

    private Collider[] GetDividedObjects()
    {
        int count = UnityEngine.Random.Range(_minCreateValue, _maxCreateValue);

        Collider[] colliders = new Collider[count];

        for (int i = 0; i < count; i++)
        {
            colliders[i] = InitializeScaledObject().GetComponent<Collider>();
        }

        return colliders;
    }

    private Collider[] GetNearestObjects()
    {
        return Physics.OverlapSphere(transform.position, _explodeRadius);
    }

    private Cube InitializeScaledObject()
    {
        Cube scaledObject = Instantiate(this);

        scaledObject.transform.localScale *= _newScale;
        scaledObject.Init(_chanceCount, _explodeForce, _explodeRadius);

        return scaledObject;
    }

    private float GetNewForce(float distance)
    {
        float newForce = _explodeForce / distance;

        return _explodeForce < newForce ? _explodeForce : newForce;
    }

    private void Init(int chanceCount, float explodeForce, float explodeRadius)
    {
        DecreaseChance(chanceCount);

        IncreaseForce(explodeForce);
        IncreaseRadius(explodeRadius);
    }

    private void DecreaseChance(int chanceCount) => _chanceCount = chanceCount * _chanceDivisor;

    private void IncreaseRadius(float radius) => _explodeRadius = radius * _radiusScaler;

    private void IncreaseForce(float force) => _explodeForce = force * _forceScaler;
}