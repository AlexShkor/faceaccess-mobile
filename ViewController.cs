using System;

using UIKit;

using AVFoundation;
using System.Threading.Tasks;
using CoreImage;
using System.Net.Http;
using System.Collections.Generic;

namespace Camera
{
	public partial class ViewController : UIViewController
	{
        private Settings _settings = new Settings();

        private CameraService _cameraService;
        private PersonObserver _personObserver;


        private AVCaptureVideoPreviewLayer videoPreviewLayer;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			await CameraService.AuthorizeCameraUse ();
            _cameraService = new CameraService();
            _personObserver = new PersonObserver(_cameraService);
            AddVideoPreview();
            _cameraService.SetupLiveCameraStream();
            _personObserver.Start();
		}

        private void AddVideoPreview()
        {
			var viewLayer = liveCameraStream.Layer;
			videoPreviewLayer = new AVCaptureVideoPreviewLayer(_cameraService.captureSession)
			{
				Frame = this.View.Frame
			};

			liveCameraStream.Layer.AddSublayer(videoPreviewLayer);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}


	}
}