using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;

    public KeyCode keyToPress;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
            if(canBePressed)
            {
                gameObject.SetActive(false);

                if (Mathf.Abs(transform.position.y) > 0.25f)
                {
                    RythmManager.instance.NormalHit();
                    Debug.Log("Hit");
                } else if (Mathf.Abs(transform.position.y) > 0.05f)
                {
                    RythmManager.instance.GoodHit();
                    Debug.Log("Good");
                } else
                {
                    RythmManager.instance.PerfectHit();
                    Debug.Log("Perfect");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Activator")
        {
            canBePressed = false;
            RythmManager.instance.NoteMissed();
        }
    }
}
