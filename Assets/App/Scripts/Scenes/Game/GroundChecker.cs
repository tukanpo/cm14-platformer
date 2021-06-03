using UnityEngine;
using UnityEngine.Serialization;

namespace App.Scenes.Game
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Vector2 _boxSize;
        [SerializeField] float _boxOffsetY;
        
        bool _initialized;
        Collider2D _collider;

        public bool IsGrounded()
        {
            var position = _collider.transform.position;
            return Physics2D.OverlapBox(
                new Vector2(position.x, position.y + _boxOffsetY),
                _boxSize, 0, _groundLayer);
        }
        
        void Start()
        {
            _initialized = true;
            _collider = GetComponent<CapsuleCollider2D>();
        }
        
        void OnDrawGizmos()
        {
            if (!_initialized)
            {
                return;
            }

            Gizmos.color = Color.red;
            var position = _collider.transform.position;
            Gizmos.DrawWireCube(new Vector2(position.x, position.y + _boxOffsetY), _boxSize);
        }
    }
}
