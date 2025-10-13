using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARObjectPlacer : MonoBehaviour
{
    private ARRaycastManager _raycastManager;
    private GameObject _spawnedObject;

    [SerializeField]
    private GameObject placeablePrefab;

    private static readonly List<ARRaycastHit> Hits = new();

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        // Проверяем касание экрана
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        // Проверяем пересечение с поверхностью
        if (_raycastManager.Raycast(touchPosition, Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = Hits[0].pose;

            // Получаем высоту объекта (по bounding box)
            float objectHeight = GetPrefabHeight(placeablePrefab);

            // Добавляем смещение вверх, чтобы объект не "утопал" в полу
            Vector3 offsetPosition = hitPose.position + new Vector3(0, objectHeight / 2f, 0);

            if (_spawnedObject == null)
            {
                _spawnedObject = Instantiate(placeablePrefab, offsetPosition, hitPose.rotation);
            }
            else
            {
                _spawnedObject.transform.SetPositionAndRotation(offsetPosition, hitPose.rotation);
            }
        }
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    /// <summary>
    /// Возвращает высоту объекта по его MeshRenderer bounds.
    /// Работает даже если меш находится в дочернем объекте.
    /// </summary>
    private float GetPrefabHeight(GameObject prefab)
    {
        var meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            return meshRenderer.bounds.size.y;
        }

        // Если нет MeshRenderer, возвращаем стандартную высоту
        return 0.1f;
    }
}
