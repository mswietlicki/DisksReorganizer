namespace DisksReorganizer.Common
open System

type File = {
    Id: Guid
    Name: string
    Extension: string
    Hash: string
    Size: int64
    SourceName: string
    Source: string
    Path: string
}