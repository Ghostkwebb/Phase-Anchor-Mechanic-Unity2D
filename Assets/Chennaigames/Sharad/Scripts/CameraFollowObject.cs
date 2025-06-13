using UnityEngine;
using System.Collections;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;

    [Header("Flip Rotation Stats")]
    public float flipRotationTime = 0.5f;

    private Coroutine turnCoroutine;

    public PlayerController playerController;

    private bool isFacingRight;

    private void Awake()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        isFacingRight = playerController.isFacingRight;
    }

    private void Update()
    {
        transform.position = playerTransform.position;
    }

    public void CallTurn()
    {
        turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = DetermineEndRotation();
        float yRotation = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flipRotationTime)
        {
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotation, elapsedTime / flipRotationTime);
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        isFacingRight = !isFacingRight;
        if (isFacingRight)
        {
            return 0f; 
        }
        else
        {
            return 180f; 
        }
    }
}
