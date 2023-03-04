using WaitlistApplication.WaitlistManager;

namespace WaitlistUnitTests
{
    public class WaitlistTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddRequest()
        {
            // Arrange
            Waitlist waitlist = new Waitlist();
            WaitlistRequest request = new WaitlistRequest();

            // Act
            waitlist.AddRequest(request);

            // Assert
            Assert.That(waitlist.GetWaitlistLength(), Is.EqualTo(1));
        }
    }
}