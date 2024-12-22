using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Image buttonImage;
    private Color normalColor;
    private Color hoverColor = Color.yellow;  // You can change this to any color you want

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();  // Get the Image component attached to the button
        normalColor = buttonImage.color;  // Store the original button color
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change the button's color when hovered
        buttonImage.color = hoverColor;  // Set the Image color to the hover color

        // Optionally, update the button's ColorBlock to change button states
        ColorBlock colors = button.colors;
        colors.normalColor = hoverColor; // Set the normal color to the hover color
        colors.highlightedColor = hoverColor;  // Optionally change the highlighted color as well
        button.colors = colors;

        // Optionally, play hover sound
        FindObjectOfType<MenuManager>().PlaySoundEffect("hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset the button's color when the pointer exits
        buttonImage.color = normalColor;  // Reset to the original color

        // Reset the ColorBlock to its original color
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor; // Reset the normal color to the original
        colors.highlightedColor = normalColor;  // Optionally reset the highlighted color
        button.colors = colors;
    }
}
