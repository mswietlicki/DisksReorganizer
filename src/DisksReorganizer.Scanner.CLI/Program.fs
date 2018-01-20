open System
open System.IO
open System.Security.Cryptography
open LiteDB
open LiteDB.FSharp
open DisksReorganizer.Common

let clearHash (s:string) = s.Replace("-", "").ToLower()
let computeHash (f:FileInfo) = File.ReadAllBytes(f.FullName) |> SHA1.Create().ComputeHash |> BitConverter.ToString |> clearHash
let fileInfoToFile (f:FileInfo, sourceName:string, source:string) : File = {
    Id = Guid.NewGuid();
    Name = f.Name;
    Extension = f.Extension;
    Hash = computeHash f;
    Size = f.Length;
    Source = source;
    SourceName = sourceName;
    Path = f.FullName;
}

[<EntryPoint>]
let main argv =
    if (argv.Length < 2) then
        printfn "Missing arguments! [SourceName] [SourcePath]"
        exit 0
    let sourceName = argv.[0]
    let scanDir = argv.[1]

    let findFiles = DirectoryInfo(scanDir).EnumerateFiles "*"

    let mapper = FSharpBsonMapper()
    use db = new LiteDatabase("..\\..\\DisksReorganizer.db", mapper)
    let filesColl = db.GetCollection<File>()

    findFiles
        |> Seq.map (fun f -> fileInfoToFile (f, sourceName, scanDir))
        |> Seq.map (fun f -> (filesColl.Insert(f),f))
        |> Seq.iter (fun (i,f) -> printfn "File insert: %s %s" f.Hash f.Path)
    0 // return an integer exit code
