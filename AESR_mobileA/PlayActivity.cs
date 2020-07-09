using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Speech;
//using Android.Speech.Tts;
using Android.Views;
using Android.Widget;
using SQLite;

namespace AESR_mobileA
{
    [Activity(Label = "@string/start_screen_title", Theme = "@style/AppTheme")]
    class PlayActivity : Activity/*, TextToSpeech.IOnInitListener*/
    {
        ImageView image;
        Button nextImgBUTTON;
        Button backPlayBUTTON;
        TextView speechresTEXTVIEW;

        //Java.Util.Locale lang;

        //TextToSpeech textToSpeech;

        int clspos;

        List<PicAndWord> picAndWords = new List<PicAndWord>();

        readonly int VOICE = 100, 
            NEEDLANG = 101;

        int increm = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.RequestFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_play);

            clspos = Intent.GetIntExtra("clspos", 0);

            image = FindViewById<ImageView>(Resource.Id.playImage);
            nextImgBUTTON = FindViewById<Button>(Resource.Id.nextImgBUTTON);
            backPlayBUTTON = FindViewById<Button>(Resource.Id.backPlayBUTTON);
            speechresTEXTVIEW = FindViewById<TextView>(Resource.Id.resultPLAY_TEXTVIEW);

            //{
            //    // set up the TextToSpeech object
            //    // third parameter is the speech engine to use
            //    textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");

            //    // set up the langauge spinner
            //    // set the top option to be default
            //    var langAvailable = new List<string> { "Default" };

            //    // our spinner only wants to contain the languages supported by the tts and ignore the rest
            //    var localesAvailable = Java.Util.Locale.GetAvailableLocales().ToList();

            //    foreach (var locale in localesAvailable)
            //    {
            //        LanguageAvailableResult res = textToSpeech.IsLanguageAvailable(locale);

            //        switch (res)
            //        {
            //            case LanguageAvailableResult.Available:
            //                langAvailable.Add(locale.DisplayLanguage);
            //                break;

            //            case LanguageAvailableResult.CountryAvailable:
            //                langAvailable.Add(locale.DisplayLanguage);
            //                break;

            //            case LanguageAvailableResult.CountryVarAvailable:
            //                langAvailable.Add(locale.DisplayLanguage);
            //                break;
            //        }
            //    }

            //    langAvailable = langAvailable.OrderBy(t => t).Distinct().ToList();

            //    // set up the speech to use the default langauge
            //    // if a language is not available, then the default language is used.
            //    lang = Java.Util.Locale.Default;
            //    textToSpeech.SetLanguage(lang);
            //}

            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            string clsname = db.Get<Cluster>(clspos + 1).clusterName;

            var piclist = db.Table<PicAndWord>().ToList();

            picAndWords = piclist.FindAll(x => x.clusterUniqID == clsname);

            nextImgBUTTON.Click += beginPlayBUTTON_Click;
            backPlayBUTTON.Click += BackPlayBUTTON_Click;
        }

        private void BackPlayBUTTON_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void beginPlayBUTTON_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < picAndWords.Count; i++)
            {
                byte[] byteimg = System.IO.File.ReadAllBytes(picAndWords[i].pathToPic);

                image.SetImageBitmap(BitmapFactory.DecodeByteArray(byteimg, 0, byteimg.Length));

                SpeechManager(this, i);
            }
        }

        void SpeechManager(Context context, int curpicpos)
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

            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, /*Application.Context.GetString(Resource.String.messageSpeakNow)*/ "HURRY UP! SPEAK NOW!");
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.English);
            voiceIntent.PutExtra("curpicpos", curpicpos);
            StartActivityForResult(voiceIntent, VOICE);
        }

        //void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        //{
        //    // if we get an error, default to the default language
        //    if (status == OperationResult.Error)
        //        textToSpeech.SetLanguage(Java.Util.Locale.Default);
        //    // if the listener is ok, set the lang
        //    if (status == OperationResult.Success)
        //        textToSpeech.SetLanguage(lang);
        //}

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        int curpicpos = Intent.GetIntExtra("curpicpos", 0);
                        if (picAndWords[curpicpos].word == matches[0])
                        {
                            speechresTEXTVIEW.Text = $"SUCCESS!TRUE:{matches[0]}";
                        }
                        else
                        {
                            speechresTEXTVIEW.Text = $"FAIL... TRUE:{matches[0]}";
                        }
                    }
                    else
                    {
                        speechresTEXTVIEW.Text = "WHAT ARE YOU SAID?";
                    }
                }
                base.OnActivityResult(requestCode, resultVal, data);
            }
            //else if (requestCode == NEEDLANG)
            //{
            //    if (resultVal == Result.Ok)
            //    {
            //        // we need a new language installed

            //        var installTTS = new Intent();

            //        installTTS.SetAction(TextToSpeech.Engine.ActionInstallTtsData);

            //        StartActivity(installTTS);
            //    }
            //}
        }
    }
}