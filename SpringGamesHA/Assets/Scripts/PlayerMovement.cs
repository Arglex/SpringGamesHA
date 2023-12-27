using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AfterImage _afterImage;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _dashTiltSpeed = 400f;
    [SerializeField] private Transform _leftCorner;
    [SerializeField] private Transform _rightCorner;
    [SerializeField] private Transform _rotationTransform;
    [SerializeField] private float _tiltSpeed;
    [SerializeField] private float _tiltAmount;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float _dashCooldown = 4f;
    [SerializeField] private float _activeDashTimer = 1f;

    private float _currentZRotation = 0.0f;
    private float _currentSpeed;
    private bool _dashReady = true;
    private bool _isDashing = false;
    private float _currentlTiltSpeed;
    private float _currentDashCoolDown;
    private float _currentActiveDashTimer;

    private void Start()
    {
        _currentSpeed = _speed;
        _currentlTiltSpeed = _tiltSpeed;
        _currentDashCoolDown = _dashCooldown;
        _currentActiveDashTimer = _activeDashTimer;
    }

    private void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        Rotate(horizontalAxis);
        Move(horizontalAxis);
        if (_dashReady)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartDash();
            }
        }
        else
        {
            _currentDashCoolDown -= Time.deltaTime;
            if (_currentDashCoolDown <= 0)
            {
                _dashReady = true;
                _currentDashCoolDown = _dashCooldown;
            }
        }

        if (_isDashing)
        {
            _currentActiveDashTimer -= Time.deltaTime;
            if (_currentActiveDashTimer <= 0)
            {
                StopDash();
            }
        }

    }
    
    private void Rotate(float horizontalAxis)
    {
        if (Mathf.Abs(horizontalAxis) > 0.01f)
        {
            float rotationAmount = -horizontalAxis * _currentlTiltSpeed * Time.deltaTime;
            _currentZRotation = Mathf.Clamp(_currentZRotation + rotationAmount, -_tiltAmount, _tiltAmount);
        }
        else
        {
            _currentZRotation = Mathf.Lerp(_currentZRotation, 0, returnSpeed * Time.deltaTime);
        }
        _rotationTransform.rotation = Quaternion.Euler(0, 0, _currentZRotation);
    }
    
    private void Move(float horizontalAxis)
    {
        Vector3 velocity = transform.right * (horizontalAxis * _currentSpeed * Time.deltaTime);
        transform.position += velocity;
        if (_leftCorner != null && _leftCorner != null)
        {
            Vector3 position = transform.position;
            float clampedXPosition = Mathf.Clamp(position.x, _leftCorner.position.x, _rightCorner.position.x);
            position = new Vector3(clampedXPosition, position.y, position.z);
            transform.position = position;
        }
    }

    private void StartDash()
    {
        _currentSpeed = _dashSpeed;
        _currentlTiltSpeed = _dashTiltSpeed;
        _isDashing = true;
        _dashReady = false;
        _afterImage.StartAfterImageEffect();
    }

    private void StopDash()
    {
        _isDashing = false;
        _currentSpeed = _speed;
        _currentlTiltSpeed = _tiltSpeed;
        _currentActiveDashTimer = _activeDashTimer;
    }
}
