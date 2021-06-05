using UnityEngine;

namespace App.Scenes.Game
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] Vector2 _boxSize;
        [SerializeField] float _boxOffsetY;

        public bool IsGrounded()
        {
            var position = transform.position;
            return Physics2D.OverlapBox(
                new Vector2(position.x, position.y + _boxOffsetY),
                _boxSize, 0, _groundLayer);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawWireCube(new Vector2(position.x, position.y + _boxOffsetY), _boxSize);
        }
    }
}
