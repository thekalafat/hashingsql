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
            if (char.IsLetter(e.KeyChar))//rakam giriliyor metin girilemiyor
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
                    frm.Show();
                    break;
                    

                }
            }







            //List<Ogrenci> ogrenciler = new List<Ogrenci>();
            //int sayac = 0;
            //while (sayac == ogrenciler.Count)
            //{
            //    Ogrenci ogr = new Ogrenci();
            //    while (rdr.Read())
            //    {

            //        ogr.tc = rdr["tc"].ToString();
            //        foreach (var kullanıcı in ogrenciler)
            //        {
            //            if (sifrelenmisVeri2 == ogr.tc)
            //            {
            //                MessageBox.Show("Bu kişi kayıtlı");

            //            }
            //            rdr.NextResult();

            //        }
            //        sayac++;

            //        break;
            //    }
            //}
            con.Close();
        }
    }

   
}
