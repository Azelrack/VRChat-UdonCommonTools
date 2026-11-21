
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class SimpleToggle : UdonSharpBehaviour
{
    [SerializeField] private bool _defaultState = true;

    [Header("--- External Behavior ---")]
    [SerializeField] private UdonSharpBehaviour _behaviorToCall;
    [SerializeField] private string _onEventName;
    [SerializeField] private string _offEventName;
    [SerializeField] private GameObject[] _objectsToToggle;

    [Header("--- Persistence ---")]
    [SerializeField] private bool _usePersistence;
    [SerializeField] private string _persistenceKey;

    [Header("--- UI Animation ---")]
    [SerializeField] RectTransform _offTransform;
    [SerializeField] RectTransform _onTransform;
    [SerializeField] RectTransform _selectionTransform;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal)
            return;
        if (_usePersistence && PlayerData.TryGetBool(player, _persistenceKey, out bool value))
            Toggle(value);
        else
            Toggle(_defaultState);
    }


    public virtual void _ToggleOff() => Toggle(false);

    public virtual void _ToggleOn() => Toggle(true);

    private void Toggle(bool toggle)
    {
        if (_behaviorToCall != null)
            _behaviorToCall.SendCustomEvent(toggle ? _onEventName : _offEventName);

        foreach (GameObject obj in _objectsToToggle)
            obj.SetActive(toggle);

        MoveSelectionTo(toggle ? _onTransform : _offTransform);

        if (_usePersistence && !string.IsNullOrEmpty(_persistenceKey))
            PlayerData.SetBool(_persistenceKey, toggle);
    }

    private void MoveSelectionTo(RectTransform destination)
    {
        _selectionTransform.transform.position = destination.transform.position;
    }
}
