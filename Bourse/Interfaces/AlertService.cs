namespace Bourse.Interfaces
{
    public class AlertService : IAlertService
    {
        public Task ShowAlertAsync(string title, string message, string cancel)
        {
            System.Diagnostics.Debug.Assert(Application.Current != null);
            return Shell.Current.DisplayAlert(title, message, cancel);
        }

        public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        {
            return Shell.Current.DisplayAlert(title, message, accept, cancel);
        }
    }
}
