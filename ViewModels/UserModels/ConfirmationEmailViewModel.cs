namespace Silerium.ViewModels.UserModels
{
    public class ConfirmationEmailViewModel
    {
        public string Email { get; set; }
        public bool EmailAlreadySent { get; set; } = false;
        public bool EmailSentOnLoad { get; set; } = false;
        public string? ReturnUrl { get; set; }
    }
}
