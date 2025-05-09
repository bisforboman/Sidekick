namespace Sidekick.Common;

/// <summary>
///     Interface to handle dialogs in the application.
/// </summary>
public interface ISidekickDialogs
{
    /// <summary>
    ///     Open a notification message with Yes/No buttons
    /// </summary>
    /// <param name="message">The message to show in the notification</param>
    /// <returns>True if the user confirmed the action.</returns>
    Task<bool> OpenConfirmationModal(string message);

    /// <summary>
    /// Open a notification message with an OK button.
    /// </summary>
    /// <param name="message">The message to display in the notification.</param>
    /// <returns>True if the user acknowledged the message.</returns>
    Task<bool> OpenOkModal(string message);
}
