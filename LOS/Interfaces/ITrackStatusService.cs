using LOS.ViewModel;

namespace LOS.Interfaces
{
    public interface ITrackStatusService
    {
        TrackStatusViewModel GetCustomerTrackStatus(int customerId);
    }
}