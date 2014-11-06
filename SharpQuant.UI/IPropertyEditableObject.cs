using System;
namespace SharpQuant.UI
{
    public enum EPropertyEditAction
    {
        None,
        Autosave,
        Ask_User
    }

    public interface IPropertyEditableObject
    {
        EPropertyEditAction Action { get; set; }
        Action UpdateMethod { get; set; }
        bool IsDirty { get; set; }
        bool IsNew { get; set; }
        object Object { get; }
    }
}
