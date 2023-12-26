using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Transform _leftCorner;
    [SerializeField] private Transform _rightCorner;
    
    private void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        Vector3 velocity = transform.right * (horizontalAxis * _speed * Time.deltaTime);
        transform.position += velocity;
        if (_leftCorner != null && _leftCorner != null)
        {
            Vector3 position = transform.position;
            float clampedXPosition = Mathf.Clamp(position.x, _leftCorner.position.x, _rightCorner.position.x);
            position = new Vector3(clampedXPosition, position.y, position.z);
            transform.position = position;
        }
    }
}
