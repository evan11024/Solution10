using ISpan.EStore.Models.ViewModels;
using ISpan.Utility;
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

namespace ISpan.EStore
{public partial class ProductsForm : Form
	{
		private ProductIndexVM[] products = null;
		public ProductsForm()
		{
			InitializeComponent();
			InitForm();
			DisplayProducts();
		}

	

		private void DisplayProducts()
		{
			string sql = @"SELECT P.Id ,P.ProductName,P.ListPrice,C.CategoryName 
                           FROM Products P 
                           INNER JOIN ProductCategories C ON P.CategoryId = C.Id";

			SqlParameter[] parameters = new SqlParameter[] { };
            int categoryId = ((productCategoryVM)categoryComboBox.SelectedItem).Id;

			if (categoryId > 0)
			{
				sql += " WHERE categoryId = @CategoryId";
				parameters = new SqlParameterBuilder().AddInt("CategoryId", categoryId).build();
			}

				sql += " ORDER BY C.DisplayOrder , P.ProductName";
				var dbHelper = new SqlDbHelper("default");
			
			products = dbHelper.Select(sql, parameters)
					   .AsEnumerable()
					   .Select(row => ParseToIndexVM(row))
					   .ToArray();
			BindData(products);
		}

		private void BindData(ProductIndexVM[] data)
		{
			dataGridView1.DataSource = data;
		}

		private ProductIndexVM ParseToIndexVM(DataRow row)
		{
			return new ProductIndexVM
			{
				Id = row.Field<int>("Id"),
				CategoryName = row.Field<string>("CategoryName"),
				ProductName = row.Field<string>("ProductName"),
				ListPrice = row.Field<int>("ListPrice")
			};	
		}
		private void InitForm()
		{
			categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			string sql = @"SELECT * 
                           FROM ProductCategories 
                           ORDER BY DisplayOrder";

			var dbHelper = new SqlDbHelper("default");
			List<productCategoryVM> categories = dbHelper.Select(sql, null)
					   .AsEnumerable()
					   .Select(row => TocategoryVM(row))
					   .Prepend(new productCategoryVM {Id = 0,CategoryName = String.Empty })
					   .ToList();
			this.categoryComboBox.DataSource = categories;
		}

		private productCategoryVM TocategoryVM(DataRow row)
		{
			return new productCategoryVM
			{
				Id = row.Field<int>("Id"),
				CategoryName = row.Field<string>("CategoryName"),
				DisplayOrder = row.Field<int>("DisplayOrder"),
			};
		}

		private void addnewButton_Click(object sender, EventArgs e)
		{
			DisplayProducts();
		}
		private void AddNewButton_Click(object sender, EventArgs e)
		{
			var frm = new CreateProductForm();
			DialogResult result = frm.ShowDialog();

			if (result == DialogResult.OK)
			{
				DisplayProducts();
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int rowIndex = e.RowIndex;

			if (rowIndex < 0) return;

			ProductIndexVM row = this.products[rowIndex];
			int id = row.Id;

			var frm = new EditProductForm(id);

			DialogResult result = frm.ShowDialog();
			if (result == DialogResult.OK)
			{
				DisplayProducts();
			}
		}
	}
}


