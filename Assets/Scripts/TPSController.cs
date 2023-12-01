using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSController : MonoBehaviour
{
    private Transform _camera;
    private CharacterController _controller;
    [SerializeField] private float _playerSpeed = 0;
    [SerializeField] private float _jumpForce = 0;
    [SerializeField] private float _gravity = -9.81f;
    private Vector3 _playerGravity;
    private Animator _anim;
    private float _horizontal;
    private float _vertical;
    private float _turnSmoothVelocity;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private Transform _sensorPosition;
    [SerializeField] private float _sensorRadius = 0;
    [SerializeField] private LayerMask _groundLayer;

    private bool _isGrounded;

    
    // Start is called before the first frame update
    void Awake()
    {
        _controller = gameObject.GetComponent<CharacterController>();
        _anim = gameObject.GetComponentInChildren<Animator>();
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        Movement();
        Jump();
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_horizontal, 0, _vertical);
        _controller.Move(direction * Time.deltaTime * _playerSpeed);

        _anim.SetFloat("VelX", 0);
        _anim.SetFloat("VelZ", direction.magnitude);

        if(direction != Vector3.zero)
        {
            float _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float _smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, _smoothAngle, 0);
            Vector3 moveDirection = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
            _controller.Move(moveDirection.normalized * _playerSpeed * Time.deltaTime);
        }

    }

    void Jump()
    {
        _isGrounded = Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);

        _playerGravity.y += _gravity * Time.deltaTime;
        _controller.Move(_playerGravity * Time.deltaTime);

        if(_isGrounded && _playerGravity.y < 0)
        {
            _playerGravity.y = -2;
            _anim.SetBool("IsJumping", !_isGrounded);
        }
        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            _playerGravity.y = _jumpForce;
            _playerGravity.y = Mathf.Sqrt(_jumpForce * -2 * _gravity);
        }
    }
}
