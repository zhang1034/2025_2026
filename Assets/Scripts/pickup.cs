using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public void Pickup()
    {
        Debug.Log("Picked up: " + gameObject.name);
        Destroy(gameObject);
    }
}
