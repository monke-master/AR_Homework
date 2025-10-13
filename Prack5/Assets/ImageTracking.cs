using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    float m_SpawnedObjectScale = 0.1f;

    [SerializeField]
    List<GameObject> m_Prefabs = new();
    Dictionary<string, GameObject> m_PrefabsDict = new();
    Dictionary<string, GameObject> m_Spawned = new();

    ARTrackedImageManager m_ImageManager;

    void Awake()
    {
        m_ImageManager = GetComponent<ARTrackedImageManager>();

        foreach (var prefab in m_Prefabs)
        {
            m_PrefabsDict[prefab.name] = prefab;
            m_Spawned[prefab.name] = null;
        }
    }

    void OnEnable()
    {
        m_ImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    void OnDisable()
    {
        m_ImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    void UpdatePos(ARTrackedImage trackedImage)
    {
        GameObject obj;
        var imageName = trackedImage.referenceImage.name;
        obj = m_Spawned[imageName];
        if (obj == null)
        {
            obj = m_Spawned[imageName] = Instantiate(m_PrefabsDict[imageName]);
            obj.transform.localScale = Vector3.one * m_SpawnedObjectScale;
        }

        obj.transform.position = trackedImage.pose.position;
        obj.SetActive(true);
    }

    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.added)
            UpdatePos(image);

        foreach (var image in args.updated)
            UpdatePos(image);

        foreach (var image in args.removed)
        {
            if (m_Spawned.ContainsKey(image.Value.name))
                m_Spawned[image.Value.name]?.SetActive(false);
        }
    }
}

