namespace WaitlistApplication.WaitlistManager
{
    public class Waitlist
    {
        private List<WaitlistRequest> pendingRequests = new List<WaitlistRequest>();

        public void AddRequest(WaitlistRequest request)
        {
            pendingRequests.Add(request);
        }

        public int GetWaitlistLength()
        {
            return pendingRequests.Count;
        }
    }
}
