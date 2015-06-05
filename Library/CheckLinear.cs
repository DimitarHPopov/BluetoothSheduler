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
using Android.Content.Res;

namespace Library
{
    public class CheckLinear : RelativeLayout
    {
        private bool _checked;
        private TextView _text;
        private string CustomText { get; set; }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;

                if(value)
                {
                    SelectedStyle();
                }
                else
                {
                    UnSelectedStyle();
                }
            }
        }

        private void SetText()
        {
            _text = new TextView(this.Context);
            _text.Text = this.CustomText;

            RelativeLayout.LayoutParams lp = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            lp.AddRule(LayoutRules.CenterInParent, 1);

            _text.LayoutParameters = lp;

            this.AddView(_text);
        }

        public CheckLinear(Context context, Android.Util.IAttributeSet atr) : base(context, atr)
        {
            this.SetPadding(10, 10, 10, 10);
            SetAttrToProp(context, atr);
            SetText();
            UnSelectedStyle();
        }

        private void UnSelectedStyle()
        {
            this.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.rounded_background_unselected));
            this._text.SetTextColor(Resources.GetColor(Resource.Color.blue_color));
        }

        private void SelectedStyle()
        {
            this.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.rounded_background_selected));
            this._text.SetTextColor(Android.Graphics.Color.White);
        }

        private void SetAttrToProp(Context context, Android.Util.IAttributeSet atr)
        {
            int[] styleAttrs = Resource.Styleable.CheckLinear;
            TypedArray a = context.ObtainStyledAttributes(atr, styleAttrs);

            string sid = a.GetString(Resource.Styleable.CheckLinear_checkText);

            this.CustomText = sid;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                this.Checked = !this.Checked;
            }

            return base.OnTouchEvent(e);
        }
    }
}