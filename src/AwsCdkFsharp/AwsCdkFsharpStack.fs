namespace AwsCdkFsharp

open Amazon.CDK
open Amazon.CDK.AWS.S3
open Amazon.CDK.AWS.Lambda
open Amazon.CDK.AWS.Logs
open Amazon.CDK.AWS.Events
open Amazon.CDK.AWS.Events.Targets

type AwsCdkFsharpStack(scope, id, props) as this =
    inherit Stack(scope, id, props)

    let bucket =
        Bucket(
            this,
            "source-bucket",
            BucketProps(
                BucketName = "source-bucket",
                Versioned = false,
                RemovalPolicy = RemovalPolicy.DESTROY,
                AutoDeleteObjects = true
            )
        )

    let lfunc =
        Function(
            this,
            "sample-function",
            FunctionProps(
                FunctionName = "SampleFunction",
                Description = "sample function.",
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./src/api/bin/Debug/"),
                Handler = "api::api.Function::FunctionHandler",
                Architecture = Architecture.ARM_64,
                MemorySize = (Some 128.0 |> Option.toNullable),
                Timeout = Duration.Seconds(10),
                LogRetention = (Some RetentionDays.ONE_DAY |> Option.toNullable)
            )
        )

    let rule =
        Rule(
            this,
            "bucket-event",
            RuleProps(
                RuleName = "buckt-event",
                Description = "bucket event.",
                EventPattern =
                    EventPattern(
                        Source = [| "aws.s3" |],
                        DetailType = [| "Object Created" |],
                        // NOTE: cannot write like below because JSII is unable to use `IDictionary<string, object>` inside the array.
                        // Detail =
                        //     dict [ ("bucket", dict [ ("name", [| bucket.BucketName |]) ])
                        //            ("object", dict [ ("key", [| dict [ ("prefix", "test/") ] |]) ]) ]
                        Detail =
                            dict [ ("bucket", dict [ ("name", [| bucket.BucketName |]) ])
                                   ("object", dict [ ("key", [||]) ]) ]
                    )
            )
        )

    // NOTE: the workaround for IDictionary<string, object>` inside the array is raw overrides.
    do
        match rule.Node.DefaultChild with
        | :? CfnRule as ep -> ep.AddPropertyOverride("EventPattern.detail.object.key.0", dict [ "prefix", "test/" ])
        | _ -> failwith "You passed a wrong variable that is not of type CfnRule!"


    do rule.AddTarget(LambdaFunction(lfunc))
