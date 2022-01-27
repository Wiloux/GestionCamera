using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewVolumeBlender : MonoBehaviour
{
    private List<AViewVolume> activeViewVolumes = new List<AViewVolume>();
    private Dictionary<AView, List<AViewVolume>> volumesPerViews = new Dictionary<AView,List<AViewVolume>>();
    public void AddVolume(AViewVolume volume) 
    { 
        if (!activeViewVolumes.Contains(volume) && volume.view) 
        { 
            activeViewVolumes.Add(volume);
            if(!volumesPerViews.ContainsKey(volume.view))
            {
                volumesPerViews.Add(volume.view, new List<AViewVolume> { volume });
                volume.view.SetActive(true);
            }
        }
        else if(!volume.view) { Debug.LogWarning("View volume named: \"" + volume.gameObject.name + "\" does not have a view attached! The volume will not be used."); }
    }
    public void RemoveVolume(AViewVolume volume) 
    { 
        bool removed = activeViewVolumes.Remove(volume); 
        if (removed && volume.view && volumesPerViews.ContainsKey(volume.view))
        {
            volumesPerViews[volume.view].Remove(volume);
            if (volumesPerViews[volume.view].Count == 0)
            {
                volumesPerViews.Remove(volume.view);
                volume.view.SetActive(false);
            }
        }
        else if (!volume.view) { Debug.LogWarning("View volume named: \"" + volume.gameObject.name + "\" does not have a view attached! The volume will not be used."); }
    }


    private static ViewVolumeBlender _instance;
    public static ViewVolumeBlender Instance { get { return _instance; } }
    private void Awake()
    {
        if (Instance != null) { Debug.LogWarning("ViewVolumeBlender instance has been destroyed since there was already one in the scene!"); Destroy(gameObject); }
        _instance = this;
    }

    public void Update()
    {
        if (activeViewVolumes.Count > 0)
        {
            AViewVolume maxPriorityVolume = activeViewVolumes[0];
            for (int i = 1; i < activeViewVolumes.Count; i++)
            { if (activeViewVolumes[i].priority > maxPriorityVolume.priority) { maxPriorityVolume = activeViewVolumes[i]; }}

            foreach (AViewVolume volume in activeViewVolumes)
            {
                if (volume.priority < maxPriorityVolume.priority) { volume.view.weight = 0; continue; }
                volume.view.weight = Mathf.Max(volume.view.weight, volume.ComputeSelfWeight());
            }
        }
    }

    private void OnGUI()
    {
        foreach (AViewVolume volume in activeViewVolumes)
        { GUILayout.Label(volume.gameObject.name); }
    }
}
