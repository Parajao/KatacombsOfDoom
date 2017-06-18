module KatacombsOfDoomTests

open Expecto

[<Tests>]
let tests =
  testList "A test group" [
    test "one test" {
      Expect.equal (2+2) 4 "2+2"
    }

    test "another test that fails" {
      let sum = 3 + 2
      Expect.equal sum  5 "3+2"
    }

    testCase "universe exists (╭ರᴥ•́)" <| fun _ ->
      let subject = true
      Expect.isTrue subject "I compute, therefore I am."
  ]

[<EntryPoint>]
let main argv =
  Tests.runTestsInAssembly defaultConfig argv