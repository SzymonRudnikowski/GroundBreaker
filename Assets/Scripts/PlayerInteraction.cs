using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerInteraction : MonoBehaviour
{
    #region Private Stuff

    private CharacterController2D controller;
    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;

    #endregion

    public float runSpeed = 40f;

    public int digPower = 1;

    public Tilemap diggableTilemap;
    public Tilemap foregroundTilemap;

    public TileBase whiteTile;
    public TileBase blackTile;

    private TilemapInformation diggableInfo;



    public bool CanDig(Vector3 digWorldPosition)
    {
        // No tilemap set
        if (diggableTilemap == null) return false;

        var digCellPosition = diggableTilemap.WorldToCell(digWorldPosition);

        // Calculate dig area
        var playerCellPosition = diggableTilemap.WorldToCell(transform.position);

        int x = playerCellPosition.x, y = playerCellPosition.y + 1, w = 1, h = 2;

        var mineZoneChecks = transform.Find("MineZoneChecks");
        for (int i = 0; i < mineZoneChecks.childCount; i++)
        {
            var mineZoneCheck = mineZoneChecks.GetChild(i);
            var zoneCellPosition = diggableTilemap.WorldToCell(mineZoneCheck.position);

            x = Mathf.Min(x, zoneCellPosition.x);
            y = Mathf.Max(y, zoneCellPosition.y);
            if (zoneCellPosition.x != playerCellPosition.x)
            {
                w = 2;
            }
        }

        // Check if click is in range
        return digCellPosition.x >= x - 1 &&
            digCellPosition.x <= x + w &&
            digCellPosition.y <= y + 1 &&
            digCellPosition.y >= y - h;
    }



    public TileBase Dig(Vector3 digWorldposition)
    {
        var digCellPosition = diggableTilemap.WorldToCell(digWorldposition);

        var tile = diggableTilemap.GetTile<DiggableTile>(digCellPosition);

        // Nothing to dig
        if (tile == null) return null;

        
        int tileHealth = diggableInfo.Get(digCellPosition, "health", -1);

        // Weird but tile should not be there
        if(tileHealth <= 0)
        {
            Debug.LogWarning("There should be no tile at " + digCellPosition + " since it has no health");
        }

        tileHealth -= digPower;
        
        // Todo - spawn death effect?

        // Tile has been destroyed and need to be removed
        if(tileHealth <= 0)
        {
            diggableInfo.Remove(digCellPosition, "health");
            diggableTilemap.SetTile(digCellPosition, null);
            Inventory.instance.Add(tile);
            return tile;
        }
        // Tile has been damaged only and health info need to be updated
        else
        {
            diggableInfo.Set(digCellPosition, "health", tileHealth);
            return null;
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        diggableInfo = diggableTilemap.GetComponent<TilemapInformation>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }


        if (Input.GetMouseButtonDown(0))
        {
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            bool canDig = CanDig(mouseWorldPosition);

            if(canDig)
            {
                Dig(mouseWorldPosition);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foregroundTilemap.ClearAllTiles();
        }
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
