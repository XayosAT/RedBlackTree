module ConsoleApp1

open System
open System.IO
open System.Text.RegularExpressions


type Color =
    | Red
    | Black

type RBTree<'T when 'T : comparison> =
    | Empty
    | Node of Color * RBTree<'T> * 'T * RBTree<'T>

let emptyTree = Empty

let makeNode color left value right = Node(color, left, value, right)

let rotateLeft (tree: RBTree<'T>) =
    match tree with
    | Node(color, left, value, Node(Red, rightLeft, rightValue, rightRight)) ->
        Node(color, Node(Red, left, value, rightLeft), rightValue, rightRight)
    | _ -> tree

let rotateRight (tree: RBTree<'T>) =
    match tree with
    | Node(color, Node(Red, leftLeft, leftValue, leftRight), value, right) ->
        Node(color, leftLeft, leftValue, Node(Red, leftRight, value, right))
    | _ -> tree

let colorFlip (tree: RBTree<'T>) =
    match tree with
    | Node(Black, Node(Red, leftLeft, leftValue, leftRight), value, Node(Red, rightLeft, rightValue, rightRight)) ->
        Node(Red, Node(Black, leftLeft, leftValue, leftRight), value, Node(Black, rightLeft, rightValue, rightRight))
    | _ -> tree

let balance (tree: RBTree<'T>) =
    tree
    |> rotateLeft
    |> rotateRight
    |> colorFlip

let rec insert value tree =
    match tree with
    | Empty -> Node(Red, Empty, value, Empty)
    | Node(color, left, nodeValue, right) ->
        if value < nodeValue then
            balance (makeNode color (insert value left) nodeValue right)
        elif value > nodeValue then
            balance (makeNode color left nodeValue (insert value right))
        else
            tree

let insertValue value tree =
    match insert value tree with
    | Node(_, left, nodeValue, right) -> Node(Black, left, nodeValue, right)
    | Empty -> Empty

let rec traverseTree tree =
    match tree with
    | Empty -> []
    | Node(_, left, value, right) -> traverseTree left @ [value] @ traverseTree right

let extractWords (line: string) =
    let pattern = "[a-zA-Z'\\-\\.]+"
    Regex.Matches(line, pattern)
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value.ToLower())
    |> Seq.toList

let buildRedBlackTree (filePath: string) : RBTree<string> =
    let lines = File.ReadLines(filePath)
    lines
    |> Seq.collect extractWords
    |> Seq.fold (fun tree word -> insertValue word tree) emptyTree

let writeWordsToFile (words: List<string>) (outputPath: string) =
    use writer = new StreamWriter(outputPath)
    words |> List.iter (fun word -> writer.WriteLine(word))

[<EntryPoint>]
let main argv =
    try

        if argv.Length < 2 then
            printfn "Error: Please provide input and output file paths."
            printfn "Usage: <program> <inputFilePath> <outputFilePath>"
            1  
        else
            
            let inputFilePath = argv.[0]
            let outputFilePath = argv.[1]

            let redBlackTree = buildRedBlackTree inputFilePath
            let sortedWords = traverseTree redBlackTree
            writeWordsToFile sortedWords outputFilePath

            
            printfn "Successfully processed the file and wrote sorted unique words to %s" outputFilePath
            0  
    with
        | :? FileNotFoundException ->
            printfn "Error: Input file not found. Please ensure the file exists."
            1  
        | ex ->
            printfn "An error occurred: %s" (ex.Message)
            1  


