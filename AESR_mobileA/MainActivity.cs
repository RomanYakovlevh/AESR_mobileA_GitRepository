using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;

namespace AESR_mobileA
{
    //Приложение: Картинка -> Юзер говорит слово на картинке -> Приложение подтвердает/опровергает верность -> Следующая картинка
    //      Картинки кончились -> Юзер заходит в менюшку -> Загружает новые картинки и подписывает их
    //Что нужно для реализации вышесказанного: Холдер картинки в интерфейсе; Служба(или что то альтернативное, главное параллельное) 
    //      которая будет записывать речь и прогонять ее через рекогнайзер, получая текст(Android.Speech.RecognizerIntent); 
    //      Способ передавать из службы новую картинку(pictureBox1.Invoke); Автоматический говорильник, который будет проговаривать верность/неверность ответа(Android.Speech.TextToSpeech);
    //      Всплывающая менюшка, через которую можно будет загрузить новые картинки, т.е. доступ к галерее и новый макет
    //Мост импортант!!! Надо научиться тестить это

    [Activity(Label = "@string/start_screen_title", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        Button startButton;
        Button settingsButton;
        TextView textView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            startButton = this.FindViewById<Button>(Resource.Id.button1);
            settingsButton = this.FindViewById<Button>(Resource.Id.button2);
            textView = this.FindViewById<TextView>(Resource.Id.textView1);

            startButton.Click += StartButton_Click;
            settingsButton.Click += SettingsButton_Click;
        }

        private void StartButton_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void SettingsButton_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStart()
        {
            base.OnStart();

            //ImageView image = this.FindViewById<ImageView>(Resource.Id.imageView1);
            //image.SetImageDrawable(this.GetDrawable(Resource.Drawable.DuckDuckMEOW));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}