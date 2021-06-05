using TMPro;
using UnityEngine;

namespace App.Scenes.Game
{
    public class CornerCorrector : MonoBehaviour
    {
        [SerializeField] float _topRaycastLength;
        [SerializeField] Vector3 _edgeRaycastOffset;
        [SerializeField] Vector3 _innerRaycastOffset;
        [SerializeField] LayerMask _cornerCorrectLayer;

        [SerializeField] GameObject _playerShadow1;
        [SerializeField] GameObject _playerShadow2;

        // 注意：コライダーが壁に接触する前に検知しないといけないのでセンサーを先に充てる
        // あとジャンプ中かどうか？とか予測が必要かも…？
        // ジャンプ時間とかカーブとか考えないといけないかも？
        // 近すぎる天井対策はジャンプ直後にも補正すればいいんじゃない？
        
        public bool CheckCollisions()
        {
            var position = transform.position;
            return
                Physics2D.Raycast(position + _edgeRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) &&
                !Physics2D.Raycast(position + _innerRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) ||
                Physics2D.Raycast(position - _edgeRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) &&
                !Physics2D.Raycast(position - _innerRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer);
        }
        
        public void CornerCorrect()
        {
            var position = transform.position;

            var hit = Physics2D.Raycast(position - _innerRaycastOffset + Vector3.up * _topRaycastLength,
                Vector3.left, _topRaycastLength, _cornerCorrectLayer);
            if (hit.collider)
            {
                var offset = Vector3.Distance(
                    new Vector3(hit.point.x, position.y, 0f) + Vector3.up * _topRaycastLength,
                    position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);

                // var before = transform.position;
                // _playerShadow1.SetActive(true);
                // _playerShadow1.transform.position = before;
                
                transform.position = new Vector3(position.x + offset, position.y, position.z);
                
                // _playerShadow2.SetActive(true);
                // _playerShadow2.transform.position = transform.position;
                
                return;
            }

            hit = Physics2D.Raycast(
                position + _innerRaycastOffset + Vector3.up * _topRaycastLength,
                Vector3.right, _topRaycastLength, _cornerCorrectLayer);
            if (hit.collider)
            {
                var offset = Vector3.Distance(
                    new Vector3(hit.point.x, position.y, 0f) + Vector3.up * _topRaycastLength,
                    position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
                
                transform.position = new Vector3(position.x - offset, position.y, position.z);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            var position = transform.position;

            Gizmos.DrawLine(position + _edgeRaycastOffset,
                position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
            Gizmos.DrawLine(position - _edgeRaycastOffset,
                position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);
            Gizmos.DrawLine(position + _innerRaycastOffset,
                position + _innerRaycastOffset + Vector3.up * _topRaycastLength);
            Gizmos.DrawLine(position - _innerRaycastOffset,
                position - _innerRaycastOffset + Vector3.up * _topRaycastLength);
            
            Gizmos.DrawLine(position - _innerRaycastOffset + Vector3.up * _topRaycastLength,
                position - _innerRaycastOffset + Vector3.up * _topRaycastLength + Vector3.left * _topRaycastLength);
            Gizmos.DrawLine(position + _innerRaycastOffset + Vector3.up * _topRaycastLength,
                position + _innerRaycastOffset + Vector3.up * _topRaycastLength + Vector3.right * _topRaycastLength);
        }
    }
}
