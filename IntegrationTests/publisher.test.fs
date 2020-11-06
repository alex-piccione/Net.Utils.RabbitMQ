module IntegrationTests.publisher

open System
open NUnit.Framework


[<Test>]
let ``Send will create the queue`` () =

    helper.deleteQueue("test", false)

    use publisher = helper.createPublisher()

    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    // act
    publisher.Send(message, "test", "")

    Assert.Pass()


[<Test>]
let ``Send do not raise an error`` () =

    use publisher = helper.createPublisher()

    let message = String.Format( "{{when:\"{0:u}\"}}", DateTime.UtcNow); 

    // act
    publisher.Send(message, "test", "")

    Assert.Pass()

[<Test>]
let ``Send publish a message in the exchange`` () =

    ()