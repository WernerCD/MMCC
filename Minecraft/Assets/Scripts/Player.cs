using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float PlayerHeight = 2f; // 1.8f plus room for error
    public float PlayerWidth = 0.15f;
    public float WalkSpeed = 3f;
    public float SprintSpeed = 6f;
    public float JumpForce = 5f;
    public float Gravity = -9.8f;
    public float CheckIncrement = 0.01f;
    public float Reach = 8f;

    public Transform HighlightBlock;
    public Transform PlaceBlock;

    public Text SelectedBlockText;
    public byte SelectedBlockIndex = BlockTypeEnums.Dirt.ToByte();

    private World _world;
    private Transform _cam;

    internal Vector3 _velocity;

    private float _horizontal;
    private float _vertical;
    private float _mouseHorizontal;
    private float _mouseVertical;
    private float _verticalMomentum = 0;
    private bool _isGrounded;
    private bool _isSprinting;
    private bool _jumpRequest;


    void Start()
    {
        _cam = GameObject.Find("Main Camera").transform;
        _world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Locked;
        SelectedBlockText.text = $"{_world.BlockTypes[SelectedBlockIndex].BlockName} block selected";
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if (_jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * _mouseHorizontal);
        _cam.Rotate(Vector3.right * -_mouseVertical);
        transform.Translate(_velocity, Space.World);
    }

    void Update()
    {
        GetPlayerInputs();
        PlaceCursorBlocks();
    }

    private void Jump()
    {
        _verticalMomentum = JumpForce;
        _isGrounded = false;
        _jumpRequest = false;
    }

    private void CalculateVelocity()
    {
        // Affect vertical momentum with gravity
        if (_verticalMomentum > Gravity)
            _verticalMomentum += Time.fixedDeltaTime * Gravity;

        // Sprinting
        if (_isSprinting)
            _velocity = ((transform.forward * _vertical) + (transform.right * _horizontal)) * Time.fixedDeltaTime * SprintSpeed;
        else
            _velocity = ((transform.forward * _vertical) + (transform.right * _horizontal)) * Time.fixedDeltaTime * WalkSpeed;

        // Vertical Momentum Application
        _velocity += Vector3.up * _verticalMomentum * Time.fixedDeltaTime;

        if ((_velocity.z > 0 && FrontBlocked) || (_velocity.z < 0 && BackBlocked)) 
            _velocity.z = 0;
        if ((_velocity.x > 0 && LeftBlocked) || (_velocity.x < 0 && RightBlocked))
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

        if (Input.GetButtonDown("Sprint"))
            _isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            _isSprinting = false;

        if (_isGrounded && Input.GetButtonDown("Jump"))
            _jumpRequest = true;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0)
                SelectedBlockIndex++;
            else
                SelectedBlockIndex--;
            if (SelectedBlockIndex > (byte)(_world.BlockTypes.Length - 1))
                SelectedBlockIndex = 1;
            if (SelectedBlockIndex < 1)
                SelectedBlockIndex = (byte)(_world.BlockTypes.Length - 1);

            SelectedBlockText.text = $"{_world.BlockTypes[SelectedBlockIndex].BlockName} block selected";
        }

        if (HighlightBlock.gameObject.activeSelf)
        {
            // Destroy Block
            if (Input.GetMouseButtonDown(0))
                _world.GetChunkFromVector3(HighlightBlock.position).EditVoxel(HighlightBlock.position, 0);

            // Place Block
            if (Input.GetMouseButtonDown(1))
                _world.GetChunkFromVector3(PlaceBlock.position).EditVoxel(PlaceBlock.position, SelectedBlockIndex);
        }
    }

    private void PlaceCursorBlocks()
    {
        float step = CheckIncrement;
        Vector3 lastPos = new Vector3();

        while (step < Reach)
        {
            Vector3 pos = _cam.position + (_cam.forward * step);
            if (_world.CheckForVoxel(pos))
            {
                HighlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                PlaceBlock.position = lastPos;
                
                HighlightBlock.gameObject.SetActive(true);
                PlaceBlock.gameObject.SetActive(true);
                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += CheckIncrement;
        }
        HighlightBlock.gameObject.SetActive(false);
        PlaceBlock.gameObject.SetActive(false);
    }

    private float CheckDownSpeed(float downSpeed)
    {
        if (_world.CheckForVoxel(new Vector3(transform.position.x - PlayerWidth, transform.position.y + downSpeed, transform.position.z - PlayerWidth)) ||
            _world.CheckForVoxel(new Vector3(transform.position.x + PlayerWidth, transform.position.y + downSpeed, transform.position.z - PlayerWidth)) ||
            _world.CheckForVoxel(new Vector3(transform.position.x - PlayerWidth, transform.position.y + downSpeed, transform.position.z + PlayerWidth)) ||
            _world.CheckForVoxel(new Vector3(transform.position.x + PlayerWidth, transform.position.y + downSpeed, transform.position.z + PlayerWidth)))
        {
            _isGrounded = true;
            return 0f;
        }
        else
        {
            return downSpeed;
        }
    }

    private float CheckUpSpeed(float upSpeed)
    {
        if (_world.CheckForVoxel(new Vector3(transform.position.x - PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z - PlayerWidth)) ||
            _world.CheckForVoxel(new Vector3(transform.position.x + PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z - PlayerWidth)) ||
            _world.CheckForVoxel(new Vector3(transform.position.x - PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z + PlayerWidth)) ||
            _world.CheckForVoxel(new Vector3(transform.position.x + PlayerWidth, transform.position.y + PlayerHeight + upSpeed, transform.position.z + PlayerWidth)))
        {
            return 0f;
        }
        else
        {
            return upSpeed;
        }
    }

    public bool FrontBlocked =>
        _world.CheckForVoxel(new Vector3(x: transform.position.x, y: transform.position.y, z: transform.position.z + PlayerWidth)) ||
        _world.CheckForVoxel(new Vector3(x: transform.position.x, y: transform.position.y + 1f, z: transform.position.z + PlayerWidth));
    public bool BackBlocked =>
        _world.CheckForVoxel(new Vector3(x: transform.position.x, y: transform.position.y, z: transform.position.z - PlayerWidth)) ||
        _world.CheckForVoxel(new Vector3(x: transform.position.x, y: transform.position.y + 1f, z: transform.position.z - PlayerWidth));
    public bool LeftBlocked =>
        _world.CheckForVoxel(new Vector3(x: transform.position.x - PlayerWidth, y: transform.position.y, z: transform.position.z)) ||
        _world.CheckForVoxel(new Vector3(x: transform.position.x - PlayerWidth, y: transform.position.y + 1f, z: transform.position.z));
    public bool RightBlocked =>
        _world.CheckForVoxel(new Vector3(x: transform.position.x + PlayerWidth, y: transform.position.y, z: transform.position.z)) ||
        _world.CheckForVoxel(new Vector3(x: transform.position.x + PlayerWidth, y: transform.position.y + 1f, z: transform.position.z));
}
