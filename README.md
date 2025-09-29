# Runner Game với Cheat System

Một game runner 2D được phát triển bằng Unity với hệ thống cheat tích hợp sẵn.

## ✨ Tính năng chính

### 🎯 Gameplay

- **Runner**: Chạy qua các chướng ngại vật
- **Hệ thống máu**: Player có máu và có thể chết khi va chạm
- **Thu thập coin**: Thu thập "Bánh Mì" để tăng điểm
- **Knockback system**: Bị đẩy lùi khi va chạm với chướng ngại vật

### 🛡️ Cheat System

- **TAB** để bật/tắt cheat mode
- **Không mất máu** khi va chạm với chướng ngại vật
- **Đi xuyên qua chướng ngại vật** mà không bị cản trở
- **Không bị knockback** khi va chạm
- **UI hiển thị trạng thái** cheat mode

## 🚀 Cách chạy game

### Yêu cầu hệ thống

- Unity 2022.3 LTS hoặc mới hơn
- Windows 10/11, macOS, hoặc Linux

### Cài đặt

1. Clone repository:

```bash
git clone https://github.com/DreamFog20/PRU_LAB_1_.git
```

2. Mở project trong Unity Editor
3. Mở scene `MainGame 2` trong thư mục `Assets/`
4. Nhấn Play để chạy game

## 🎮 Hướng dẫn chơi

### Điều khiển cơ bản

- **A/D** hoặc **Mũi tên trái/phải**: Di chuyển trái/phải
- **Space** hoặc **W**: Nhảy
- **ESC**: Pause game

### Cheat System

- **TAB**: Bật/tắt cheat mode
- Khi cheat mode bật:
  - Không mất máu khi va chạm
  - Đi xuyên qua chướng ngại vật
  - Không bị knockback
- Trạng thái cheat hiển thị ở góc trên bên trái màn hình

## 🔧 Cấu trúc project

```
Assets/
├── Scripts/                 # Các script chính
│   ├── Player.cs           # Logic player và máu
│   ├── PlayerMovement.cs   # Di chuyển và nhảy
│   ├── GameManager.cs      # Quản lý game
│   └── ObjDamage.cs        # Chướng ngại vật gây sát thương
├── CheatSystem/            # Hệ thống cheat
│   ├── CheatManager.cs     # Quản lý cheat mode
│   ├── CheatUI.cs          # UI hiển thị trạng thái
│   ├── CheatInstructions.cs # Hướng dẫn sử dụng
│   └── CheatSystemSetup.cs # Script thiết lập
├── Scenes/                 # Các scene game
├── Arts/                   # Sprites và artwork
├── Audio/                  # Âm thanh và nhạc
└── prefabs/               # Prefabs game objects
```

## 🛠️ Cài đặt Cheat System

### Tự động (Khuyến nghị)

1. Mở Unity Editor
2. Tìm script `CheatSystemSetup` trong Assets
3. Click chuột phải → "Setup Cheat System"
4. Chạy game và bấm TAB để test

### Thủ công

1. Tạo GameObject mới tên "CheatSystemSetup"
2. Attach script `CheatSystemSetup.cs`
3. Chạy game

## 🎨 Tùy chỉnh

### Thay đổi phím cheat

Sửa trong `CheatManager.cs`:

```csharp
public KeyCode cheatToggleKey = KeyCode.Tab; // Thay đổi phím ở đây
```

### Thay đổi vị trí UI

Sửa trong `CheatUI.cs`:

```csharp
rectTransform.anchoredPosition = new Vector2(150, -30); // Thay đổi vị trí
```

### Tắt hiển thị hướng dẫn

Sửa trong `CheatInstructions.cs`:

```csharp
// Ẩn hướng dẫn ngay lập tức
HideInstructions();
```

## 🐛 Troubleshooting

### Lỗi "Script class cannot be found"

- Kiểm tra Console có lỗi compile không
- Nhấn Ctrl+R để recompile
- Đảm bảo tên file và class name khớp nhau

### Cheat không hoạt động

- Kiểm tra Player có tag "Player" không
- Kiểm tra chướng ngại vật có component `ObjDamage` không
- Xem Console để debug

### UI không hiển thị

- Kiểm tra có Canvas trong scene không
- Script `CheatUI` đã được attach chưa

## 📝 Changelog

### v1.0.0

- ✅ Hệ thống cheat cơ bản
- ✅ UI hiển thị trạng thái
- ✅ Tự động reset collision khi tắt cheat
- ✅ Hướng dẫn sử dụng

---

**Chúc bạn chơi game vui vẻ! 🎮**
