using UnityEngine;

namespace App.Scenes.Game
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Vector2 _boxSize;
        [SerializeField] Collider2D _collider;
        
        RaycastHit2D _hit;
        
        public bool IsGrounded()
        {
            var bounds = _collider.bounds;
            _hit = Physics2D.BoxCast(bounds.center, bounds.size, 0, Vector2.down, 0.1f, _groundLayer);
            return _hit;
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (_hit.collider)
            {
                 Gizmos.DrawWireCube(_hit.point, _boxSize);
            }
        }
    }
}
