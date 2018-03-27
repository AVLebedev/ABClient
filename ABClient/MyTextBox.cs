using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ABClient
{
   public class MyTextBox : TextBox
    {
       /// <summary>
       /// Текст, отображаемый внутри незаполненного текстбокса
       /// </summary>
       public string PlaceHolder
       {
           get
           {
               return placeHolder;
           }
           set
           {
               placeHolder = value;
               this.Text = placeHolder;
           }
       }
        private string placeHolder;

        public MyTextBox()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            GotFocus += MyTextBox_GotFocus;
            LostFocus += MyTextBox_LostFocus;
        }

        private void MyTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Text) || String.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = placeHolder;
            }
        }

        void MyTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Text == placeHolder)
            {
                this.Text = "";
            }
        }
    }
}
