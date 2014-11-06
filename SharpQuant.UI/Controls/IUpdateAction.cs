using System;
namespace SharpQuant.UI.Controls
{
    public interface IUpdateAction
    {
        void SetAfterLookup(Action afterLookup);
    }
}
