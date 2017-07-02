namespace KatacombsOfDoom

open System

module Main =       
    type Game = string -> string
    let game : Game =
        fun command -> "see you in hell"

    //**************************
    // MONADIC IO
    //**************************
    module MonadicIO =

        type IO<'a> = | IO of (unit -> 'a)
        let run : IO<'a> -> 'a = fun (IO thunk) -> thunk()
    
        // return aka unit
        let unit a = IO(fun () -> a)
        // bind
        let (>>=) (action:IO<'a>)  (fnction: 'a -> IO<'b>): IO<'b> = 
            IO(fun () -> run <| fnction (run action ))

        let getLine : IO<string> = IO (fun () -> Console.ReadLine())   
        let putStrLn : string -> IO<unit> =
            fun s -> IO(fun () -> printfn "%s" s)
    
    module MonadicMain =
        open MonadicIO

        type MInputReader = IO<string>
        type MOutputWriter = string -> IO<unit>
        type MGameEngine = Game -> MInputReader -> MOutputWriter -> int
        let rec mGameEngine : MGameEngine =
            fun game reader writer ->
                reader >>= 
                    (fun input -> 
                        writer <| game input >>=
                            match input with
                            | "suicide" ->     
                                (fun _ -> 
                                    unit 0)
                            | _ ->  
                                (fun _ -> 
                                    unit (mGameEngine game reader writer)))
                |> run
                
    
    //**************************
    // HASKELL LIKE MONADIC IO
    //**************************
    module HMonadIO =
    
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
    
        let io = new IOBuilder()
        let (|Action|) io = run io
    
    module HPreludeIO =
        open HMonadIO

        let putChar  (c:char)   = Action (fun () -> stdout.Write(c))
        let putStr   (s:string) = Action (fun () -> stdout.Write(s))
        let putStrLn (s:string) = Action (fun () -> stdout.WriteLine(s))
        let print x             = Action (fun () -> printfn "%A" x)
        let getChar     = Action (fun () -> stdin.Read() |> char |> string)
        let getLine     = Action (fun () -> stdin.ReadLine())
        let getContents = Action (fun () -> stdin.ReadToEnd())

    module HMain =
        open HMonadIO
        open HPreludeIO

        type HInputReader = HIO<string>
        type HOutputWriter = string -> HIO<unit>
        type HGameEngine = Game -> HInputReader -> HOutputWriter -> int
        let rec hGameEngine : HGameEngine =
            fun game reader writer ->
                let action = io{
                    let! input = reader
                    return! writer <| game input 
                    match input with
                    | "suicide" -> 
                        return 0
                    |_ -> 
                        return hGameEngine game reader writer
                }
                run action
                    
                    

    //**************************
    // IMPURE IO
    //**************************
    type InputReader = Unit -> string
    let readInput : InputReader =
        fun () -> Console.ReadLine()

    type OutputWriter = string -> Unit
    let writeOutput : OutputWriter = Console.WriteLine

    type GameEngine = Game -> InputReader -> OutputWriter -> int
    let rec gameEngine : GameEngine =
        fun game reader writer ->
            let input = reader ()
            writer <| game input
            match input with
            | "suicide" ->
                0
            | _ ->
                gameEngine game reader writer
            

    [<EntryPoint>]
    let main argv = 
        let result = gameEngine game readInput writeOutput
        readInput () |> ignore
        result 