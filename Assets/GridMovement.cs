using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    GameObject Level;

    Vector2 StartPos;
    Vector2 TargetPos;

    Vector2 GridOffset;
    Vector2Int GridPosition;

    public bool Active = true;

    // TODO input map
    public Vector2Int UpAction = Vector2Int.up;
    public Vector2Int DownAction = Vector2Int.down;
    public Vector2Int LeftAction = Vector2Int.left;
    public Vector2Int RightAction = Vector2Int.right;

    float MoveTime = 0f;

    const float MovesPerSecond = 5f;

    // Start is called before the first frame update
    void Start()
    {
        GridOffset = new Vector2(
            Mathf.Repeat(transform.position.x, 1.0f),
            Mathf.Repeat(transform.position.y, 1.0f)
        );
        GridPosition = new Vector2Int(
            (int) transform.position.x,
            (int) transform.position.y
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active) return;

        if (MoveTime > 0f) {
            transform.position = new Vector3(
                Mathf.SmoothStep(StartPos.x, TargetPos.x, 1f - MoveTime),
                Mathf.SmoothStep(StartPos.y, TargetPos.y, 1f - MoveTime),
                0f
            );

            MoveTime -= Time.deltaTime * MovesPerSecond;
        }
        else {
            transform.position = new Vector3(
                GridPosition.x + GridOffset.x,
                GridPosition.y + GridOffset.y,
                0f
            );

            float horiz = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            if (horiz > 0) {
                Move(RightAction);
            }
            else if (horiz < 0) {
                Move(LeftAction);
            }
            else if (vert > 0) {
                Move(UpAction);
            }
            else if (vert < 0) {
                Move(DownAction);
            }
        }
    }

    void Move(Vector2Int offset) {
        GridPosition += offset;
        StartPos = new Vector2(
            transform.position.x,
            transform.position.y
        );
        TargetPos = GridPosition + GridOffset;

        MoveTime = 1f;
    }
}
