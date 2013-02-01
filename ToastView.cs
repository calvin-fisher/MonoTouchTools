using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.ObjCRuntime;

// From http://stackoverflow.com/questions/4221169/android-toast-in-iphone

namespace MonoTouchTools
{
    /// <summary>
    /// Android-like Toast popup.
    /// </summary>
    /// <remarks>
    /// Works like MonoTouch UIAlertView - just create and Show().
    /// </remarks>        
    public class ToastView : NSObject
    {
        
        ToastSettings theSettings = new ToastSettings ();
        
        private string text = null;
        UIView view;
        public ToastView (string Text, int durationMilliseonds)
        {
            text = Text;
            theSettings.Duration = durationMilliseonds;
        }
        
        int offsetLeft = 0;
        int offsetTop = 0;
        public ToastView SetGravity (ToastGravity gravity, int OffSetLeft, int OffSetTop)
        {
            theSettings.Gravity = gravity;
            offsetLeft = OffSetLeft;
            offsetTop = OffSetTop;
            return this;
        }
        
        public ToastView SetPosition (PointF Position)
        {
            theSettings.Position = Position;
            return this;
        }
        
        public void Show ()
        {
            UIButton v = UIButton.FromType (UIButtonType.Custom);
            view = v;
            
            UIFont font = UIFont.SystemFontOfSize (16);
            SizeF textSize = view.StringSize (text, font, new SizeF (280, 60));
            
            UILabel label = new UILabel (new RectangleF (0, 0, textSize.Width + 5, textSize.Height + 5));
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = UIColor.White;
            label.Font = font;
            label.Text = text;
            label.Lines = 0;
            label.ShadowColor = UIColor.DarkGray;
            label.ShadowOffset = new SizeF (1, 1);
            
            
            v.Frame = new RectangleF (0, 0, textSize.Width + 10, textSize.Height + 10);
            label.Center = new PointF (v.Frame.Size.Width / 2, v.Frame.Height / 2);
            v.AddSubview (label);
            
            v.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0.7f);
            v.Layer.CornerRadius = 5;
            
            UIWindow window = UIApplication.SharedApplication.Windows[0];
            
            PointF point = new PointF (window.Frame.Size.Width / 2, window.Frame.Size.Height / 2);
            
            if (theSettings.Gravity == ToastGravity.Top)
            {
                point = new PointF (window.Frame.Size.Width / 2, 45);
            }
            else if (theSettings.Gravity == ToastGravity.Bottom)
            {
                point = new PointF (window.Frame.Size.Width / 2, window.Frame.Size.Height - 45);
            }
            else if (theSettings.Gravity == ToastGravity.Center)
            {
                point = new PointF (window.Frame.Size.Width / 2, window.Frame.Size.Height / 2);
            }
            else
            {
                point = theSettings.Position;
            }
            
            point = new PointF (point.X + offsetLeft, point.Y + offsetTop);
            v.Center = point;
            window.AddSubview (v);
            v.AllTouchEvents += delegate { HideToast (); };
            
            NSTimer.CreateScheduledTimer (theSettings.DurationSeconds, HideToast);
            
        }
        
        
        void HideToast ()
        {
            UIView.BeginAnimations ("");
            view.Alpha = 0;
            UIView.CommitAnimations ();
        }
        
        void RemoveToast ()
        {
            view.RemoveFromSuperview ();
        }
        
    }

    public class ToastSettings
    {
        public ToastSettings ()
        {
            this.Duration = 500;
            this.Gravity = ToastGravity.Center;
        }
        
        public int Duration
        {
            get;
            set;
        }
        
        public double DurationSeconds
        {
            get { return (double) Duration/1000 ;}
            
        }
        
        public ToastGravity Gravity
        {
            get;
            set;
        }
        
        public PointF Position
        {
            get;
            set;
        }
    }

    public enum ToastGravity
    {
        Top = 0,
        Bottom = 1,
        Center = 2
    }
}