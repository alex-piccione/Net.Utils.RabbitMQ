module helper

open System.Net.Http
open System
open Microsoft.Extensions.Configuration
open System.Net.Http.Headers
open System.Buffers.Text
open System.Text
open NUnit.Framework
open FsUnit
open Newtonsoft.Json.Linq


let secret = ConfigurationBuilder().AddUserSecrets("5a837560-b6ce-4bd1-aefa-187bd319e09a").Build()
let baseAddress = secret.["RabbitMQ:API base address"]
let vhost = secret.["RabbitMQ:vhost"]
let username = secret.["RabbitMQ:username"]
let password = secret.["RabbitMQ:password"]


/// Encodes a UTF8 string as a Base64 string
let encodeBase64 text =
    let tripletToList ending (x, y, z) =
        // RFC 4648: The Base 64 Alphabet
        let A = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/="

        // RFC 4648: The "URL and Filename safe" Base 64 Alphabet
        // let A = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_="

        let triplet = (int x <<< 16)
                    ||| (int y <<<  8)
                    ||| (int z)
        let a = (triplet &&& 0xFC0000) >>> 18
        let b = (triplet &&& 0x03F000) >>> 12
        let c = (triplet &&& 0x000FC0) >>>  6
        let d = (triplet &&& 0x00003F)
        match ending with
        | 1 -> [A.[a]; A.[b];  '=' ;  '=' ;] // 01==
        | 2 -> [A.[a]; A.[b]; A.[c];  '=' ;] // 
        | _ -> [A.[a]; A.[b]; A.[c]; A.[d];] // 

    let rec parse result input =
        match input with
        | a :: b :: c :: tail -> parse (result @ tripletToList 3 (a, b, c)) tail
        | a :: b :: []        -> result @ tripletToList 2 (a,   b, 0uy)
        | a :: []             -> result @ tripletToList 1 (a, 0uy, 0uy)
        | []                  -> result

    (text:string)
    |> System.Text.Encoding.UTF8.GetBytes
    |> Array.toList
    |> parse []
    |> List.toArray
    |> System.String.Concat

let encodeBase64_new (text:string) =
    Convert.ToBase64String( Encoding.UTF8.GetBytes(text))
//    let stream = Encoding. StremReader(text) // Some stream, for example: new MemoryStream([| 1uy; 2uy; 3uy; 4uy |])
//    let buffer = Array.zeroCreate (int stream.Length)
//    stream.Read(buffer, 0, buffer.Length)
//    Convert.ToBase64String(buffer)

let basiAuth () = 
    let base64Encode (s:string) =
        let bytes = Encoding.UTF8.GetBytes(s)
        Convert.ToBase64String(bytes)

    sprintf "%s:%s" username password |> base64Encode |> sprintf "Basic %s"


[<Test>]
let listQueues () = 

    use client = new HttpClient()
    client.BaseAddress <- Uri(sprintf "%s/%s" baseAddress vhost)
    client.DefaultRequestHeaders.Add("Authorization", basiAuth())
    let response = client.GetAsync("api/queues").Result

    //Assert.That(response.IsSuccessStatusCode)
    if not(response.IsSuccessStatusCode) then failwithf "Failed to load queues. (%O) %s" response.StatusCode response.ReasonPhrase

    let json = response.Content.ReadAsStringAsync().Result

    let parseQueue (json:JToken) = (json.Value<string>("name"), json.Value<int>("messages"))
    let queues = JArray.Parse(json) |> Seq.map parseQueue |> List.ofSeq
    
    // assume there is at least a queue
    queues.Length |> should be (greaterThan 0)
    queues |> List.exists (fun x -> fst x = "test") |> should be True    



let deleteQueue name = 

    // /api/queues/vhost/name

    use client = new HttpClient()
    client.BaseAddress <- Uri(baseAddress)
    //client.DefaultRequestHeaders.Add("Accept", "application/json")
    //client.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("" BasicAuth ("gydvvzsv")
    client.DefaultRequestHeaders.Add("Authorization", basiAuth())


    let url = sprintf "/api/queues/gydvvzsv/%s" name
    let response = client.DeleteAsync(url).Result

    ()

