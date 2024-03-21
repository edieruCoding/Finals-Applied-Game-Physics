using System; //ANDRES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform startingPos;
    public bool canAim = true;
    public float maxDragDistance = 5f; // Set your max distance here
    public LayerMask dragLayer;
    public LayerMask ballLayer;
    public float forceMultiplier;
    public float drawMultiplier = 1f;

    public LineRenderer lineRenderer;
    public int linePoints = 150;
    public float timeIntervalInPoints = 0.01f;
    
    private Rigidbody _rb;
    private Vector3 initialPosition;
    private bool _isDragging = false;
    private Vector3 _forceDirection;
    private SphereCollider _collider;
    private bool _canDrawTrajectory;


    private void OnEnable()
    {
        GameManager.StartingPosEvent += ResetPosition;
        GameManager.RestartEvent += ResetPositionOnRestart;
    }
    
    private void OnDisable()
    {
        GameManager.StartingPosEvent -= ResetPosition;
        GameManager.RestartEvent -= ResetPositionOnRestart;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        AimingHandler();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition(initialPosition);
        }
    }

    private void FixedUpdate()
    {
        DrawTrajectory();
    }

    private void AimingHandler()
    {
        if (!canAim) return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, ballLayer) && hit.transform == transform)
            {
                _isDragging = true;
                _rb.isKinematic = true;
                _collider.enabled = false;
                
                lineRenderer.enabled = true;
                _canDrawTrajectory = true;
            }
        }

        if (_isDragging && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, dragLayer))
            {
                Vector3 desiredPosition = hit.point;
                
                float distance = Vector3.Distance(desiredPosition, initialPosition);

                if (distance > maxDragDistance)
                {
                    Vector3 direction = (desiredPosition - initialPosition).normalized;
                    desiredPosition = initialPosition + direction * maxDragDistance;
                }
                
                _forceDirection = (initialPosition - transform.position) * forceMultiplier;

                desiredPosition.x = 0;
                _rb.position = desiredPosition;
            }
        }

        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            lineRenderer.enabled = false;
            ResetLineRenderer();
            _canDrawTrajectory = false;
            
            Shoot(_forceDirection);
        }
        
    }

    private void DrawTrajectory()
    {
        if (!_canDrawTrajectory) return;

        Vector3[] positions = new Vector3[linePoints];
        for (int i = 0; i < linePoints; i++)
        {
            float t = i * timeIntervalInPoints;
            // Adjust the force direction by the mass of the object
            Vector3 adjustedForceDirection = _forceDirection / _rb.mass;
            Vector3 pos = transform.position + adjustedForceDirection * t + 0.5f * Physics.gravity * t * t;

            positions[i] = pos;
        }

        lineRenderer.positionCount = linePoints;
        lineRenderer.SetPositions(positions);
    }

    private void ResetLineRenderer()
    {
        lineRenderer.positionCount = 0;
    }


    private void Shoot(Vector3 direction)
    {
        _collider.enabled = true;
        _rb.isKinematic = false;
        _rb.AddForce(direction, ForceMode.Impulse);
        canAim = false;
    }

    private void ResetPosition(Vector3 t)
    {
        _rb.isKinematic = true;
        initialPosition = t;
        _rb.position = t;
        canAim = true;
    }
    
    private void ResetPositionOnRestart()
    {
        _rb.isKinematic = true;
        transform.position = initialPosition;
        canAim = true;
    }
}