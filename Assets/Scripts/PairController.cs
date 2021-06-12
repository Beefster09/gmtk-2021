using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairController : MonoBehaviour
{
    public float InputDeadzone = 0.25f;
    public float InputRepeatDelay = 0.5f;
    public float InputRepeat = 0.1f;

    bool isHeld = false;
    bool isRepeating = false;
    Vector2 HoldReferenceDir = Vector2.zero;
    float HoldTime = 0f;

    public float SelectorFollowTime = 0.05f;

    GridMovement[] CharacterPair = new GridMovement[]{null, null};
    Transform[] SelectorPair = new Transform[]{null, null};
    Vector3[] SelectorPairVel = new Vector3[]{Vector3.zero, Vector3.zero};

    Level map;
    bool Active = true;

    // Start is called before the first frame update
    void Start()
    {
        SelectorPair[0] = transform.GetChild(0);
        SelectorPair[1] = transform.GetChild(1);
        map = FindObjectOfType<Level>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active && CharacterPair[0] != null && CharacterPair[1] != null) {
            Vector2 move = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            // Process input
            if (move.magnitude > InputDeadzone) {
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
                    Vector2 charPos = new Vector2(
                        character.transform.position.x,
                        character.transform.position.y
                    );
                    if (Vector2.Distance(charPos, clickPoint) < 0.5f) {
                        if (Object.ReferenceEquals(character, CharacterPair[1 - i])) {
                            if (CharacterPair[i] == null) {
                                CharacterPair[i] = character;
                                CharacterPair[1 - i] = null;

                                SelectorPair[1 - i].GetComponent<ParticleSystem>()?.Stop(false);

                                SelectorPair[i].position = character.transform.position;
                                SelectorPair[i].GetComponent<ParticleSystem>()?.Play();
                                SelectorPairVel[i] = Vector3.zero;
                            }
                            else {
                                // It's the other character in the pair, so swap the selections
                                var tmp = CharacterPair[0];
                                CharacterPair[0] = CharacterPair[1];
                                CharacterPair[1] = tmp;
                            }
                        }
                        else if (Object.ReferenceEquals(character, CharacterPair[i])) {
                            // Same character, deactivate
                            CharacterPair[i] = null;
                            SelectorPair[i].GetComponent<ParticleSystem>()?.Stop(false);
                            break;
                            // TODO? cycle select for characters on the same cell?
                        }
                        else {
                            if (CharacterPair[i] == null) {
                                SelectorPair[i].position = character.transform.position;
                                SelectorPairVel[i] = Vector3.zero;
                            }
                            CharacterPair[i] = character;
                            SelectorPair[i].GetComponent<ParticleSystem>()?.Play();
                        }
                        break;
                    }
                }
            }

            if (CharacterPair[i] != null) {
                SelectorPair[i].position = Vector3.SmoothDamp(
                    SelectorPair[i].position,
                    CharacterPair[i].transform.position,
                    ref SelectorPairVel[i],
                    SelectorFollowTime
                );
            }
        }
    }

    void MovePair(Vector2 move) {
        foreach (var character in CharacterPair) {
            character.MappedMove(move);
        }
        switch (map.CheckEndConditions()) {
            case Level.EndCondition.Win:
                Debug.Log("WINNER!");
                // map.WinLevel()
                Active = false;
                break;
            case Level.EndCondition.Lose:
                Debug.Log("LOSER!");
                // map.LoseLevel()
                Active = false;
                break;
        }
    }
}
