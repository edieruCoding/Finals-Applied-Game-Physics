using System;
using System.Collections;
using UnityEngine;

public class CupCollider : MonoBehaviour
{
    public static event Action CupColliderEvent;
    private CapsuleCollider _collider;
    private float _timer = 0f;
    private bool _isColliding = false;
    private float _collisionTime = 3f;

    private void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (_isColliding)
        {
            _timer += Time.deltaTime;

            if (_timer >= _collisionTime)
            {
                CupColliderEvent?.Invoke();
                _timer = 0f;
                _isColliding = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            _isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            _isColliding = false;
            _timer = 0f;
        }
    }
}
//ANDRES