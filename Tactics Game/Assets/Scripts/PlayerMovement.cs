using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private float       playerSpeed;

    private GameObject  maps;
    private Tilemap     currentMap;
    private int         currentMapIndex;
    private Vector3     currentFullOffsets;
    private Vector3     currentTilemapOffset;
    private Vector3     playerPosition;

    private Vector3     destination;
    private bool        moveToDestination = false;


    private void Awake() {

        maps = GameObject.FindGameObjectWithTag("Map");
        
        for (int i = maps.transform.childCount - 1; i >= 0; i--) { // Sets inital values
            Tilemap map = maps.transform.GetChild(i).GetComponent<Tilemap>();
            if (CTMF.DoesTileExistOnLevel(transform.position, map)) {
                currentMapIndex = i;
                currentMap = map;
                currentTilemapOffset = GetTileHeight(transform.position);
                currentFullOffsets = THV.fullHeight * Mathf.Max(i - 1, 0f);
                break;
            }
        }

        SetPlayerAndSpritePosition();

        Debug.Log("[PlayerMovment] Current map is " + currentMap.name + " and position is " + currentMap.WorldToCell(playerPosition));

    }

    private void Update() {
        CheckMovePlayerInput();
        if (moveToDestination) MovePlayer();
    }

    private void CheckMovePlayerInput() { // Checks for player input
        if (Input.GetKeyDown(KeyCode.W)) {
            CheckIfDestinationIsAvailable(new Vector3Int(1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            CheckIfDestinationIsAvailable(new Vector3Int(0, 1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            CheckIfDestinationIsAvailable(new Vector3Int(-1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            CheckIfDestinationIsAvailable(new Vector3Int(0, -1, 0));
        }
    }
    private void CheckIfDestinationIsAvailable(Vector3Int tileDirection) { // Checks if destination is available on tilemap
        Vector3 direction = currentMap.CellToWorld(tileDirection);
        Vector3 dest = (playerPosition - GetPlayerTotalOffset() + THV.fullHeight * currentMapIndex) + direction;
        if (DoesTileMapExistWithTile(1, dest)) { // Checks on tilemap of higher index
            //Debug.Log(currentMap.WorldToCell(dest + THV.fullHeight));
        }
        else if (CTMF.DoesTileExistOnLevel(dest, currentMap)) { // Checks on current tilemap
            destination = dest - GetPlayerTotalOffset() + currentFullOffsets + GetTileHeight(dest);
            currentTilemapOffset = GetTileHeight(dest);
            moveToDestination = true;
            //Debug.Log("[PlayerMovement] Tile move available");
        }
        else if (DoesTileMapExistWithTile(-1, dest)) { // Checks on tilemap of lower index
            //Debug.Log(currentMap.WorldToCell(dest - THV.fullHeight));
        }
        else {
            Debug.Log("[PlayerMovement] Tile move unavailable");
        }
    }
    private void MovePlayer() { // Moves the player
        transform.position = Vector3.MoveTowards(transform.position, destination - GetPlayerTotalOffset(), playerSpeed * Time.deltaTime);
        if (transform.position == destination - GetPlayerTotalOffset()) {
            playerPosition = transform.position + GetPlayerTotalOffset();
            moveToDestination = false;
            Debug.Log("[PlayerMovement] Arrived at destination");
        }
    }
    private void MovePlayerSprite() {
        transform.position = Vector3.MoveTowards(transform.position, destination - GetPlayerTotalOffset(), playerSpeed * Time.deltaTime);
    }
    private void SetPlayerAndSpritePosition() { // Sets parent of player to position (on base layer) and sprite of player to correct position
        transform.position = CTMF.AlignToTile(transform.position, currentMap) - (currentFullOffsets + THV.fullHeight);
        transform.GetChild(0).localPosition = GetPlayerTotalOffset();
        playerPosition = transform.position + GetPlayerTotalOffset();
    }
    private bool DoesTileMapExistWithTile(int val, Vector3 pos) { // Checks if tilemap exists and if tile exists on the tilemap
        if (maps.transform.GetChild(currentMapIndex + val) != null) {
            return CTMF.DoesTileExistOnLevel(pos + THV.fullHeight, maps.transform.GetChild(currentMapIndex + val).GetComponent<Tilemap>());
        }
        else {
            return false;
        }
    }
    private Vector3 GetPlayerTotalOffset() { // Returns total player offset
        return currentFullOffsets + currentTilemapOffset;
    }
    private Vector3 GetTileHeight(Vector3 pos) { // Returns height for specific tile
        string tileName = currentMap.GetTile(currentMap.WorldToCell(pos)).name;
        Vector3 height = Vector3.zero;

        switch (tileName) {

            case var _ when tileName.Contains("FullRampBR"):
                Debug.Log("[PlayerMovement] Full ramp bottom-right tile");
                height = THV.halfHeight;
                break;

            case var _ when tileName.Contains("FullRampBL"):
                Debug.Log("[PlayerMovement] Full ramp bottom-left tile");
                height = THV.halfHeight;
                break;

            case var _ when tileName.Contains("RampHighBR"):
                Debug.Log("[PlayerMovement] Half ramp high bottom-right tile");
                height = THV.halfHeight + THV.quarterHeight;
                break;

            case var _ when tileName.Contains("RampHighBL"):
                Debug.Log("[PlayerMovement] Half ramp high bottom-left tile");
                height = THV.halfHeight + THV.quarterHeight;
                break;

            case var _ when tileName.Contains("RampLowBR"):
                Debug.Log("[PlayerMovement] Half ramp low bottom-right tile");
                height = THV.quarterHeight;
                break;

            case var _ when tileName.Contains("RampLowBL"):
                Debug.Log("[PlayerMovement] Half ramp low bottom-left tile");
                height = THV.quarterHeight;
                break;

            case var _ when tileName.Contains("Full"):
                Debug.Log("[PlayerMovement] Full tile");
                height = THV.fullHeight;
                break;

            case var _ when tileName.Contains("Half"):
                Debug.Log("[PlayerMovement] Half tile");
                height = THV.halfHeight;
                break;
        }

        return height;
    }
}
