namespace KatacombsOfDoom

open System

module Main =       
    type Game = Game

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
        let mGameEngine : MGameEngine =
            fun game reader writer ->
                reader >>= (fun input -> 
                writer "see you in hell")
                |> run
                0
    
    //**************************
    // HASKELL LIKE MONADIC IO
    //**************************
    module HMonadIO =
    
        type HIO<'T> = Action of (unit -> 'T)

        let private raw  (Action f) = f
        let private run  io         = raw io ()
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
        let hGameEngine : HGameEngine =
            fun game reader writer ->
                let (Action ()) = io{
                    let! input = reader
                    return! writer "see you in hell"
                }
                0

    //**************************
    // IMPURE IO
    //**************************
    type InputReader = Unit -> string
    let readInput : InputReader =
        fun () -> Console.ReadLine()

    type OutputWriter = string -> Unit
    let writeOutput : OutputWriter =
        fun message -> Console.WriteLine message

    type GameEngine = Game -> InputReader -> OutputWriter -> int
    let gameEngine : GameEngine =
        fun game reader writer ->
            writer "see you in hell"
            0

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv
        0 // restituisce un intero come codice di uscita
