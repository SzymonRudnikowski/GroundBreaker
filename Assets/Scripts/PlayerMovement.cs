using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Tilemap playerTilemap;
    public Tilemap foregroundTilemap;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Pozycja myszki (pixele): " + Input.mousePosition);

            var mouseWorldCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Pozycja myszki (swiat): " + mouseWorldCoords);

            var mouseCellCoords = playerTilemap.WorldToCell(mouseWorldCoords);
            Debug.Log("Pozycja komorki: " + mouseCellCoords);

            var playerCellCoords = playerTilemap.WorldToCell(transform.position);
            Debug.Log("Pozycja gracza: " + playerCellCoords);

            var distance = Vector3Int.Distance(mouseCellCoords, playerCellCoords);
            Debug.Log("Odleglosc: " + distance);

            if (Mathf.Abs(playerCellCoords.x - mouseCellCoords.x) > 1)
            {
                Debug.Log("[X] Za daleko!");
                return;
            }

            // W gore max. 2
            if (mouseCellCoords.y - playerCellCoords.y > 2)
            {
                Debug.Log("[Y] Za daleko w gore!");
                return;
            }
            // W dol max. 1
            else if (playerCellCoords.y - mouseCellCoords.y > 1)
            {
                Debug.Log("[Y] Za daleko w dol!");
                return;
            }

            TileBase clickedTile = null; // playerTilemap.GetTile<DiggableTile>(mouseCellCoords);
            if(clickedTile == null)
            {
                Debug.Log("Tego nie moge kopac");
                return;
            }
            
            //if(clickedTile.hitPoints > 0)
            //{
            //    clickedTile.hitPoints--;
            //    return;
            //}

            // Kop dziure
            playerTilemap.SetTile(mouseCellCoords, null);

            // Odslonic kolejne klocki
            //foregroundTilemap.SetTile(mouseCellCoords + new Vector3Int(0, -1, 0), null);
            for (int x = mouseCellCoords.x - 1; x <= mouseCellCoords.x + 1; x++)
            {
                for (int y = mouseCellCoords.y - 1; y <= mouseCellCoords.y + 1; y++)
                {
                    foregroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }

            // TODO
        }


    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
