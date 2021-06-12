using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{

    Tilemap GameplayLayer;

    // Start is called before the first frame update
    void Start()
    {
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
}
