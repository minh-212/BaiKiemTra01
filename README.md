# BaiKiemTra01
//Nguyễn Ngọc Minh 24810310280

I. PHẦN LÝ THUYẾT & CÂU HỎI NGẮN
Câu 1: Sự khác nhau giữa Value Types và Reference Types

Value Types (Kiểu giá trị): Bao gồm int, double, bool, struct, v.v. Biến lưu trữ trực tiếp dữ liệu của nó. Bộ nhớ được cấp phát trên Stack. Tốc độ cấp phát/thu hồi nhanh, tự động dọn dẹp khi ra khỏi phạm vi (scope).

Reference Types (Kiểu tham chiếu): Bao gồm class, string, array, delegate, v.v. Biến chỉ lưu địa chỉ tham chiếu (con trỏ) trên Stack, còn dữ liệu thực tế được cấp phát trên Heap. Cần có Garbage Collector (GC) để dọn dẹp vùng nhớ khi không còn tham chiếu nào trỏ tới.

Câu 2: Init-only Properties (init) vs Thuộc tính có set

set: Cho phép thay đổi giá trị của thuộc tính bất cứ lúc nào trong suốt vòng đời của đối tượng.

init: Chỉ cho phép gán giá trị một lần duy nhất trong quá trình khởi tạo đối tượng (thông qua constructor hoặc Object Initializer). Sau khi khởi tạo xong, thuộc tính đó trở thành Read-Only (chỉ đọc).

Trường hợp sử dụng: Rất hữu ích khi tạo các đối tượng bất biến (Immutable objects) như DTO (Data Transfer Objects), cấu hình hệ thống, hoặc Record types, nơi bạn muốn dùng cú pháp { T = ... } gọn gàng nhưng không muốn dữ liệu bị thay đổi sau đó.

Câu 3: Phương thức virtual vs override trong Đa hình (Polymorphism)

virtual (Lớp cha): Khai báo một phương thức có cài đặt mặc định ở lớp cha, nhưng cho phép các lớp con được quyền thay đổi/viết lại nội dung của phương thức đó.

override (Lớp con): Được sử dụng ở lớp con để ghi đè (thay thế hoàn toàn) phương thức virtual hoặc abstract của lớp cha. Khi gọi phương thức qua một biến kiểu lớp cha nhưng tham chiếu đến đối tượng lớp con, C# sẽ tự động tìm và chạy phương thức override ở lớp con tại thời gian chạy (Runtime).

Câu 4: Truy xuất thành phần static
Thành phần static thuộc về cấp độ Lớp (Class level), dùng chung cho tất cả các đối tượng của lớp đó, không phụ thuộc vào bất kỳ trạng thái nào của một Object cụ thể. C# thiết kế bắt buộc phải gọi qua tên Lớp (VD: ClassName.Method()) để tránh sự nhầm lẫn về mặt ngữ nghĩa (ngăn lập trình viên hiểu lầm rằng thành phần static đó là dữ liệu riêng của một Object instance).
