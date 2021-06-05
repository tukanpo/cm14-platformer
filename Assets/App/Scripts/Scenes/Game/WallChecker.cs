using UnityEngine;

namespace App.Scenes.Game
{
    public class WallChecker : MonoBehaviour
    {
        [SerializeField] LayerMask _wallLayer;
        [SerializeField] float _wallRaycastLength;

        bool _onWall;
        bool _onRightWall;
        
        public bool CheckCollisions()
        {
            _onWall = Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer) ||
                      Physics2D.Raycast(transform.position, Vector2.left, _wallRaycastLength, _wallLayer);
            _onRightWall = Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer);

            return true;
        }

        public bool OnWall()
        {
            return Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer) ||
                   Physics2D.Raycast(transform.position, Vector2.left, _wallRaycastLength, _wallLayer);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _wallRaycastLength);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * _wallRaycastLength);
        }
    }
}
