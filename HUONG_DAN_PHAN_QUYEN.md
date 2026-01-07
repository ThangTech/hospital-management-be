# Hướng Dẫn Kiểm Thử Phân Quyền (RBAC Test Guide)

Tài liệu này tổng hợp các quyền hạn của từng vai trò (Role) đối với 3 module trọng tâm: **Bệnh nhân**, **Nhập viện** và **Xuất viện** để hỗ trợ bạn thực hiện kiểm thử.

## 1. Bảng Tổng Hợp Quyền Hạn (Matrix)

| Module | Chức năng (Endpoint) | Admin | Y Tá (YTa) | Bác Sĩ (BacSi) | Kế Toán (KeToan) |
| :--- | :--- | :---: | :---: | :---: | :---: |
| **Bệnh Nhân** | Xem danh sách / Chi tiết / Tìm kiếm | ✅ | ✅ | ✅ | ✅ |
| | Thêm mới (Create) | ✅ | ✅ | ❌ | ❌ |
| | Cập nhật thông tin (Update) | ✅ | ✅ | ❌ | ❌ |
| | Xóa (Delete) - *Chỉ khi sạch dữ liệu* | ✅ | ❌ | ❌ | ❌ |
| **Nhập Viện** | Xem danh sách / Chi tiết / Tìm kiếm | ✅ | ✅ | ✅ | ✅ |
| | Thực hiện nhập viện mới | ✅ | ✅ | ❌ | ❌ |
| | Cập nhật thông tin nhập viện | ✅ | ✅ | ❌ | ❌ |
| | Chuyển giường | ✅ | ✅ | ❌ | ❌ |
| | Xóa phiếu nhập viện | ✅ | ✅ | ❌ | ❌ |
| **Xuất Viện** | Xem lịch sử / Danh sách chờ | ✅ | ✅ | ✅ | ✅ |
| | Xem trước (Preview) hóa đơn | ✅ | ✅ | ✅ | ✅ |
| | Xác nhận xuất viện (Confirm Discharge) | ✅ | ✅ | ❌ | ❌ |
| **Hóa Đơn** | Quản lý Hóa đơn / Thanh toán | ✅ | ❌ | ❌ | ✅ |

---

## 2. Chi Tiết Nghiệp Vụ Từng Vai Trò

### 👑 Admin
- **Quyền hạn**: Cao nhất, toàn quyền truy cập tất cả các module.
- **Kịch bản test**: Có thể làm mọi thứ. Đặc biệt, chỉ Admin mới có quyền **Xóa bệnh nhân** (với điều kiện bệnh nhân đó không còn trong quá trình điều trị hoặc nợ tiền).

### 🏥 Y Tá (YTa) - Chìa khóa của quy trình
- **Quyền hạn**: Tập trung vào quản lý hành chính và quy trình nội trú.
- **Làm được**: 
    - Đăng ký thông tin bệnh nhân mới.
    - Làm thủ tục nhập viện, chọn giường, chuyển giường.
    - Xác nhận bệnh nhân đã đủ điều kiện xuất viện.
- **Không làm được**: Không được xóa bệnh nhân khỏi hệ thống, không được quản lý hóa đơn/thanh toán.

### 👨‍⚕️ Bác Sĩ (BacSi)
- **Quyền hạn**: Tập trung vào chuyên môn lâm sàng.
- **Làm được**: 
    - Xem toàn bộ lịch sử điều trị, thông tin bệnh nhân, danh sách giường để nắm tình hình.
- **Không làm được**: Không được chỉnh sửa thông tin hành chính của bệnh nhân, không được thực hiện các thủ tục nhập/xuất viện (việc này dành cho Y tá).

### 💰 Kế Toán (KeToan)
- **Quyền hạn**: Quản lý tài chính.
- **Làm được**:
    - Xem thông tin bệnh nhân để đối chiếu.
    - Toàn quyền tạo, sửa và xác nhận thanh toán hóa đơn.
- **Không làm được**: Không được can thiệp vào quy trình nhập viện, xuất viện hay chỉnh sửa thông tin y khoa của bệnh nhân.

---

## 3. Quy Trình Test Đề Xuất

1.  **Bước 1**: Đăng nhập bằng tài khoản **ThuNgan** -> Thử vào module Nhập viện -> Phải nhận lỗi **403 Forbidden**.
2.  **Bước 2**: Đăng nhập bằng tài khoản **BacSi** -> Thử sửa tên bệnh nhân -> Phải nhận lỗi **403 Forbidden**.
3.  **Bước 3**: Đăng nhập bằng tài khoản **YTa** -> Thực hiện Nhập viện cho 1 bệnh nhân -> Xóa bệnh nhân đó -> Hệ thống phải chặn lại báo lỗi "Đang điều trị" (**400 Bad Request**).
4.  **Bước 4**: Đăng nhập bằng **Admin** -> Xóa 1 bệnh nhân không có dữ liệu điều trị -> Phải thành công (**200 OK**).
