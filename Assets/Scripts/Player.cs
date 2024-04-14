using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;
    
    
    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteraction;
    }

    private void GameInput_OnInteraction(object sender, System.EventArgs e){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }
        float interactDistance = 2f;
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, lastInteractDir, out raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)){
                // Has ClearCounter
                clearCounter.Interact();
            }
        } else {
            Debug.Log("-");
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }
        float interactDistance = 2f;
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, lastInteractDir, out raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)){
                // Has ClearCounter
                // clearCounter.Interact();
            }
        } else {
            Debug.Log("-");
        }
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            // cannot move towards moveDir

            // Attempt movement only in X direction 
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if(canMove) {
                // can move only on the X
                moveDir = moveDirX;
            } else {
                // cannot move on X
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                // Attempt Z only movement
                if (canMove) {
                    moveDir = moveDirZ;
                } else {
                    // cannot move in any direction
                }

            }

        }

        if (canMove) {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        // transform.position += moveDir * moveSpeed * Time.deltaTime;
        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);



        // Debug.Log(Time.deltaTime);
    }
}
