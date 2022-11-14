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
	public partial class ProductCategoriesForm : Form
	{
		DataTable categories;
		public ProductCategoriesForm()
		{
			InitializeComponent();
			DisplayProductCategories();
		}

		private void DisplayProductCategories()
		{
			string sql = "SELECT * FROM ProductCategories ORDER BY DisplayOrder";
			categories = new SqlDbHelper("default").Select(sql, null);

			BindData(categories);
		}
		private void BindData(DataTable model)
		{
			dataGridView1.DataSource = model ;
		}

		private void AddNewButton_Click(object sender, EventArgs e)
		{
			var frm = new CreateProductCategoryForm();
			DialogResult result = frm.ShowDialog();

			if (result == DialogResult.OK)
			{
				DisplayProductCategories();
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int rowIndex = e.RowIndex;

			if (rowIndex < 0) return;

			DataRow row = this.categories.Rows[rowIndex];
			int id = row.Field<int>("id");

			var frm = new EditProductCategoryForm(id);
			DialogResult result = frm.ShowDialog();

			if (result == DialogResult.OK)
			{
				DisplayProductCategories();
			}
		}
	}
}
