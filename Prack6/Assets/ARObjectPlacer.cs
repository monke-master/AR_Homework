using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ARObjectPlacer : MonoBehaviour
{
    [SerializeField] private int maxPrefabSpawnCount = 5;
    private int placedPrefabCount;
    public GameObject placeablePrefab;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private ARRaycastManager raycastManager;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        // To test in Unity
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            touchPosition = Mouse.current.position.ReadValue();
            return true;
        }
#else
        // На устройстве сенсор
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }
#endif
        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (raycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;
            if (placedPrefabCount < maxPrefabSpawnCount)
            {
                SpawnPrefab(hitPose);
            }
        }
    }

    private void SpawnPrefab(Pose hitPose)
    {
        var spawnedObject = Instantiate(placeablePrefab, hitPose.position, hitPose.rotation);
        placedPrefabCount++;
    }

    public void SetPrefabType(GameObject prefabType)
    {
        placeablePrefab = prefabType;
    }
}
