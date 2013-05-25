using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System;
using System.Windows.Navigation;
using System.Diagnostics;

namespace SimpleTODO.SimpleTask.Data
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {

        public CheckBox chkbox;
        public CheckBox starcheck;
        //public Button removeme;

        private TextDecoration td = new TextDecoration();
        private ColorTasks ColorID;

        public TaskControl()
        {
            InitializeComponent();
            chkbox = checkBox_Status;
            starcheck = checkBox_Star;
            //removeme = this.Remove;
        }

        
        /// <summary>
        /// set or get the background color
        /// </summary>
        public ColorTasks colorID
        {
            get 
            {
                return ColorID;
            }
            set 
            {
                ColorID = value;
                if(ColorID == ColorTasks.Black)
				{
                    border.Background = (Brush)this.FindResource("brush_Black");
					this.textBlock.Foreground = Brushes.White;
				}
                else if(ColorID == ColorTasks.Blue)
                    border.Background = (Brush)this.FindResource("brush_DoneGreen");
                else if(ColorID == ColorTasks.Silver)
                    border.Background = (Brush)this.FindResource("brush_Silver");
				else if(ColorID == ColorTasks.DoneGreen)
				{
                    border.Background = (Brush)this.FindResource("brush_Blue");
					this.textBlock.Foreground = Brushes.Black;
				}
				else if(ColorID == ColorTasks.Brown)
				{
                    border.Background = (Brush)this.FindResource("brush_Brown");
					this.textBlock.Foreground = Brushes.White;
				}
            }
        }
        /// <summary>
        /// Set or Get if current task DONE .
        /// </summary>
        public bool? isCheck_Done 
        {
            get 
            {
                return checkBox_Status.IsChecked;
            }
            set
            {
                checkBox_Status.IsChecked = value;
                // if the task is done then do the visual effects
                if (checkBox_Status.IsChecked == true)
                {
                    //this.Opacity = 0.39;
                    td.Location = TextDecorationLocation.Strikethrough;
                    textBlock.TextDecorations.Add(td);
					border.Background = (Brush)this.FindResource("brush_DoneGreen");
                }
                else
                {
                    //this.Opacity = 1.0;
                    textBlock.TextDecorations.Remove(td);
                }
            }
        }
        /// <summary>
        /// Set or Get task STAR .
        /// </summary>
        public bool? isCheck_Star
        {
            get
            {
                return checkBox_Star.IsChecked;
            }
            set
            {
                checkBox_Star.IsChecked = value;
            }
        }
        /// <summary>
        /// Set or Get the task Text .
        /// </summary>
        public string taskText
        {
            get
            {
                return textBlock.Text;
                //return textBlock.ToString();
            }
            set
            {
                textBlock.Text = value;
                ConvertToHyperlinkedText(textBlock.Text);
                //textBlock.AppendText(value);
                
            }
        }

        // needed to make visual changes when isCheck_Done used
        public void checkBox_Status_Click(object sender, RoutedEventArgs e)
        {
            isCheck_Done = checkBox_Status.IsChecked;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                textBox1.Visibility = System.Windows.Visibility.Hidden;
                if(textBox1.Text != "")
                    this.taskText = textBox1.Text;
                textBox1.IsHitTestVisible = false;
            }
        }

        private void menu_edittxt_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Visibility = System.Windows.Visibility.Visible;
            textBox1.IsHitTestVisible = true;
            textBox1.Text = this.taskText;
            textBox1.SelectAll();
            textBox1.Focusable = true;
            textBox1.Focus();
        }

        private void menu_blackColor_Click(object sender, RoutedEventArgs e)
        {            
            colorID = ColorTasks.Black;
        }

        private void menu_blueColor_Click(object sender, RoutedEventArgs e)
        {
           // border.Background = (Brush)this.FindResource("brush_Blue");
            colorID = ColorTasks.Blue;
        }

        private void menu_whiteColor_Click(object sender, RoutedEventArgs e)
        {
            //border.Background = (Brush)this.FindResource("brush_Silver");
            colorID = ColorTasks.Silver;
        }

		private void menu_yellowColor_Click(object sender, RoutedEventArgs e)
        {
            //border.Background = (Brush)this.FindResource("brush_Silver");
            colorID = ColorTasks.DoneGreen;
        }
		//menu_BrColor_Click
        private void menu_BrColor_Click(object sender, RoutedEventArgs e)
        {
            //border.Background = (Brush)this.FindResource("brush_Silver");
            colorID = ColorTasks.Brown;
        }

        private void ConvertToHyperlinkedText(string GetText)
        {

            Regex Regex = new Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            //Paragraph para = new Paragraph();
            Hyperlink hyp = new Hyperlink();

            foreach (Match Match in Regex.Matches(GetText))
            {
                string link = Match.Value;
                int link_start = Match.Index;
                int link_end = Match.Index + link.Length;

                //para.Inlines.Add(Strings.Mid(GetText, 0, link_start - 1));

                Hyperlink hyperLink = new Hyperlink { NavigateUri = new Uri(link) };
                hyperLink.Inlines.Add(link);
                hyperLink.NavigateUri = new Uri(link);

                hyperLink.RequestNavigate += new RequestNavigateEventHandler(delegate(object sender, RequestNavigateEventArgs e)
                {
                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    e.Handled = true;
                });
                hyperlinkxaml = hyperLink;
            }


            //try
            //{
            //    if (hyp != null)
            //        textBlock.Text.Replace(GetText, hyp.NavigateUri.ToString());
            //}
            //catch { }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Remove.Visibility = System.Windows.Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Remove.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
