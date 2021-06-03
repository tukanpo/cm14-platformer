using UnityEngine;
using UnityEngine.UI;

namespace App.Scenes.Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Text _velocityText;
        [SerializeField] Text _hangtimeText;
        
        [Header("Movement")]
        [SerializeField] float _movementAcceleration = 15f;
        [SerializeField] float _maxMoveSpeed = 7f;
        [SerializeField] float _movementDeceleration = 8f;

        [Header("Ground Check")]
        [SerializeField] public bool _isGrounded;

        [Header("Jump")]
        [SerializeField] float _jumpForce = 20f;
        [SerializeField] float _fallMultiplier = 8f;
        [SerializeField] float _hangTime = 0.05f;

        [SerializeField] float _topRaycastLength;
        [SerializeField] Vector3 _edgeRaycastOffset;
        [SerializeField] Vector3 _innerRaycastOffset;
        [SerializeField] LayerMask _groundLayer;
        bool _canCornerCorrect;        
        
        Rigidbody2D _rb;
        Vector2 _inputDirection;
        GroundChecker _groundChecker;
        bool _jumpRequest;
        float _hangTimeCounter;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _groundChecker = GetComponent<GroundChecker>();
        }

        void Update()
        {
            _inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            _isGrounded = _groundChecker.IsGrounded();

            if (_isGrounded)
            {
                _hangTimeCounter = _hangTime;
            }
            else
            {
                _hangTimeCounter -= Time.deltaTime;
            }
            
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
            CheckCollisions();
            
            // 徐々に加速させる為、AddForce (ForceMode2D.Force) を使う
            //_rb.AddForce(Vector2.right * (_inputDirection.x * _movementAcceleration));
            _rb.velocity += (Vector2.right * (_inputDirection.x * _movementAcceleration)) * Time.fixedDeltaTime / _rb.mass;
            
            // 最高速に到達したら速度を保つ
            if (Mathf.Abs(_rb.velocity.x) > _maxMoveSpeed)
            {
                // Mathf.Sign は符号（正か 0 の場合は 1 を、負の場合は -1）を返す
                _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * _maxMoveSpeed, _rb.velocity.y);
            }

            if (_jumpRequest)
            {
                // ジャンプ！
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


            if (_canCornerCorrect)
            {
                CornerCorrect(_rb.velocity.y);
            }
            
            
            // 独自の重力処理
            // velocity += Vector2.up * (Physics.gravity.y * Time.deltaTime);

            _rb.velocity = velocity;
            
            UpdateDebugTexts();
        }

        void CornerCorrect(float yVelocity)
        {
            // Push player to the right
            var hit = Physics2D.Raycast(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength,
                Vector3.left, _topRaycastLength, _groundLayer);
            if (hit.collider != null)
            {
                var newPos = Vector3.Distance(
                    new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLength,
                    transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);
                transform.position = new Vector3(transform.position.x + newPos, transform.position.y,
                    transform.position.z);
                _rb.velocity = new Vector2(_rb.velocity.x, yVelocity);
            }

            // Push player to the left
            hit = Physics2D.Raycast(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength,
                Vector3.right, _topRaycastLength, _groundLayer);
            if (hit.collider != null)
            {
                var newPos = Vector3.Distance(
                    new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLength,
                    transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
                transform.position = new Vector3(transform.position.x - newPos, transform.position.y,
                    transform.position.z);
                _rb.velocity = new Vector2(_rb.velocity.x, yVelocity);
            }
        }

        void CheckCollisions()
        {
            _canCornerCorrect =
                Physics2D.Raycast(transform.position + _edgeRaycastOffset, Vector2.up, _topRaycastLength, _groundLayer) &&
                !Physics2D.Raycast(transform.position + _innerRaycastOffset, Vector2.up, _topRaycastLength, _groundLayer) ||
                Physics2D.Raycast(transform.position - _edgeRaycastOffset, Vector2.up, _topRaycastLength, _groundLayer) &&
                !Physics2D.Raycast(transform.position - _innerRaycastOffset, Vector2.up, _topRaycastLength, _groundLayer);
        }

        void UpdateDebugTexts()
        {
            _velocityText.text = $"{_rb.velocity}";
            _hangtimeText.text = $"{_hangTimeCounter}";
        }
    }
}
