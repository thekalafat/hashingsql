using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace hashingsql
{
    public partial class Form2 : Form
    {
        
        public Form2()
        {
            InitializeComponent();
        }

        public string secim;
        string sifrelenmisOy;
        public string sifreliTc2;
        string aday;
        double secmenDecimal;
        double scc1, scc2, scc3;

        int[] sonucBinary;

        class Secmen
        {
            private bool[] oy;//bool tipinde dizi oluşturduk 

            public void setOy(bool[] oy)
            {
                this.oy = oy;
            }

            public bool[] getOy()
            {
                return this.oy;
            }

            public void oyVer(String _secim)
            {
                String secim = _secim; //radio buttondan gelen sonuç

                bool[] dizi = new bool[16];//16 bitlik 0 dolu diziyi oluşturduk

                if (secim == "NAZIM HİKMET")//nazım hikmet secilirse dizinin baştan 4.elemanı 1 yapıyor
                {
                    dizi[3] = true;
                }
                else if (secim == "CEMAL SÜREYA")//cemal süreya secilirse 7.elemanı 1 yap
                {
                    dizi[7] = true;
                }
                else if (secim == "ATİLLA İLHAN")//atilla ilhan secilirse 11.elemanı 1 yap
                {
                    dizi[11] = true;
                }
                else if (secim == "MEHMET AKİF ERSOY")//mehmet akif ersoy secilirse 15.biti 1 yap
                {
                    dizi[15] = true;
                }

                setOy(dizi);

            }
        }
    
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            secim = ((RadioButton)sender).Text;
            aday = secim;
            SHA256 sha = new SHA256CryptoServiceProvider();
            sifrelenmisOy = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(secim)));
           
        }

        private void button1_Click(object sender, EventArgs e) //Oy kullan butonu
        {
            SqlCommand cmd;
            cmd = new SqlCommand();
            SqlConnection con1 = new SqlConnection("Data Source=DESKTOP-HVC54QF;Initial Catalog=user;Integrated Security=True");
            con1.Open();
            cmd.Connection = con1;
            cmd.CommandText = "insert into oylar(sifrelenmisOy,secmenTc) values('"+sifrelenmisOy+"','"+sifreliTc2+"') ";
            cmd.ExecuteNonQuery();
            con1.Close();
            MessageBox.Show("Oy Kullanma işlemi başarılı şekilde gerçekleşti.");
            this.Hide();
            
            Secmen secmen = new Secmen(); //secmen classından yeni bir secmen nesnesi yarattık

            secmen.oyVer(aday);//radiobuttondaki stringi alıp oyver fonksiyonuna gönderiyor
            bool[] secimSonucu = secmen.getOy();//bitini 1 yaptığımız diziyi secimsonucu adlı değişkene attık
            

            if (secimSonucu[3] == true) //eğer dizinin 4.elemanı 1 olursa 2 üzeri 12 yap
            {
                secmenDecimal = Math.Pow(2, 12);
            }
            else if (secimSonucu[7] == true)//eğer dizinin 8.elemanı 1 olursa 2 üzeri 8 yap
            {
                secmenDecimal = Math.Pow(2, 8);
            }
            else if (secimSonucu[11] == true)//eğer dizinin 12.elemanı 1 olursa 2 üzeri 4 yap
            {
                secmenDecimal = Math.Pow(2, 4);
            }
            else if (secimSonucu[15] == true)//eğer dizinin 16.elemanı 1 olursa 2 üzeri 0 yap
            {
                secmenDecimal = Math.Pow(2, 0);
            }

            Random rastgele = new Random();
            int randomKatsayi = rastgele.Next(1, 13107); //random değer üret

            double secmenCC1 = secmenCC(randomKatsayi, 1, secmenDecimal);//cc1 ürettiğimiz fonksiyon
            double secmenCC2 = secmenCC(randomKatsayi, 2, secmenDecimal);//cc2 ürettiğimiz fonksiyon
            double secmenCC3 = secmenCC(randomKatsayi, 3, secmenDecimal);//cc3 ürettiğimiz fonksiyon

            insertSonuc(secmenCC1, secmenCC2, secmenCC3);//cc1,cc2 ve cc3 değerlerini veritabanına atan fonksiyon

           
        }

        public void insertSonuc(double secmenCC1,double secmenCC2,double secmenCC3) //bulunan cc1,cc2 ve cc3 bilgilerini veritabanına atıyor
        {
            SqlCommand cmd;
            cmd = new SqlCommand();
            SqlConnection con1 = new SqlConnection("Data Source=DESKTOP-HVC54QF;Initial Catalog=user;Integrated Security=True");
            con1.Open();
            cmd.Connection = con1;
            cmd.CommandText = "insert into sonuc(cc1,cc2,cc3) values('" + secmenCC1 + "','" + secmenCC2 + "','" + secmenCC3 + "') ";
            cmd.ExecuteNonQuery();
            con1.Close();
        }
        //secmenCC polinomunu oluşturmak için yarattığımız fonksiyon 1 adet random sayı alıyor,i alıyor,ve secmendecimal alıyor
        public double secmenCC(double randomKatsayi,int i,double secmenDecimal) 
        {
            return ((randomKatsayi * i) + secmenDecimal) % 65535;
        }

       
    }
}
