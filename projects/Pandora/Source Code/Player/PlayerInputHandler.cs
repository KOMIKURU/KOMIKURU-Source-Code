using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    public float MoveInput { get; private set; }
    public bool JumpHeld { get; private set; }

    public bool LookUpHeld { get; private set; }
    public bool LookDownHeld { get; private set; }

    public bool InteractPressed { get; private set; }

    public event Action OnInteractAction;
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    public bool JumpPressed { get; private set; }

    public event Action OnMagicGeneratePressed;
    public event Action OnRightMagicChangePressed;
    public event Action OnLeftMagicChangePressed;
    public bool MagicPressed { get; private set; }

    public event Action OnOpenOptionPressed;
    public event Action OnCloseOptionPressed;

    private float lastTime = 0f;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveInput = context.ReadValue<Vector2>().x;
        }
        else if (context.canceled) MoveInput = 0;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpPressed = true;
        }

        if (context.performed) JumpHeld = true;

        if (context.canceled) JumpHeld = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DashPressed = true;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            InteractPressed = true;
            OnInteractAction?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackPressed = true;
        }
    }

    public void OnLookUp(InputAction.CallbackContext context)
    {
        if (context.performed) LookUpHeld = true;
        if (context.canceled) LookUpHeld = false;
    }

    public void OnLookDown(InputAction.CallbackContext context)
    {
        if (context.performed) LookDownHeld = true;
        if (context.canceled) LookDownHeld = false;
    }

    public void OnMagicGenerate(InputAction.CallbackContext context)
    {
        if (context.started) OnMagicGeneratePressed?.Invoke();
    }

    public void OnRightElementChange(InputAction.CallbackContext context)
    {
        if(context.started) OnRightMagicChangePressed?.Invoke();
    }

    public void OnLeftElementChange(InputAction.CallbackContext context)
    {
        if (context.started) OnLeftMagicChangePressed?.Invoke();
    }

    public void OnOpenOption(InputAction.CallbackContext context)
    {
        if (context.started && Time.time - lastTime >= Time.deltaTime)
        {
            OnOpenOptionPressed?.Invoke();
            lastTime = Time.time;
        }
    }

    public void OnCloseOption(InputAction.CallbackContext context)
    {
        if (context.started && Time.time - lastTime >= Time.deltaTime)
        {
            OnCloseOptionPressed?.Invoke();
            lastTime = Time.time;
        }
            
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        DashPressed = false;
        AttackPressed = false;
        InteractPressed = false;
        MagicPressed = false;
    }
}
