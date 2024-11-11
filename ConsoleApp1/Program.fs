open System
open System.IO
open System.Text.RegularExpressions

// Define the Red-Black Tree data structure
// Enum for color of nodes
type Color =
    | Red
    | Black

// Type to represent the Red-Black Tree node
type RBTree<'T when 'T : comparison> =
    | Empty
    | Node of Color * RBTree<'T> * 'T * RBTree<'T>

// Function to create a new empty Red-Black Tree
let emptyTree = Empty

// Utility function to create a new node
let makeNode color left value right = Node(color, left, value, right)

// Function to rotate left
let rotateLeft (tree: RBTree<'T>) =
    match tree with
    | Node(color, left, value, Node(Red, rightLeft, rightValue, rightRight)) ->
        Node(color, Node(Red, left, value, rightLeft), rightValue, rightRight)
    | _ -> tree

// Function to rotate right
let rotateRight (tree: RBTree<'T>) =
    match tree with
    | Node(color, Node(Red, leftLeft, leftValue, leftRight), value, right) ->
        Node(color, leftLeft, leftValue, Node(Red, leftRight, value, right))
    | _ -> tree

// Function to color flip
let colorFlip (tree: RBTree<'T>) =
    match tree with
    | Node(Black, Node(Red, leftLeft, leftValue, leftRight), value, Node(Red, rightLeft, rightValue, rightRight)) ->
        Node(Red, Node(Black, leftLeft, leftValue, leftRight), value, Node(Black, rightLeft, rightValue, rightRight))
    | _ -> tree

// Balance function to ensure tree remains balanced after insertion
let balance (tree: RBTree<'T>) =
    tree
    |> rotateLeft
    |> rotateRight
    |> colorFlip

// Function to insert a value into the Red-Black Tree
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

// Ensure the root is always black after insertion
let insertValue value tree =
    match insert value tree with
    | Node(_, left, nodeValue, right) -> Node(Black, left, nodeValue, right)
    | Empty -> Empty

// Traverse the Red-Black Tree in order and return the sorted list of words
let rec traverseTree tree =
    match tree with
    | Empty -> []
    | Node(_, left, value, right) -> traverseTree left @ [value] @ traverseTree right

// Define a function to parse and extract words from a line.
let extractWords (line: string) =
    let pattern = "[a-zA-Z]+"
    Regex.Matches(line, pattern)
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value.ToLower())
    |> Seq.toList

// Read the file and parse words, inserting them into the Red-Black Tree
let buildRedBlackTree (filePath: string) : RBTree<string> =
    let lines = File.ReadLines(filePath)
    lines
    |> Seq.collect extractWords
    |> Seq.fold (fun tree word -> insertValue word tree) emptyTree

// Write the sorted words to an output file
let writeWordsToFile (words: List<string>) (outputPath: string) =
    use writer = new StreamWriter(outputPath)
    words |> List.iter (fun word -> writer.WriteLine(word))

// Main function
[<EntryPoint>]
let main argv =
    try
        // File paths
        let inputFilePath = "C:\Users\Christoph\RiderProjects\ConsoleApp1\ConsoleApp1\war_and_peace.txt"
        let outputFilePath = "C:\Users\Christoph\RiderProjects\ConsoleApp1\ConsoleApp1\output.txt"

        // Build the Red-Black Tree and write to file
        let redBlackTree = buildRedBlackTree inputFilePath
        let sortedWords = traverseTree redBlackTree
        writeWordsToFile sortedWords outputFilePath

        printfn "Successfully processed the file and wrote sorted unique words to %s" outputFilePath
    with
        | :? FileNotFoundException ->
            printfn "Error: Input file not found. Please ensure 'war_and_peace.txt' is in the correct location."
        | ex ->
            printfn "An error occurred: %s" (ex.Message)

    0 // Return code

// Note: Make sure the "war_and_peace.txt" file is available in the correct directory.
// You can adjust the file paths in the main function accordingly.
