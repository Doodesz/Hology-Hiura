using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPointIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cam;
    public Image icon;
    public Transform target;
    public Vector3 offset;

    [Header("Variables")]
    [Range(0,100)][SerializeField] float xMarginPercent;
    [Range(0,100)][SerializeField] float yMarginPercent;

    private void Start()
    {
        cam = Camera.main.gameObject;
    }

    private void Update()
    {
        // Giving limits to the icon so it sticks on the screen
        // Below calculations witht the assumption that the icon anchor point is in the middle
        // Minimum X position: half of the icon width
        float minX = icon.GetPixelAdjustedRect().width / 2;
        // Maximum X position: screen width - half of the icon width
        float maxX = Screen.width - minX;

        // Minimum Y position: half of the height
        float minY = icon.GetPixelAdjustedRect().height / 2;
        // Maximum Y position: screen height - half of the icon height
        float maxY = Screen.height - minY;

        // Temporary variable to store the converted position from 3D world point to 2D screen point
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        // Check if the target is behind us, to only show the icon once the target is in front
        if (Vector3.Dot((target.position - cam.transform.position), cam.transform.forward) < 0)
        {
            // Check if the target is on the left side of the screen
            if (pos.x < Screen.width / 2)
            {
                // Place it on the right (Since it's behind the player, it's the opposite)
                pos.x = maxX;
            }
            else
            {
                // Place it on the left side
                pos.x = minX;
            }

            if (pos.y < Screen.height / 2)
            {
                pos.y = maxY;
            }
            else
            {
                pos.y = minY;
            }

            pos.y = maxY / 2;
        }

        // Limit the X and Y positions
        pos.x = Mathf.Clamp(pos.x, minX + (Screen.width * (xMarginPercent/100)), maxX - (Screen.width * (xMarginPercent / 100)));
        pos.y = Mathf.Clamp(pos.y, minY + (Screen.height * (yMarginPercent/100)), maxY - (Screen.height * (yMarginPercent / 100)));

        // Update the marker's position
        icon.transform.position = Vector3.Slerp(icon.transform.position, pos, 0.4f);
    }
}