open System
open LiteDB
open LiteDB.FSharp
open DisksReorganizer.Common
open System.Linq

let strContainsOnlyNumber (s:string) = System.Int32.TryParse s |> fst
let printDuplicatedFiles (g:string, fs:File[]) = 
    printfn "Duplicate %s:" g
    fs |> Seq.indexed |> Seq.iter (fun (i,f) -> printfn "    %d. File [%s] %s" (i+1) f.SourceName f.Path)

let giveOptionToDeleteFile (fs:File[], filesColl:LiteCollection<File>) =
    printfn "Which file you wish to DELETE:"
    let answer = Console.ReadLine()
    match System.Int32.TryParse answer with
        | true, index -> 
            let f = fs.[index-1]
            printfn "Delete [%s] %s" f.SourceName f.Path
            filesColl.Delete(new BsonValue(f.Id)) |> ignore
            System.IO.File.Delete(f.Path)
        | false, _ -> ()

[<EntryPoint>]
let main argv =
    let mapper = FSharpBsonMapper()
    use db = new LiteDatabase("DisksReorganizer.db", mapper)
    let filesColl = db.GetCollection<File>()

    filesColl.Find(Query.All("Size", Query.Descending), 0, 100).OrderByDescending(fun f -> f.Size)
        |> Seq.indexed
        |> Seq.iter (fun (i, f) -> printfn "%d. File [%s] %s (%.2f Mb)" (i) f.SourceName f.Path ((float f.Size) / 1024.0 / 1024.0))

    //printfn "Loading files..."
    //filesColl.FindAll()
    //    |> Seq.groupBy (fun f -> f.Hash)
    //    |> Seq.filter (fun (g,fs) -> fs.Count() > 1)
    //    |> Seq.map (fun (g,fs) -> (g,fs.ToArray()))
    //    |> Seq.iter (fun (g,fs) -> 
    //        printDuplicatedFiles(g, fs)
    //        giveOptionToDeleteFile(fs, filesColl))
    0
