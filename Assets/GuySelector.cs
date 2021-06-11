using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuySelector : MonoBehaviour
{

    public GameObject SelectedGuy;

    public int MouseButton = 0;

    GridMovement[] Guys;

    // Start is called before the first frame update
    void Start()
    {
        Guys = FindObjectsOfType<GridMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(MouseButton)) {
            Vector2 mousePos = new Vector2(
                Input.mousePosition.x,
                Input.mousePosition.y
            );

            Vector3 clickPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
            clickPoint.z = 0f;
            Debug.Log(clickPoint);
            foreach (var guy in Guys) {
                Debug.Log(guy);
                Debug.Log(Vector3.Distance(guy.transform.position, clickPoint));
                if (Vector3.Distance(guy.transform.position, clickPoint) < 0.5f) {
                    SelectedGuy.GetComponent<GridMovement>().Active = false;
                    SelectedGuy = guy.gameObject;
                    guy.Active = true;
                    break;
                }
            }
        }
        transform.position = SelectedGuy.transform.position;
    }
}
