namespace LOS.ViewModel
{
    public class TrackStepViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public bool IsRejected { get; set; }
    }
}
