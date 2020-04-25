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

    public partial class Form1 : Form
    {
        double resultAday1;
        double resultAday2;
        double resultAday3;
        double resultAday4;
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-HVC54QF;Initial Catalog=user;Integrated Security=True");
        private void Form1_Load(object sender, EventArgs e) //form yüklendiğinde veritabanındaki var olan verileri gösteriyor
        {
            griddoldur();
        }
        //veritabanı bağlantılarını sağlamak için 
        SqlConnection con;
        SqlDataAdapter da;
        SqlCommand cmd;
        DataSet ds;
        void griddoldur() //datagridview doldurmak için veritabanını kullanıyor
        {
            con = new SqlConnection("Data Source=DESKTOP-HVC54QF;Initial Catalog=user;Integrated Security=True");
            da = new SqlDataAdapter("Select *From ogrenci", con);
            ds = new DataSet();
            con.Open();
            da.Fill(ds,"ogrenci");
            dataGridView1.DataSource = ds.Tables["ogrenci"];
            con.Close();
        }
         
        private void button1_Click(object sender, EventArgs e) //ekleme butonuna basınca veritabanına hashlenmiş tcyi yazıyor
        {
            //sha hashing algoritmasını kullanmak için gerekli fonksiyonlar
            SHA256 sha = new SHA256CryptoServiceProvider();
            string Tc = textBox1.Text; //textboxtaki veriyi tc olarak al
            string sifrelenmisVeri = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(Tc))); //tc olarak aldıgın veriyi hashle
            cmd = new SqlCommand();
            con.Open();
            cmd.Connection = con;           
            cmd.CommandText = " insert into ogrenci(tc,ad) values ('" + sifrelenmisVeri + "','" + textBox2.Text + "')";
            cmd.ExecuteNonQuery();
            con.Close();
            griddoldur();
        }
        
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))//tc yazma yerine rakam giriliyor ama metin girilemiyor
                {
                    e.Handled = true;
                }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int karaktersayisi = textBox1.TextLength;
            if(karaktersayisi == 12)
            {
                MessageBox.Show("TC No sınırına ulaştınız !","Uyarı");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SHA256 sha = new SHA256CryptoServiceProvider();
            string Tc = textBox1.Text; //textboxtaki veriyi tc olarak al
            string sifrelenmisVeri2 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(Tc))); //tc olarak aldıgın veriyi hashle
            con.Open();
            SqlCommand cmd = new SqlCommand("select tc from ogrenci",con);
            SqlDataReader rdr = cmd.ExecuteReader();
            
            List<string> tcler = new List<string>(); //tcler adında bir liste oluştur
            while (rdr.Read())
            {
                tcler.Add(rdr["tc"].ToString());//veritabanındaki tcleri ekle
            }
            foreach (var kullaniciTc in tcler) //veritabanındaki tclerle listedeki tcleri karşılaştır
            {
                if (kullaniciTc == sifrelenmisVeri2)
                {
                    
                    MessageBox.Show("Kayıtlı");
                    Form2 frm = new Form2();
                    frm.sifreliTc2 = sifrelenmisVeri2;
                    frm.Show();
                    break;
                    

                }
            }



            con.Close();
        }

        double scc1, scc2, scc3;
        int[] sonucBinary;

        private void button3_Click(object sender, EventArgs e)
        {
            sccHesapla();

            double result = lagrange();
            if (result != -1)
            {
                sonucBinary = decimalToBinary(result);
                oylarSonuc(sonucBinary);
            }

            Form3 frm = new Form3();
            frm.aday1Sonuc = resultAday1.ToString();
            frm.aday2Sonuc = resultAday2.ToString();
            frm.aday3Sonuc = resultAday3.ToString();
            frm.aday4Sonuc = resultAday4.ToString();
            frm.Show();


        }

        public void sccHesapla() //sccleri hesaplamak için veritabanına attığımız cc1,cc2 ve cc3 bilgilerini toplamamız gerek
        {

            SqlCommand cmd;
            cmd = new SqlCommand();
            SqlConnection con1 = new SqlConnection("Data Source=DESKTOP-HVC54QF;Initial Catalog=user;Integrated Security=True");
            con1.Open();
            cmd.Connection = con1;

            cmd.CommandText = "Select sum(cc1) as toplam from sonuc"; //veritabanındaki cc1 sütununu topluyor
            scc1 = Convert.ToDouble(cmd.ExecuteScalar());

            cmd.CommandText = "Select sum(cc2) as toplam from sonuc"; //veritabanındaki cc2 sütununu topluyor
            scc2 = Convert.ToDouble(cmd.ExecuteScalar());

            cmd.CommandText = "Select sum(cc3) as toplam from sonuc"; //veritabanındaki cc3 sütununu topluyor
            scc3 = Convert.ToDouble(cmd.ExecuteScalar());

            con1.Close();
        }

        public double lagrange() //lagrange interpolasyon yaptığımız fonksiyon
        {
            double islem1 = ((scc1 * 3) / 2);
            double islem2 = ((scc3 * 1) / -2);
            double q01 = (scc1 * (2 / (2 - 1))) + (scc2 * (1 / (1 - 2)));
            double q02 = islem1 + islem2;           
            double q03 = (scc2 * (3 / (3 - 2))) + (scc3 * (2 / (2 - 3)));

            q01 = q01 % 65535;
            q02 = q02 % 65535;
            q03 = q03 % 65535;

            if (q01 == q02 && q01 == q03)
            {
                return q01;
            }
            else
            {
                return -1;
            }

        }

        public int[] decimalToBinary(double q01) //en son ulaştığımız decimal sonucu binary sonuca cevirmek için oluşturduk
        {
            int intQ01 = Convert.ToInt32(q01);

            int[] dizi = new int[16];

            for (int i = 0; intQ01 > 0; i++)
            {
                int sayi = intQ01 % 2;
                dizi[i] = sayi;
                intQ01 = intQ01 / 2;
            }

            int[] resultDizi = new int[16];
            for (int i = 15; i >= 0; i--)
            {
                resultDizi[15 - i] = dizi[i];
            }

            return resultDizi;
        }

        public void oylarSonuc(int[] dizi) //hangi adaya oy verilmişse 1 arttıracak
        {
            int[] aday1 = new int[4];
            int[] aday2 = new int[4];
            int[] aday3 = new int[4];
            int[] aday4 = new int[4];

            int sayac = 0;
            for (int i = 0; i < 16; i++)
            {
                if(i == 4 || i ==8 || i == 12)
                {
                    sayac = 0;
                }
                    if (i <= 3)
                    {
                        aday1[sayac] = dizi[i];
                    }
                    else if (i >= 4 && i <= 7)
                    {
                        aday2[sayac] = dizi[i];
                    }
                    else if (i >= 8 && i <= 11)
                    {
                        aday3[sayac] = dizi[i];
                    }
                    else if (i >= 12 && i <= 15)
                    {
                        aday4[sayac] = dizi[i];
                    }
                sayac++;
            }

             resultAday1 = binaryToDecimal(aday1);
             resultAday2 = binaryToDecimal(aday2);
             resultAday3 = binaryToDecimal(aday3);
             resultAday4 = binaryToDecimal(aday4);




        }

        public double binaryToDecimal(int[] dizi)
        {
            double toplam = 0;
            for(int i = 0; i < 4; i++)
            {
                toplam += (Math.Pow(2, (3 - i)) * dizi[i]);
            }

            return toplam;
        }
    }

   
}
