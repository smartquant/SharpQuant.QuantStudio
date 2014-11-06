using System;
using System.Windows.Forms;

namespace SharpQuant.UI.UserMessage
{
    public interface IUserMessageService
    {
        DialogResult ShowUserMessage(UserMessageCategories category, string caption, string message);
    }
}
