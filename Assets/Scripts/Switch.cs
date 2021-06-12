using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    public int GroupId = 1;

    public Vector2Int GridPosition {get; private set;}

    GridMovement[] AllCharacters;
    List<Door> AffectedDoors;

    Vector2 GridOffset = new Vector2(0.5f, 0.5f);


    // Start is called before the first frame update
    void Start()
    {
        GridPosition = new Vector2Int(
            (int) Mathf.Floor(Mathf.Floor(transform.position.x) + GridOffset.x),
            (int) Mathf.Floor(Mathf.Floor(transform.position.y) + GridOffset.y)
        );
        AllCharacters = FindObjectsOfType<GridMovement>();
        AffectedDoors = new List<Door>();
        foreach (var door in FindObjectsOfType<Door>()) {
            if (door.GroupId == GroupId) AffectedDoors.Add(door);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var character in AllCharacters) {
            if (character.GridPosition == GridPosition) {
                SetAllDoors(true);
                return;
            }
        }
        SetAllDoors(false);
    }

    void SetAllDoors(bool state) {
        foreach (var door in AffectedDoors) {
            door.set(state);
        }
    }
}
