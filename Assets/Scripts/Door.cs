using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class Door : MonoBehaviour
{
    public int GroupId = 1;
    public Sprite OpenDoor;
    public Sprite ClosedDoor;

    public bool DefaultOpen = false;

    public bool isOpen {get; private set;}

    public Vector2Int GridPosition {get; private set;}
    Vector2 GridOffset = new Vector2(0.5f, 0.5f);

    SpriteRenderer gfx;
    // Start is called before the first frame update
    void Start()
    {
        GridPosition = new Vector2Int(
            (int) Mathf.Floor(Mathf.Floor(transform.position.x) + GridOffset.x),
            (int) Mathf.Floor(Mathf.Floor(transform.position.y) + GridOffset.y)
        );
        // TODO: automatically tint sprite so that matching GroupIds are the same color?
        isOpen = DefaultOpen;
        gfx = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(
            GridPosition.x + GridOffset.x,
            GridPosition.y + GridOffset.y,
            0f
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set(bool state) {
        isOpen = state != DefaultOpen;
        gfx.sprite = isOpen? OpenDoor : ClosedDoor;
    }
}
