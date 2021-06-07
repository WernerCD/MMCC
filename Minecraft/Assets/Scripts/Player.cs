using UnityEngine;

public class Player : MonoBehaviour
{
    public float WalkSpeed = 3f;
    public float SprintSpeed = 6f;
    public float JumpForce = 5f;
    public float Gravity = -9.8f;
    
    public bool IsGrounded;
    public bool IsSprinting;

    private World _world;
    private Transform _cam;

    public float PlayerHeight = 2f; // 1.8f plus room for error
    public float PlayerWidth = 0.15f;
    public float BoundsTolerance = 0.1f;

    private float _horizontal;
    private float _vertical;
    private float _mouseHorizontal;
    private float _mouseVertical;
    private float _jump;
    private float _sprint;
    private Vector3 _velocity;
    private float _verticalMomentum;
    private bool _jumpRequest;


    // Start is called before the first frame update
    void Start()
    {
        _cam = GameObject.Find("Main Camera").transform;
        _world = GameObject.Find("World").GetComponent<World>();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if (_jumpRequest) Jump();

        transform.Rotate(Vector3.up * _mouseHorizontal);
        _cam.Rotate(Vector3.right * -_mouseVertical);
        transform.Translate(_velocity, Space.World);
    }
    private void Jump()
    {
        _verticalMomentum = JumpForce;
        IsGrounded = false;
        _jumpRequest = false;
    }

    private void CalculateVelocity()
    {
        // Affect vertical momentum with gravity
        if (_verticalMomentum > Gravity)
            _verticalMomentum += Time.fixedDeltaTime * Gravity;

        // Sprinting
        if (IsSprinting)
            _velocity = (transform.forward * _vertical) + (transform.right * _horizontal) * Time.deltaTime * SprintSpeed;
        else
            _velocity = (transform.forward * _vertical) + (transform.right * _horizontal) * Time.deltaTime * WalkSpeed;

        // Vertical Momentum Application
        _velocity += Vector3.up * _verticalMomentum * Time.fixedDeltaTime;

        if ((_velocity.z > 0 && FrontBlocked) || (_velocity.z > 0 && BackBlocked))
            _velocity.z = 0;
        if ((_velocity.x > 0 && LeftBlocked) || (_velocity.x > 0 && RightBlocked))
            _velocity.x = 0;

        if (_velocity.y < 0)
            _velocity.y = CheckDownSpeed(_velocity.y);
        else if (_velocity.y > 0)
            _velocity.y = CheckUpSpeed(_velocity.y);


    }

    private void GetPlayerInputs()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _mouseHorizontal = Input.GetAxis("Mouse X");
        _mouseVertical = Input.GetAxis("Mouse Y");
        _jump = Input.GetAxis("Jump");
        _sprint = Input.GetAxis("Sprint");

        if (Input.GetButtonDown("Sprint"))
            IsSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            IsSprinting = false;

        if (IsGrounded && Input.GetButtonDown("Jump"))
            _jumpRequest = true;
    }

    private float CheckDownSpeed(float downSpeed)
    {
        if (_world.CheckForVoxel(transform.position.x - PlayerWidth, transform.position.y + downSpeed, transform.position.z - PlayerWidth) ||
            _world.CheckForVoxel(transform.position.x + PlayerWidth, transform.position.y + downSpeed, transform.position.z - PlayerWidth) ||
            _world.CheckForVoxel(transform.position.x - PlayerWidth, transform.position.y + downSpeed, transform.position.z + PlayerWidth) ||
            _world.CheckForVoxel(transform.position.x + PlayerWidth, transform.position.y + downSpeed, transform.position.z + PlayerWidth))
        {
            IsGrounded = true;
            return 0f;
        }
        else
        {
            return downSpeed;
        }

    }
    private float CheckUpSpeed(float upSpeed)
    {
        if (_world.CheckForVoxel(transform.position.x - PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z - PlayerWidth) ||
            _world.CheckForVoxel(transform.position.x + PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z - PlayerWidth) ||
            _world.CheckForVoxel(transform.position.x - PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z + PlayerWidth) ||
            _world.CheckForVoxel(transform.position.x + PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z + PlayerWidth))
        {
            IsGrounded = true;
            return 0f;
        }
        else
        {
            return upSpeed;
        }

    }

    public bool FrontBlocked =>
        _world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z + PlayerWidth) ||
        _world.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z + PlayerWidth);
    public bool BackBlocked =>
        _world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z - PlayerWidth) ||
        _world.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z - PlayerWidth);
    public bool LeftBlocked =>
        _world.CheckForVoxel(transform.position.x - PlayerWidth, transform.position.y, transform.position.z) ||
        _world.CheckForVoxel(transform.position.x - PlayerWidth, transform.position.y + 1f, transform.position.z);
    public bool RightBlocked =>
        _world.CheckForVoxel(transform.position.x + PlayerWidth, transform.position.y, transform.position.z + PlayerWidth) ||
        _world.CheckForVoxel(transform.position.x + PlayerWidth, transform.position.y + 1f, transform.position.z - PlayerWidth);
}
