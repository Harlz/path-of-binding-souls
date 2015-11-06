//---------------------------------
//	File:	DungeonRoom.cs
//	Author: Harley Laurie
//	Brief:	Holds information for a room in the dungeon
//---------------------------------

using UnityEngine;
using System.Collections;

public class DungeonRoom : MonoBehaviour
{
    public RoomTypes RoomType; //The type of room this is
    public GameObject DungeonFloor; //The floor of this room
    public int RoomNumber; //The number of this room in the dungeon
    public Vector2 RoomPosition; //The rooms position in the dungeon grid (1,1 etc)

    //Pathfinding
    public int GScore = 1;
    public int HScore;
    public int FScore;
    public DungeonRoom ParentRoom;

    void Start()
    {
        //Rooms all start off as void rooms and are changed later when
        //they are given their purpose during generation of the dungeon layout
        RoomType = RoomTypes.VoidRoom;
    }

    private void CalculateGScore()
    {
        GScore = ParentRoom.GetComponent<DungeonRoom>().GScore + 1;
    }

    private void CalculateHScore()
    {
        //We use the manhattan distance method to find the h score
        DungeonRoom StartRoom = GameObject.Find("System").GetComponent<DungeonCreator>().GetStartRoom();
        int XDistance = (int)(StartRoom.RoomPosition.x > RoomPosition.x ? StartRoom.RoomPosition.x - RoomPosition.x : RoomPosition.x - StartRoom.RoomPosition.x);
        int YDistance = (int)(StartRoom.RoomPosition.y > RoomPosition.y ? StartRoom.RoomPosition.y - RoomPosition.y : RoomPosition.y - StartRoom.RoomPosition.y);
        HScore = XDistance + YDistance;
    }

    public int CalculateFScore()
    {
        //Calculate G and H Scores before finding the F Score
        CalculateGScore();
        CalculateHScore();
        return GScore + HScore;
    }
}