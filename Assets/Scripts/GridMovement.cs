using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    Level map;

    Vector2 StartPos;
    Vector2 TargetPos;

    public Vector2Int GridPosition {get; private set;}
    Vector2 GridOffset = new Vector2(0.5f, 0.5f);

    // input map
    [SerializeField]
    Vector2Int UpAction = Vector2Int.up;
    [SerializeField]
    Vector2Int DownAction = Vector2Int.down;
    [SerializeField]
    Vector2Int LeftAction = Vector2Int.left;
    [SerializeField]
    Vector2Int RightAction = Vector2Int.right;

    float MoveTime = 0f;

    [SerializeField]
    float MoveSpeed = 5f;
    const float ROOT2_2 = 0.707106f;

    public bool AutoSelect = false;

    // Start is called before the first frame update
    void Start()
    {
        GridPosition = new Vector2Int(
            (int) Mathf.Floor(Mathf.Floor(transform.position.x) + GridOffset.x),
            (int) Mathf.Floor(Mathf.Floor(transform.position.y) + GridOffset.y)
        );
        map = FindObjectOfType<Level>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveTime > 0f) {
            transform.position = new Vector3(
                Mathf.SmoothStep(StartPos.x, TargetPos.x, 1f - MoveTime),
                Mathf.SmoothStep(StartPos.y, TargetPos.y, 1f - MoveTime),
                0f
            );

            MoveTime -= Time.deltaTime * MoveSpeed;
        }
        else {
            transform.position = new Vector3(
                GridPosition.x + GridOffset.x,
                GridPosition.y + GridOffset.y,
                0f
            );
        }
    }

    public void MappedMove(Vector2 inputVec) {
        if (inputVec.sqrMagnitude < 0.01f) return;  // It's basically zero; nothing to do

        var norm = inputVec.normalized;
        if (Vector2.Dot(norm, Vector2.right) > ROOT2_2) {
            Move(RightAction);
        }
        else if (Vector2.Dot(norm, Vector2.left) > ROOT2_2) {
            Move(LeftAction);
        }
        else if (Vector2.Dot(norm, Vector2.up) > ROOT2_2) {
            Move(UpAction);
        }
        else if (Vector2.Dot(norm, Vector2.down) > ROOT2_2) {
            Move(DownAction);
        }
    }

    public void Move(Vector2Int offset) {
        var dest = GridPosition + offset;
        switch (map.CellTypeAt(dest)) {
            case CellType.ConveyorUp:
                dest += Vector2Int.up;
                break;
            case CellType.ConveyorDown:
                dest += Vector2Int.down;
                break;
            case CellType.ConveyorLeft:
                dest += Vector2Int.left;
                break;
            case CellType.ConveyorRight:
                dest += Vector2Int.right;
                break;
            case CellType.Wall:
                if (Math.Abs(offset.x) == 2 || Math.Abs(offset.y) == 2) {  // Short hops
                    Move(offset / 2);
                    return;
                }
                else if (Math.Abs(offset.x) == 1 && Math.Abs(offset.y) == 1) {  // Diagonal slide
                    var horiz = new Vector2Int(offset.x, 0);
                    var vert = new Vector2Int(0, offset.y);
                    var horizSolid = map.CellTypeAt(GridPosition + horiz) == CellType.Wall || IsOccupied(GridPosition + horiz);
                    var vertSolid  = map.CellTypeAt(GridPosition + vert) == CellType.Wall  || IsOccupied(GridPosition + vert);
                    if (horizSolid && !vertSolid) {
                        Move(vert);
                        return;
                    }
                    else if (!horizSolid && vertSolid) {
                        Move(horiz);
                        return;
                    }
                }
                Move(Vector2Int.zero);
                return;
            default:
                if (Math.Abs(offset.x) == 2 || Math.Abs(offset.y) == 2) {
                    if (map.CellTypeAt(GridPosition + offset / 2) == CellType.Wall) {  // Wall is in the way for long jump
                        Move(Vector2Int.zero);
                        return;
                    }
                }
                break;
        }
        var destType = map.CellTypeAt(dest);
        if (destType != CellType.Wall && !IsOccupied(dest)) {
            GridPosition = dest;

            StartPos = new Vector2(
                transform.position.x,
                transform.position.y
            );
            TargetPos = GridPosition + GridOffset;

            MoveTime = 1f;
        }
    }

    bool IsOccupied(Vector2Int pos) {
        foreach(var character in FindObjectsOfType<GridMovement>()) {
            if (character.GridPosition == pos) return true;
        }
        return false;
    }

    int ManhattanMagnitude(Vector2Int vec) {
        return Math.Abs(vec.x) + Math.Abs(vec.y);
    }
}
