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
	public partial class CreateProductForm : Form
	{
		public CreateProductForm()
		{
			InitializeComponent();
			InitForm();
		}
		private void InitForm()
		{
			categoryIdComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
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
		private void saveButton_Click(object sender, EventArgs e)
		{
			int categoryId = ((productCategoryVM)categoryIdComboBox.SelectedItem).Id;
			string productName = productNameTextBox.Text;
			int listprice = listPriceTextBox.Text.ToInt(-1);

			ProductVM model = new ProductVM()
			{
				Id = categoryId,
				ProductName = productName,
				ListPrice = listprice
			};

			string errormsg = string.Empty;
			if (string.IsNullOrEmpty(model.ProductName)) errormsg += "商品名稱必填\r\n";
			if (listprice<0) errormsg+= "商品價格必須大於等於0\r\n";

			if (string.IsNullOrEmpty(errormsg) == false)
			{
				MessageBox.Show(errormsg);
				return;
			}

			string sql = @"INSERT INTO Products (CategoryId,ProductName,ListPrice)
						values (@CategoryId,@ProductName,@ListPrice)";
			var parameters = new SqlParameterBuilder().AddInt("CategoryId",model.Id)
													  .AddNVarchar("ProductName", 50, model.ProductName)
													  .AddInt("ListPrice", model.ListPrice)
													  .build();
			new SqlDbHelper("default").ExecuteNonQuery(sql, parameters);
			this.DialogResult = DialogResult.OK;

		}
	
		



	}
}
