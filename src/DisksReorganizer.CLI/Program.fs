// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Security.Cryptography
open LiteDB
open LiteDB.FSharp

type File = {
    Id: Guid
    Name: string
    Extension: string
    Hash: string
    Size: int64
    Source: string
    Path: string
}
let clearHash (s:string) = s.Replace("-", "").ToLower()
let computeHash (f:FileInfo) = File.ReadAllBytes(f.FullName) |> SHA1.Create().ComputeHash |> BitConverter.ToString |> clearHash

let fileInfoToFile (f:FileInfo, source:string) : File = {
    Id = Guid.NewGuid();
    Name = f.Name;
    Extension = f.Extension;
    Hash = computeHash f;
    Size = f.Length;
    Source = source;
    Path = f.FullName;
}
[<EntryPoint>]
let main argv =
    let scanDir = @"C:\Users\mswietlicki\Pictures\Tapety"
    let findFiles = DirectoryInfo(scanDir).EnumerateFiles "*"

    let mapper = FSharpBsonMapper()
    use db = new LiteDatabase("DisksReorganizer.db", mapper)
    let filesColl = db.GetCollection<File>()

    findFiles
        |> Seq.map (fun f -> fileInfoToFile (f, scanDir))
        |> Seq.map (fun f -> filesColl.Insert(f))
        |> Seq.iter (fun r -> printfn "File insert: %s" r.AsString)
    0 // return an integer exit code
