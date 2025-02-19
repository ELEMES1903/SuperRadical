using UnityEngine;

public class RythmButton : MonoBehaviour
{

    private SpriteRenderer sr;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
            sr.sprite = pressedImage;
            

        }
        if(Input.GetKeyUp(keyToPress))
        {
            sr.sprite = defaultImage;
        }
    }
}
