namespace WebUi.ViewModels
{
    public class DisputeViewModel
    {
        public DateTime CreatedOn { get; set; } = DateTime.MinValue;
        public string Branch { get; set; } = string.Empty;
        public string TerminalID { get; set; } = string.Empty;
        public string DisputeType { get; set; } = string.Empty;
    }
}
