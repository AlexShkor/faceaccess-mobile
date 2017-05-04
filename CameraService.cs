using System;
using AVFoundation;
using Foundation;
using System.Threading.Tasks;

namespace Camera
{
    public class CameraService
    {

        public AVCaptureSession captureSession { get; private set; }

        private AVCaptureDeviceInput captureDeviceInput;
        private AVCaptureStillImageOutput stillImageOutput;


        public CameraService()
        {
            captureSession = new AVCaptureSession();
        }


        public void SetupLiveCameraStream()
        {
            var captureDevice = GetCameraForOrientation(AVCaptureDevicePosition.Front);
            ConfigureCameraForDevice(captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
            captureSession.AddInput(captureDeviceInput);

            var dictionary = new NSMutableDictionary();
            dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
            stillImageOutput = new AVCaptureStillImageOutput()
            {
                OutputSettings = new NSDictionary()
            };
            captureSession.AddOutput(stillImageOutput);
            captureSession.StartRunning();
        }

        public async Task<byte[]> GetCurrentFrame()
        {
			var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
			var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);
			var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
			var jpegAsByteArray = jpegImageAsNsData.ToArray();
            return jpegAsByteArray;
        }

        private AVCaptureDevice GetCameraForOrientation (AVCaptureDevicePosition orientation)
        {
            var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);

            foreach (var device in devices) {
                if (device.Position == orientation) {
                    return device;
                }
            }

            return null;
        }

        public static async Task AuthorizeCameraUse ()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized) {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
            }
        }



        private void ConfigureCameraForDevice (AVCaptureDevice device)
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
