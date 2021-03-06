﻿open System; open System.IO
open System.Security.Cryptography
open LiteDB; open LiteDB.FSharp
open DisksReorganizer.Common

let db = new LiteDatabase("DisksReorganizer.db", FSharpBsonMapper())
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
let fileToHash (f:File) : Hash = {
    Id = Guid.NewGuid();
    Hash = f.Hash
}
let checkFile (f:FileInfo):bool = try (f.Open FileMode.Open).Close(); true with _ ->  false
let filterFile (f:FileInfo):bool =
    match f.FullName with
        | name when name.Contains(".git") -> false
        | name when name.Contains(".vs") -> false
        | _ -> true
let saveFile (f:File) = (db.GetCollection<File>().Insert(f), db.GetCollection<Hash>().Upsert(fileToHash f), f)

let scanSource (sourceName, scanDir) =
    DirectoryInfo(scanDir).EnumerateFiles("*", SearchOption.AllDirectories)
        |> Seq.filter checkFile
        |> Seq.filter filterFile
        |> Seq.map (fun f -> fileInfoToFile(f, sourceName, scanDir))
        |> Seq.map saveFile
        |> Seq.iter (fun (_,_,_) -> printf ".") //printfn "File insert: %s %s" f.Hash f.Path)

[<EntryPoint>]
let main argv =
    match argv with
        | [| sourceName; scanDir |] -> scanSource(sourceName, scanDir)
        | _ -> printfn "Missing arguments! [SourceName] [SourcePath]"
    0