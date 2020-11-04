module publisher


type Config = {Url:string}

type Publisher(config:Config) =



    member this.Publish(message:string) = 



        ()

