using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private Vector2 _direction = Vector2.zero;
    private Vector2[] _boundaryVertices = new Vector2[0];

    [SerializeField]
    private float Speed;
    [SerializeField]
    private float CollisionSkin = 0.001f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Translate();
    }

    public void Activate(Vector2 targetPosition, Vector2[] boundaryVertices) 
    {
        _direction = (targetPosition - (Vector2)transform.position).normalized;
        _boundaryVertices = boundaryVertices ?? new Vector2[0];
    }

    private void Translate() 
    {
        if (_direction == Vector2.zero)
            return;

        var currentPosition = (Vector2)transform.position;
        var remainingDistance = Speed * Time.deltaTime;
        var bounceCount = 0;

        while (remainingDistance > 0f && bounceCount < 3)
        {
            var targetPosition = currentPosition + (_direction * remainingDistance);

            if (!TryGetNearestIntersection(currentPosition, targetPosition, out var hitPoint, out var edgeNormal, out var distanceTravelled))
            {
                currentPosition = targetPosition;
                break;
            }

            currentPosition = hitPoint + (edgeNormal * CollisionSkin);
            remainingDistance -= distanceTravelled;
            _direction = Vector2.Reflect(_direction, edgeNormal).normalized;
            bounceCount++;
        }

        transform.position = currentPosition;
    }

    private bool TryGetNearestIntersection(
        Vector2 moveStart,
        Vector2 moveEnd,
        out Vector2 hitPoint,
        out Vector2 edgeNormal,
        out float distanceTravelled)
    {
        hitPoint = Vector2.zero;
        edgeNormal = Vector2.zero;
        distanceTravelled = 0f;

        if (_boundaryVertices == null || _boundaryVertices.Length < 2)
            return false;

        var movement = moveEnd - moveStart;
        var movementLength = movement.magnitude;

        if (movementLength <= Mathf.Epsilon)
            return false;

        var hasHit = false;
        var nearestT = float.MaxValue;
        const float epsilon = 0.00001f;

        for (int i = 0; i < _boundaryVertices.Length; i++)
        {
            var edgeStart = _boundaryVertices[i];
            var edgeEnd = _boundaryVertices[(i + 1) % _boundaryVertices.Length];

            if (!TryGetSegmentIntersection(moveStart, moveEnd, edgeStart, edgeEnd, out var t, out var _))
                continue;

            if (t <= epsilon || t >= 1f)
                continue;

            if (t < nearestT)
            {
                nearestT = t;
                var edge = edgeEnd - edgeStart;
                edgeNormal = new Vector2(-edge.y, edge.x).normalized;
                hitPoint = moveStart + (movement * t);
                hasHit = true;
            }
        }

        if (!hasHit)
            return false;

        distanceTravelled = movementLength * nearestT;
        return true;
    }

    private bool TryGetSegmentIntersection(
        Vector2 p1,
        Vector2 p2,
        Vector2 q1,
        Vector2 q2,
        out float t,
        out float u)
    {
        t = 0f;
        u = 0f;

        var r = p2 - p1;
        var s = q2 - q1;
        var denominator = Cross(r, s);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
            return false;

        var qMinusP = q1 - p1;
        t = Cross(qMinusP, s) / denominator;
        u = Cross(qMinusP, r) / denominator;

        return t >= 0f && t <= 1f && u >= 0f && u <= 1f;
    }

    private float Cross(Vector2 a, Vector2 b)
    {
        return (a.x * b.y) - (a.y * b.x);
    }
}
