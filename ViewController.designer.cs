// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Camera
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView liveCameraStream { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton takePhotoButton { get; set; }

        [Action ("TakePhotoButtonTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TakePhotoButtonTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (liveCameraStream != null) {
                liveCameraStream.Dispose ();
                liveCameraStream = null;
            }

            if (takePhotoButton != null) {
                takePhotoButton.Dispose ();
                takePhotoButton = null;
            }
        }
    }
}