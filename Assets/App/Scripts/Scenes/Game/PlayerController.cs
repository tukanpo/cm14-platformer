using UnityEngine;
using UnityEngine.UI;

namespace App.Scenes.Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Text _velocityText;
        [SerializeField] Text _hangtimeText;
        [SerializeField] Text _gravityScaleText;

        [SerializeField] float _gravityScale = 8.5f;
        
        [Header("Movement")]
        [SerializeField] float _movementAcceleration = 15f;
        [SerializeField] float _maxMoveSpeed = 7f;
        [SerializeField] float _movementDeceleration = 8f;

        [Header("Ground Check")]
        [SerializeField] public bool _isGrounded;

        [Header("Wall Checker")]
        [SerializeField] public bool _onWall;
        
        [Header("Jump")]
        [SerializeField] float _jumpForce = 20f;
        [SerializeField] float _fallMultiplier = 8f;
        [SerializeField] float _hangTime = 0.05f;

        [Header("Test")]
        [SerializeField] bool _isCornerCorrect;

        [SerializeField] float _wallSlideTime = 1f;
        
        bool _canCornerCorrect;
        
        Rigidbody2D _rb;
        Vector2 _inputDirection;
        bool _jumpRequest;
        float _hangTimeCounter;

        GroundChecker _groundChecker;
        CornerCorrector _cornerCorrector;
        WallChecker _wallChecker;
        
        float _gravityScaleModifier;

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
            
            // 上昇中にジャンプボタンを離したら小ジャンプ扱いにして落下させる
            if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0)
            {
                var velocity = _rb.velocity;
                _rb.velocity = new Vector2(velocity.x, velocity.y * 0.5f);
            }
        }

        void FixedUpdate()
        {
            _isGrounded = _groundChecker.IsGrounded();
            _canCornerCorrect = _cornerCorrector.CheckCollisions();
            _onWall = _wallChecker.OnWall();

            // 徐々に加速して移動
            // _rb.AddForce(Vector2.right * (_inputDirection.x * _movementAcceleration));
            _rb.velocity += Vector2.right * (_inputDirection.x * _movementAcceleration * Time.deltaTime) / _rb.mass;
            
            // 最高速に到達したら速度を保つ
            if (Mathf.Abs(_rb.velocity.x) > _maxMoveSpeed)
            {
                // Mathf.Sign は符号（正か 0 の場合は 1 を、負の場合は -1）を返す
                _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * _maxMoveSpeed, _rb.velocity.y);
            }

            // ジャンプ！
            if (_isGrounded)
            {
                _hangTimeCounter = _hangTime;
            }
            else
            {
                _hangTimeCounter -= Time.deltaTime;
            }

            if (_jumpRequest)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                _hangTimeCounter = 0;
                _jumpRequest = false;
            }

            var velocity = _rb.velocity;

            var isChangingDirection = _inputDirection.x > 0 && velocity.x < 0 || _inputDirection.x < 0 && velocity.x > 0;

            // しきい値(0.4f)はアナログスティック入力を考慮した適当な値
            if (Mathf.Abs(_inputDirection.x) < 0.4f || isChangingDirection)
            {
                // 移動の減速処理
                velocity = new Vector2(velocity.x * (1 - Time.deltaTime * _movementDeceleration), velocity.y);
            }

            // if (!_isGrounded)
            // {
            //     if (_rb.velocity.y < 0)
            //     {
            //         // ジャンプからの落下中は落下用重力を適用
            //         velocity += Vector2.up * (Physics.gravity.y * _fallMultiplier * Time.deltaTime);
            //     }
            // }

            // コーナーでのジャンプ補正
            if (_canCornerCorrect && !_isGrounded)
            {
                if (!_isCornerCorrect)
                    return;

                // ここ position いじってるけど外部でやっていい？
                _cornerCorrector.CornerCorrect();
            }

            _gravityScaleModifier = 1;
            
            // Wall Slide
            if (_onWall && !_isGrounded && _rb.velocity.y < 0f)
            {
                velocity = new Vector2(velocity.x, 0);
                _gravityScaleModifier = 0.5f;
            }

            // 重力処理
            velocity += Vector2.up * (Physics.gravity.y * (_gravityScale * _gravityScaleModifier) * Time.deltaTime);

            _rb.velocity = velocity;
            
            UpdateDebugTexts();
        }

        void UpdateDebugTexts()
        {
            _velocityText.text = $"{_rb.velocity}";
            _hangtimeText.text = $"{_hangTimeCounter}";
            _gravityScaleText.text = $"{_gravityScale * _gravityScaleModifier}";
        }
    }
}
