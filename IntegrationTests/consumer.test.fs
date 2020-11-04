module IntegrationTests.consumer

open System
open Microsoft.Extensions.Configuration
open NUnit.Framework
open consumer

let getConsumer() =
    let secret = ConfigurationBuilder().AddUserSecrets("5a837560-b6ce-4bd1-aefa-187bd319e09a").Build()
    let configuration = {Url=secret.["RabbitMQ:URL"]}
    Consumer(configuration)


[<Test>]
let ``Consume`` () =

    let consumer = getConsumer()

    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    let message = consumer.Consume("test", "")

    Assert.Pass()

