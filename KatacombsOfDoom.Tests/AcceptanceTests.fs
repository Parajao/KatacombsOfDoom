namespace KatacombsOfDoom.Tests

module AcceptanceTests =
    open KatacombsOfDoom
    open Swensen.Unquote
    open Xunit

    [<Fact>]
    let ``demo Unquote xUnit support`` () =
        test <@ ([3; 2; 1; 0] |> List.map ((+) 1)) = [1 + 3..1 + 0] @>

    [<Fact>]
    let ``demo Unquote xUnit support 2`` () =
        ([3; 2; 1; 0] |> List.map ((+) 1)) =! [1 + 3..1 + 0]
