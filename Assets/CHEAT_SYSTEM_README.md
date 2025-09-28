# 🎮 CHEAT SYSTEM - HƯỚNG DẪN SỬ DỤNG

## 📋 Tổng quan
Cheat System cho phép người chơi bật/tắt chế độ cheat để:
- **Không mất máu** khi va chạm với chướng ngại vật
- **Đi xuyên qua chướng ngại vật** mà không bị cản trở
- **Không bị knockback** khi va chạm

## 🚀 Cách sử dụng

### 1. Thiết lập tự động
- Mở Unity Editor
- Tìm script `CheatSystemSetup` trong Assets
- Click chuột phải → "Setup Cheat System" để thiết lập tự động
- Hoặc tạo GameObject mới và attach script `CheatSystemSetup`

### 2. Sử dụng trong game
- **Bấm TAB** để bật/tắt cheat mode
- Khi cheat mode bật, text "CHEAT MODE: BẬT" sẽ hiển thị màu đỏ ở góc trên bên trái
- Khi cheat mode tắt, text "CHEAT MODE: TẮT" sẽ hiển thị màu trắng

## 🔧 Các script đã tạo

### 1. `CheatManager.cs`
- Quản lý trạng thái cheat mode
- Xử lý input phím TAB
- Cung cấp API cho các script khác

### 2. `Player.cs` (đã sửa đổi)
- Kiểm tra cheat mode trước khi nhận damage
- Không mất máu khi cheat mode bật

### 3. `PlayerMovement.cs` (đã sửa đổi)
- Không bị knockback khi cheat mode bật
- Đi xuyên qua chướng ngại vật bằng cách tắt collision

### 4. `ObjDamage.cs` (đã sửa đổi)
- Không gây damage và knockback khi cheat mode bật

### 5. `CheatUI.cs`
- Hiển thị trạng thái cheat mode trên UI
- Tự động tạo UI text nếu chưa có

### 6. `CheatInstructions.cs`
- Hiển thị hướng dẫn sử dụng cheat
- Tự động ẩn sau 5 giây

### 7. `CheatSystemSetup.cs`
- Script tiện ích để thiết lập cheat system
- Có thể test cheat system

## 🎯 Tính năng

### ✅ Đã hoàn thành
- [x] Bấm TAB để bật/tắt cheat mode
- [x] Không mất máu khi cheat mode bật
- [x] Đi xuyên qua chướng ngại vật
- [x] Không bị knockback
- [x] UI hiển thị trạng thái cheat mode
- [x] Hướng dẫn sử dụng tự động hiển thị

### 🔧 Cách tùy chỉnh
- Thay đổi phím cheat: Sửa `cheatToggleKey` trong `CheatManager.cs`
- Thay đổi vị trí UI: Sửa `anchoredPosition` trong `CheatUI.cs`
- Thay đổi thời gian hiển thị hướng dẫn: Sửa `Invoke("HideInstructions", 5f)` trong `CheatInstructions.cs`

## 🐛 Debug
- Mở Console để xem log khi bật/tắt cheat mode
- Sử dụng "Test Cheat System" trong `CheatSystemSetup` để kiểm tra

## 📝 Lưu ý
- Cheat system sử dụng Singleton pattern cho `CheatManager`
- Tất cả script đều có null check để tránh lỗi
- Cheat system tương thích với Input System của Unity
- UI sẽ tự động tạo nếu chưa có Canvas

---
**Chúc bạn chơi game vui vẻ! 🎮**
