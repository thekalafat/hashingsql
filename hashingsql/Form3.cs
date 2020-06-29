using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography; //sha algoritması için gerekli
using System.Data.SqlClient; //sql bağlantısı için gerekli

namespace hashingsql
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        SqlConnection con;
        SqlCommand cmd;
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

            //sha hashing algoritmasını kullanmak için gerekli fonksiyonlar
            SHA256 sha = new SHA256CryptoServiceProvider();
            string SonucH1 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(aday1Sonuc))); //1.Sonucu hashle
            string SonucH2 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(aday2Sonuc))); //2.Sonucu hashle
            string SonucH3 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(aday3Sonuc))); //3.Sonucu hashle
            string SonucH4 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(aday4Sonuc))); //4.Sonucu hashle

            cmd = new SqlCommand();
            con = new SqlConnection("Data Source=DESKTOP-HVC54QF;Initial Catalog=user;Integrated Security=True");
            con.Open();
            cmd.Connection = con;
            // cmd.CommandText = " insert into adaylar(aday_ismi,oy_sayisi) values ('" + "NAZIM HİKMET" + "','" + SonucH1 + "')" + " insert into adaylar(aday_ismi,oy_sayisi) values ('" + "CEMAL SÜREYA" + "','" + SonucH2 + "')" +" insert into adaylar(aday_ismi,oy_sayisi) values ('" + "ATİLLA İLHAN" + "','" + SonucH3 + "')" +" insert into adaylar(aday_ismi,oy_sayisi) values ('" + "MEHMET AKİF ERSOY" + "','" + SonucH4 + "')";

            cmd.CommandText = "update adaylar set oy_sayisi='" + SonucH1 + "' where aday_ismi = 'NAZIM HİKMET' " + "update adaylar set oy_sayisi='" + SonucH2 + "' where aday_ismi = 'CEMAL SÜREYA' " + " update adaylar set oy_sayisi='" + SonucH3 + "' where aday_ismi = 'ATİLLA İLHAN' " + " update adaylar set oy_sayisi='" + SonucH4 + "' where aday_ismi = 'MEHMET AKİF ERSOY'"; // şifrelenmiş yeni oy sayısı veri tabanından güncelleniyor.
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
