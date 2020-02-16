using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Speech;
using Android.Speech.Tts;

namespace AESR_mobileA
{
    [Service]
    public class Work : IntentService
    {
        protected override void OnHandleIntent(Android.Content.Intent intent)
        {
            Console.WriteLine("perform some long running work");
            Console.WriteLine("work complete");
        }

        public Work() : base("WorkIntentService")
        {
        }

        public void Recog(Context context)
        {
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                var alert = new AlertDialog.Builder(context);
                alert.SetTitle("You don't seem to have a microphone to record with");
                alert.SetPositiveButton("OK", (sender, e) =>
                {
                    return;
                });
                alert.Show();
            }

            // create the voice intent  
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

            // message and modal dialog  
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");

            // end capturing speech if there is 3 seconds of silence  
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 3000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 3000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 30000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

            // method to specify other languages to be recognised here if desired  
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            ///StartActivityForResult(voiceIntent, 10);
        }
    }
}