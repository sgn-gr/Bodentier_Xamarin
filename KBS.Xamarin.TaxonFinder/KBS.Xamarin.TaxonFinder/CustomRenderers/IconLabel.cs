using Xamarin.Forms;

namespace KBS.App.TaxonFinder.CustomRenderers
{
    public class IconLabel : Label
    {
        //Must match the exact "Name" of the font which you can get by double clicking the TTF in Windows
        public const string Typeface = "FontAwesome";

        public IconLabel()
        {
            FontFamily = Typeface;    //iOS is happy with this, Android needs a renderer to add ".ttf"
        }

        public static BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(FontIcon?), typeof(IconLabel), null, BindingMode.Default, null, IconChanged);

        private static void IconChanged(BindableObject bindable, object oldValue, object newValue)
        {

            if (newValue != null)
            {
                switch ((FontIcon)newValue)
                {
                    case FontIcon.Mars:
                        ((IconLabel)bindable).Text = "\uf222";
                        break;
                    case FontIcon.Venus:
                        ((IconLabel)bindable).Text = "\uf221";
                        break;
                    case FontIcon.VenusMars:
                        ((IconLabel)bindable).Text = "\uf228";
                        break;
                    case FontIcon.Delete:
                        ((IconLabel)bindable).Text = "\uf014";
                        break;
                    case FontIcon.CheckedCheckbox:
                        ((IconLabel)bindable).Text = "\uf046";
                        break;
                    case FontIcon.UncheckedCheckbox:
                        ((IconLabel)bindable).Text = "\uf096";
                        break;
                    case FontIcon.Check:
                        ((IconLabel)bindable).Text = "\uf00c";
                        break;
                    case FontIcon.Plus:
                        ((IconLabel)bindable).Text = "\uf055";
                        break;
                    case FontIcon.Play:
                        ((IconLabel)bindable).Text = "\uf144";
                        break;
                    case FontIcon.Pause:
                        ((IconLabel)bindable).Text = "\uf28b";
                        break;
                    case FontIcon.Stop:
                        ((IconLabel)bindable).Text = "\uf28d";
                        break;
                    case FontIcon.Record:
                        ((IconLabel)bindable).Text = "\uf111";
                        break;
                    default:
                        ((IconLabel)bindable).Text = "";
                        break;
                }
            }
            else
            {
                ((IconLabel)bindable).Text = "";
            }
        }

        public FontIcon? Icon
        {
            set
            {
                SetValue(IconProperty, value);

            }
            get { return (FontIcon)GetValue(IconProperty); }
        }
        /// <summary>
        /// Get more icons from http://fortawesome.github.io/Font-Awesome/cheatsheet/
        /// Tip: Just copy and past the icon picture here to get the icon
        /// </summary>
        public enum FontIcon
        {
            Unknown,
            Mars,
            Venus,
            VenusMars,
            Delete,
            CheckedCheckbox,
            Check,
            UncheckedCheckbox,
            Plus,
            Play,
            Pause,
            Stop,
            Record
        }
    }
}
