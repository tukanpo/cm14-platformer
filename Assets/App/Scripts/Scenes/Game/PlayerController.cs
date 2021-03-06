using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace App.Scenes.Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Text _velocityText;
        [SerializeField] Text _hangtimeText;
        [SerializeField] Text _gravityScaleText;

        [Header("Gravity")]
        [SerializeField] float _gravityScale = 8.5f;
        
        [Header("Movement")]
        [SerializeField] float _movementAcceleration = 15f;
        [SerializeField] float _maxMoveSpeed = 7f;
        [SerializeField] float _movementDeceleration = 8f;

        [Header("Collision Check")]
        [SerializeField] public bool _isGrounded;
        [SerializeField] public bool _onWall;
        
        [Header("Jump")]
        [SerializeField] float _jumpForce = 20f;
        [SerializeField] float _jumpFallMultiplier = 8f;
        [SerializeField] float _hangTime = 0.05f;

        [Header("WallSlide")]
        [SerializeField] float _wallSlideDeceleration;

        [Header("Test")]
        [SerializeField] bool _isCornerCorrect;

        
        bool _canCornerCorrect;
        
        Rigidbody2D _rb;
        Vector2 _inputDirection;
        bool _jumpRequest;
        float _hangTimeCounter;

        GroundChecker _groundChecker;
        CornerCorrector _cornerCorrector;
        WallChecker _wallChecker;

        float _jumpFallGravityMultiplier;
        float _wallSlideGravityMultiplier;

        bool _isJumping;
        bool _isWallSliding;
        float _wallSlideTimeCounter;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _groundChecker = GetComponent<GroundChecker>();
            _cornerCorrector = GetComponent<CornerCorrector>();
            _wallChecker = GetComponent<WallChecker>();
        }

        void Update()
        {
            _inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

            if (Input.GetButtonDown("Jump") && _hangTimeCounter > 0f)
            {
                _jumpRequest = true;
            }
            
            // ?????????????????????????????????????????????????????????????????????????????????????????????
            if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0)
            {
                var velocity = _rb.velocity;
                _rb.velocity = new Vector2(velocity.x, velocity.y * 0.5f);
            }
        }

        void FixedUpdate()
        {
            var velocity = _rb.velocity;

            _isGrounded = _groundChecker.IsGrounded();
            _canCornerCorrect = _cornerCorrector.CheckCollisions();
            _onWall = _wallChecker.OnWall();

            velocity = Move(velocity);
            velocity = Jump(velocity);
            velocity = WallSlide(velocity);
            velocity = ApplyGravity(velocity);

            _rb.velocity = velocity;
            
            UpdateDebugTexts();
        }

        Vector2 Move(Vector2 velocity)
        {
            // ???????????????????????????????????????????????????????????????
            velocity += Vector2.right * (_inputDirection.x * _movementAcceleration * Time.deltaTime);
            if (Mathf.Abs(velocity.x) > _maxMoveSpeed)
            {
                velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * _maxMoveSpeed, velocity.y);
            }

            // ??????????????????????????????????????????
            var isChangingDirection = _inputDirection.x > 0 && velocity.x < 0 || _inputDirection.x < 0 && velocity.x > 0;

            // ???????????? (0.4f) ???????????????????????????????????????????????????????????????????????????
            if (Mathf.Abs(_inputDirection.x) < 0.4f || isChangingDirection)
            {
                velocity = new Vector2(velocity.x * (1 - Time.deltaTime * _movementDeceleration), velocity.y);
            }

            return velocity;
        }

        Vector2 Jump(Vector2 velocity)
        {
            _jumpFallGravityMultiplier = 1;
            
            // ???????????????
            if (_isGrounded || _onWall)
            {
                _isJumping = false;
                _hangTimeCounter = _hangTime;
            }
            else
            {
                _hangTimeCounter -= Time.deltaTime;
            }

            if (_jumpRequest)
            {
                velocity = new Vector2(velocity.x, 0);
                velocity += Vector2.up * _jumpForce;
                _hangTimeCounter = 0;
                _jumpRequest = false;
                _isJumping = true;
            }

            if (!_isJumping)
            {
                return velocity;
            }

            if (velocity.y < 0)
            {
                // ??????????????????????????????????????????????????????
                _jumpFallGravityMultiplier = _jumpFallMultiplier;
            }

            // ??????????????????????????????
            if (_canCornerCorrect)
            {
                if (_isCornerCorrect)
                {
                    // ?????? transform ????????????????????????????????????????????????
                    _cornerCorrector.CornerCorrect();
                }
            }

            return velocity;
        }

        Vector2 WallSlide(Vector2 velocity)
        {
            // ??????????????????????????????????????????????????????
            //      ????????????????????????????????????

            if (_onWall && !_isGrounded && velocity.y < 0f)
            {
                if (!_isWallSliding)
                {
                    _isWallSliding = true;
                    _isJumping = false;
                    _wallSlideGravityMultiplier = 0;
                    velocity = new Vector2(velocity.x, 0);
                }
                
                // TODO: ????????????????????????
                _wallSlideGravityMultiplier = Mathf.Clamp01(_wallSlideGravityMultiplier + _wallSlideDeceleration * Time.deltaTime);
            }
            else
            {
                _isWallSliding = false;
                _wallSlideGravityMultiplier = 1f;
            }

            return velocity;
        }

        Vector2 ApplyGravity(Vector2 velocity)
        {
            var multiplier = _jumpFallGravityMultiplier *
                             _wallSlideGravityMultiplier;

            // ???????????????
            _finalGravityScale = multiplier;
            
            return velocity + Vector2.up * (Physics.gravity.y * _gravityScale * multiplier * Time.deltaTime);
        }

        float _finalGravityScale;

        void UpdateDebugTexts()
        {
            _velocityText.text = $"{_rb.velocity}";
            _hangtimeText.text = $"{_hangTimeCounter}";
            _gravityScaleText.text = $"{_finalGravityScale}";
        }
    }
}
