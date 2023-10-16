using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    public static event Action OnClosed;

    private void Start()
    {
        _continueButton.onClick.AddListener(ContinueClickHandler);
    }

    private void ContinueClickHandler()
    {
        OnClosed?.Invoke();
    }
}
