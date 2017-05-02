using System;

using UIKit;
using Foundation;
using AVFoundation;
using System.Threading.Tasks;
using CoreImage;
using System.Net.Http;

namespace Camera
{
	public partial class ViewController : UIViewController
	{
		bool flashOn = false;

		AVCaptureSession captureSession;
		AVCaptureDeviceInput captureDeviceInput;
		AVCaptureStillImageOutput stillImageOutput;
		AVCaptureVideoPreviewLayer videoPreviewLayer;
		CIDetector detector;
		HttpClient client;
		Task timer;

		AVSpeechSynthesizer speech;

		bool isNotSpeacking = true;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			await AuthorizeCameraUse ();
			SetupLiveCameraStream ();
			detector = CIDetector.CreateFaceDetector(null, true);
			client = new HttpClient();
			client.BaseAddress = new Uri("http://localhost:5000/api/");

			timer = new Task(async () =>
			{
				while (true)
				{
					await Task.Delay(500);
					var result = await CheckCurrentFrame();
					if (result)
					{
						await Task.Delay(2000);
					}
				}
			});
			timer.Start();
			speech = new AVSpeechSynthesizer();
			speech.DidStartSpeechUtterance += (sender, e) => isNotSpeacking = false;
			speech.DidFinishSpeechUtterance += (sender, e) => isNotSpeacking = true;
			var speechUtterance = new AVSpeechUtterance("Hi Alex! Welcome to Paralect office!")
			{
				Rate = 0.4f,
				Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
				Volume = 1f,
				PitchMultiplier = 1.0f
			};
			speech.SpeakUtterance(speechUtterance);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		async Task<bool> CheckCurrentFrame(){
			var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
			var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

			var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
			var jpegAsByteArray = jpegImageAsNsData.ToArray();

			return true;

			/* var content = new ByteArrayContent(jpegAsByteArray);
			var result = await client.PostAsync("check", content);
			var json = System.Json.JsonValue.Parse(result.Content.ToString());
			var name = json["Name"];
			var access = bool.Parse(json["Access"]);
			if (access && isNotSpeacking)
			{
				var speechUtterance = new AVSpeechUtterance("Hi " + name + "! Welcome to our office!")
				{
					Rate = AVSpeechUtterance.MaximumSpeechRate / 4,
					Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
					Volume = 0.5f,
					PitchMultiplier = 1.0f
				};
				speech.SpeakUtterance(speechUtterance);
			}*/
		}

		async partial void TakePhotoButtonTapped (UIButton sender)
		{
			var videoConnection = stillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
			var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync (videoConnection);

			var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer);
			var jpegAsByteArray = jpegImageAsNsData.ToArray ();


			var image = CIImage.FromData(jpegImageAsNsData);
			var features = detector.FeaturesInImage(image);
		}

		internal class CheckResponse
		{
			public bool Access { get; set; }
			public Guid PersonId { get; set; }
			public double Confidence { get; set; }
			public string Name { get; set; }
		}

		public AVCaptureDevice GetCameraForOrientation (AVCaptureDevicePosition orientation)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);

			foreach (var device in devices) {
				if (device.Position == orientation) {
					return device;
				}
			}

			return null;
		}

		async Task AuthorizeCameraUse ()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized) {
				await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
			}
		}

		public void SetupLiveCameraStream ()
		{
			captureSession = new AVCaptureSession ();

			var viewLayer = liveCameraStream.Layer;
			videoPreviewLayer = new AVCaptureVideoPreviewLayer (captureSession) {
				Frame = this.View.Frame
			};
			liveCameraStream.Layer.AddSublayer (videoPreviewLayer);

			var captureDevice = GetCameraForOrientation(AVCaptureDevicePosition.Front);
			ConfigureCameraForDevice (captureDevice);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice (captureDevice);
			captureSession.AddInput (captureDeviceInput);

			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
			stillImageOutput = new AVCaptureStillImageOutput () {
				OutputSettings = new NSDictionary ()
			};

			captureSession.AddOutput (stillImageOutput);
			captureSession.StartRunning ();
		}

		void ConfigureCameraForDevice (AVCaptureDevice device)
		{
			var error = new NSError ();
			if (device.IsFocusModeSupported (AVCaptureFocusMode.ContinuousAutoFocus)) {
				device.LockForConfiguration (out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration ();
			} else if (device.IsExposureModeSupported (AVCaptureExposureMode.ContinuousAutoExposure)) {
				device.LockForConfiguration (out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration ();
			} else if (device.IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance)) {
				device.LockForConfiguration (out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration ();
			}
		}
	}
}