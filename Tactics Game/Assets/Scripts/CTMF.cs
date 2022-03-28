using UnityEngine;
using UnityEngine.Tilemaps;
public static class THV {

    public static readonly Vector3 fullHeight = new Vector3(0f, 0.16f, 0f);
    public static readonly Vector3 halfHeight = new Vector3(0f, 0.08f, 0f);
    public static readonly Vector3 quarterHeight = new Vector3(0f, 0.04f, 0f);
}
public static class CTMF {

    public static Vector3Int ScreentoTilePosition(Vector3 pos, Tilemap map) { // Screen Position to Tile Position on Map

        Camera cam = Camera.main;
        Vector3 point = cam.ScreenToWorldPoint(pos);

        return map.WorldToCell(point);
    }

    public static Vector3 TileToScreenPosition(Vector3Int pos, Tilemap map) { // Converts Tile Position on map to Screen Position

        Camera cam = Camera.main;
        Vector3 point = map.CellToWorld(pos);
        
        return cam.WorldToScreenPoint(point);
    }

    public static Vector3 AlignToTile(Vector3 pos, Tilemap map) { // Aligns object onto tile

        Vector3Int tilePos = map.WorldToCell(pos);
        return map.CellToWorld(tilePos);
    }

    public static bool DoesTileExistOnLevel(Vector3 pos, Tilemap map) { // Checks if tile exists on current tilemap

        return map.HasTile(map.WorldToCell(pos));
    }

}
