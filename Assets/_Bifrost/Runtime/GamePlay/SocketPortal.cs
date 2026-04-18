using System.Collections;
using UnityEngine;
using _Bifrost.Runtime.Portals;
using System.Linq;

namespace _Bifrost.Runtime.Managers.GamePlay
{
    public enum SocketType
    {
        Giving,    // дающий сокет
        Receiving  // берущий сокет
    }

    public class SocketPortal : InteractiveObject
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _placePoint; // куда вставляется предмет
        [SerializeField] private Crystal _current;
        [SerializeField] private WorldType[] _acceptedTypes = new WorldType[0]; // разрешенные типы кристаллов
        [SerializeField] private SocketType _socketType = SocketType.Giving; // тип сокета
        [SerializeField] private Portal _linkedPortal; // связанный портал

        public WorldType[] AcceptedTypes => _acceptedTypes;
        public SocketType SocketType => _socketType;
        public Portal LinkedPortal => _linkedPortal;

        public bool CanInsert(Crystal crystal)
        {
            if (_current != null) return false; // сокет уже занят
            
            // проверяем, что портал открыт
            if (_linkedPortal != null && _linkedPortal.state == PortalState.Closed) return false;
            
            // если не указаны разрешенные типы, принимаем любой
            if (_acceptedTypes.Length == 0) return true;
            
            // проверяем, что тип кристалла разрешен
            foreach (var acceptedType in _acceptedTypes)
            {
                if (crystal.CrystalType == acceptedType)
                    return true;
            }
            
            return false;
        }

        public void Insert(Crystal crystal)
        {
            _current = crystal;
            
            // для берущих сокетов обновляем стабильность портала
            if (_socketType == SocketType.Receiving && _linkedPortal != null)
            {
                _linkedPortal.AddCrystal(crystal);
            }
            
            StartCoroutine(InsertAnimation(crystal));
        }
        
        public Crystal Take()
        {
            if (_current == null) return null;

            // проверяем, что портал открыт
            if (_linkedPortal != null && _linkedPortal.state == PortalState.Closed) return null;

            var obj = _current;
            _current = null;

            // для берущих сокетов обновляем стабильность портала
            if (_socketType == SocketType.Receiving && _linkedPortal != null)
            {
                _linkedPortal.RemoveCrystal(obj);
            }

            obj.transform.SetParent(null);
            TakeAnimation(obj, _playerTransform);
            return obj;
        }
        
        public bool HasItem()
        {
            return _current != null;
        }
        
        public Crystal GetCurrentCrystal()
        {
            return _current;
        }
        
        private IEnumerator InsertAnimation(Crystal crystal)
        {
            crystal.Show();

            Vector3 startPos = crystal.transform.position;
            Vector3 endPos = _placePoint.position;

            float time = 0f;
            float duration = 0.3f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                crystal.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            
            crystal.transform.position = endPos;
            crystal.transform.SetParent(_placePoint);
        }
        
        private IEnumerator TakeAnimation(Crystal crystal, Transform player)
        {
            Vector3 start = crystal.transform.position;
            Vector3 end = player.position + player.forward * 1.5f;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                crystal.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }
    }
}