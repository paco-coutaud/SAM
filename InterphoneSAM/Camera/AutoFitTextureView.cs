using Android.Content;
using Android.Util;
using Android.Views;
using System;

namespace InterphoneSAM
{
    public class AutoFitTextureView : TextureView
    {
        private int RatioWidth = 0;
        private int RatioHeight = 0;

        public AutoFitTextureView (Context context) : base(context, null)
        {

        }

        public AutoFitTextureView(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {

        }
        public AutoFitTextureView(Context context, IAttributeSet attrs , int defStyle) : base(context ,attrs,defStyle)
        {

        }
        public void SetAspectRatio(int width, int height)
        {
            if (width == 0 || height == 0)
            {
                throw new ArgumentException("Size cannot be negative");
            }
            RatioWidth = width;
            RatioHeight = height; ;
            RequestLayout();
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            if (0 == RatioWidth || 0 == RatioHeight)
            {
                SetMeasuredDimension(width, height);
            }
            else
            {
                if (width < (float)height * RatioWidth / (float)RatioHeight)
                {
                    SetMeasuredDimension(width, width * RatioHeight / RatioWidth);
                }
                else
                {
                    SetMeasuredDimension(height * RatioWidth / RatioHeight, height);
                }
            }
        }

    }
}