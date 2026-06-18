# Avalime

IME Frontend for Rime with Avalonia

## Android Release 編譯

### 前置條件

- .NET 10 SDK
- Android SDK（API 23+）
- 已連接的 Android 設備或模擬器
- librime.so 及依賴（`libc++_shared.so`、`libCsRimeLua.so`）已放入 `/sdcard/rime/`
- Rime 配置檔案已放入 `/sdcard/rime/`

### 編譯與安裝

```bash
cd Avalime.Frontend/proj/Avalime.Android

# Release 編譯
dotnet build -c Release

# 安裝到已連接的設備
adb install -r bin/Release/net10.0-android/com.CompanyName.Avalime-Signed.apk

# 或一步到位
dotnet run -c Release
```

### 啟用輸入法

安裝後在 Android 系統設定中啟用 Avalime 輸入法：

1. 設定 → 語言與輸入 → 虛擬鍵盤 → 管理鍵盤
2. 開啟 Avalime
3. 在任意輸入框中切換到 Avalime

### 首次使用

點擊鍵盤上方「連接 Rime」按鈕載入引擎，之後即可開始打字。
