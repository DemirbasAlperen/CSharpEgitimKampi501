using CSharpEgitimKampi501.Dtos;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpEgitimKampi501
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection connection = new SqlConnection("Server=ALPEREN\\MSSQL2022;initial Catalog=EgitimKampi501Db;integrated security=true");    // Sql bağlantısı yapıldı
        private async void btnList_Click(object sender, EventArgs e)    // aşağıda QueryAsync kullandığım için buraya da async eklerim. Ayrıca bu metot asenkron olduğu için aşağıda da await keyword unu kullanırım.
        {
            string query = "Select * From TblProduct";  // Dapper işleminde önce query tanımlanıp içine sql sorgudu yazdık.   
            var values = await connection.QueryAsync<ResultProductDto>(query);   //  QueryAsync ; Dapper da verileri listelemek için kullanılan bir metot  // ResultProductDto bu sınıf query den gelen sorgu ile map(mapping) lenecek
            dataGridView1.DataSource = values;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string query = "insert into TblProduct (ProductName,ProductStock,ProductPrice,ProductCategory) values (@productName,@productStock,@productPrice,@productCategory)";   // Ekleme için Sql sorgusu yazdım. Bu sordu içinde eklenecek tablo ismi, tablonun sütun isimleri ve bu eklenecek değerleri nereden alacağını yazdık.
            var parameters = new DynamicParameters();   // Dapper da parametre oluşturma, burada yukarıdaki sorgu içinde bulunan propertylere bir atama yapacağımı bildirmiş oluyorum.
            parameters.Add("@productName", txtProductName.Text);   // Propertlerimi bu şekilde atadım.
            parameters.Add("@productStock", txtProductStock.Text);
            parameters.Add("@productPrice", txtProductPrice.Text);
            parameters.Add("@productCategory", txtProductCategory.Text);
            await connection.ExecuteAsync(query, parameters);   // connection(veritabanına) a ExecuteAsync kullanarak query yi parameters den gelen değerler ile veri tabanına yansıtmam gerekecek.
            MessageBox.Show("Yeni Kitap Ekleme İşlemi Başarılı");
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            string query = "Delete From TblProduct Where ProductId=@productId";   // Silme işlemi için gerekli olan sql sorgumu yazdım. Silme işlemini id üzerinden yapacağım
            var parameters = new DynamicParameters();   // Dapper da parametre oluşturma işlemi
            parameters.Add("@productId", txtProductId.Text);   // Silmek için id parametremi atadım
            await connection.ExecuteAsync(query, parameters);  
            MessageBox.Show("Kitap Silme İşlemi Başarılı");
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            string query = "Update TblProduct Set ProductName=@productName,ProductPrice=@productPrice,ProductStock=@productStock,ProductCategory=@productCategory where ProductId=@productId";   // Güncelleme için gerekli Sql sorgumu yazdım
            var parameters = new DynamicParameters();  // Dapper da parametre oluşturma işlemi
            parameters.Add("@productName", txtProductName.Text);
            parameters.Add("@productStock", txtProductStock.Text);
            parameters.Add("@productPrice", txtProductPrice.Text);
            parameters.Add("@productCategory", txtProductCategory.Text);
            parameters.Add("@productId", txtProductId.Text);
            await connection.ExecuteAsync(query, parameters);
            MessageBox.Show("Kitap Güncelleme İşlemi Başarılı Bir Şekilde Yapıldı", "Güncelleme", MessageBoxButtons.OK, MessageBoxIcon.Information);     // 4 parametre kullanarak bir mesaj gösterdik
        }

        private async void Form1_Load(object sender, EventArgs e)     
        {
            // Toplam Kitap Sayısı Hesaplama:
            string query1 = "Select Count(*) From TblProduct";       // Sql TblProduct tablosunda sayma(Count) sorgusu yazıldı
            var productTotalCount = await connection.QueryFirstOrDefaultAsync<int>(query1);   // int veri tipinde query yi tek bir değer olarak döndürdük ve çalıştı.
            lblTotalProductCount.Text = productTotalCount.ToString();  // productTotalCount u string olarak döndürdük ve ekranda gösterdik.

            // En Pahalı Kitap:
            string query2 = "Select ProductName From TblProduct Where ProductPrice=(Select Max(ProductPrice) From TblProduct)";  // burada Sql sorgusu yazarken fiyatı en yüksek olan sayıyı buldum ve onu hangi kitabın fiyatı ile eşit ise o kitabın ismini göstermesini sağladım
            var maxPriceProductName = await connection.QueryFirstOrDefaultAsync<string>(query2);  // string veri tipinde query yi tek bir değer olarak döndürdük    
            lblMaxPriceProductName.Text = maxPriceProductName.ToString();

            // Kaç Farklı Kategori Var:
            string query3 = "Select Count(Distinct(ProductCategory)) From TblProduct";    // Sql sorgusu: Categori türlerini çekmek için Distinct kullandık ve bu isimlerin kaç adet olduğunu sayması için de count içine atadık 
            var distinctProductCount = await connection.QueryFirstOrDefaultAsync<int>(query3);     // sql sorgusundan gelen değeri int veri türünde döndürüp distinctProductCount içine atadık
            lblDistinctCategoryCount.Text = distinctProductCount.ToString();       // distinctProductCount içine atılan int veriyi string e çevirip lblDistinctCategoryCount label ına atadık.
        }
    }
}
/*
 * btnList kodlarını Toplam kitap sayısı hesaplamak için kullanabiliriz.
 string query = "Select * From TblProduct";     
 var values = await connection.QueryAsync<ResultProductDto>(query); 
 dataGridView1.DataSource = values;
 */