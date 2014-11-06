using System;


namespace SharpQuant.UI.UserMessage
{
    #region Enums

    public enum UserMessageCategories { Error, Warning, Information, YesNo, OkCancel }

    #endregion

    #region Event Args

    public class UserMessagePostedEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="caption">The caption for the message</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="category">The category of the message.</param>
        public UserMessagePostedEventArgs(UserMessageCategories category, string caption, string message)
        {
            this.Category = category;
            this.Caption = caption;
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The caption for the message box.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// The text of the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The category of message 
        /// </summary>
        public UserMessageCategories Category { get; set; }

        #endregion
    }

    #endregion
}
