namespace KatacombsOfDoom

module Domain =
    //**************
    // MODELS
    //**************
    type Game = string -> string
    let game : Game =
        fun command -> "see you in hell"
    
    type Item = {
            Name : string;
            Description: string
        }
    type Items = Item List
    type Location = Location of int * int
    type Room = {
            Location : Location;
            Content : string;
            Items : Items
        }
    type RoomDescription = RoomDescription of string
    type Maze = Room List
    type Player = {
            CurrentRoom : Room;
            Items : Items
        }
    type Direction = NORTH
                   | EAST
                   | SOUTH
                   | WEST

    //**************
    // COMMANDS
    //**************

    type Look = Player -> RoomDescription
    type Move = Player -> Direction -> Maze  -> Player
    type PickUp = Player -> Item -> Maze ->  Player * Maze
    type Suicide = Unit -> Unit