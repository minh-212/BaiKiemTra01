using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSpeedLogistics
{
    // ==========================================
    // 1. ABSTRACT CLASS PHUONGTIEN
    // ==========================================
    public abstract class PhuongTien
    {
        private string _maPT;
        private string _tenHang;
        private int _namSanXuat;
        private decimal _giaGoc;

        public string MaPT
        {
            get => _maPT;
            set => _maPT = string.IsNullOrWhiteSpace(value) ? "PT000" : value.Trim();
        }

        public string TenHang
        {
            get => _tenHang;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên hãng không được để trống.");
                _tenHang = value.Trim();
            }
        }

        public int NamSanXuat
        {
            get => _namSanXuat;
            set
            {
                int currentYear = DateTime.Now.Year;
                if (value < 1900 || value > currentYear)
                    throw new ArgumentException($"Năm sản xuất phải nằm trong khoảng 1900 và {currentYear}.");
                _namSanXuat = value;
            }
        }

        public decimal GiaGoc
        {
            get => _giaGoc;
            set
            {
                if (value <= 0) throw new ArgumentException("Giá gốc phải lớn hơn 0.");
                _giaGoc = value;
            }
        }

        protected PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
        {
            MaPT = maPT;
            TenHang = tenHang;
            NamSanXuat = namSanXuat;
            GiaGoc = giaGoc;
        }

        public abstract decimal TinhGiaLanBanh();

        public virtual string GetInfo()
        {
            return $"Mã PT: {MaPT,-6} | Hãng: {TenHang,-10} | Năm SX: {NamSanXuat} | Giá gốc: {GiaGoc,12:N0} VNĐ";
        }
    }

    // ==========================================
    // 2. CLASS OTO 
    // ==========================================
    public class OTo : PhuongTien
    {
        private int _soChoNgoi;
        private double _dungTichDongCo;

        public int SoChoNgoi
        {
            get => _soChoNgoi;
            set
            {
                if (value <= 0) throw new ArgumentException("Số chỗ ngồi phải > 0.");
                _soChoNgoi = value;
            }
        }

        public double DungTichDongCo
        {
            get => _dungTichDongCo;
            set
            {
                if (value <= 0) throw new ArgumentException("Dung tích động cơ phải > 0.");
                _dungTichDongCo = value;
            }
        }

        public OTo(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int soChoNgoi, double dungTichDongCo)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            SoChoNgoi = soChoNgoi;
            DungTichDongCo = dungTichDongCo;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (SoChoNgoi <= 9)
                return GiaGoc + (GiaGoc * 0.12m) + (GiaGoc * 0.30m);
            else
                return GiaGoc + (GiaGoc * 0.10m);
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Ô Tô: {SoChoNgoi} chỗ, {DungTichDongCo}L";
        }
    }

    // ==========================================
    // 3. CLASS XEMAY
    // ==========================================
    public class XeMay : PhuongTien
    {
        private int _dungTichXylanh;

        public int DungTichXylanh
        {
            get => _dungTichXylanh;
            set
            {
                if (value <= 0) throw new ArgumentException("Dung tích xi-lanh phải > 0.");
                _dungTichXylanh = value;
            }
        }

        public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (DungTichXylanh < 175)
                return GiaGoc + (GiaGoc * 0.02m);
            else
                return GiaGoc + (GiaGoc * 0.05m);
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Xe Máy: {DungTichXylanh}cc";
        }
    }

    // ==========================================
    // 4. CLASS QUANLYPHUONGTIEN
    // ==========================================
    public class QuanLyPhuongTien
    {
        private readonly List<PhuongTien> _danhSachPT;

        public QuanLyPhuongTien()
        {
            _danhSachPT = new List<PhuongTien>();
        }

        public void AddPhuongTien(PhuongTien pt)
        {
            if (pt != null) _danhSachPT.Add(pt);
        }

        public void DisplayAll()
        {
            if (!_danhSachPT.Any())
            {
                Console.WriteLine("\n[!] Danh sách phương tiện hiện đang trống.");
                return;
            }

            Console.WriteLine("\n" + new string('-', 95));
            foreach (var pt in _danhSachPT)
            {
                Console.WriteLine($"{pt.GetInfo()} | Giá lăn bánh: {pt.TinhGiaLanBanh(),12:N0} VNĐ");
            }
            Console.WriteLine(new string('-', 95));
        }

        public PhuongTien FindMaxGiaLanBanh()
        {
            return _danhSachPT.MaxBy(pt => pt.TinhGiaLanBanh());
        }

        public List<PhuongTien> SearchByName(string keyword)
        {
            return _danhSachPT
                .Where(pt => pt.TenHang.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    // ==========================================
    // 5. CHƯƠNG TRÌNH CHÍNH & XỬ LÝ NHẬP LIỆU
    // ==========================================
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            QuanLyPhuongTien ql = new QuanLyPhuongTien();
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n================ QUẢN LÝ PHƯƠNG TIỆN AUTOSPEED ================");
                Console.WriteLine("1. Thêm Ô tô");
                Console.WriteLine("2. Thêm Xe máy");
                Console.WriteLine("3. Hiển thị danh sách phương tiện");
                Console.WriteLine("4. Tìm phương tiện có giá lăn bánh cao nhất");
                Console.WriteLine("5. Tìm kiếm phương tiện theo tên hãng");
                Console.WriteLine("0. Thoát chương trình");
                Console.WriteLine("===============================================================");
                Console.Write("Mời bạn chọn chức năng (0-5): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ThemOTo(ql);
                        break;
                    case "2":
                        ThemXeMay(ql);
                        break;
                    case "3":
                        ql.DisplayAll();
                        break;
                    case "4":
                        var maxPt = ql.FindMaxGiaLanBanh();
                        if (maxPt != null)
                        {
                            Console.WriteLine("\n>>> PHƯƠNG TIỆN CÓ GIÁ LĂN BÁNH CAO NHẤT <<<");
                            Console.WriteLine($"{maxPt.GetInfo()} | Mức giá: {maxPt.TinhGiaLanBanh():N0} VNĐ");
                        }
                        else
                        {
                            Console.WriteLine("\n[!] Danh sách trống.");
                        }
                        break;
                    case "5":
                        Console.Write("\nNhập tên hãng cần tìm: ");
                        string keyword = Console.ReadLine();
                        var kq = ql.SearchByName(keyword);
                        if (kq.Any())
                        {
                            Console.WriteLine($"\n>>> KẾT QUẢ TÌM KIẾM '{keyword}' <<<");
                            foreach (var pt in kq)
                                Console.WriteLine($"{pt.GetInfo()} | Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ");
                        }
                        else
                        {
                            Console.WriteLine($"\n[!] Không tìm thấy phương tiện nào của hãng '{keyword}'.");
                        }
                        break;
                    case "0":
                        isRunning = false;
                        Console.WriteLine("\nCảm ơn bạn đã sử dụng hệ thống!");
                        break;
                    default:
                        Console.WriteLine("\n[!] Lựa chọn không hợp lệ. Vui lòng thử lại.");
                        break;
                }
            }
        }

        // --- CÁC HÀM XỬ LÝ NGHIỆP VỤ NHẬP LỚP ---

        static void ThemOTo(QuanLyPhuongTien ql)
        {
            Console.WriteLine("\n--- NHẬP THÔNG TIN Ô TÔ ---");
            try
            {
                string ma = NhapChuoi("Mã PT (Bỏ trống sẽ nhận PT000): ", choPhepRong: true);
                string hang = NhapChuoi("Tên hãng: ");
                int nam = NhapSoNguyen("Năm sản xuất: ");
                decimal gia = NhapSoThucDecimal("Giá gốc (VNĐ): ");
                int soCho = NhapSoNguyen("Số chỗ ngồi: ");
                double dungTich = NhapSoThucDouble("Dung tích động cơ (Lít): ");

                OTo oto = new OTo(ma, hang, nam, gia, soCho, dungTich);
                ql.AddPhuongTien(oto);
                Console.WriteLine("[+] Đã thêm Ô tô thành công!");
            }
            catch (Exception ex) // Bắt lỗi Validation từ Encapsulation (Properties)
            {
                Console.WriteLine($"\n[LỖI]: {ex.Message} Thêm thất bại!");
            }
        }

        static void ThemXeMay(QuanLyPhuongTien ql)
        {
            Console.WriteLine("\n--- NHẬP THÔNG TIN XE MÁY ---");
            try
            {
                string ma = NhapChuoi("Mã PT (Bỏ trống sẽ nhận PT000): ", choPhepRong: true);
                string hang = NhapChuoi("Tên hãng: ");
                int nam = NhapSoNguyen("Năm sản xuất: ");
                decimal gia = NhapSoThucDecimal("Giá gốc (VNĐ): ");
                int dungTich = NhapSoNguyen("Dung tích xi-lanh (cc): ");

                XeMay xm = new XeMay(ma, hang, nam, gia, dungTich);
                ql.AddPhuongTien(xm);
                Console.WriteLine("[+] Đã thêm Xe máy thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[LỖI]: {ex.Message} Thêm thất bại!");
            }
        }

        // --- CÁC HÀM HỖ TRỢ NHẬP DỮ LIỆU AN TOÀN TỪ BÀN PHÍM ---
        // (Sử dụng TryParse và vòng lặp while để ép người dùng nhập đúng định dạng)

        static string NhapChuoi(string thongBao, bool choPhepRong = false)
        {
            while (true)
            {
                Console.Write(thongBao);
                string input = Console.ReadLine();
                if (!choPhepRong && string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("[!] Không được để trống. Vui lòng nhập lại.");
                    continue;
                }
                return input;
            }
        }

        static int NhapSoNguyen(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;
                Console.WriteLine("[!] Vui lòng nhập một số nguyên hợp lệ!");
            }
        }

        static decimal NhapSoThucDecimal(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                if (decimal.TryParse(Console.ReadLine(), out decimal value))
                    return value;
                Console.WriteLine("[!] Vui lòng nhập một số tiền hợp lệ!");
            }
        }

        static double NhapSoThucDouble(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                if (double.TryParse(Console.ReadLine(), out double value))
                    return value;
                Console.WriteLine("[!] Vui lòng nhập một số thực hợp lệ (VD: 1.5, 2.0)!");
            }
        }
    }
}