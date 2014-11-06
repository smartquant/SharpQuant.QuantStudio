using System;
using System.Windows.Forms;
using System.Text;

namespace SharpQuant.UI.UserMessage
{


    public class UserMessageService : IUserMessageService
    {
        /// <summary>
        /// Displays a message from the app to the user.
        /// </summary>
        /// <param name="category">The category of the message (e.g., warning, notification).</param>
        /// <param name="caption">The message box caption.</param>
        /// <param name="message">The message box text.</param>
        public DialogResult ShowUserMessage(UserMessageCategories category, string caption, string message)
        {
            if (!Environment.UserInteractive) return DialogResult.Ignore;

            var userMessageForm = new UserMessageForm(category, caption, message);
            return userMessageForm.ShowDialog();
        }
    }

}
