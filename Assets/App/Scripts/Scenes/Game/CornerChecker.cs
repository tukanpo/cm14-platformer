using UnityEngine;

namespace App.Scenes.Game
{
    public class CornerChecker : MonoBehaviour
    {
        [SerializeField] float _topRaycastLength;
        [SerializeField] Vector3 _edgeRaycastOffset;
        [SerializeField] Vector3 _innerRaycastOffset;

        bool _canCornerCorrect;
        
        
    }
}
