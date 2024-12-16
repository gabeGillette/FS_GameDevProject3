using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamage
{
    public itemDropOnDeath itemDrop;
    [SerializeField] int baseHealth;
    public Color glowColor = Color.yellow;  // The color of the glow
    public float minGlowIntensity = 0f;     // Minimum glow intensity
    public float maxGlowIntensity = 5f;     // Maximum glow intensity
    public float pulseSpeed = 2f;           // Speed of the pulsing effect
    private Renderer itemRenderer;          // The renderer of the item
    private Material itemMaterial;
    public void TakeDamage(int amount)
    {
        baseHealth -= amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        itemRenderer = GetComponent<Renderer>();

        // Ensure the item has a renderer and a material
        if (itemRenderer != null)
        {
            // If the object uses multiple materials, we target the first material.
            itemMaterial = itemRenderer.material;
        }
        if (itemMaterial != null)
        {
            // Use Mathf.PingPong to make the intensity pulse between min and max intensity
            float glowIntensity = Mathf.PingPong(Time.time * pulseSpeed, maxGlowIntensity - minGlowIntensity) + minGlowIntensity;

            // Set the emission color to create the pulsing glow effect
            itemMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);

            // Optional: Update the global illumination to make the pulsing more noticeable in the scene
            DynamicGI.SetEmissive(itemRenderer, glowColor * glowIntensity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Set the emission color based on the glowColor and glowIntensity
        if (itemMaterial != null)
        {
            // Use Mathf.PingPong to make the intensity pulse between min and max intensity
            float glowIntensity = Mathf.PingPong(Time.time * pulseSpeed, maxGlowIntensity - minGlowIntensity) + minGlowIntensity;

            // Set the emission color to create the pulsing glow effect
            itemMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);

            // Optional: Update the global illumination to make the pulsing more noticeable in the scene
            DynamicGI.SetEmissive(itemRenderer, glowColor * glowIntensity);
        }

        if (baseHealth <= 0)
        {
            itemDrop.DropRandomItem(transform.position);
            Destroy(gameObject);
        }
    }
}
