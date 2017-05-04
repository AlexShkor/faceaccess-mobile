using System;
using System.Threading.Tasks;
using CoreImage;
using System.Collections.Generic;

namespace Camera
{
    public class PersonObserver
    {
		private Task timer;

		private FaceAccessClient client;
		private SpeechService speechService;

		private CIDetector detector;
		private CameraService _cameraService;

		private Dictionary<string, DateTime> persons = new Dictionary<string, DateTime>();

        public PersonObserver(CameraService cameraService)
        {
            _cameraService = cameraService;
			detector = CIDetector.CreateFaceDetector(null, true);
			client = new FaceAccessClient();
			speechService = new SpeechService();
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
        }

        public void Start()
        {
            timer.Start();
        }

        private async Task<bool> CheckCurrentFrame(){

            var jpegAsByteArray = await _cameraService.GetCurrentFrame();

            var data = await client.CheckAssess(jpegAsByteArray);
            if (data.Access)
            {
                speechService.WelcomePerson(data.Name);
            }
            return data.Access;
        }
    }
}
