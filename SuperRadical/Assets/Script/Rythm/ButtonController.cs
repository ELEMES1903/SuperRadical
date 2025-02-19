using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

    private Image image;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;

    private RectTransform thisRect;
    public RectTransform prefabArrow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        thisRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
            image.sprite = pressedImage;
            CheckForArrowOverlap();

        }
        if(Input.GetKeyUp(keyToPress))
        {
            image.sprite = defaultImage;
        }
    }


    private void CheckForArrowOverlap()
    {
        // Get all arrow objects in the scene
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Arrow"); // Assuming your arrows have the "Arrow" tag

        foreach (GameObject arrow in arrows)
        {
            RectTransform arrowRect = arrow.GetComponent<RectTransform>();
            float overlapPercentage = GetOverlapPercentage(thisRect, arrowRect);

            if (overlapPercentage > 0) // If there is any overlap
            {
                Debug.Log("Arrow overlap: " + overlapPercentage + "%");

                // Optionally destroy the arrow if you want to do so based on overlap
                Destroy(arrow);
                Debug.Log("Arrow destroyed!");
            }
        }
    }

    private float GetOverlapPercentage(RectTransform rect1, RectTransform rect2)
    {
        // Convert both RectTransforms to screen space coordinates
        Rect rect1InWorld = new Rect(rect1.position.x, rect1.position.y, rect1.rect.width, rect1.rect.height);
        Rect rect2InWorld = new Rect(rect2.position.x, rect2.position.y, rect2.rect.width, rect2.rect.height);

        // Check if the rectangles overlap
        if (rect1InWorld.Overlaps(rect2InWorld))
        {
            // Calculate the intersection rectangle
            float overlapWidth = Mathf.Max(0, Mathf.Min(rect1InWorld.xMax, rect2InWorld.xMax) - Mathf.Max(rect1InWorld.xMin, rect2InWorld.xMin));
            float overlapHeight = Mathf.Max(0, Mathf.Min(rect1InWorld.yMax, rect2InWorld.yMax) - Mathf.Max(rect1InWorld.yMin, rect2InWorld.yMin));

            // Calculate the area of overlap
            float overlapArea = overlapWidth * overlapHeight;

            // Calculate the area of the arrow (rect2)
            float arrowArea = rect2InWorld.width * rect2InWorld.height;

            // Calculate the percentage of overlap
            float overlapPercentage = (overlapArea / arrowArea) * 100;
            
            return overlapPercentage;
        }

        return 0; // No overlap
    }
}
