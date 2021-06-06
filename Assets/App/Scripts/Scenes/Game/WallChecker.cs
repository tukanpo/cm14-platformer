using UnityEngine;

namespace App.Scenes.Game
{
    public class WallChecker : MonoBehaviour
    {
        [SerializeField] LayerMask _wallLayer;
        [SerializeField] float _wallRaycastLength;
        [SerializeField] Vector3 _wallRaycastOffset;
            
        public bool OnWall()
        {
            return OnLeftWall() || OnRightWall();
        }

        public bool OnLeftWall()
        {
            return Physics2D.Raycast(transform.position + _wallRaycastOffset, Vector2.left, _wallRaycastLength, _wallLayer);
        }

        public bool OnRightWall()
        {
            return Physics2D.Raycast(transform.position + _wallRaycastOffset, Vector2.right, _wallRaycastLength, _wallLayer);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawLine(position + _wallRaycastOffset, position + _wallRaycastOffset + Vector3.right * _wallRaycastLength);
            Gizmos.DrawLine(position + _wallRaycastOffset, position + _wallRaycastOffset + Vector3.left * _wallRaycastLength);
        }
    }
}
