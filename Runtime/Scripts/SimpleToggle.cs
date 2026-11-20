
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class SimpleToggle : UdonSharpBehaviour
{
    [Header("--- External Behavior")]
    [SerializeField] private UdonSharpBehaviour _behaviorToCall;
    [SerializeField] private string _onEventName;
    [SerializeField] private string _offEventName;
    [SerializeField] private GameObject[] _objectsToToggle;
    [SerializeField] private bool _defaultState = true;

    [Header("--- Persistence")]
    [SerializeField] private bool _usePersistence;
    [SerializeField] private string _persistenceKey;

    [Header("--- UI Animation")]
    [SerializeField] RectTransform _offTransform;
    [SerializeField] RectTransform _onTransform;
    [SerializeField] RectTransform _selectionTransform;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal)
            return;
        if (_usePersistence && PlayerData.TryGetBool(player, _persistenceKey, out bool value))
        {
            if (value)
                _ToggleOn();
            else
                _ToggleOff();
        }
        else if (_defaultState)
            _ToggleOn();
        else
            _ToggleOff();
    }


    public virtual void _ToggleOff()
    {
        if (_behaviorToCall != null)
            _behaviorToCall.SendCustomEvent(_offEventName);
        foreach (GameObject obj in _objectsToToggle)
            obj.SetActive(false);
        MoveSelectionTo(_offTransform);
        if (_usePersistence && !string.IsNullOrEmpty(_persistenceKey))
            PlayerData.SetBool(_persistenceKey, false);
    }

    public virtual void _ToggleOn()
    {
        if (_behaviorToCall != null)
            _behaviorToCall.SendCustomEvent(_onEventName);
        foreach (GameObject obj in _objectsToToggle)
            obj.SetActive(true);
        MoveSelectionTo(_onTransform);
        if (_usePersistence && !string.IsNullOrEmpty(_persistenceKey))
            PlayerData.SetBool(_persistenceKey, true);
    }

    private void MoveSelectionTo(RectTransform destination)
    {
        _selectionTransform.transform.position = destination.transform.position;
    }
}
