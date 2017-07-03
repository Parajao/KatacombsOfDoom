namespace KatacombsOfDoom

open System
open Domain


//**************************
// MONADIC IO
//**************************
module MIO =

    type MIO<'a> = | MIO of (unit -> 'a)
    let run : MIO<'a> -> 'a = fun (MIO thunk) -> thunk()
    
    // return aka unit
    let unit a = MIO(fun () -> a)
    // bind
    let (>>=) (action:MIO<'a>)  (fnction: 'a -> MIO<'b>): MIO<'b> = 
        MIO(fun () -> run <| fnction (run action ))

    
module MMain =
    open MIO    

    type MInputReader = MIO<string>
    let mReadInput : MInputReader = 
        MIO (fun () -> Console.ReadLine())   

    type MOutputWriter = string -> MIO<unit>
    let mWriteOutput : MOutputWriter =
        fun s -> MIO(fun () -> printfn "%s" s)
                    
    type MGameEngine = Game -> MInputReader -> MOutputWriter -> int
    let rec mGameEngine : MGameEngine =
        fun game reader writer ->
            reader >>= 
                (fun input -> 
                    writer <| game input >>=
                        (fun _ -> 
                            let next =
                                match input with
                                | "suicide" -> 0
                                | _ ->  mGameEngine game reader writer
                            unit next
                        ))
            |> run
                
    
//**************************
// HASKELL LIKE MONADIC IO
//**************************
module HIO =
    
    type HIO<'T> = Action of (unit -> 'T)

    let private raw  (Action f) = f
    let run  io         = raw io ()
    let private eff  g   io     = raw io () |> g
    let private bind io  rest   = Action (fun () -> io |> eff rest |> run)
    let private comb io1 io2    = Action (fun () -> run io1; run io2)
    
    type IOBuilder() =
        member b.Return(x)              = Action (fun () -> x)
        member b.ReturnFrom(io) : HIO<_> = io
        member b.Delay(g) : HIO<_>       = g ()
        member b.Bind(io, rest)         = bind io rest
        member b.Combine(io1, io2)      = comb io1 io2
        member b.Zero()                 = Action (fun () -> ())
    
    let io = new IOBuilder()
    let (|Action|) io = run io
    
module HPreludeIO =
    open HIO

    let putChar  (c:char)   = Action (fun () -> stdout.Write(c))
    let putStr   (s:string) = Action (fun () -> stdout.Write(s))
    let putStrLn (s:string) = Action (fun () -> stdout.WriteLine(s))
    let print x             = Action (fun () -> printfn "%A" x)
    let getChar     = Action (fun () -> stdin.Read() |> char |> string)
    let getLine     = Action (fun () -> stdin.ReadLine())
    let getContents = Action (fun () -> stdin.ReadToEnd())

module HMain =
    open HIO
    open HPreludeIO

    type HInputReader = HIO<string>
    let hReadInput : HInputReader = getLine

    type HOutputWriter = string -> HIO<unit>
    let hWriteOutput : HOutputWriter = print

    type HGameEngine = Game -> HInputReader -> HOutputWriter -> int
    let rec hGameEngine : HGameEngine =
        fun game reader writer ->
            let readCommand = io{
                let! input = reader
                return input
            }
            let command = run readCommand
            
            let writeOutput = io{
                return! writer <| game command 
            }
            run writeOutput

            match command with
            | "suicide" -> 0
            | _ -> hGameEngine game reader writer
            
                    
                    

//**************************
// IMPURE IO
//**************************
module IMain =
    type IInputReader = Unit -> string
    let iReadInput : IInputReader = fun () -> Console.ReadLine()

    type IOutputWriter = string -> Unit
    let iWriteOutput : IOutputWriter = Console.WriteLine

    type GameEngine = Game -> IInputReader -> IOutputWriter -> int
    let rec iGameEngine : GameEngine =
        fun game reader writer ->
            let input = reader ()
            writer <| game input
            match input with
            | "suicide" -> 0
            | _ -> iGameEngine game reader writer
            
module App =
    open MMain
    open HMain
    open IMain

    [<EntryPoint>]
    let main argv = 
        let engineType =
            if argv.Length = 1
            then argv.[0]
            else "i"
        let result =
            match engineType with
            | "m" -> mGameEngine game mReadInput mWriteOutput
            | "h" -> hGameEngine game hReadInput hWriteOutput
            | _   -> iGameEngine game iReadInput iWriteOutput
        iReadInput () |> ignore
        result 