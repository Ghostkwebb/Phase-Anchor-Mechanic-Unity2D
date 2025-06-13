using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string tutorialMessage;
    
    public TutorialTextController textController; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (textController != null)
            {
                textController.ShowText(tutorialMessage);
            }
            // Deactivate so it only triggers once
            gameObject.SetActive(false);
        }
    }
}