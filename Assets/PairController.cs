using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairController : MonoBehaviour
{
    GridMovement[] CharacterPair = new GridMovement[]{null, null};

    public float InputRepeatDelay = 2.0f;
    public float InputRepeat = 0.25f;

    bool isHeld = false;
    bool isRepeating = false;
    Vector2 HoldReferenceDir = Vector2.zero;
    float HoldTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterPair[0] != null && CharacterPair[1] != null) {
            Vector2 move = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            // Process input
            if (move.magnitude > 0.25f) {
                if (isHeld && Vector2.Dot(HoldReferenceDir, move.normalized) < 0.5) {
                    isRepeating = false;
                    HoldReferenceDir = move.normalized;
                    HoldTime = 0f;
                    MovePair(move);
                }
                if (isRepeating) {  // waiting for next repeat
                    if (HoldTime > InputRepeat) {
                        HoldTime = 0f;
                        MovePair(move);
                    }
                }
                else if (isHeld) {  // waiting for repeat or moving the stick enough
                    if (HoldTime > InputRepeatDelay) {
                        HoldTime = 0f;
                        isRepeating = true;
                    }
                }
                else {  // wasn't previously holding a direction
                    isHeld = true;
                    HoldReferenceDir = move.normalized;
                    HoldTime = 0f;
                    MovePair(move);
                }
                HoldTime += Time.deltaTime;
            }
            else {
                isHeld = false;
                isRepeating = false;
            }
        }
        else {
            isHeld = false;
            isRepeating = false;
        }

        // Check mouse clicks and reassign one character of the pair
        for (int i = 0; i < 2; i++) {
            if (Input.GetMouseButtonDown(i)) {
                Vector3 clickPoint3d = Camera.main.ScreenToWorldPoint(
                    new Vector3(
                        Input.mousePosition.x,
                        Input.mousePosition.y,
                        0f
                    )
                );
                Vector2 clickPoint = new Vector2(clickPoint3d.x, clickPoint3d.y);
                foreach (var character in FindObjectsOfType<GridMovement>()) {
                    if (Object.ReferenceEquals(character, CharacterPair[1 - i])) continue;
                    Vector2 charPos = new Vector2(
                        character.transform.position.x,
                        character.transform.position.y
                    );
                    if (Vector2.Distance(charPos, clickPoint) < 0.5f) {
                        CharacterPair[i] = character;
                        // TODO particle effects on selection
                        // TODO? cycle select for characters on the same cell?
                    }
                }
            }
        }

        // TODO Move selection indicators
    }

    void MovePair(Vector2 move) {
        foreach (var character in CharacterPair) {
            character.MappedMove(move);
        }
    }
}
