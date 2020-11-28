using Android.Content;
using Android.Graphics.Drawables;
using Kdd.UIKit.Forms.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Kdd.UIKit.Forms.Droid
{
    //HACK Xamarin forms made these extensions internal for some reason. No other way for now to implement renderers logic properly without this.
    //Remove it if they will make these extensions public. 
    //See: https://github.com/xamarin/Xamarin.Forms/blob/52a2581dfe4bd52f8bcabfb714c63e784735fee3/Xamarin.Forms.Platform.Android/ResourceManager.cs
    public static class FormsResourceManager
    {
        public static Task ApplyDrawableAsync(IVisualElementRenderer renderer,
                                              BindableProperty imageSourceProperty,
                                              Context context,
                                              Action<Drawable> onSet,
                                              Action<bool> onLoading = null,
                                              CancellationToken cancellationToken = default)
        {
            var type = typeof(ResourceManager);
            var parameters = type.ProduceParameters((typeof(IVisualElementRenderer), renderer),
                                                    (typeof(BindableProperty), imageSourceProperty),
                                                    (typeof(Context), context),
                                                    (typeof(Action<Drawable>), onSet),
                                                    (typeof(Action<bool>), onLoading),
                                                    (typeof(CancellationToken), cancellationToken));

            return type.InvokeNonPublicStaticMethod<Task>(parameters);
        }
    }
}
