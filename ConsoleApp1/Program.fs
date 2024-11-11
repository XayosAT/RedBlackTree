open System
open System.IO
open System.Text.RegularExpressions

// Define Red-Black Tree data structure
// In this implementation, we approximate it using a Set, which internally acts as a balanced tree.
type RedBlackTree<'T when 'T: comparison> = Set<'T>

// Function to insert a word into the Red-Black Tree
let insertWord (tree: RedBlackTree<string>) (word: string) : RedBlackTree<string> =
    tree.Add(word)

// Function to traverse the Red-Black Tree in order and return the sorted list of words
let traverseTree (tree: RedBlackTree<string>) : List<string> =
    tree |> Set.toList

// Define a function to parse and extract words from a line.
let extractWords (line: string) =
    let pattern = "[a-zA-Z]+"
    Regex.Matches(line, pattern)
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value.ToLower())
    |> Seq.toList

// Read the file and parse words, inserting them into the Red-Black Tree
let buildRedBlackTree (filePath: string) : RedBlackTree<string> =
    let lines = File.ReadLines(filePath)
    let emptyTree = Set.empty
    lines
    |> Seq.collect extractWords
    |> Seq.fold insertWord emptyTree

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
