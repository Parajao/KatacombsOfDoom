module Domain

// the player can move in different rooms of a maze
// each room in the maze is connected to the adjacent rooms
// there are only 4 main directions NORTH, EAST, SOUTH and WEST
// when the player moves into a room it tell the location of the room
// looking in a room shows the room description the possible exits and the items
// to terminate the game the player has to commit suicide
// the room can contain items to collect

//--------------------------
// EXAMPLE OF
// HIDING THE CONSTRUCTOR
//--------------------------

// module OrderDomain =
//     type UnitQuantity = private UnitQuantity of int // ^ private constructor
//     /// "Smart constructor" for UnitQuantity
//     let createUnitQuantity qty =
//         // int -> Result<UnitQuantity,string>
//         if qty < 0 then
//         // failure
//             Error "UnitQuantity can not be negative"
//         else if qty > 1000 then
//         // failure
//             Error "UnitQuantity can not be more than 1000"
//         else
//               // success -- construct the return value
//               Ok (UnitQuantity qty)

//--------------------------
// EXAMPLE OF
// ACTIVE PATTERN
// TO PATTERN MATCH
//  THE CONSTRUCTOR
//--------------------------

// module OrderDomain =
// type UnitQuantity = private UnitQuantity of int
//   // Active pattern to replace constructor pattern matching
// let (|UnitQuantity|) unitQuantity =
//     // extract the quantity
//     let (UnitQuantity qty) = unitQuantity // and return it
//     qty


//**************
// MODELS
//**************

type Item = { Name : string; Description: string }
type Items = Item List
type Location = Location of int * int
type Room = { Location : Location; Description : string; Items : Items }
type Maze = Room List
type Player = { CurrentRoom : Room; Items : Items }
type Direction = NORTH
               | EAST
               | SOUTH
               | WEST

//**************
// ACTIONS
//**************
type Look = Player -> string
let look : Look =
    fun player -> player.CurrentRoom.Description

type Move = Direction -> Room -> Room
type Pick = Player -> Item -> Player
type RetrieveExits = Maze -> Room -> Direction List
