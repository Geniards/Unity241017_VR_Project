using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ModeButton : XRPushButton
{
    [SerializeField] private GameManager.GameMode modeToActivate;
    [SerializeField] private ModeButton[] otherModeButtons;

    protected override void Start()
    {
        base.Start();
        toggleValue = false;
    }

    public void ActivateMode()
    {
        toggleValue = true;
        GameManager.Instance.SetGameMode(modeToActivate);
        GameManager.Instance.ResetGame();
        Debug.Log($"{modeToActivate} ��� Ȱ��ȭ");

        // �ٸ� ��ư�� ��Ȱ��ȭ
        foreach (var otherButton in otherModeButtons)
        {
            if (otherButton != this)
                otherButton.DeactivateMode();
        }

    }

    public void DeactivateMode()
    {
        toggleValue = false;
        Debug.Log($"{modeToActivate} ��� ��Ȱ��ȭ");
    }
}
