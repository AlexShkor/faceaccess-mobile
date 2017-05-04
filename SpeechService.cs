using System;
using AVFoundation;

namespace Camera
{
    public class SpeechService
    {
        private AVSpeechSynthesizer _speech;
        private bool isSpeacking;

        public SpeechService()
        {

            _speech = new AVSpeechSynthesizer();
            _speech.DidStartSpeechUtterance += (sender, e) => isSpeacking = true;
            _speech.DidFinishSpeechUtterance += (sender, e) => isSpeacking = false;
        }

        public void WelcomePerson(string name)
        {
            if (!isSpeacking)
            {
                var speechUtterance = CreateUtterance(name);
                _speech.SpeakUtterance(speechUtterance);
            }
        }

        private AVSpeechUtterance CreateUtterance(string name)
		{
			return new AVSpeechUtterance("Hi " + name + "! Welcome to Paralect office!")
			{
				Rate = 0.4f,
				Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
				Volume = 1f,
				PitchMultiplier = 1.0f
			};   
        }
    }
}
