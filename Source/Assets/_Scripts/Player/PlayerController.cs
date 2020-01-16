using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private float rotateVelocity = 200f;
    private Quaternion targetRotation;
    private Animator anim = null;

    void Start()
    {
        this.targetRotation = transform.rotation;
        this.anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        //Get movement vector (walking, running, strafing)
        Vector3 movementVector = new Vector3(Input.GetAxisRaw("Strafing"), 0, Input.GetAxisRaw("Vertical"));

        //Set animation state based on input (idle, walking, running, strafing)
        SetAnimationState(movementVector);

        this.Rotate();

        //Move character based on input axes
        anim.SetFloat("MovementX", movementVector.x);
        anim.SetFloat("MovementZ", movementVector.z);
    }

    void Rotate()
    {
        //Get rotation input
        float rotationInput = Input.GetAxis("Horizontal");

        targetRotation = Quaternion.AngleAxis(rotateVelocity * rotationInput * Time.deltaTime, Vector3.up) * targetRotation;
        transform.rotation = targetRotation;
    }

    void SetAnimationState(Vector3 input)
    {
        bool strafing = (input.x != 0.0f);
        bool running = (input != Vector3.zero && Input.GetKey(KeyCode.LeftShift));
        bool walking = (input != Vector3.zero && !strafing && !running);

        anim.SetBool("IsWalking", walking);
        anim.SetBool("IsRunning", running);
        anim.SetBool("IsStrafing", strafing);
    }

    public void PlayFootstep()
    {
        SoundManager.Instance.PlayerAudioSource.clip = SoundManager.Instance.WalkClip;
        SoundManager.Instance.PlayerAudioSource.Play();
    }
}
