using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    void FixedUpdate()
    {
        Interactable interactable = GetClosestInteractable();
        if (interactable != null)
        {
            Dictionary<string, System.Action<object[]>> actions = interactable.GetActions();

            if (Input.GetKeyDown(KeyCode.F))
            { 
                if (interactable.GetObjectName() == "Key")
                {
                    Player player = transform.parent.GetComponent<Player>();
                    object[] args = new object[] { player };
                    actions["Pickup"](args);

                    HUD.Instance.UpdateKeySlots(player.CollectedKeys.ToArray());
                }
                else if(interactable.GetObjectName() == "Door")
                {
                    Player player = transform.parent.GetComponent<Player>();
                    object[] args = new object[] { player };
                    actions["OpenDoor"](args);

                    HUD.Instance.UpdateKeySlots(player.CollectedKeys.ToArray());
                }
                else if(interactable.GetObjectName() == "Potionshelf")
                {
                    Player player = transform.parent.GetComponent<Player>();
                    object[] args = new object[] { player };
                    actions["Pickup"](args);
                }
            }

            HUD.Instance.InteractText.text = interactable.GetInteractText();
        }
        else
        {
            HUD.Instance.InteractText.text = string.Empty;
        }
    }

    Interactable GetClosestInteractable()
    {
        Interactable closestInteractable = null;
        Collider closestFacingCollider = null;

        //Get all colliders on the Interactable Collision Layer.
        foreach (Collider col in Physics.OverlapSphere(transform.position, 2.0f, 1 << 8, QueryTriggerInteraction.Collide))
        {
            bool onTop = (Vector3.Distance(col.transform.position, transform.position) < 1f);
            bool inFront = (Vector3.Dot(transform.forward, (col.transform.position - transform.position).normalized) >= 0.6f);
            if (onTop || inFront)
            {
                //Check if the object is interactable
                if (col.gameObject.GetComponent<Interactable>() != null)
                {
                    //If the interactable object is closer than the closestFacingCollider, replace it.
                    if (closestFacingCollider != null)
                    {
                        float currentClosestDistance = Vector3.Distance(transform.position, closestFacingCollider.transform.position);
                        float newClosestDistance = Vector3.Distance(transform.position, col.transform.position);
                        if (newClosestDistance < currentClosestDistance)
                        {
                            closestFacingCollider = col;
                        }
                    }
                    else
                    {
                        closestFacingCollider = col;
                    }
                }
            }
        }

        //If we have an interactable collider that is closest to us (only added if has interactable component), 
        //get the interactable component of the object and return it
        if (closestFacingCollider != null)
        {
            closestInteractable = closestFacingCollider.gameObject.GetComponent<Interactable>();
        }

        return closestInteractable;
    }
}