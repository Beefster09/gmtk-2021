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
    GridMovement[] AllCharacters;
    List<GridMovement> DeOverlapChars;

    Level map;
    bool Active = true;

    // Start is called before the first frame update
    void Start()
    {
        AllCharacters = FindObjectsOfType<GridMovement>();
        SelectorPair[0] = transform.GetChild(0);
        SelectorPair[1] = transform.GetChild(1);
        map = FindObjectOfType<Level>();
        int i = 0;
        foreach (var character in AllCharacters) {
            if (character.AutoSelect) SelectCharacter(i++, character);
            if (i >= 2) break;
        }
        DeOverlapChars = new List<GridMovement>(AllCharacters.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterPair[0] != null && CharacterPair[1] != null) {
            Vector2 move = Active? new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ) : Vector2.zero;

            map.Music.SetTrackVolume(1, 1f);

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
            map.Music.SetTrackVolume(1, 0f);
            isHeld = false;
            isRepeating = false;
        }

        if (Input.GetButtonDown("Jump")) {
            MovePair(Vector2.zero);
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
                foreach (var character in AllCharacters) {
                    Vector2 charPos = new Vector2(
                        character.transform.position.x,
                        character.transform.position.y
                    );
                    if (Vector2.Distance(charPos, clickPoint) < 0.5f) {
                        if (Object.ReferenceEquals(character, CharacterPair[1 - i])) {
                            if (CharacterPair[i] == null) {
                                DeselectCharacter(1 - i);
                                SelectCharacter(i, character);
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
                            DeselectCharacter(i);
                            break;
                        }
                        else {
                            SelectCharacter(i, character);
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
        DeOverlapChars.Clear();
        foreach (var character in AllCharacters) {
            if (Object.ReferenceEquals(character, CharacterPair[0]) || Object.ReferenceEquals(character, CharacterPair[1])) {
                character.MappedMove(move);
            }
            else {
                character.Move(Vector2Int.zero);
                DeOverlapChars.Add(character);
            }
        }
        if (CharacterPair[1] != null) DeOverlapChars.Add(CharacterPair[1]);
        if (CharacterPair[0] != null) DeOverlapChars.Add(CharacterPair[0]);

        bool overlapped = true;
        while (overlapped) {
            overlapped = false;
            foreach (var character in DeOverlapChars) {
                if (character.IsOverlapping() && character.CanRollback()) {
                    character.Rollback();
                    overlapped = true;
                }
            }
        }

        switch (map.CheckEndConditions()) {
            case Level.EndCondition.Win:
                Debug.Log("WINNER!");
                map.WinLevel();
                Active = false;
                break;
            case Level.EndCondition.Lose:
                Debug.Log("LOSER!");
                map.LoseLevel();
                Active = false;
                break;
        }
    }

    void SelectCharacter(int index, GridMovement character) {
        if (CharacterPair[index] == null) {
            SelectorPair[index].position = character.transform.position;
            SelectorPairVel[index] = Vector3.zero;
        }
        CharacterPair[index] = character;
        SelectorPair[index].GetComponent<ParticleSystem>()?.Play();
    }


    void DeselectCharacter(int index) {
        CharacterPair[index] = null;
        SelectorPair[index].GetComponent<ParticleSystem>()?.Stop(false);
    }
}
