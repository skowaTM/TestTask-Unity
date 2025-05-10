using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInputSystem
{
    public Vector2 KeyboardInput { get; private set; }
    public Vector2 MouseInput { get; private set; }

    public Vector2 StickMovementInput { get; private set; }
    public Vector2 StickLookInput { get; private set; }

    private Transform stickMovementPos;
    private Transform stickLookPos;

    private Vector3 stickMovementCenter;
    private Vector3 stickLookCenter;

    float stickRange;


    public MyInputSystem(Transform stickMovement, Transform stickLook, float stickRange)
    {
        this.stickRange = stickRange;

        stickMovementPos = stickMovement.GetChild(0).gameObject.transform;
        stickLookPos = stickLook.GetChild(0).gameObject.transform;

        stickMovementCenter = stickMovement.GetChild(1).gameObject.transform.localPosition;
        stickLookCenter = stickLook.GetChild(1).gameObject.transform.localPosition;
    }


    public void CheckInputs()
    {
        KeyboardInput = HandleKeyboardInput();
        MouseInput = HandleMouseInput();

        StickMovementInput = HandleStickInput(stickMovementPos.localPosition, stickMovementCenter);
        StickLookInput = HandleStickInput(stickLookPos.localPosition, stickLookCenter);
    }


    // Handling of keyboard input
    private Vector2 HandleKeyboardInput()
    {
        Vector2 input = Vector2.zero;

        input.x += Input.GetKey(KeyCode.A) ? -1f : 0f;
        input.x += Input.GetKey(KeyCode.D) ? 1f : 0f;

        input.y += Input.GetKey(KeyCode.W) ? 1f : 0f;
        input.y += Input.GetKey(KeyCode.S) ? -1f : 0f;

        return input.normalized;
    }

    // Handling of mouse input
    private Vector2 HandleMouseInput() =>
        new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));


    // Handling of on-screen joystick input
    private Vector2 HandleStickInput(Vector3 stickPos, Vector3 stickCenter)
    {
        Vector3 input = stickPos - stickCenter;

        if (input != Vector3.zero)
        {
            input /= stickRange;        // scaling by joystick movement range

            return (Vector2)input;
        }

        return Vector2.zero;
    }
}
