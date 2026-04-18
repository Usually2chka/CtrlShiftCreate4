using System.Collections;
using UnityEngine;

namespace _Bifrost.Runtime.Managers.GamePlay
{
    public class SocketPortal : InteractiveObject
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _placePoint; // куда вставляется предмет
        private InteractiveObject _current;

        public bool CanInsert(InteractiveObject obj)
        {
            return _current == null; // можно расширить (тип кристалла и т.д.)
        }

        public void Insert(InteractiveObject obj)
        {
            _current = obj;
            StartCoroutine(InsertAnimation(obj));
        }
        
        public InteractiveObject Take()
        {
            if (_current == null) return null;

            var obj = _current;
            _current = null;

            obj.transform.SetParent(null);
            TakeAnimation(obj, _playerTransform);
            return obj;
        }
        
        public bool HasItem()
        {
            return _current != null;
        }
        
        private IEnumerator InsertAnimation(InteractiveObject obj)
        {
            obj.gameObject.SetActive(true);

            Vector3 startPos = obj.transform.position;
            Vector3 endPos = _placePoint.position;

            float time = 0f;
            float duration = 0.3f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                obj.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            
            obj.transform.position = endPos;
            obj.transform.SetParent(_placePoint);
        }
        
        private IEnumerator TakeAnimation(InteractiveObject obj, Transform player)
        {
            Vector3 start = obj.transform.position;
            Vector3 end = player.position + player.forward * 1.5f;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                obj.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }
    }
}