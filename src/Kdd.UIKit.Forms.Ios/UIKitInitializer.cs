using Kdd.UIKit.Forms.Ios.Renderers;

namespace Kdd.UIKit.Forms.Ios
{
    public static class UIKitInitializer
    {
        private static SafeSliderRenderer _safeSliderRenderer;

        /// <summary>
        /// Initializer ensures that linker will not shrink the renderers
        /// </summary>
        public static void Init()
        {
            _safeSliderRenderer = default;
        }
    }
}