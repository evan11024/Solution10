using ISpan.EStore.infra.Extensions;
using ISpan.EStore.Models.ViewModels;
using ISpan.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISpan.EStore
{
	public partial class EditProductForm : Form
	{
		private int id;

		public EditProductForm(int id)
		{
			InitializeComponent();
			InitForm();
			this.id = id;
			BindData(id);
			
		}
		private void InitForm()
		{
		
			string sql = @"SELECT * 
                           FROM ProductCategories 
                           ORDER BY DisplayOrder";

			var dbHelper = new SqlDbHelper("default");
			List<productCategoryVM> categories = dbHelper.Select(sql, null)
					   .AsEnumerable()
					   .Select(row => TocategoryVM(row))
					   .ToList();
			this.categoryIdComboBox.DataSource = categories;
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

		private void BindData(int id)
		{

			string sql = "SELECT * FROM Products WHERE Id = @Id";
			var parameters = new SqlParameterBuilder().AddInt("Id", id).build();

			DataTable data = new SqlDbHelper("default").Select(sql, parameters);
			if (data.Rows.Count == 0)
			{
				MessageBox.Show("抱歉找不到要編輯的記錄");
				this.DialogResult = DialogResult.OK;
				//this.Close();
				return;
			}

			ProductVM model = ToProductVM(data.Rows[0]);

			categoryIdComboBox.SelectedItem = ((List<productCategoryVM>)categoryIdComboBox.DataSource)
											.FirstOrDefault(x => x.Id == model.CategoryId);
			productNameTextBox.Text = model.ProductName;
			listPriceTextBox.Text = model.ListPrice.ToString();
		}

		private ProductVM ToProductVM(DataRow row)
		{
			return new ProductVM
			{
				Id = row.Field<int>("Id"),
				CategoryId = row.Field<int>("CategoryID"),
				ProductName = row.Field<string>("ProductName"),
				ListPrice = row.Field<int>("ListPrice"),
			};
		}


		private void updateButton_Click(object sender, EventArgs e)
		{
			int categoryId = ((productCategoryVM)categoryIdComboBox.SelectedItem).Id;
			string productName = productNameTextBox.Text;
			int listprice = listPriceTextBox.Text.ToInt(-1);

			ProductVM model = new ProductVM()
			{
				CategoryId = categoryId,
				ProductName = productName,
				ListPrice = listprice
			};

			string errormsg = string.Empty;
			if (string.IsNullOrEmpty(model.ProductName)) errormsg += "商品名稱必填\r\n";
			if (listprice < 0) errormsg += "商品價格必須大於等於0\r\n";

			if (string.IsNullOrEmpty(errormsg) == false)
			{
				MessageBox.Show(errormsg);
				return;
			}
			

			string sql = @"UPDATE Products SET CategoryID = @CategoryID,
						ProductName = @ProductName,
						ListPrice = @ListPrice
						WHERE Id = @Id";

			var parameters = new SqlParameterBuilder().AddInt("CategoryID", model.CategoryId)
													  .AddNVarchar("ProductName", 50, model.ProductName)
													  .AddInt("ListPrice", model.ListPrice)
													  .AddInt("Id", this.id)
													  .build();
			new SqlDbHelper("default").ExecuteNonQuery(sql, parameters);
			this.DialogResult = DialogResult.OK;
		}
		private void deleteButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("您真的要刪除嗎?",
								"刪除紀錄",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			string sql = @"DELETE FROM Products WHERE Id = @Id";

			var parameters = new SqlParameterBuilder().AddInt("Id", this.id)
													  .build();

			new SqlDbHelper("default").ExecuteNonQuery(sql, parameters);
			this.DialogResult = DialogResult.OK;
		}
	}
}
