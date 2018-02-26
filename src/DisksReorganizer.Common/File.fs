namespace DisksReorganizer.Common
open System

type Hash = {
    Id: Guid
    Hash: string
}

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