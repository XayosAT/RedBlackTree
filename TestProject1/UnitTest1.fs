module TestProject1

open System.IO
open NUnit.Framework
open FsUnit
open ConsoleApp1

[<Test>]
let ``Insert should add elements to the tree`` () =
    let tree = emptyTree
    let treeWithElements = tree |> insertValue "apple" |> insertValue "banana" |> insertValue "cherry"
    let sortedValues = traverseTree treeWithElements
    sortedValues |> should equal ["apple"; "banana"; "cherry"]

[<Test>]
let ``Insert should not add duplicate values`` () =
    let tree = emptyTree
    let treeWithElements = tree |> insertValue "apple" |> insertValue "banana" |> insertValue "banana"
    let sortedValues = traverseTree treeWithElements
    sortedValues |> should equal ["apple"; "banana"]

[<Test>]
let ``Tree should maintain Red-Black properties after insertion`` () =
    let tree = emptyTree
    let treeWithElements = tree |> insertValue "banana" |> insertValue "apple" |> insertValue "cherry"
    match treeWithElements with
    | Node(Black, _, _, _) -> () 
    | _ -> Assert.Fail("Root node is not black after insertion")


type ExtractWordsTests() =
    [<Test>]
    member this.``Extract words should extract words correctly`` () =
        let line = "Hello, World! 123 F# is amazing."
        let words = extractWords line
        words |> should equal ["hello"; "world"; "f"; "is"; "amazing"]

[<Test>]
let ``BuildRedBlackTree should create a tree with unique words from a file`` () =
    
    let tempFilePath = Path.GetTempFileName()
    File.WriteAllText(tempFilePath, "banana apple banana cherry")
    
    let tree = buildRedBlackTree tempFilePath
    let sortedWords = traverseTree tree
    sortedWords |> should equal ["apple"; "banana"; "cherry"]
    
    File.Delete(tempFilePath)

[<Test>]
let ``WriteWordsToFile should correctly write words to output file`` () =
    let words = ["apple"; "banana"; "cherry"]
    let tempFilePath = Path.GetTempFileName()
    
    ConsoleApp1.writeWordsToFile words tempFilePath

    let writtenContent = File.ReadAllLines(tempFilePath) |> List.ofArray
    writtenContent |> should equal words
    
    File.Delete(tempFilePath)
