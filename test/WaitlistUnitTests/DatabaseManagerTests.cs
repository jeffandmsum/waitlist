using WaitlistApplication;

namespace WaitlistUnitTests
{
    public class DatabaseManagerTests
    {
        [SetUp]
        public void Setup()
        {
            // Initialize an instance of the database manager, which should pre-populate the schema
            DatabaseManager dbManager = DatabaseManager.Instance;
        }

        [Test]
        public void VerifyRestaurantTableExists()
        {
            // Arrange
            DatabaseManager dbManager = DatabaseManager.Instance;
            using (var dbConn = dbManager.CreateConnection())
            {
                dbConn.Open();

                using (var sqlCmd = dbConn.CreateCommand())
                {
                    // Act
                    sqlCmd.CommandText = "SELECT COUNT(*) FROM Restaurant";
                    var result = sqlCmd.ExecuteScalar();

                    // Assert
                    Assert.That(result, Is.Not.Null);
                }
            }
        }
    }
}