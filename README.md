# CDK F# trial project

I want to write an `EventPattern` like this.

```yaml
EventPattern:
  source:
    - aws.s3
  detail-type:
    - Object Created
  detail:
    bucket:
      name:
        - Ref: bucket
    object:
      key:
        - prefix: test/
```

But I can't write the following.

```fsharp
EventPattern =
    EventPattern(
        Source = [| "aws.s3" |],
        DetailType = [| "Object Created" |],
        Detail =
            dict [ ("bucket", dict [ ("name", [| bucket.BucketName |]) ])
                    ("object", dict [ ("key", [| dict [ ("prefix", "test/") ] |]) ]) ]
    )
```

It will occurs a error.
[DotNet: Unable to pass interface instance through in a Dictionary<string, object> · Issue #1044 · aws/jsii](https://github.com/aws/jsii/issues/1044)

```plaintext
Unhandled exception. System.ArgumentException: Could not infer JSII type for .NET type 'IDictionary`2' (Parameter 'type')
```

The workaround for `IDictionary<string, object>` inside the array is raw overrides.

```fsharp
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
                        // Set an empty array as the value of `detail.object.key`.
                        Detail =
                            dict [ ("bucket", dict [ ("name", [| bucket.BucketName |]) ])
                                   ("object", dict [ ("key", [||]) ]) ]
                    )
            )
        )
    // And overrides it.
    do
        match rule.Node.DefaultChild with
        | :? CfnRule as ep -> ep.AddPropertyOverride("EventPattern.detail.object.key.0", dict [ "prefix", "test/" ])
        | _ -> failwith "You passed a wrong variable that is not of type CfnRule!"
```

Finally i got the expected output like this.

```yaml
EventPattern:
  detail:
    bucket:
      name:
        - Ref: sourcebucketE323AAE3
    object:
      key:
        - prefix: test/
  detail-type:
    - Object Created
  source:
    - aws.s3
```
