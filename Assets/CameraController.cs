using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Inputs Inputs { get; set; }

    public Vector3 angles;
    public Vector3 expectedAngles;
    public float cameraMoveSpeed;

    private void OnEnable() => Inputs.CameraControl.Enable();
    private void OnDisable ()=> Inputs.CameraControl.Disable();

    private void Awake()
    {
        Inputs = new Inputs();
    }

    private void Update()
    {
        MoveCamera(Inputs.CameraControl.Move.ReadValue<Vector2>());
    }

    private void MoveCamera(Vector2 vec2)
    {
        transform.position += (transform.right * vec2.x + new Vector3(transform.forward.x, 0, transform.forward.z).normalized * vec2.y) * cameraMoveSpeed;
    }
}
