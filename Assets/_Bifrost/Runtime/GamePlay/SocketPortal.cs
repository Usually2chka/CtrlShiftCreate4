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
        private Transform _playerTransform;
        private Transform _placePoint; // куда вставляется предмет
        [SerializeField] private Crystal _current;
        private WorldType[] _acceptedTypes = new WorldType[0]; // разрешенные типы кристаллов
        [SerializeField] private SocketType _socketType = SocketType.Giving; // тип сокета
        private Portal _linkedPortal; // связанный портал

        public WorldType[] AcceptedTypes => _acceptedTypes;
        public SocketType SocketType => _socketType;
        public Portal LinkedPortal => _linkedPortal;

        private void Start()
        {
            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    _playerTransform = player.transform;
                }
            }
            // Автоматически находим связанный портал (среди детей родительского объекта)
            if (_linkedPortal == null)
            {
                _linkedPortal = transform.parent.GetComponentsInChildren<Portal>().FirstOrDefault();
            }

            // Автоматически устанавливаем _acceptedTypes на основе типа сокета и конфига портала
            if (_linkedPortal != null && _linkedPortal.config != null)
            {
                if (_socketType == SocketType.Giving)
                {
                    _acceptedTypes = new WorldType[] { _linkedPortal.config.worldType };
                }
                else if (_socketType == SocketType.Receiving)
                {
                    _acceptedTypes = _linkedPortal.config.stabilizationCost.Select(cr => cr.type).ToArray();
                }
            }
            if (_placePoint == null)
            {
                _placePoint = transform.GetChild(0).transform; // если точка не указана, используем позицию сокета
            }
            if (_placePoint != null && _current == null)
            {
                _current = _placePoint.GetComponentInChildren<Crystal>();
            }

            TrySpawnInitialCrystal();
        }

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
            AudioManager.Instance.PlayPutOnCrystalIntoPortalSound(); // воспроизводим звук вставки

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
            AudioManager.Instance.PlayPutOnCrystalIntoPortalSound(); // воспроизводим звук вставки
            
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
        private void TrySpawnInitialCrystal()
        {
            CrystalDatabase crystalDatabase = GameManager.Instance._crystalDatabase;
            // Только для дающих сокетов
            if (_socketType != SocketType.Giving)
                return;

            // Уже есть кристалл — ничего не делаем
            if (_current != null)
                return;

            if (_linkedPortal == null || _linkedPortal.config == null)
                return;
 
            if (crystalDatabase == null)
            {
                Debug.LogWarning("CrystalDatabase не назначен", this);
                return;
            }

            var type = _linkedPortal.config.worldType;
            var prefab = crystalDatabase.GetPrefab(type);

            if (prefab == null)
                return;

            var rotation = Quaternion.Euler(90f, 0f, 0f);
            var crystal = Instantiate(prefab, _placePoint.position, rotation, _placePoint);
            _current = crystal;

            crystal.Show();
        }
    }
    
}