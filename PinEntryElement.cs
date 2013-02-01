using System;
using System.Linq;
using MonoTouch;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace MonoTouchTools
{
    /// <summary>EntryElement tuned to handle 4-digit numeric-only PINs.</summary>
    public class PinEntryElement : EntryElementWorkaround
    {
        public PinEntryElement(string caption, string placeholder, string value)
            : base(caption, placeholder, value, true)
        {
            this.EditingChanged += HandleEditingChanged;
            this.AutocapitalizationType = UITextAutocapitalizationType.None;
            this.AutocorrectionType = UITextAutocorrectionType.No;
            this.KeyboardType = UIKeyboardType.NumberPad;
            ExpectedLength = 4;
        }

        // Awful but functional
        public static readonly char[] Numerics = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'} ;
        private string StripNonNumerics(string originalString)
        {
            for (int i = 0; i < originalString.Length; i++)
            {
                if (!Numerics.Contains(originalString [i]))
                {
                    var stripped = originalString.Remove(i,1);
                    return StripNonNumerics(stripped);
                }
            }
            return originalString;
        }
        
        protected void HandleEditingChanged(object sender, EventArgs e)
        {
            // Hacky validation
            this.Value = StripNonNumerics(this.Value);
            if (this.Value.Length > 4)
                this.Value = this.Value.Substring(0, 4);
        }

        /// <summary>
        /// The expected length of the pin, in number of digits.
        /// </summary>
        public int ExpectedLength { get; set; }

        /// <summary>
        /// Whether the user has entered a numeric-only PIN of the expected length.
        /// </summary>
        public bool HasValidPin
        {
            get
            {
                return this.Value.Length == ExpectedLength
                    && this.Value.ToCharArray().All(x => Numerics.Contains(x));
            }
        }
    }
}