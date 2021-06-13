using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MultitrackMusicPlayer))]
public class Level : MonoBehaviour
{

    public enum EndCondition {
        None,
        Win,
        Lose
    }

    public MultitrackMusicPlayer Music {get; private set;}

    Tilemap GameplayLayer;
    GridMovement[] Characters;
    Door[] Doors;

    [SerializeField]
    SceneField NextLevelPath;

    // Start is called before the first frame update
    void Start()
    {
        Characters = FindObjectsOfType<GridMovement>();
        Doors = FindObjectsOfType<Door>();
        foreach (var tilemap in FindObjectsOfType<Tilemap>()) {
            if (tilemap.gameObject.tag == "Gameplay") {
                GameplayLayer = tilemap;
                break;
            }
        }
        Music = GetComponent<MultitrackMusicPlayer>();
    }

    public CellType CellTypeAt(Vector2Int pos) {
        foreach (var door in Doors) {
            if (!door.isOpen && door.GridPosition == pos) return CellType.Wall;
        }
        var tile = GameplayLayer.GetTile(new Vector3Int(pos.x, pos.y, 0)) as PuzzleTile;

        if (tile != null) {
            return tile.Type;
        }
        else return CellType.None;
    }

    public EndCondition CheckEndConditions() {
        foreach (var character in Characters) {
            if (CellTypeAt(character.GridPosition) == CellType.DeathPit) {
                return EndCondition.Lose;
            }
        }
        for (int i = 0; i < Characters.Length; i++) {
            var character = Characters[i];
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
        }
        return EndCondition.None;
    }

    bool isAdjacent(Vector2Int a, Vector2Int b) {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) <= 1;
    }

    public void WinLevel() {
        NextLevel();
    }

    public void LoseLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void NextLevel() {
        SceneManager.LoadScene(NextLevelPath);
    }
}
