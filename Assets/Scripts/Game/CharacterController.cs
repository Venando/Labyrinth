using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveLerp = 5f;

    [SerializeField] private Tilemap wallsTilemap;

    [SerializeField] private Pathfinder pathfinder;

    private Vector2 movementInput;
    private Vector3Int movementDestination;
    private Vector3 desiredPosition;

    private Vector3Int currentTileCoord;

    public event Action<Vector3Int> StepOnTile;

    private readonly Stack<Vector3Int> activePath = new();

    private void Start()
    {
        desiredPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveLerp * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (activePath.Count > 0)
        {
            Vector3Int currentDestination = activePath.Peek();
            Vector3Int currentTileCoord = GetCurrentCoordinates();

            Vector3Int result = currentDestination - currentTileCoord;

            Move(new Vector2(result.x, result.y));

            if (currentDestination == GetCurrentCoordinates())
            {
                activePath.Pop();   
            }
        }
        else
        {
            Move(movementInput);
        }
    }

    public void Move(Vector2 movement)
    {
        UpdateIsMoving(movement);

        if (IsZeroDirection(movement))
            return;

        movement = NormalizeMovement(movement);

        if (TryMoveHorizontal(movement))
            return;

        if (TryMoveVertical(movement))
            return;

        SetAnimatorDirection(movement.x, movement.y);
    }

    private void UpdateIsMoving(Vector2 movement)
    {
        animator.SetBool("IsMoving", movement.sqrMagnitude > 0);
    }

    private bool IsZeroDirection(Vector2 dir)
    {
        return Mathf.Approximately(dir.sqrMagnitude, 0f);
    }

    private Vector2 NormalizeMovement(Vector2 movement)
    {
        movement.Normalize();

        if (Mathf.Abs(movement.x) > 0)
            movement.x = Mathf.Sign(movement.x);

        if (Mathf.Abs(movement.y) > 0)
            movement.y = Mathf.Sign(movement.y);

        return movement;
    }

    private bool TryMoveHorizontal(Vector2 movement)
    {
        if (Mathf.Abs(movement.x) <= 0)
            return false;

        if (TryToMove(new Vector2(movement.x, 0)))
        {
            SetAnimatorDirection(movement.x, 0);
            return true;
        }

        return false;
    }

    private bool TryMoveVertical(Vector2 movement)
    {
        if (Mathf.Abs(movement.y) <= 0)
            return false;

        if (TryToMove(new Vector2(0, movement.y)))
        {
            SetAnimatorDirection(0, movement.y);
            return true;
        }

        return false;
    }

    private void SetAnimatorDirection(float x, float y)
    {
        animator.SetFloat("DirectionX", x);
        animator.SetFloat("DirectionY", y);
    }

    private bool TryToMove(Vector2 movementDirection)
    {
        if (!TryGetMoveDestination(movementDirection, out Vector3 moveDestination))
            return false;

        float distanceToDestination = Vector2.Distance(desiredPosition, moveDestination);

        if (Mathf.Approximately(distanceToDestination, 0f))
            return true;

        float moveDistance = Mathf.Min(distanceToDestination, moveSpeed * Time.fixedDeltaTime);
        Vector2 moveDelta = ((Vector2)(moveDestination - desiredPosition)).normalized * moveDistance;
        desiredPosition += (Vector3)moveDelta;

        UpdateSteppedTile();

        return true;
    }

    private void UpdateSteppedTile()
    {
        Vector3Int cellCoordinates = GetCurrentCoordinates();

        if (cellCoordinates != currentTileCoord)
        {
            currentTileCoord = cellCoordinates;
            StepOnTile?.Invoke(cellCoordinates);
        }
    }

    private bool TryGetMoveDestination(Vector2 movementDirection, out Vector3 moveDestination)
    {
        Vector3Int currentCellIndex = GetCurrentCoordinates();
        Vector3 currentCellCenter = GetCellCenter(currentCellIndex);

        if (IsMovementAlignedTo(movementDirection, currentCellCenter))
        {
            moveDestination = currentCellCenter;
            return true;
        }

        Vector3Int nextCellIndex = currentCellIndex + new Vector3Int((int)movementDirection.x, (int)movementDirection.y, 0);
        if (IsCellBlocked(nextCellIndex))
        {
            moveDestination = default;
            return false;
        }

        Vector3 nextCellCenter = GetCellCenter(nextCellIndex);
        if (!IsMovementAlignedTo(movementDirection, nextCellCenter))
        {
            if (IsMovementAlignedTo(currentCellCenter, movementDirection, nextCellCenter))
            {
                moveDestination = currentCellCenter;
                return true;
            }
            moveDestination = default;
            return false;
        }

        moveDestination = nextCellCenter;
        return true;
    }

    private Vector3Int GetCurrentCoordinates()
    {
        return wallsTilemap.WorldToCell(desiredPosition);
    }

    private Vector3 GetCellCenter(Vector3Int cellIndex)
    {
        return wallsTilemap.CellToWorld(cellIndex) + wallsTilemap.cellSize / 2f;
    }

    private bool IsMovementAlignedTo(Vector3 from, Vector2 movementDirection, Vector3 targetPosition)
    {
        var dirToTarget = (targetPosition - from).normalized;
        float dot = Vector2.Dot(movementDirection, dirToTarget);
        return Mathf.Approximately(dot, 1f);
    }

    private bool IsMovementAlignedTo(Vector2 movementDirection, Vector3 targetPosition)
    {
        return IsMovementAlignedTo(desiredPosition, movementDirection, targetPosition);
    }

    private bool IsCellBlocked(Vector3Int cellIndex)
    {
        return wallsTilemap.GetTile(cellIndex) != null;
    }

    public void SetMoveSpeed(int speed)
    {
        moveSpeed = speed;
    }

    public void SetMovementInput(Vector2 input)
    {
        activePath.Clear();
        movementInput = input;
    }

    public void SetMovementDestination(Vector3Int cellCoordinates)
    {
        movementInput = Vector2.zero;

        movementDestination = cellCoordinates;
        
        activePath.Clear();

        if (pathfinder.TryToCalculatePath(GetCurrentCoordinates(), cellCoordinates, 999, out List<Vector3Int> path))
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                Vector3Int pathPoint = path[i];
                activePath.Push(pathPoint);
            }
        }
    }

    public bool HasPath()
    {
        return activePath.Count > 0;
    }

    public void StopCharacter()
    {
        enabled = false;
        movementInput = new Vector2();
        animator.SetBool("IsMoving", false);
    }
}
