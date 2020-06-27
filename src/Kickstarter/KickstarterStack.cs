using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.APIGateway;
using Amazon.DynamoDBv2.Model;

namespace Kickstarter
{
    public class KickstarterStack : Stack
    {
        internal KickstarterStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // No-SQL Dynamo Tables
            ITableProps postsTableProps = new TableProps()
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                TableName = "posts",
                PartitionKey = new Attribute() {
                    Name = "id", Type =  AttributeType.STRING
                }
            };
            ITable postsTable = new Table(this, "postsTable", postsTableProps);

            ITableProps commentsTableProps = new TableProps()
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                TableName = "comments",
                PartitionKey = new Attribute()
                {
                    Name = "id", Type = AttributeType.STRING
                },
                
            };
            Table commentsTable = new Table(this, "commentsTable", commentsTableProps);

            GlobalSecondaryIndexProps globalSecondaryIndexProps = new GlobalSecondaryIndexProps()
            {
                PartitionKey = new Attribute()
                {
                    Name = "post_id", Type = AttributeType.STRING
                },
                IndexName = "post_id-index"
            };
            commentsTable.AddGlobalSecondaryIndex(globalSecondaryIndexProps);

            // SQL RDS Instance
            Vpc vpc = new Vpc(this, "kickstarterVPC");

            IDatabaseInstanceProps databaseInstanceProps = new DatabaseInstanceProps()
            {
                Engine = DatabaseInstanceEngine.MYSQL,
                InstanceClass = InstanceType.Of(InstanceClass.BURSTABLE2, InstanceSize.MICRO),
                Vpc = vpc,
                VpcPlacement = new SubnetSelection()
                {
                    SubnetType = SubnetType.PUBLIC
                },
                MultiAz = false,
                AutoMinorVersionUpgrade = false,
                AllocatedStorage = 20,
                StorageType = StorageType.GP2,
                BackupRetention = Duration.Days(3),
                DeletionProtection = false,
                MasterUsername = "admin",
                MasterUserPassword = new SecretValue("password"),
                DatabaseName = "kickstarter",
                Port = 3306,
                RemovalPolicy = RemovalPolicy.DESTROY,
                InstanceIdentifier = "kickstarter"
            };
            IDatabaseInstance databaseInstance = new DatabaseInstance(this, "databaseInstance", databaseInstanceProps);
        }
    }
}
