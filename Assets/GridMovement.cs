using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    GameObject Level;

    Vector2 StartPos;
    Vector2 TargetPos;

    Vector2Int GridPosition;

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

    // Start is called before the first frame update
    void Start()
    {
        GridPosition = new Vector2Int(
            (int) Mathf.Round(transform.position.x),
            (int) Mathf.Round(transform.position.y)
        );
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
                GridPosition.x,
                GridPosition.y,
                0f
            );
        }
    }

    void Move(Vector2Int offset) {
        GridPosition += offset;
        StartPos = new Vector2(
            transform.position.x,
            transform.position.y
        );
        TargetPos = GridPosition;

        MoveTime = 1f;
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
}
