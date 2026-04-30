using UnityEngine;
using UnityEngine.Events;

public class HurtboxScript : MonoBehaviour
{
    public int damage;

    public GameObject owner;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision: " + other.name);
        //Check if collider is a hurtbox
        if (other.TryGetComponent<HitboxScript>(out HitboxScript hitbox))
        {
            Debug.Log("Hit");
            //Prevent damaging self
            if (hitbox.owner != owner)
            {
                EventManager.TriggerEvent("Damager", hitbox.damage, owner);
            }
        }
    }
}
