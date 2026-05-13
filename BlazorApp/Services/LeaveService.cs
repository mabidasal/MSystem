using BlazorApp.Models;

namespace BlazorApp.Services
{
    public class LeaveService
    {
        private List<LeaveRequest> _requests = new();

        public List<LeaveRequest> GetRequests() => _requests;

        public void AddRequest(LeaveRequest request)
        {
            request.Id = _requests.Count + 1;
            _requests.Add(request);
        }

        public void UpdateStatus(int id, string status)
        {
            var req = _requests.FirstOrDefault(r => r.Id == id);
            if (req != null) req.Status = status;
        }
    }
}