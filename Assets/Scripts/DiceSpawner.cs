using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpawner : MonoBehaviour
{
    [Header("References")]
    // public GameObject dicePrefab;
    public GridSpawner gridGenerator;

    private List<Transform> gridCells = new List<Transform>();
    private HashSet<Transform> occupiedCells = new HashSet<Transform>();
    public DicePool dicePool; // Reference to your ScriptableObject pool


    void Start()
    {
        StartCoroutine(InitializeAfterGridReady());
    }

    IEnumerator InitializeAfterGridReady()
    {
        yield return null;

        if (gridGenerator == null || dicePool == null)
        {
            Debug.LogError("DiceSpawner is missing references!");
            yield break;
        }

        foreach (Transform child in gridGenerator.transform)
        {
            gridCells.Add(child);
        }

        Debug.Log("Grid cells collected: " + gridCells.Count);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.IsCombatActive)
    {
        SpawnDiceOnRandomFreeCell();
    }
    }

    void SpawnDiceOnRandomFreeCell()
    {
        if (dicePool == null)
        {
            Debug.LogError("DicePool not assigned!");
            return;
        }

        List<Transform> availableCells = new List<Transform>();
        foreach (var cell in gridCells)
        {
            if (!occupiedCells.Contains(cell))
                availableCells.Add(cell);
        }

        if (availableCells.Count == 0)
        {
            Debug.Log("No free cells remaining!");
            return;
        }

        Transform chosenCell = availableCells[Random.Range(0, availableCells.Count)];

        DiceData randomDiceData = dicePool.GetRandomDice();
        if (randomDiceData == null)
        {
            Debug.LogError("No dice available in pool!");
            return;
        }

        GameObject dice = Instantiate(randomDiceData.prefab, chosenCell.position, Quaternion.identity);
        dice.transform.SetParent(chosenCell);
        occupiedCells.Add(chosenCell);

        DiceDrag drag = dice.GetComponent<DiceDrag>();
        if (drag != null)
        {
            drag.SetOriginalPosition(chosenCell.position);
            drag.SetParentCell(chosenCell);
        }

        Debug.Log("Spawned dice: " + randomDiceData.name + " at " + chosenCell.name);
    }


    public void ReleaseCell(Transform cell)
    {
        occupiedCells.Remove(cell);
    }

    public bool IsCellOccupied(Transform cell)
    {
        return occupiedCells.Contains(cell);
    }

    public void OccupyCell(Transform cell)
    {
        occupiedCells.Add(cell);
    }

    public Transform GetNearestFreeCell(Vector3 pos)
    {
        Transform best = null;
        float minDist = float.MaxValue;

        foreach (var cell in gridCells)
        {
            float dist = Vector3.Distance(pos, cell.position);
            if (dist < minDist && !occupiedCells.Contains(cell))
            {
                minDist = dist;
                best = cell;
            }
        }

        return best;
    }


}
