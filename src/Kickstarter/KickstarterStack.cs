using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;

namespace Kickstarter
{
    public class KickstarterStack : Stack
    {
        internal KickstarterStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // NO-SQL Dynamo Tables
            ITableProps postsTableProps = new TableProps()
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                TableName = "posts",
                PartitionKey = new Attribute() {
                    Name = "id", Type =  AttributeType.STRING
                }
            };
            ITable postsTable = new Table(this, "postsTable", postsTableProps);


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
                DatabaseName = "kickstarter",
                Port = 3306
            };
            IDatabaseInstance databaseInstance = new DatabaseInstance(this, "databaseInstance", databaseInstanceProps);
        }
    }
}
