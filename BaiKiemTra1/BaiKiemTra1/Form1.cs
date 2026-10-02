using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaiKiemTra1
{
    public partial class Form1 : Form
    {
        // Sử dụng BindingList và BindingSource để tự động hóa Data Binding
        private BindingList<Product> _productList;
        private BindingSource _bindingSource;

        // Đã sửa tên hàm khởi tạo thành Form1 để khắc phục lỗi CS1520
        public Form1()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // 1. Khởi tạo dữ liệu
            _productList = new BindingList<Product>();
            _bindingSource = new BindingSource { DataSource = _productList };

            // 2. Cấu hình ComboBox Danh mục
            var categories = new[]
            {
                new { Id = "DT", Name = "Điện thoại" },
                new { Id = "LT", Name = "Laptop" },
                new { Id = "PK", Name = "Phụ kiện" }
            };
            cboCategory.DataSource = categories;
            cboCategory.DisplayMember = "Name";
            cboCategory.ValueMember = "Id";

            // 3. Cấu hình DataGridView
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.DataSource = _bindingSource;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Tự định nghĩa các cột
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Mã SP", Name = "colId" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Tên SP", Name = "colName" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Danh Mục", Name = "colCategory" });

            var priceCol = new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Đơn Giá", Name = "colPrice" };
            priceCol.DefaultCellStyle.Format = "N0"; // Format N0 cho tiền tệ
            dgvProducts.Columns.Add(priceCol);

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Số Lượng", Name = "colQuantity" });

            UpdateStatus();
        }

        // --- VALIDATION DỮ LIỆU ---
        private bool ValidateInput()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                errorProvider.SetError(txtProductId, "Mã SP không được để trống!");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                errorProvider.SetError(txtProductName, "Tên SP không được để trống!");
                isValid = false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal price) || price <= 0)
            {
                errorProvider.SetError(txtUnitPrice, "Đơn giá phải lớn hơn 0!");
                isValid = false;
            }

            if (!int.TryParse(txtQuantity.Text, out int qty) || qty < 0)
            {
                errorProvider.SetError(txtQuantity, "Số lượng không hợp lệ!");
                isValid = false;
            }

            return isValid;
        }

        // --- HÀNH ĐỘNG: THÊM MỚI ---
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            // Kiểm tra trùng Mã SP
            if (_productList.Any(p => p.Id == txtProductId.Text))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newProduct = new Product
            {
                Id = txtProductId.Text,
                Name = txtProductName.Text,
                CategoryId = cboCategory.SelectedValue.ToString(),
                CategoryName = cboCategory.Text,
                UnitPrice = decimal.Parse(txtUnitPrice.Text),
                Quantity = int.Parse(txtQuantity.Text),
                ImagePath = picAvatar.ImageLocation
            };

            _productList.Add(newProduct);
            UpdateStatus();
            ClearInput();
        }

        // --- HÀNH ĐỘNG: CẬP NHẬT ---
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            if (!ValidateInput()) return;

            var product = (Product)dgvProducts.CurrentRow.DataBoundItem;

            // Chỉ cập nhật nếu không đổi ID, hoặc ID mới chưa tồn tại
            if (product.Id != txtProductId.Text && _productList.Any(p => p.Id == txtProductId.Text))
            {
                MessageBox.Show("Mã sản phẩm mới đã tồn tại, không thể cập nhật!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            product.Id = txtProductId.Text;
            product.Name = txtProductName.Text;
            product.CategoryId = cboCategory.SelectedValue.ToString();
            product.CategoryName = cboCategory.Text;
            product.UnitPrice = decimal.Parse(txtUnitPrice.Text);
            product.Quantity = int.Parse(txtQuantity.Text);
            product.ImagePath = picAvatar.ImageLocation;

            // Refresh grid
            _bindingSource.ResetBindings(false);
        }

        // --- HÀNH ĐỘNG: XÓA ---
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var product = (Product)dgvProducts.CurrentRow.DataBoundItem;
                _productList.Remove(product);
                UpdateStatus();
                ClearInput();
            }
        }

        // --- SỰ KIỆN: CLICK CHỌN ROW ĐỂ NẠP DỮ LIỆU LÊN FORM ---
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow != null && dgvProducts.CurrentRow.DataBoundItem is Product p)
            {
                txtProductId.Text = p.Id;
                txtProductName.Text = p.Name;
                cboCategory.SelectedValue = p.CategoryId;
                txtUnitPrice.Text = p.UnitPrice.ToString("0");
                txtQuantity.Text = p.Quantity.ToString();

                if (!string.IsNullOrEmpty(p.ImagePath) && File.Exists(p.ImagePath))
                {
                    picAvatar.ImageLocation = p.ImagePath;
                }
                else
                {
                    picAvatar.Image = null;
                }
            }
        }

        // --- HÀNH ĐỘNG: CHỌN ẢNH ---
        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Chọn ảnh sản phẩm";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picAvatar.ImageLocation = ofd.FileName;
                    picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        // --- TÌM KIẾM THEO THỜI GIAN THỰC (LIVE SEARCH) ---
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                _bindingSource.DataSource = _productList;
            }
            else
            {
                var filteredList = _productList.Where(p => p.Name.ToLower().Contains(keyword)).ToList();
                _bindingSource.DataSource = new BindingList<Product>(filteredList);
            }
            UpdateStatus();
        }

        // --- EXPORT CSV (Menu Item) ---
        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_productList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV file (*.csv)|*.csv";
                sfd.FileName = "TechMart_Products.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Mã SP,Tên SP,Danh Mục,Đơn Giá,Số Lượng");

                        foreach (var item in _productList)
                        {
                            string safeName = item.Name.Replace(",", ";");
                            sb.AppendLine($"{item.Id},{safeName},{item.CategoryName},{item.UnitPrice},{item.Quantity}");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file CSV thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // --- TIỆN ÍCH ---
        private void UpdateStatus()
        {
            if (lblStatus != null && _bindingSource != null && _bindingSource.DataSource != null)
            {
                lblStatus.Text = $"Tổng số sản phẩm: {((BindingList<Product>)_bindingSource.DataSource).Count}";
            }
        }

        private void ClearInput()
        {
            txtProductId.Clear();
            txtProductName.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            picAvatar.Image = null;
            errorProvider.Clear();
            txtProductId.Focus();
        }

        // Thoát ứng dụng
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    // --- LỚP DỮ LIỆU SẢN PHẨM ---
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
    }
}