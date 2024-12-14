using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemDropOnDeath : MonoBehaviour
{
    // Item class to hold the item and its weight
    [System.Serializable]
    public class Item
    {
        public string itemName;
        public float weight;  // The weight (chance) of the item being selected
    }

    [SerializeField] GameObject healthPrefab;
    [SerializeField] GameObject pistolammoPrefab;
    [SerializeField] GameObject shotgunammoPrefab;
    [SerializeField] GameObject batteryPrefab;


    // List of possible items to drop
    public List<Item> items = new List<Item>();

    // Method to drop a random item based on weight
    public void DropRandomItem(Vector3 dropPosition)
    {
        // Calculate total weight of all items
        float totalWeight = 0f;
        foreach (Item item in items)
        {
            totalWeight += item.weight;
        }

        // Generate a random number between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);

        // Iterate through items and select one based on the random value
        float currentWeight = 0f;
        foreach (Item item in items)
        {
            currentWeight += item.weight;

            if (randomValue <= currentWeight)
            {
                Debug.Log("Dropped item: " + item.itemName);
                // Here you can instantiate or do whatever you need to do with the item
                DropItem(item, dropPosition);
                break;
            }
        }
    }

    // Method to instantiate the dropped item (optional)
    void DropItem(Item item, Vector3 dropPosition)
    {
        float raiseHeight = 1.0f;
        Vector3 raisedPosition = new Vector3(dropPosition.x, dropPosition.y + raiseHeight, dropPosition.z);

        // Assuming you have prefabs for each item
        GameObject itemPrefab = GetItemPrefab(item.itemName);

        if (itemPrefab != null)
        {
            // Instantiate the prefab at the current position of the ItemDropper
            Instantiate(itemPrefab, raisedPosition, Quaternion.identity);
            Debug.Log(itemPrefab + "Was Generated");
        }
        else
        {
            Debug.LogWarning("No prefab found for item: " + item.itemName);
        }
    }

    // This method maps item names to corresponding prefabs (you can customize this)
    GameObject GetItemPrefab(string itemName)
    {
        // For example, return the prefab based on itemName
        if (itemName == "Health")
        {
            return healthPrefab; // Replace with actual reference to the Health Potion prefab
        }
        else if (itemName == "PistolAmmo")
        {
            return pistolammoPrefab; // Replace with actual reference to the Ammo prefab
        }
        else if (itemName == "ShotgunAmmo")
        {
            return shotgunammoPrefab; // Replace with actual reference to the Weapon Upgrade prefab
        }
        else if (itemName == "Battery")
        {
            return batteryPrefab; // Replace with actual reference to the Weapon Upgrade prefab
        }
        else
        {
            return null;
        }
    }


}