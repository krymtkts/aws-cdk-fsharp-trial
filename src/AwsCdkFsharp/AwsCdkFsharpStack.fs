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

    do
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
                        Detail =
                            dict [ ("bucket", dict [ ("name", [| bucket.BucketName |]) ])
                                   ("object", dict [ ("key", [| dict [ ("prefix", "test/") ] |]) ]) ]
                    )
            )
        )
            .AddTarget(LambdaFunction(lfunc))
