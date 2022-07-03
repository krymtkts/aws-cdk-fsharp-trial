# CDK F# trial project

Now not working due to the following areas.

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

```plaintext
Unhandled exception. System.ArgumentException: Could not infer JSII type for .NET type 'IDictionary`2' (Parameter 'type')
```
