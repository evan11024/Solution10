using ISpan.EStore.infra.Extensions;
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
	public partial class CreateProductCategoryForm : Form
	{
		public CreateProductCategoryForm()
		{
			InitializeComponent();
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			string categoryName = nameTextBox.Text;
			int displayOrder = displayTextBox.Text.ToInt(-1);

			string sql = @"INSERT INTO ProductCategories (CategoryName , DisplayOrder)
						values (@CategoryName,@DisplayOrder)";
			var parameters = new SqlParameterBuilder().AddNVarchar("CategoryName", 50, categoryName)
													  .AddInt("DisplayOrder", displayOrder)
													  .build();
			new SqlDbHelper("default").ExecuteNonQuery(sql, parameters);
			this.DialogResult = DialogResult.OK;	

		}
	}
}
