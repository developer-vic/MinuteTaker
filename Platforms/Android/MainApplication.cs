using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;
using CommunityToolkit.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace MinuteTaker
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        { 
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            var samsungProvider = new SamsungBadgeProvider();
            BadgeFactory.SetBadgeProvider("com.sec.android.app.launcher", samsungProvider);
            BadgeFactory.SetBadgeProvider("com.sec.android.app.twlauncher", samsungProvider);
        }
        
        protected override MauiApp CreateMauiApp()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CustomEntry", (hander, view) =>
            {
                hander.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            });
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("CustomEditor", (hander, view) =>
            {
                hander.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            });
            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("CustomPicker", (hander, view) =>
            {
                hander.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            });
            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("CustomDatePicker", (hander, view) =>
            {
                hander.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            });
            Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("CustomTimePicker", (hander, view) =>
            {
                hander.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            }); 
            return MauiProgram.CreateMauiApp();
        }

    }
}
