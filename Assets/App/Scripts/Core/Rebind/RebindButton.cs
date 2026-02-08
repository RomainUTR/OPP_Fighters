using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    [Title("Input Configuration")]
    [SerializeField, Required, OnValueChanged("UpdateBindingIndex")]
    private InputActionReference actionReference;

    [ShowInInspector, ValueDropdown("GetBindingOptions"), Required, SerializeField]
    [InfoBox("Select the direction (ex: Up/Down) or the specific button.")]
    [OnValueChanged("UpdateBindingIndex")]
    private string selectedBindingId;

    [Title("Configuration UI")]
    [SerializeField] private TMP_Text actionLabel;
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private GameObject listeningOverlay;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    private int _bindingIndex;

    private void Start()
    {
        if (actionReference != null)
            RebindStorage.Load(actionReference.action.actionMap.asset);

        UpdateBindingIndex();
        UpdateUI();
    }

    public void StartRebinding()
    {
        if (actionReference == null) return;

        actionReference.action.Disable();
        if (listeningOverlay) listeningOverlay.SetActive(true);
        if (bindingText) bindingText.text = "...";

        var binding = actionReference.action.bindings[_bindingIndex];

        _rebindingOperation = actionReference.action.PerformInteractiveRebinding(_bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op => FinishRebinding())
            .OnCancel(op => CancelRebinding());

        if (binding.groups.Contains("Keyboard"))
            _rebindingOperation.WithControlsExcluding("Mouse");

        _rebindingOperation.Start();
    }

    void FinishRebinding()
    {
        CleanUp();
        actionReference.action.Enable();
        RebindStorage.Save(actionReference.action.actionMap.asset);
        UpdateUI();
    }

    void CancelRebinding()
    {
        CleanUp();
        actionReference.action.Enable();
        UpdateUI();
    }

    private void CleanUp()
    {
        if (listeningOverlay) listeningOverlay.SetActive(false);
        _rebindingOperation?.Dispose();
        _rebindingOperation = null;
    }

    void UpdateUI()
    {
        if (actionReference == null) return;

        if (actionLabel)
        {
            string actionName = actionReference.action.name;
            var binding = actionReference.action.bindings[_bindingIndex];

            if (binding.isPartOfComposite)
                actionName += $" ({binding.name})";

            actionLabel.text = actionName;
        }

        if (bindingText)
        {
            string displayString = actionReference.action.GetBindingDisplayString(_bindingIndex,
                InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            bindingText.text = displayString.ToUpper();
        }
    }

    private void UpdateBindingIndex()
    {
        if (actionReference == null || string.IsNullOrEmpty(selectedBindingId)) return;

        if (System.Guid.TryParse(selectedBindingId, out var uuid))
        {
            _bindingIndex = actionReference.action.bindings.IndexOf(x => x.id == uuid);
        }
    }

    private IEnumerable<ValueDropdownItem<string>> GetBindingOptions()
    {
        var options = new List<ValueDropdownItem<string>>();

        if (actionReference != null && actionReference.action != null)
        {
            var bindings = actionReference.action.bindings;
            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                if (binding.isComposite) continue;

                string name = binding.path;

                string readablePath = InputControlPath.ToHumanReadableString(
                    binding.effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );

                if (binding.isPartOfComposite)
                {
                    name = $"Composite: {binding.name.ToUpper()} [{readablePath}]";
                }
                else
                {
                    name = $"Single: {binding.groups} [{readablePath}]";
                }

                options.Add(new ValueDropdownItem<string>(name, binding.id.ToString()));
            }
        }

        if (!string.IsNullOrEmpty(selectedBindingId))
        {
            if (options.All(x => x.Value != selectedBindingId))
            {
                options.Add(new ValueDropdownItem<string>($"[SAVED] {selectedBindingId}", selectedBindingId));
            }
        }

        return options;
    }
}