using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public int levelWidth, levelHeight, roomSize;
    GameObject levelGrid;

    public Material gridMat1, gridMat2;

    public GameObject roomPrefab, levelRooms;
    public Room[,] rooms;

    public Room currentRoom;
    public GameObject player;
    public int computers;
    private int pathLength = 0;

    private NavMeshSurface navMeshSurface;

    void Start()
    {
        GenerateGrid();
        CreateRooms();
        CreatePath();
        BakeNavMesh();

    }
    void GenerateGrid()
    {
        levelGrid = new GameObject("_LevelGrid");
        levelGrid.transform.position = new Vector3(
            (-levelWidth / 2) * roomSize, 0, (levelHeight / 2) * roomSize
        );

        Material activeGridMat = gridMat1;

        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                GameObject floorTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                floorTile.transform.parent = levelGrid.transform;
                floorTile.transform.localScale = new Vector3(roomSize, roomSize, 1);
                floorTile.transform.localEulerAngles = new Vector3(90, 0, 0);
                floorTile.transform.localPosition = new Vector3(
                    x * roomSize, 0, -y * roomSize
                );

                floorTile.GetComponent<MeshRenderer>().material = activeGridMat;
                activeGridMat = (activeGridMat == gridMat1) ? gridMat2 : gridMat1;
            }
        }
        //navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface = levelGrid.AddComponent<NavMeshSurface>();
        navMeshSurface.agentTypeID = 0;
        navMeshSurface.collectObjects = CollectObjects.Children;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        int layerToIgnore = LayerMask.NameToLayer("DoorTop");
        int ignoreLayerMask = ~(1 << layerToIgnore);
        navMeshSurface.layerMask = ignoreLayerMask;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
    }

    [ContextMenu("Bake Cool mesh")]
    void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            Debug.LogWarning("Cool bake mesh");
            navMeshSurface.BuildNavMesh();
        }
    }


    void CreateRooms()
    {
        levelRooms = new GameObject("_LevelRooms");

        levelRooms.transform.SetParent(levelGrid.transform);
        levelRooms.transform.position = new Vector3((-levelWidth / 2) * roomSize, 0, (levelHeight / 2) * roomSize);
        rooms = new Room[levelWidth, levelHeight];
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                GameObject room = (GameObject)Instantiate(roomPrefab, levelRooms.transform);
                room.name = "Room : " + x + "," + y;
                room.transform.localPosition = new Vector3(x * roomSize, 0, -y * roomSize);


                Room roomInfo = room.GetComponent<Room>();
                roomInfo.gridX = x;
                roomInfo.gridY = y;
                rooms[x, y] = roomInfo;


                if (x == 0)
                {
                    roomInfo.limits.Add("L");
                }
                if (x == levelWidth - 1)
                {
                    roomInfo.limits.Add("R");
                }
                if (y == 0)
                {
                    roomInfo.limits.Add("U");
                }
                if (y == levelHeight - 1)
                {
                    roomInfo.limits.Add("D");
                }
            }
        }
    }

    public void CreatePath()
    {
        int entranceX = Random.Range(0, levelWidth - 1);
        currentRoom = rooms[entranceX, 0];
        Room roomInfo = rooms[entranceX, 0].GetComponent<Room>();
        roomInfo.onPath = true;
        roomInfo.isEntrance = true;
        player.transform.position = roomInfo.transform.position;
        FindNextRoom();
    }

    public List<Room> GetNeighbours(Room room)
    {
        List<Room> neighbours = new List<Room>();

        if (room.gridX - 1 >= 0)
        {
            Room leftRoom = rooms[room.gridX - 1, room.gridY];
            if (!leftRoom.onPath)
            {
                neighbours.Add(leftRoom);
            }
        }

        if (room.gridX + 1 < levelWidth)
        {
            Room rightRoom = rooms[room.gridX + 1, room.gridY];
            if (!rightRoom.onPath)
            {
                neighbours.Add(rightRoom);
            }
        }

        if (room.gridY < levelHeight - 1)
        {
            Room downRoom = rooms[room.gridX, room.gridY + 1];
            if (!downRoom.onPath)
            {
                neighbours.Add(downRoom);
            }
        }
        return neighbours;
    }

    public void FindNextRoom()
    {
        currentRoom.neighbours = GetNeighbours(currentRoom);

        if (currentRoom.neighbours.Count > 0)
        {
            pathLength++;
            int nextIndex = Random.Range(0, currentRoom.neighbours.Count);
            Room nextRoom = currentRoom.neighbours[nextIndex];
            nextRoom.onPath = true;
            if (nextRoom.gridX == currentRoom.gridX - 1)
            {
                currentRoom.openings.Add("L");
                nextRoom.openings.Add("R");
            }
            if (nextRoom.gridX == currentRoom.gridX + 1)
            {
                currentRoom.openings.Add("R");
                nextRoom.openings.Add("L");
            }
            if (nextRoom.gridY == currentRoom.gridY + 1)
            {
                currentRoom.openings.Add("D");
                nextRoom.openings.Add("U");
            }
            currentRoom = nextRoom;
            FindNextRoom();
        }
        else
        {
            currentRoom.isExit = true;
            PlaceRooms();
        }
    }

    void PlaceRooms()
    {
        int pathLeft = pathLength;
        foreach (Room room in rooms)
        {
            bool computer = false;
            if (room.onPath && computers > 0)
            {
                float chance = computers / pathLeft;
                if (Random.value <= chance)
                {
                    computer = true;
                    computers--;
                }

                pathLeft--;
            }
            room.PlaceRoom(computer);
        }
    }
}
