using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class CharacterInput : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap fogTilemap;
    [SerializeField] private Camera mainCamera;

    public void Update()
    {
        // Get input
        var movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (!characterController.HasPath() || movementInput.sqrMagnitude > 0)
        {
            characterController.SetMovementInput(movementInput);
        }

        if (!EventSystem.current.IsPointerOverGameObject() && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            var mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var cellCoordinates = groundTilemap.WorldToCell(mouseWorldPosition);
            
            var tile = fogTilemap.GetTile(cellCoordinates);
            
            if (tile == null)
            {
                characterController.SetMovementDestination(cellCoordinates);
            }
        }
    }
}
