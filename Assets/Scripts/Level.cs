using UnityEngine;

public class Level : MonoBehaviour
{
    public MapGenerator mapGenerator;

    public RoomTile[,] Rooms
    {
        get { return mapGenerator.Grid; }
    }

    public void GenerateLevel()
    {
        mapGenerator.GenerateMap();
    }
}