using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public AudioClip pickupSound;

    public void Collect()
    {
        // 1. Tell the GameManager to increase the score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKey();
        }
        else
        {
            Debug.LogError("No GameManager found in the scene!");
        }

        // 2. Play a sound at this location before destroying the object
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // 3. Destroy the key so it can't be picked up again
        Destroy(gameObject);
    }
}