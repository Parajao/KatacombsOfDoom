namespace KatacombsOfDoom.Tests

module AcceptanceTests =
    open KatacombsOfDoom
    open KatacombsOfDoom.Domain
    open Swensen.Unquote
    open Xunit
    open FsUnit.Xunit    

    //let testPrintf f arg =
    //    let oldOut = System.Console.Out
    //    use out = new System.IO.StringWriter()
    //    System.Console.SetOut(out)
    //    let res = f arg
    //    System.Console.SetOut(oldOut)
    //    (res, out.GetStringBuilder().ToString())
    
    //[<Fact>]
    //let ``demo Unquote xUnit support 2`` () =
    //    test<@ ([3; 2; 1; 0] |> List.map ((+) 1)) = [1 + 3..1 + 0] @>

    module ImpureAcceptance =
        open IMain

        [<Fact>]
        let ``quits then game when suiciding`` () =
            let reader : IInputReader =
                fun () -> "suicide"
            let mutable output = []
            let writer : IOutputWriter = 
                fun message -> 
                    output <- message::output

            let result = iGameEngine game reader writer

            result |> should equal 0
            output |> should equal ["see you in hell"]
    
    module MonadicIOAcceptance =
        open MIO
        open MMain

        [<Fact>]
        let ``monadically quits then game when suiciding`` () =
            let reader : MInputReader =
                MIO(fun () -> "suicide")

            let mutable output = []
            let writer : MOutputWriter = 
                fun message -> MIO(fun () -> output <- message::output)

            let result = mGameEngine game reader writer

            result |> should equal 0
            output |> should equal ["see you in hell"]

    module HaskellIOAcceptance =
        open HIO
        open HMain
        open System.Collections.Generic

        [<Fact>]
        let ``monadically quits then game when suiciding`` () =
            let inputs = new Queue<string>(["suicide"])
            let reader : HInputReader =
                Action(fun () -> inputs.Dequeue())

            let mutable output = []
            let writer : HOutputWriter = 
                fun message -> Action(fun () -> output <- message::output)

            let result = hGameEngine game reader writer

            result |> should equal 0
            output |> should equal ["see you in hell"]
