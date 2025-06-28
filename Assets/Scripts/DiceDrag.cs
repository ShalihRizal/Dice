using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceDrag : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private bool isOverTrashZone = false;
    private Transform parentCell;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private DiceDrag currentHighlightedDice;
    private int originalSortingOrder;

    void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null){
            originalColor = spriteRenderer.color;
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    public void SetParentCell(Transform cell)
    {
        parentCell = cell;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.IsCombatActive) return;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        spriteRenderer.sortingOrder = 10;
    }

   void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;

            // Highlight nearby swappable dice
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.4f);
            DiceDrag nearestDice = null;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Dice") && hit.gameObject != this.gameObject)
                {
                    nearestDice = hit.GetComponent<DiceDrag>();
                    break;
                }
            }

            // If we're over a new dice, highlight it
            if (nearestDice != null && nearestDice != currentHighlightedDice)
            {
                ClearHighlight(); // clear previous
                HighlightDice(nearestDice);
            }
            else if (nearestDice == null)
            {
                ClearHighlight();
            }
        }
    }

    void HighlightDice(DiceDrag dice)
{
    currentHighlightedDice = dice;
    if (dice.spriteRenderer != null)
    {
        dice.spriteRenderer.color = Color.yellow; // Highlight color
    }
}

void ClearHighlight()
{
    if (currentHighlightedDice != null && currentHighlightedDice.spriteRenderer != null)
    {
        currentHighlightedDice.spriteRenderer.color = currentHighlightedDice.originalColor;
    }
    currentHighlightedDice = null;
}



    void OnMouseUp()
    {
        ClearHighlight();

        isDragging = false;

        spriteRenderer.sortingOrder = originalSortingOrder;

        DiceSpawner spawner = FindObjectOfType<DiceSpawner>();

        if (isOverTrashZone)
        {
            if (spawner != null && parentCell != null)
                spawner.ReleaseCell(parentCell);

            Destroy(gameObject);
            return;
        }

        // Find nearby dice instead of relying on exact position
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.4f); // ← increased range
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Dice") && hit.gameObject != this.gameObject)
            {
                DiceDrag otherDice = hit.GetComponent<DiceDrag>();
                if (otherDice != null && otherDice != this)
                {
                    // Swap logic
                    Transform otherCell = otherDice.parentCell;
                    Transform thisCell = this.parentCell;

                    Vector3 tempPos = otherDice.transform.position;
                    otherDice.transform.position = originalPosition;
                    this.transform.position = tempPos;

                    otherDice.SetOriginalPosition(originalPosition);
                    this.SetOriginalPosition(tempPos);

                    otherDice.SetParentCell(thisCell);
                    this.SetParentCell(otherCell);

                    if (spawner != null)
                    {
                        spawner.ReleaseCell(thisCell);
                        spawner.ReleaseCell(otherCell);
                        spawner.OccupyCell(otherCell);
                        spawner.OccupyCell(thisCell);
                    }

                    return;
                }
            }
        }

        // Try dropping to empty cell
        if (spawner != null)
        {
            Transform nearestCell = spawner.GetNearestFreeCell(transform.position);
            if (nearestCell != null && !spawner.IsCellOccupied(nearestCell))
            {
                spawner.ReleaseCell(parentCell);
                spawner.OccupyCell(nearestCell);

                transform.position = nearestCell.position;
                SetOriginalPosition(nearestCell.position);
                SetParentCell(nearestCell);
                return;
            }
        }

        // Return to original if nothing valid found
        transform.position = originalPosition;
    }




    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f; // Adjust based on camera distance
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    public void SetTrashZoneStatus(bool isInside)
    {
        isOverTrashZone = isInside;
    }

    public void SetOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
    }
}
