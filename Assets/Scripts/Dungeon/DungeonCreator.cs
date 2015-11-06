//---------------------------------
//	File:	DungeonCreator.cs
//	Author: Harley Laurie
//	Brief:	Generates a random dungeon layout for the player
//  to adventure through
//---------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonCreator : MonoBehaviour
{
    //Public config options for dungeon generation
    public Vector2 DungeonSize; //The length and width of the dungeon
    public int DetourRoomCount; //Amount of detour rooms that are used
    public int StartEndMinimumDistance; //Required distance between the start room and boss room
    public bool UseAStarDungeonGeneration;  //Will we use A* to create the paths through the dungeon?
    public bool RemoveExtraRoomFloors; 

    //Prefabs for dungeon generation
    public GameObject DungeonContainer; //Empty object where all dungeon rooms are kept to declutter the Hierarchy during runtime
    public GameObject DungeonFloor1x1Prefab; //The floor of each room in the dungeon
    public GameObject DungeonTeleporterPrefab; //The spawn room indicator
    public GameObject DungeonBossPrefab; //The boss room indicator
    public GameObject DungeonDetourRoomPrefab; //The detour room indicator
    public GameObject DungeonPathPrefab; //The dungeon path indicator

    //List of all rooms in the dungeon
    private GameObject DungeonRoot; //Simple root object which is the parent of all dungeon rooms
    List<DungeonRoom> DungeonRooms; //List of every room in the dungeon
    List<DungeonRoom> DetourRooms; //List of every detour room in the dungeon

    //Room numbers of specific rooms
    private int StartRoomNumber; //The spawn room number
    private int BossRoomNumber; //The boss room number

    //Important rooms
    private DungeonRoom DungeonStartRoom;
    private DungeonRoom DungeonBossRoom;
    
    //How many rooms are in this dungeon
    private int RoomCount;

    //Random, used to place certain objective rooms at random locations
    System.Random RNG;

    void Start ()
    {
        //Init members
        DungeonRooms = new List<DungeonRoom>();
        DetourRooms = new List<DungeonRoom>();
        RNG = new System.Random();

        //Create a root object to contain all rooms in the dungeon
        DungeonRoot = (GameObject)Instantiate(DungeonContainer, Vector3.zero, Quaternion.identity);

        //Generate the dungeon
        CreateDungeon();
	}

    public void CreateDungeon()
    {
        RoomCount = (int)(DungeonSize.x * DungeonSize.y);   //Store the size of the dungeon
        CreateFloor();  //Create the floor of the dungeon
        PlaceObjectives();  //Put the teleporter room and the boss room in random locations
        PlaceDetourRooms(); //Place the required amount of detour rooms into the dungeon
        PathToDetourRooms(UseAStarDungeonGeneration);    //Place paths from the start room to each detour room
        PathToRoom(DungeonBossRoom, UseAStarDungeonGeneration);    //Place a path to the boss room
        if(RemoveExtraRoomFloors)
            RemoveExtraRooms(); //Remove the extra empty rooms
    }

    public DungeonRoom GetStartRoom()
    {
        return DungeonStartRoom;
    }

    //Returns the room with the number that is passed in
    public DungeonRoom GetRoom(int RoomNumber)
    {
        DungeonRoom RequestedRoom = null;

        //Loop through each room in the dungeon until we find the room that
        //has been requested, then return that room
        for ( int i = 0; i < RoomCount; i++ )
        {
            if (DungeonRooms[i].RoomNumber == RoomNumber)
                return DungeonRooms[i];
        }

        //Print an error message if the room was not found
        print("No room with requested number " + RoomNumber + " was found in this dungeon.");
        return RequestedRoom;
    }

    //Returns the room with the position that is passed in
    public DungeonRoom GetRoom(Vector2 RoomPosition)
    {
        DungeonRoom RequestedRoom = null;

        //Loop through each room in the dungeon until we find the room that
        //has been requested, then return that room
        for ( int i = 0; i < RoomCount; i++ )
        {
            if (DungeonRooms[i].RoomPosition == RoomPosition)
                return DungeonRooms[i];
        }

        //Print an error message if the room was not found
        print("No room with requested position" + RoomPosition + " was found in this dungeon.");
        return null;
    }
    
    private void CreateFloor()
    {
        //The location for the next section of the dungeon floor to be created at
        Vector3 SpawnLocation = new Vector3(0, 0, 0);
        //The room number that is being added next
        int NextRoomNumber = 0;
        //The dimensions/size of a section of the dungeon floor
        Vector2 FloorSectionDimensions = new Vector2(0, 0);
        FloorSectionDimensions.x = DungeonFloor1x1Prefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        FloorSectionDimensions.y = DungeonFloor1x1Prefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;

        //Loop through the length and width of the room to instantiate each room
        for ( int i = 0; i < DungeonSize.x; i++ )
        {
            for ( int j = 0; j < DungeonSize.y; j++ )
            {
                //Create a room object to hold all the objects in the room
                DungeonRoom NewRoom = new DungeonRoom();
                //Give the room its number
                NewRoom.RoomNumber = NextRoomNumber;
                NextRoomNumber++;
                //Give the room its position in the grid
                NewRoom.RoomPosition = new Vector2(i, j);
                //Instantiate the floor of the new room
                NewRoom.DungeonFloor = (GameObject) Instantiate(DungeonFloor1x1Prefab, SpawnLocation, Quaternion.identity);
                //Set the floor as a child of the dungeon root object
                NewRoom.DungeonFloor.transform.parent = DungeonRoot.transform;
                //Add the new room to our list of all dungeon rooms
                DungeonRooms.Add(NewRoom);
                //Offset the x position for spawning the next floor section
                SpawnLocation.x += FloorSectionDimensions.x;
            }
            //Offset the z position and reset the x pos for spawning the next floor section
            SpawnLocation.x = 0;
            SpawnLocation.z += FloorSectionDimensions.y;
        }
    }

    private void RemoveExtraRooms()
    {
        //Loop through each room in the dungeon grid
        for(int i = 0; i < RoomCount; i++ )
        {
            //Check the type of the room
            DungeonRoom Check = DungeonRooms[i];
            //If its a void room, remove the floor
            if (Check.RoomType == RoomTypes.VoidRoom)
                GameObject.Destroy(Check.DungeonFloor);
        }
    }

    private void PlaceObjectives()
    {
        //Select a room where the player will start
        int StartRoomNumber = RNG.Next(0, (int)(DungeonSize.x * DungeonSize.y));
        //Select a second room, which isnt the same, for the boss
        int BossRoomNumber = StartRoomNumber;
        bool BossRoomPlaced = false;

        DungeonRoom StartRoom = GetRoom(0);
        DungeonRoom BossRoom = GetRoom(0);

        //If this exceeds 10, the required distance is probably too small
        //for the current size of the dungeon, if this happens we just
        //place the boss room somewhere that breaks this rule and print
        //an error message, so we dont get stuck in an infinite loop of
        //trying to find a place far enough away to place the boss room
        int BossRoomPlacementFailures = 0;

        while(!BossRoomPlaced)
        {
            //Find a spot to potentially place the boss room
            while (BossRoomNumber == StartRoomNumber)
                BossRoomNumber = RNG.Next(0, (int)(DungeonSize.x * DungeonSize.y));
            //Find these two rooms in the dungeon
            StartRoom = GetRoom(StartRoomNumber);
            BossRoom = GetRoom(BossRoomNumber);
            //If the required distance is 0, dont check for it, just place the rooms
            if (StartEndMinimumDistance == 0)
            {
                BossRoomPlaced = true;
                break;
            }
            //Check if these two rooms are far enough away as required by the distance
            //set in the inspector
            float StartBossDistance = Vector3.Distance(StartRoom.DungeonFloor.transform.position, BossRoom.DungeonFloor.transform.position);
            if (StartBossDistance >= StartEndMinimumDistance)
                BossRoomPlaced = true;
            else
            {
                //If the rooms are too close, reset the boss room number so it is placed again
                BossRoomNumber = StartRoomNumber;
                BossRoomPlacementFailures++;
                //If we have failed too many times, stop trying
                if(BossRoomPlacementFailures == 10)
                {
                    print("Failed to place boss room too many times, breaking distance rule to avoid infinite loop");
                    BossRoomPlaced = true;
                }
            }  
        }
        //Change the types of these two rooms
        StartRoom.RoomType = RoomTypes.StartRoom;
        BossRoom.RoomType = RoomTypes.BossRoom;
        //Create the Start and Boss rooms indicators
        GameObject StartRoomIndicator = (GameObject)Instantiate(DungeonTeleporterPrefab, StartRoom.DungeonFloor.transform.position, Quaternion.identity);
        GameObject BossRoomIndicator = (GameObject)Instantiate(DungeonBossPrefab, BossRoom.DungeonFloor.transform.position, Quaternion.identity);
        //Set the room indicators as child objects to the room they are in
        StartRoomIndicator.transform.parent = StartRoom.DungeonFloor.transform;
        BossRoomIndicator.transform.parent = BossRoom.DungeonFloor.transform;
        //Store the room numbers of the Start and Boss rooms
        StartRoomNumber = StartRoom.RoomNumber;
        BossRoomNumber = BossRoom.RoomNumber;
        //Also store the two rooms for later access
        DungeonStartRoom = StartRoom;
        DungeonBossRoom = BossRoom;
    }

    private void PlaceDetourRooms()
    {
        for(int i = 0; i < DetourRoomCount; i++)
        {
            //Select a random room number
            int RandomRoomNumber = 0;
            //Make sure this is not the start or end room
            //or another one of the already placed detour rooms
            bool RoomSelected = false;
            while(!RoomSelected)
            {
                //Generate a random room number
                RandomRoomNumber = RNG.Next(0, RoomCount);
                
                //Make sure this isnt the start room
                if(RandomRoomNumber != StartRoomNumber)
                {
                    //Make sure this isnt the boss room
                    if (RandomRoomNumber != BossRoomNumber)
                    {
                        //Make sure not to place detour rooms on top of each other
                        for (int j = 0; j < DetourRooms.Count; j++)
                            if (RandomRoomNumber == DetourRooms[j].RoomNumber)
                                continue;
                        //If all tests pass, this room can be used as a detour room
                        RoomSelected = true;
                    }
                }
            }
            //We found a location where we can place a detour room
            DungeonRoom NewDetourRoom = GetRoom(RandomRoomNumber);
            //Add it to our list of already selected detour rooms
            DetourRooms.Add(NewDetourRoom);
            //Place an indicator in this room to show that its a detour room
            Instantiate(DungeonDetourRoomPrefab, NewDetourRoom.DungeonFloor.transform.position, Quaternion.identity);
            //Set the detour indicator as a child object of the room that it is in
            //DetourIndicator.transform.parent = NewDetourRoom.DungeonFloor.transform;
        }
    }

    private void PathToDetourRooms(bool UseAStar)
    {
        //Loop through each detour room in the dungeon
        for ( int i = 0; i < DetourRoomCount; i++ )
            PathToRoom(DetourRooms[i], UseAStar);
    }

    //Returns the closest non void room to the room that is passed in
    private DungeonRoom GetClosestRoom(DungeonRoom TargetRoom, bool PathFromBoss)
    {
        DungeonRoom ClosestRoom = null;
        float ClosestRoomDistance = 0;

        //Loop through every room in the dungeon
        for ( int i = 0; i < RoomCount; i++ )
        {
            //Make sure this is not the room we are checking against
            if(DungeonRooms[i].DungeonFloor.transform.position == TargetRoom.DungeonFloor.transform.position)
                continue;

            //If the room is a void type room, skip to the next room
            if (DungeonRooms[i].RoomType == RoomTypes.VoidRoom)
                continue;

            //If we are ignoring pathing from the boss room, skip to the next room
            if (!PathFromBoss && DungeonRooms[i].RoomType == RoomTypes.BossRoom)
                continue;

            //If the ClosestRoomDistance is 0, we have nothing to compare to
            //so we store this room as the closest until we crosscheck it
            if (ClosestRoomDistance == 0)
            {
                ClosestRoom = DungeonStartRoom;
                ClosestRoomDistance = Vector3.Distance(ClosestRoom.DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position);
                continue;
            }

            //Check if this room is closer than the current closest room
            if ( Vector3.Distance(DungeonRooms[i].DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position) < ClosestRoomDistance)
            {
                //Store this as the new closest room
                ClosestRoom = DungeonRooms[i];
                ClosestRoomDistance = Vector3.Distance(DungeonRooms[i].DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position);
            }
        }
        if (ClosestRoom == null)
            print("Get Closest Room returned null");
        return ClosestRoom;
    }

    private DungeonRoom GetNorthRoom(DungeonRoom StartRoom)
    {
        //Get the position of the north room
        Vector2 NorthPosition = new Vector2(StartRoom.RoomPosition.x, StartRoom.RoomPosition.y + 1);
        //Return the room in this location
        return GetRoom(NorthPosition);
    }

    private DungeonRoom GetEastRoom(DungeonRoom StartRoom)
    {
        //Get the position of the north room
        Vector2 EastPosition = new Vector2(StartRoom.RoomPosition.x + 1, StartRoom.RoomPosition.y);
        //Return the room in this location
        return GetRoom(EastPosition);
    }

    private DungeonRoom GetSouthRoom(DungeonRoom StartRoom)
    {
        //Get the position of the north room
        Vector2 SouthPosition = new Vector2(StartRoom.RoomPosition.x, StartRoom.RoomPosition.y - 1);
        //Return the room in this location
        return GetRoom(SouthPosition);
    }

    private DungeonRoom GetWestRoom(DungeonRoom StartRoom)
    {
        //Get the position of the north room
        Vector2 WestPosition = new Vector2(StartRoom.RoomPosition.x - 1, StartRoom.RoomPosition.y);
        //Return the room in this location
        return GetRoom(WestPosition);
    }

    private bool IsRoomAdjacent(DungeonRoom StartRoom, DungeonRoom TargetRoom)
    {
        //Find the 4 positions the 4 adjacent rooms would be in
        Vector2 NorthPos = new Vector2(StartRoom.RoomPosition.x, StartRoom.RoomPosition.y + 1);
        Vector2 EastPos = new Vector2(StartRoom.RoomPosition.x + 1, StartRoom.RoomPosition.y);
        Vector2 SouthPos = new Vector2(StartRoom.RoomPosition.x, StartRoom.RoomPosition.y - 1);
        Vector2 WestPos = new Vector2(StartRoom.RoomPosition.x - 1, StartRoom.RoomPosition.y);
        //Check if these room positions are within the bounds of the dungeon size
        bool NorthPosValid =    NorthPos.x >= DungeonSize.x || 
                                NorthPos.y >= DungeonSize.y ||
                                NorthPos.x < 0 ||
                                NorthPos.y < 0 
                                ? false : true;
        bool EastPosValid =     EastPos.x >= DungeonSize.x || 
                                EastPos.y >= DungeonSize.y ||
                                EastPos.x < 0 ||
                                EastPos.y < 0
                                ? false : true;
        bool SouthPosValid =    SouthPos.x >= DungeonSize.x || 
                                SouthPos.y >= DungeonSize.y  ||
                                SouthPos.x < 0 ||
                                SouthPos.y < 0
                                ? false : true;
        bool WestPosValid =     WestPos.x >= DungeonSize.x || 
                                WestPos.y >= DungeonSize.y ||
                                WestPos.x < 0 ||
                                WestPos.y < 0
                                ? false : true;
        /*
        //Print if these room positions are valid
        print("North Valid: " + NorthPosValid);
        print("East Valid: " + EastPosValid);
        print("South Valid: " + SouthPosValid);
        print("West Valid: " + WestPosValid);
        */
        //Get the 4 adjacent dungeon rooms if they are valid
        DungeonRoom NorthRoom = NorthPosValid ? GetNorthRoom(StartRoom) : null;
        DungeonRoom EastRoom = EastPosValid ? GetEastRoom(StartRoom) : null;
        DungeonRoom SouthRoom = SouthPosValid ? GetSouthRoom(StartRoom) : null;
        DungeonRoom WestRoom = WestPosValid ? GetWestRoom(StartRoom) : null;
        //Check each room to see if any are the target room
        if (NorthPosValid && NorthRoom.DungeonFloor.transform.position == TargetRoom.DungeonFloor.transform.position)
            return true;
        if (EastPosValid && EastRoom.DungeonFloor.transform.position == TargetRoom.DungeonFloor.transform.position)
            return true;
        if (SouthPosValid && SouthRoom.DungeonFloor.transform.position == TargetRoom.DungeonFloor.transform.position)
            return true;
        if (WestPosValid && WestRoom.DungeonFloor.transform.position == TargetRoom.DungeonFloor.transform.position)
            return true;
        //We have check each adjacent room, and found that none of them are the
        //room that we are checking for
        return false;
    }

    //Takes in what is presumed to be a void type room and turns it into a path type
    //room, spawns a box to indicate this
    private void SetToPathRoom(DungeonRoom TargetRoom)
    {
        //If this is not a void room, dont do anything
        if (TargetRoom.RoomType != RoomTypes.VoidRoom)
            return;

        //Change the rooms type
        TargetRoom.RoomType = RoomTypes.PathRoom;
        //Add an indicator to show what type this room is
        GameObject DungeonPath = (GameObject)Instantiate(DungeonPathPrefab, TargetRoom.DungeonFloor.transform.position, Quaternion.identity);
        //Set the indicator as a child object to the room its indicating
        DungeonPath.transform.parent = TargetRoom.DungeonFloor.transform;
    }

    //Creates a path to the target room so it can be reached
    //by the player
    private void PathToRoom(DungeonRoom TargetRoom, bool UseAStar)
    {
        //If we are going to use A*, pass the job onto another function
        if (UseAStar)
        {
            PathToRoomAStar(TargetRoom);
            return;
        }

        //Find the room we will start the path from, this will be the
        //closest non boss, non void type room to the target room
        DungeonRoom CurrentRoom = GetClosestRoom(TargetRoom, false);

        bool PathComplete = false;

        while (!PathComplete)
        {
            //If the current room is adjacent to the target room then the path has been completed
            if (IsRoomAdjacent(CurrentRoom, TargetRoom))
            {
                PathComplete = true;
                return;
            }

            //Get the positions the 4 adjacent rooms would be in
            Vector2 NorthPos = new Vector2(CurrentRoom.RoomPosition.x, CurrentRoom.RoomPosition.y + 1);
            Vector2 EastPos = new Vector2(CurrentRoom.RoomPosition.x + 1, CurrentRoom.RoomPosition.y);
            Vector2 SouthPos = new Vector2(CurrentRoom.RoomPosition.x, CurrentRoom.RoomPosition.y - 1);
            Vector2 WestPos = new Vector2(CurrentRoom.RoomPosition.x - 1, CurrentRoom.RoomPosition.y);

            //Check if these room positions are within the bounds of the dungeon size
            bool NorthPosValid = NorthPos.x >= DungeonSize.x ||
                                 NorthPos.y >= DungeonSize.y ||
                                 NorthPos.x < 0 ||
                                 NorthPos.y < 0
                                 ? false : true;
            bool EastPosValid = EastPos.x >= DungeonSize.x ||
                                EastPos.y >= DungeonSize.y ||
                                EastPos.x < 0 ||
                                EastPos.y < 0
                                ? false : true;
            bool SouthPosValid = SouthPos.x >= DungeonSize.x ||
                                 SouthPos.y >= DungeonSize.y ||
                                 SouthPos.x < 0 ||
                                 SouthPos.y < 0
                                 ? false : true;
            bool WestPosValid = WestPos.x >= DungeonSize.x ||
                                WestPos.y >= DungeonSize.y ||
                                WestPos.x < 0 ||
                                WestPos.y < 0
                                ? false : true;

            //Get the four adjacent rooms if they are valid
            DungeonRoom NorthRoom = NorthPosValid ? GetNorthRoom(CurrentRoom) : null;
            DungeonRoom EastRoom = EastPosValid ? GetEastRoom(CurrentRoom) : null;
            DungeonRoom SouthRoom = SouthPosValid ? GetSouthRoom(CurrentRoom) : null;
            DungeonRoom WestRoom = WestPosValid ? GetWestRoom(CurrentRoom) : null;

            //Calculate the distance from each of these adjacent rooms to the target room, if they are valid
            float NorthDistance = NorthPosValid ? Vector3.Distance(NorthRoom.DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position) : -1;
            float EastDistance = EastPosValid ? Vector3.Distance(EastRoom.DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position) : -1;
            float SouthDistance = SouthPosValid ? Vector3.Distance(SouthRoom.DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position) : -1;
            float WestDistance = WestPosValid ? Vector3.Distance(WestRoom.DungeonFloor.transform.position, TargetRoom.DungeonFloor.transform.position) : -1;

            //Find which of these, which isnt a boss room, is a valid room and is closest to the CurrentRoom
            DungeonRoom ClosestRoom = null;
            float ClosestDistance = 0.0f;
            if (NorthPosValid && NorthRoom.RoomType != RoomTypes.BossRoom)
            {
                ClosestRoom = NorthRoom;
                ClosestDistance = NorthDistance;
            }
            if (EastPosValid && EastRoom.RoomType != RoomTypes.BossRoom)
            {
                //If closest distance still equals 0, then north room was the boss room
                //Otherwise, check if this room is closer
                if (ClosestDistance == 0.0f || EastDistance < ClosestDistance)
                {
                    ClosestRoom = EastRoom;
                    ClosestDistance = EastDistance;
                }
            }
            if (SouthPosValid && SouthRoom.RoomType != RoomTypes.BossRoom)
            {
                if (SouthDistance < ClosestDistance)
                {
                    ClosestRoom = SouthRoom;
                    ClosestDistance = SouthDistance;
                }
            }
            if (WestPosValid && WestRoom.RoomType != RoomTypes.BossRoom)
            {
                if (WestDistance < ClosestDistance)
                {
                    ClosestRoom = WestRoom;
                    ClosestDistance = WestDistance;
                }
            }

            //Now we know the first step on our path toward the TargetRoom
            CurrentRoom = ClosestRoom;
            //If this room is a void room, change it to a path room
            if (CurrentRoom.RoomType == RoomTypes.VoidRoom)
                SetToPathRoom(ClosestRoom);
        }
    }

    //Uses A* pathfinding to create a path to the target room
    //so it can be reached by the player
    private void PathToRoomAStar(DungeonRoom TargetRoom)
    {
        //Layout of this A* pathfinder inspired by the tutorial at
        //http://www.raywenderlich.com/4946/introduction-to-a-pathfinding

        //The start and end nodes of the path
        DungeonRoom StartRoom = GetClosestRoom(TargetRoom, false);
        DungeonRoom EndRoom = TargetRoom;

        //Dungeon Lists
        List<DungeonRoom> OpenList = new List<DungeonRoom>();   //Rooms being considered to find the shortest path
        List<DungeonRoom> ClosedList = new List<DungeonRoom>();    //Rooms that will no longer be considered
        List<DungeonRoom> CameFrom = new List<DungeonRoom>();   //The path to follow

        //We begin by adding the start room to the open list
        OpenList.Add(StartRoom);
        //Set the start nodes G Score to 0
        StartRoom.GScore = 0;
        //Set the start nodes F Score
        StartRoom.FScore = StartRoom.GScore + CalculateHScore(StartRoom, EndRoom);

        //Loop through the open set until its empty
        while (OpenList.Count > 0)
        {
            //Find the node in OpenSet which has the lowest
            //F Score value
            DungeonRoom Current = GetLowestFScore(OpenList);

            //If the Current room is the Target room, then the pathway
            //has been completed
            if (Current == EndRoom)
            {
                ReconstructPath(StartRoom, EndRoom);
                return;
            }

            //Move the Current room from the open set to the closed set
            OpenList.Remove(Current);
            ClosedList.Add(Current);

            //Find the neighbouring rooms of the Current room
            List<DungeonRoom> CurrentNeighbours = new List<DungeonRoom>();
            AddAdjacentRooms(Current, CurrentNeighbours);

            //Loop through the Current neighbouring rooms
            for ( int i = 0; i < CurrentNeighbours.Count; i++ )
            {
                //Get the neighbour room we are checking
                DungeonRoom Neighbour = CurrentNeighbours[i];

                //If the neighbour room is in the closed list, ignore it
                if (ClosedList.Contains(Neighbour))
                    continue;
                //Length of this path
                int TentativeGScore = Current.GScore + (int)(Vector3.Distance(Current.DungeonFloor.transform.position, Neighbour.DungeonFloor.transform.position));
                //Discover a new node
                if (!OpenList.Contains(Neighbour))
                    OpenList.Add(Neighbour);
                else if (TentativeGScore >= Current.GScore)
                    continue; //This is not a better path

                //This path is the best until now, record it!
                Neighbour.ParentRoom = Current;
                Neighbour.GScore = TentativeGScore;
                Neighbour.FScore = Neighbour.GScore + CalculateHScore(Neighbour, EndRoom);
            }
        }
    }

    private void ReconstructPath(DungeonRoom StartRoom, DungeonRoom EndRoom)
    {
        //Follow the EndRoom through its parents all the way back to the StartRoom
        //Placing indicators and changing room types as we go along
        print("reconstruction path");
        DungeonRoom CurrentRoom = EndRoom;
        int i = 0;
        while(true)
        {
            //If the CurrentRooms parent is the StartRoom, we have finished the path
            if (CurrentRoom == StartRoom)
            {
                print("reconstruction complete after " + i + " iterations");
                return;
            }

            //Follow the CurrentRoom to its parent
            CurrentRoom = CurrentRoom.ParentRoom;
            //If the room type is void, change it to path and place an indicator
            if(CurrentRoom.RoomType == RoomTypes.VoidRoom)
            {
                //Change room type
                SetToPathRoom(CurrentRoom);
                //Place indicator
                GameObject PathIndicator = (GameObject)Instantiate(DungeonPathPrefab, CurrentRoom.DungeonFloor.transform.position, Quaternion.identity);
                //Set indicator as child of the room
                PathIndicator.transform.parent = CurrentRoom.DungeonFloor.transform;
            }

            i++;
            //Break out if we get stuck in a loop too many times
            if(i>100)
            {
                print("Reconstruct Path iterations exceeded 100, breaking out to avoid inifinite loop.");
                print("If you are generating extremely large levels, this limit may need to be increased.");
            }
        }
    }

    private int CalculateHScore(DungeonRoom FirstRoom, DungeonRoom SecondRoom)
    {
        //We use the manhattan distance method to find the h score
        int XDistance = (int)(FirstRoom.RoomPosition.x > SecondRoom.RoomPosition.x ? FirstRoom.RoomPosition.x - SecondRoom.RoomPosition.x : SecondRoom.RoomPosition.x - FirstRoom.RoomPosition.x);
        int YDistance = (int)(FirstRoom.RoomPosition.y > SecondRoom.RoomPosition.y ? FirstRoom.RoomPosition.y - SecondRoom.RoomPosition.y : SecondRoom.RoomPosition.y - FirstRoom.RoomPosition.y);
        return XDistance + YDistance;
    }

    //Takes in a list of dungeon rooms and returns the one with the lowest f score
    private DungeonRoom GetLowestFScore(List<DungeonRoom> RoomList)
    {
        DungeonRoom LowestScoreRoom = RoomList[0];

        for (int i = 1; i < RoomList.Count; i++)
            LowestScoreRoom = RoomList[i].FScore < LowestScoreRoom.FScore ? RoomList[i] : LowestScoreRoom;

        return LowestScoreRoom;
    }

    //Takes in a list and a room, adds any rooms adjacent that can be moved in between
    private void AddAdjacentRooms(DungeonRoom TargetRoom, List<DungeonRoom> TargetList)
    {
        //Get the positions the 4 adjacent rooms would be in
        Vector2 NorthPos = new Vector2(TargetRoom.RoomPosition.x, TargetRoom.RoomPosition.y + 1);
        Vector2 EastPos = new Vector2(TargetRoom.RoomPosition.x + 1, TargetRoom.RoomPosition.y);
        Vector2 SouthPos = new Vector2(TargetRoom.RoomPosition.x, TargetRoom.RoomPosition.y - 1);
        Vector2 WestPos = new Vector2(TargetRoom.RoomPosition.x - 1, TargetRoom.RoomPosition.y);
        //Check if these room positions are within the bounds of the dungeon size
        bool NorthPosValid = NorthPos.x >= DungeonSize.x ||
                             NorthPos.y >= DungeonSize.y ||
                             NorthPos.x < 0 ||
                             NorthPos.y < 0
                             ? false : true;
        bool EastPosValid = EastPos.x >= DungeonSize.x ||
                            EastPos.y >= DungeonSize.y ||
                            EastPos.x < 0 ||
                            EastPos.y < 0
                            ? false : true;
        bool SouthPosValid = SouthPos.x >= DungeonSize.x ||
                             SouthPos.y >= DungeonSize.y ||
                             SouthPos.x < 0 ||
                             SouthPos.y < 0
                             ? false : true;
        bool WestPosValid = WestPos.x >= DungeonSize.x ||
                            WestPos.y >= DungeonSize.y ||
                            WestPos.x < 0 ||
                            WestPos.y < 0
                            ? false : true;
        //Get the four adjacent rooms if they are valid
        DungeonRoom NorthRoom = NorthPosValid ? GetNorthRoom(TargetRoom) : null;
        DungeonRoom EastRoom = EastPosValid ? GetEastRoom(TargetRoom) : null;
        DungeonRoom SouthRoom = SouthPosValid ? GetSouthRoom(TargetRoom) : null;
        DungeonRoom WestRoom = WestPosValid ? GetWestRoom(TargetRoom) : null;
        //Add the adjacent valid rooms to the list
        if (NorthPosValid)
            TargetList.Add(NorthRoom);
        if (EastPosValid)
            TargetList.Add(EastRoom);
        if (SouthPosValid)
            TargetList.Add(SouthRoom);
        if (WestPosValid)
            TargetList.Add(WestRoom);
    }
}