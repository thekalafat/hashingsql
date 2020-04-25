using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hashingsql
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        public string aday1Sonuc { get; set; }
        public string aday2Sonuc { get; set; }
        public string aday3Sonuc { get; set; }
        public string aday4Sonuc { get; set; }


        private void Form3_Load(object sender, EventArgs e)//sonuç göster ekranı açıldığında
        {
            label6.Text = aday1Sonuc; //label6'da nazım hikmet'in oy sayısını göster
            label7.Text = aday2Sonuc; //label7'da cemal süreya'nın oy sayısını göster
            label8.Text = aday3Sonuc; //label8'de atilla ilhan'ın oy sayısını göster
            label9.Text = aday4Sonuc; //label9'da mehmet akif ersoy'un  oy sayısını göster
        }
    }
}
