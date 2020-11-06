module IntegrationTests.consumer

open System
open Microsoft.Extensions.Configuration
open NUnit.Framework
open FsUnit
open common
open consumer

let createConsumer() =
    let secret = ConfigurationBuilder().AddUserSecrets("5a837560-b6ce-4bd1-aefa-187bd319e09a").Build()
    let configuration = {Url=secret.["RabbitMQ:URL"]}
    Consumer(configuration)


[<Test>]
let ``Consume <should> create exchange and queue`` () =

    let queue = "test 1"
    let exchange = "test"
    helper.deleteQueue(queue, false)
    helper.deleteExchange(exchange, false)

    let consumer = createConsumer()

    let message = String.Format("{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    // act
    let message = consumer.Consume(queue, exchange, "key 1")

    
    let queues = helper.listQueues()
    queues |> List.exists (fun x -> fst x = queue) |> should be True  

    let exchanges = helper.listExchanges()
    exchanges |> List.exists (fun x -> x.Name = exchange) |> should be True

    helper.deleteQueue(queue, false)
    helper.deleteExchange(exchange, false)