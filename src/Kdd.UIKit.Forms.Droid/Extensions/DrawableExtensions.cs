using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Graphics.Drawable;
using Xamarin.Forms.Platform.Android;
using FormsColor = Xamarin.Forms.Color;

namespace Kdd.UIKit.Forms.Droid.Extensions
{
    public static class DrawableExtensions
	{
		public static ColorFilter GetColorFilter(this Drawable drawable)
		{
			if (drawable is null)
			{
				return null;
			}

			return DrawableCompat.GetColorFilter(drawable);
		}

		public static void SetColorFilter(this Drawable drawable, FormsColor color, ColorFilter defaultColorFilter, BlendMode mode)
		{
			if (drawable is null)
			{
				return;
			}

			if (color.IsDefault && defaultColorFilter is null)
			{
				DrawableCompat.ClearColorFilter(drawable);
				return;
			}

			if (color.IsDefault)
			{
				drawable.SetColorFilter(defaultColorFilter);
				return;
			}

			var colorFilter = new BlendModeColorFilter(color.ToAndroid(), mode);
			drawable.SetColorFilter(colorFilter);
		}
	}
}