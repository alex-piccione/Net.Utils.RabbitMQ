[<NUnit.Framework.Category("Publisher")>]
module IntegrationTests.publisher

open System
open NUnit.Framework
open FsUnit

open Alex75.Utils.RabbitMQ


[<Test>]
let ``Send <should> create the exchange`` () =

    helper.deleteExchange("test", false)
    use publisher = new Publisher(secrets.URL, "test")
    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    // act
    publisher.Send(message, "test", "")

    helper.listExchanges() |> List.exists (fun ex -> ex.Name = "test") |> should be True

    helper.deleteExchange("test", false)


[<Test>]
let ``Send <should> publish a message in the exchange`` () =
    // use a diffrent name to avoid interferences with other tests
    helper.deleteExchange("test--1", false)

    use publisher = new Publisher(secrets.URL, "test--1")
    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    // act
    publisher.Send(message, "test--1", "")    
    
    // API find the message after some time...
    let mutable found = false
    for n in 1..50 do
        if not found then
            let exchanges = helper.listExchanges() |> List.find (fun ex -> ex.Name = "test--1")
            found <- exchanges.MessageStats_PublishIn = 1
            if exchanges.MessageStats_PublishIn > 1 then failwith "too many !!!"
            Threading.Thread.Sleep(250)

    found |> should be True

    helper.deleteExchange("test", false)