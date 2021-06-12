using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{

    Tilemap GameplayLayer;

    public enum EndCondition {
        None,
        Win,
        Lose
    }

    GridMovement[] Characters;

    // Start is called before the first frame update
    void Start()
    {
        Characters = FindObjectsOfType<GridMovement>();
        foreach (var tilemap in FindObjectsOfType<Tilemap>()) {
            if (tilemap.gameObject.tag == "Gameplay") {
                GameplayLayer = tilemap;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CellType CellTypeAt(Vector2Int pos) {
        var tile = GameplayLayer.GetTile(new Vector3Int(pos.x, pos.y, 0)) as PuzzleTile;

        if (tile != null) {
            return tile.Type;
        }
        else return CellType.None;
    }

    public EndCondition CheckEndConditions() {
        for (int i = 0; i < Characters.Length; i++) {
            var character = Characters[i];
            switch (CellTypeAt(character.GridPosition)) {
                case CellType.DeathPit:
                    return EndCondition.Lose;
                default:
                    for (int j = i + 1; j < Characters.Length; j++) {
                        var other = Characters[j];
                        if (isAdjacent(character.GridPosition, other.GridPosition)) {
                            var contiguous = new List<GridMovement>();
                            contiguous.Add(character);
                            contiguous.Add(other);
                            while (contiguous.Count < Characters.Length) {
                                bool found = false;
                                foreach (var more in Characters) {
                                    if (contiguous.Contains(more)) continue;
                                    foreach (var clustered in contiguous) {
                                        if (isAdjacent(more.GridPosition, clustered.GridPosition)) {
                                            found = true;
                                            contiguous.Add(more);
                                            break;
                                        }
                                    }
                                }
                                if (!found) break;
                            }
                            if (contiguous.Count >= Characters.Length) return EndCondition.Win;
                        }
                    }
                    break;
            }
        }
        return EndCondition.None;
    }

    bool isAdjacent(Vector2Int a, Vector2Int b) {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) <= 1;
    }
}
