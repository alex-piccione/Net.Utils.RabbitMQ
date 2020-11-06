module common

open System
open System.Text.Json
open System.Text


type Config = {Url:string}

type Exchange = {
    Name:string; 
    VHost:string; 
    Type:string
}



let Deserialize<'a> (bytes:ReadOnlyMemory<byte>):'a =
    let json = 
        try 
            Encoding.UTF8.GetString(bytes.Span)
        with e -> failwithf "Failed to deserialize message to JSON. %A" e

    try
        JsonSerializer.Deserialize<'a>(json)
    with e -> failwithf "Failed to deserialize JSON to %s. %A" (typedefof<'a>.Name) e