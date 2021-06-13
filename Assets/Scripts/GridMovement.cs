using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GridMovement : MonoBehaviour
{

    public bool AutoSelect = false;

    // input map
    [SerializeField]
    Vector2Int UpAction = Vector2Int.up;
    [SerializeField]
    Vector2Int DownAction = Vector2Int.down;
    [SerializeField]
    Vector2Int LeftAction = Vector2Int.left;
    [SerializeField]
    Vector2Int RightAction = Vector2Int.right;

    [SerializeField]
    string UpAnimation;
    [SerializeField]
    string DownAnimation;
    [SerializeField]
    string LeftAnimation;
    [SerializeField]
    string RightAnimation;
    [SerializeField]
    string WinAnimation;
    [SerializeField]
    string LoseAnimation;

    Level map;

    Vector2 StartPos;
    Vector2 TargetPos;

    public Vector2Int GridPosition {get; private set;}
    Vector2 GridOffset = new Vector2(0.5f, 0.5f);

    float MoveTime = 0f;

    [SerializeField]
    float MoveSpeed = 5f;
    const float ROOT2_2 = 0.707106f;

    GridMovement[] AllCharacters;
    List<Vector2Int> RollbackBuffer;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        GridPosition = new Vector2Int(
            (int) Mathf.Floor(Mathf.Floor(transform.position.x) + GridOffset.x),
            (int) Mathf.Floor(Mathf.Floor(transform.position.y) + GridOffset.y)
        );
        map = FindObjectOfType<Level>();
        AllCharacters = FindObjectsOfType<GridMovement>();
        RollbackBuffer = new List<Vector2Int>();
        anim = GetComponent<Animator>();
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
        if (inputVec.sqrMagnitude < 0.01f) {
            Move(Vector2Int.zero);
            return;
        }

        var norm = inputVec.normalized;
        if (Vector2.Dot(norm, Vector2.right) > ROOT2_2) {
            Move(RightAction);
            anim.Play("Base Layer." + RightAnimation, -1, 0f);
        }
        else if (Vector2.Dot(norm, Vector2.left) > ROOT2_2) {
            Move(LeftAction);
            anim.Play("Base Layer." + LeftAnimation, -1, 0f);
        }
        else if (Vector2.Dot(norm, Vector2.up) > ROOT2_2) {
            Move(UpAction);
            anim.Play("Base Layer." + UpAnimation, -1, 0f);
        }
        else if (Vector2.Dot(norm, Vector2.down) > ROOT2_2) {
            Move(DownAction);
            anim.Play("Base Layer." + DownAnimation, -1, 0f);
        }
    }

    public void Move(Vector2Int offset) {
        bool isFrogMove = Math.Abs(offset.x) == 2 || Math.Abs(offset.y) == 2;
        if (isFrogMove) {
            if (map.CellTypeAt(GridPosition + offset / 2) == CellType.Wall) {  // Wall is in the way for long jump
                Move(Vector2Int.zero);
                return;
            }
        }
        RollbackBuffer.Clear();
        var dest = GridPosition + offset;
        if (dest != GridPosition) {
            RollbackBuffer.Add(GridPosition);
        }
        if (isFrogMove) {  // Frog: enable rollback to intermediate position
            RollbackBuffer.Add(GridPosition + offset / 2);
        }
        switch (map.CellTypeAt(dest)) {
            case CellType.ConveyorUp:
                RollbackBuffer.Add(dest);
                dest += Vector2Int.up;
                break;
            case CellType.ConveyorDown:
                RollbackBuffer.Add(dest);
                dest += Vector2Int.down;
                break;
            case CellType.ConveyorLeft:
                RollbackBuffer.Add(dest);
                dest += Vector2Int.left;
                break;
            case CellType.ConveyorRight:
                RollbackBuffer.Add(dest);
                dest += Vector2Int.right;
                break;
            case CellType.Wall:
                if (isFrogMove) {  // Short hops
                    Move(offset / 2);
                    return;
                }
                else if (Math.Abs(offset.x) == 1 && Math.Abs(offset.y) == 1) {  // Diagonal slide
                    var horiz = new Vector2Int(offset.x, 0);
                    var vert = new Vector2Int(0, offset.y);
                    var horizSolid = map.CellTypeAt(GridPosition + horiz) == CellType.Wall;
                    var vertSolid  = map.CellTypeAt(GridPosition + vert) == CellType.Wall;
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
                break;
        }
        var destType = map.CellTypeAt(dest);
        if (destType != CellType.Wall) {
            GridPosition = dest;

            StartPos = new Vector2(
                transform.position.x,
                transform.position.y
            );
            TargetPos = GridPosition + GridOffset;

            MoveTime = 1f;
        }
    }

    // corrects the position after moving
    public void Rollback() {
        GridPosition = RollbackBuffer[RollbackBuffer.Count - 1];
        TargetPos = GridPosition + GridOffset;
        RollbackBuffer.RemoveAt(RollbackBuffer.Count - 1);
    }

    public bool CanRollback() {
        return RollbackBuffer.Count > 0;
    }

    public bool IsOverlapping() {
        foreach(var character in AllCharacters) {
            if (ReferenceEquals(this, character)) continue;
            if (character.GridPosition == GridPosition) return true;
        }
        return false;
    }

    public void Win() {
        anim.Play("Base Layer." + WinAnimation);
    }

    public void Lose() {
        anim.Play("Base Layer." + LoseAnimation);
    }
}
