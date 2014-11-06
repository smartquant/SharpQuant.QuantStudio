using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using SharpQuant.Common.Validation;
using SharpQuant.UI.UserMessage;

namespace SharpQuant.UI.PropertyGrid
{

    public partial class PropertyWindow : UserControlBase, IPropertyEditableObjectEditor
    {
        private IPropertyEditableObject _editedObject;
        private IValidationProvider _validationProvider;
        private IUserMessageService _message;
        private Func<bool> _validate = ()=>true;

        public PropertyWindow()
        {
            InitializeComponent();

            //Add toolstrip menu to property grid
            ToolStrip ts = (ToolStrip)propertyGrid.Controls[3];
            ts.AllowMerge = true;
            ToolStripManager.Merge(toolStrip, ts);
            CheckDBSaveStatus();
            
        }


        public void Init(IValidationProvider validationProvider, IUserMessageService message)
        {
            _validationProvider = validationProvider;
            _message = message;
        }


        public IPropertyEditableObject SelectedObject
        {
            get { return _editedObject; }
        }

        public void EditObject<T>(IPropertyEditableObject value) where T:class
        {
            if (value != null && value.Action != EPropertyEditAction.None)
            {
                IValidator<T> validator = null;
                if (_validationProvider != null)
                {
                    validator = _validationProvider.GetValidator<T>();
                    if (validator != null)
                    {
                        _validate = () =>
                        {
                            if (validator.IsValid(value.Object as T)) return true;
                            var msg = validator.GetErrorMessages(value.Object as T);
                            _message.ShowUserMessage(UserMessageCategories.Error, "Validation error", "Entity has some invalid values:\r\n" + string.Join("\r\n", msg));
                            return false;
                        };
                    }
                    else
                    {
                        _validate = () => true;
                    }
                }
            }

            if (_editedObject == null || _editedObject.Action == EPropertyEditAction.None) 
                goto set_value;
            if (!_editedObject.IsNew && !_editedObject.IsDirty) 
                goto set_value; 


            try
            {
                if (_editedObject.Action == EPropertyEditAction.Autosave)
                    SaveEditedObject();
                else
                {
                    DialogResult result = _message.ShowUserMessage(UserMessageCategories.YesNo,"Save object", "Do you want to save the changes to the object?");
                    if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                        SaveEditedObject();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        set_value:
            _editedObject = value;
            propertyGrid.SelectedObject = (_editedObject == null) ? null : _editedObject.Object;
            CheckDBSaveStatus();
            return;


        }

        private void SaveEditedObject()
        {
            if (_editedObject == null || _editedObject.UpdateMethod == null || (!_editedObject.IsNew && !_editedObject.IsDirty)) return;

            if (_validate())
            {
                if (_editedObject.IsNew)
                {
                    _editedObject.UpdateMethod.Invoke();
                    _editedObject.IsNew = false;
                }
                else
                {
                    _editedObject.UpdateMethod.Invoke();
                    _editedObject.IsDirty = false;
                }
                CheckDBSaveStatus();
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (_editedObject != null)
            {
                //add validation here
                _editedObject.IsDirty = true;
                CheckDBSaveStatus();

            }
            
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            try
            {
                //add validation here

                SaveEditedObject();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {           
            propertyGrid.SelectedObject = null;
            _editedObject = null;
            CheckDBSaveStatus();
        }

        private void CheckDBSaveStatus()
        {
            if (_editedObject == null || _editedObject.UpdateMethod == null || (!_editedObject.IsDirty && !_editedObject.IsNew))
                tsbSave.Enabled = false;
            else
                tsbSave.Enabled = true;
        }

        protected override void OnChangeSettings()
        {
            //throw new NotImplementedException();
        }
    }
}