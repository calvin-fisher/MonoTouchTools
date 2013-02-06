using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.ObjCRuntime;

// Inspired by http://stackoverflow.com/questions/4221169/android-toast-in-iphone

namespace MonoTouchTools
{
    /// <summary>
    /// Android-like Toast popup.
    /// </summary>
    /// <remarks>
    /// Create and Show() like MonoTouch UIAlertView, or static Show() like .NET MessageBox.
    /// </remarks>        
    public class ToastView : NSObject
    {
        public string Text { get; protected set; }
        public int DurationMilliseconds { get; set; }
        public double DurationSeconds { get { return DurationMilliseconds / 1000; } }
        public UITextAlignment TextAlignment { get; set; }
        public ToastGravity Gravity { get; set; }
        public int OffsetLeft { get; set; }
        public int OffsetTop { get; set; }
        public int Padding { get; set; }
        public UIFont Font { get; set; }
        public UIColor TextColor { get; set; }
        public UIColor BackgroundColor { get; set; }
        public UIColor ShadowColor { get; set; }
        public int ShadowOffset { get; set; }

        protected const int defaultDurationMilliseconds = 1000;
        protected const UITextAlignment defaultTextAlignment = UITextAlignment.Center;
        protected const int defaultPadding = 15;
        protected const ToastGravity defaultGravity = ToastGravity.Center;
        protected const int defaultOffsetLeft = 0;
        protected const int defaultOffsetTop = 0;

        protected UIButton button;
        protected UIView view;
        protected UIWindow window;

        public ToastView (string text): this (text, defaultDurationMilliseconds) {}
        public ToastView (string text, int durationMilliseconds) : this (text, durationMilliseconds, defaultGravity) {}
        public ToastView (string text, int durationMilliseconds, ToastGravity gravity) : this (text, durationMilliseconds, gravity, defaultPadding) {}
        public ToastView (string text, int durationMilliseconds, ToastGravity gravity, int padding) : this (text, durationMilliseconds, gravity, padding, defaultTextAlignment) {}
        public ToastView (string text, int durationMilliseconds, ToastGravity gravity, int padding, UITextAlignment textAlignment)
        {
            Text = text;
            DurationMilliseconds = durationMilliseconds;
            TextAlignment = textAlignment;
            Padding = padding;
            Gravity = gravity;

            button = UIButton.FromType (UIButtonType.Custom);
            view = button;
            window = UIApplication.SharedApplication.Windows[0];

            OffsetLeft = 0;
            OffsetTop = 0;
            Font = UIFont.SystemFontOfSize (16);
            TextColor = UIColor.White;
            BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0.7f);
            ShadowColor = UIColor.DarkGray;
            ShadowOffset = 1;
        }

        public static void Show (string text)
        {
            new ToastView(text).Show();
        }
        public static void Show (string text, int durationMilliseconds)
        {
            new ToastView(text, durationMilliseconds).Show();
        }
        public static void Show (string text, int durationMilliseconds, ToastGravity gravity)
        {
            new ToastView(text, durationMilliseconds, gravity).Show();
        }
        public static void Show (string text, int durationMilliseconds, ToastGravity gravity, int padding)
        {
            new ToastView(text, durationMilliseconds, gravity, padding).Show();
        }
        public static void Show (string text, int durationMilliseconds, ToastGravity gravity, int padding, UITextAlignment textAlignment)
        {
            new ToastView (text, durationMilliseconds, gravity, padding, textAlignment).Show();
        }

        public void Show ()
        {
            var textSize = view.StringSize (Text, Font, new SizeF (280,60));
            UILabel label = new UILabel (new RectangleF (0, 0, textSize.Width + 5, textSize.Height + 5))
            {
                TextColor = TextColor,
                BackgroundColor = UIColor.Clear,
                Font = Font,
                Text = Text,
                Lines = 0,
                ShadowColor = ShadowColor,
                ShadowOffset = new SizeF (ShadowOffset, ShadowOffset),
                TextAlignment = TextAlignment,
            };
            
            button.Frame = new RectangleF (0, 0, textSize.Width + Padding, textSize.Height + Padding);
            label.Center = new PointF (button.Frame.Size.Width / 2, button.Frame.Height / 2);

            button.BackgroundColor = BackgroundColor;
            button.Layer.CornerRadius = 5;
            button.Center = Position;

            button.AddSubview (label);
            window.AddSubview (button);
            button.AllTouchEvents += delegate { HideToast (); };
            
            NSTimer.CreateScheduledTimer (DurationSeconds, HideToast);
        }

        public PointF Position
        {
            get
            {
                switch (Gravity)
                {
                    case ToastGravity.Top:
                        return new PointF(
                            (window.Frame.Size.Width / 2) + OffsetLeft, 
                            45 + OffsetTop
                            );
                        
                    case ToastGravity.Bottom:
                        return new PointF(
                            (window.Frame.Size.Width / 2) + OffsetLeft, 
                            window.Frame.Size.Height - 45 + OffsetTop
                            );
                        
                    case ToastGravity.Center:
                        return new PointF (
                            (window.Frame.Size.Width / 2) + OffsetLeft,
                            (window.Frame.Size.Height / 2) + OffsetTop
                            );
                        
                    default:
                        throw new IndexOutOfRangeException("Unknown ToastGravity value " + Gravity);
                }
            }
        }

        protected void HideToast ()
        {
            UIView.BeginAnimations ("");
            view.Alpha = 0;
            UIView.CommitAnimations ();
        }
        
        protected void RemoveToast ()
        {
            view.RemoveFromSuperview ();
        }
    }
    
    public enum ToastGravity
    {
        Top = 0,
        Bottom = 1,
        Center = 2
    }
}