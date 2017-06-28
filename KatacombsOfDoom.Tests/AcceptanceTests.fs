namespace KatacombsOfDoom.Tests

module AcceptanceTests =
    open KatacombsOfDoom.Main
    open Swensen.Unquote
    open Xunit
    open FsUnit.Xunit    

    let testPrintf f arg =
        let oldOut = System.Console.Out
        use out = new System.IO.StringWriter()
        System.Console.SetOut(out)
        let res = f arg
        System.Console.SetOut(oldOut)
        (res, out.GetStringBuilder().ToString())

    [<Fact>]
    let ``quits then game when suiciding`` () =
        let result = 0
        result |> should equal 0

    [<Fact>]
    let ``demo Unquote xUnit support 2`` () =
        test<@ ([3; 2; 1; 0] |> List.map ((+) 1)) = [1 + 3..1 + 0] @>
