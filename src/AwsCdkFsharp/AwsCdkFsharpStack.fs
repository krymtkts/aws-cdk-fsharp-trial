namespace AwsCdkFsharp

open Amazon.CDK
open Amazon.CDK.AWS.SNS
open Amazon.CDK.AWS.SNS.Subscriptions
open Amazon.CDK.AWS.SQS

type AwsCdkFsharpStack(scope, id, props) as this =
    inherit Stack(scope, id, props)

    let queue = Queue(this, "AwsCdkFsharpQueue", QueueProps(VisibilityTimeout = Duration.Seconds(300.)))

    let topic = Topic(this, "AwsCdkFsharpTopic")
    do topic.AddSubscription(SqsSubscription(queue))
