using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utilities
{
    public class FieldsTabulation : MonoBehaviour, ITabulation
    {
        [SerializeField] 
        private UIInputManager uiManager;
        
        private List<Selectable> fields;
        private int selectedFieldIndex = -1;

        void Update()
        {
            HandleInputTabbing();
        }

        // Handles tabbing between inputs and buttons
        public void HandleInputTabbing()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ResetIndexAccordingToManualSelect();

                // update index to where we need to tab to
                selectedFieldIndex++;

                if (selectedFieldIndex >= fields.Count)
                {
                    // reset back to first input
                    selectedFieldIndex = 0;
                }
                fields[selectedFieldIndex].Select();
            }
        }

        // If the user selects an input via mouse click, then the _selectedFieldIndex 
        // may not be accurate as the focused field wasn't change by tabbing. Here we
        // correct the _selectedFieldIndex in case they wish to start tabing from that point on.
        public void ResetIndexAccordingToManualSelect()
        {
            for (var i = 0; i < fields.Count; i++)
            {
                if (fields[i] is InputField && ((InputField)fields[i]).isFocused && selectedFieldIndex != i)
                {
                    // Debug.Log("_selectedFieldIndex is : " + _selectedFieldIndex + ", Reset _selectedFieldIndex to: " + i);
                    selectedFieldIndex = i;
                    break;
                }
            }
        }

        private void Start()
        {
            fields = new List<Selectable> { 
                uiManager.emailFieldLogin,
                uiManager.passwordFieldLogin,
                uiManager.loginButton,
                uiManager.emailRegisterField,
                uiManager.usernameRegisterField,
                uiManager.passwordRegisterField,
                uiManager.signupButton 
            };

        }

        public void SetFieldIndex(int index) => selectedFieldIndex = index;
    }
}
