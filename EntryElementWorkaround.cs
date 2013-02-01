using System;
using MonoTouch;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace MonoTouchTools
{
    public class EntryElementWorkaround : EntryElement
    {
        public EntryElementWorkaround(string caption, string placeholder, string value)
            : base(caption, placeholder, value)
        { }
        
        public EntryElementWorkaround(string caption, string placeholder, string value, bool isPassword)
            : base(caption, placeholder, value, isPassword)
        { }
        
        protected override UITextField CreateTextField(System.Drawing.RectangleF frame)
        {
            var field = base.CreateTextField(frame);
            field.EditingChanged += this.EditingChanged;
            return field;
        }

        /// <summary>
        /// Fires when the user updates the contents of the EntryElement.  This is what one would expect EntryElement.Changed to do.
        /// </summary>
        public event EventHandler EditingChanged;        
    }
}