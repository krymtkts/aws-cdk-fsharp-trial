open Amazon.CDK
open AwsCdkFsharp

[<EntryPoint>]
let main _ =
    let app = App(null)

    AwsCdkFsharpStack(app, "AwsCdkFsharpStack", StackProps()) |> ignore

    app.Synth() |> ignore
    0
