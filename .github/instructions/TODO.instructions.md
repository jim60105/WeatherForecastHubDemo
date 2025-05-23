---
applyTo: '**'
---

# 氣象預報程式 POC 開發計劃

## 階段一：環境設定與專案初始化

### 後端設定

- [x] 建立 C# .NET 9 Web API 專案
- [x] 設定相依性注入容器
- [x] 建立資料夾結構 (Controllers, Services, Repositories, Models)
- [x] 設定 Swagger 文件
- [x] 設定 appsettings.json，包含資料庫連接字串和中央氣象署 API 金鑰
- [x] 設定靜態檔案中介軟體，將前端頁面提供在路徑根目錄
- [x] 設定 API 路由，確保所有 API 端點都在 `/api` 路徑之下
- [x] 安裝必要的 NuGet 套件：
  - [x] Entity Framework Core
  - [x] Microsoft.EntityFrameworkCore.Sqlite
  - [x] Microsoft.EntityFrameworkCore.Design
  - [x] Swashbuckle.AspNetCore

### 資料庫設定

- [x] 建立 WeatherDbContext 類別
- [x] 定義 City 實體類別
- [x] 設定 Entity Framework 遷移
- [x] 執行初始遷移建立資料庫

## 階段二：後端功能實作

### 資料存取層 (Repository)

- [x] 建立 ICityRepository 介面
- [x] 實作 CityRepository 類別，包含：
  - [x] GetAllCities()
  - [x] GetCityById(int id)
  - [x] AddCity(City city)
  - [x] UpdateCity(City city)
  - [x] DeleteCity(int id)

### 服務層 (Service)

- [x] 建立 ICityService 介面
- [x] 實作 CityService 類別
- [x] 建立 IWeatherService 介面
- [x] 實作 WeatherService 類別，包含從 API 獲取資料並轉換為應用程式模型的邏輯
  - [x] 實作請求建構邏輯
  - [x] 處理 API 回應資料解析
  - [x] 實作錯誤處理機制

### 控制器層 (Controller)

- [x] 設定控制器路由前綴為 `/api`
- [x] 實作 CitiesController，包含：
  - [x] GET /api/cities - 取得所有關注的城市
  - [x] GET /api/cities/{id} - 取得指定 ID 的關注城市
  - [x] POST /api/cities - 新增關注的城市
  - [x] PUT /api/cities/{id} - 更新關注的城市
  - [x] DELETE /api/cities/{id} - 刪除關注的城市
- [x] 實作 WeatherController，包含：
  - [x] GET /api/weather/{cityId} - 取得指定城市的氣象預報資料

## 階段三：前端開發

### 基礎結構

- [x] 在專案的 wwwroot 資料夾中建立 HTML 檔案
- [x] 引入 Tailwind CSS
- [x] 設定頁面基本布局
- [x] 建立 JavaScript 檔案結構
- [x] 確保前端頁面可從路徑根目錄 (`/`) 存取

### 城市管理功能

- [x] 建立城市選擇下拉選單
- [x] 實作關注城市列表顯示
- [x] 實作新增關注城市功能
- [x] 實作刪除關注城市功能

### 氣象預報顯示

- [x] 建立氣象預報顯示區塊
- [x] 實作未來 3 天預報資料的視覺化
- [x] 顯示溫度、濕度、風速和降雨機率
- [ ] 設計天氣狀態圖示顯示

### API 整合

- [x] 建立 API 服務類別
- [x] 實作 API 請求函式，確保使用 `/api` 路徑前綴
- [x] 處理 API 錯誤和異常情況
- [x] 整合前端 UI 與 API 呼叫

## 階段四：測試與調整

### 後端測試

- [ ] 單元測試 Repository 層
- [ ] 單元測試 Service 層
- [ ] 整合測試 API 端點
- [ ] 中央氣象署 API 整合測試

### 前端測試

- [ ] 測試頁面佈局與響應性
- [ ] 測試城市管理功能
- [ ] 測試氣象預報顯示
- [ ] 測試 API 整合
- [ ] 測試靜態檔案提供功能
- [ ] 驗證 API 路徑前綴設定

### 功能調整與優化

- [ ] 優化 API 呼叫效能
- [ ] 改善使用者介面
- [ ] 處理邊緣情況和錯誤提示
- [ ] 最終功能檢查

## 階段五：文件與部署

### 文件

- [ ] 更新 API 文件，確保所有路徑正確包含 `/api` 前綴
- [ ] 編寫使用說明
- [ ] 準備專案展示資料

### 部署準備

- [ ] 確認所有功能正常運作
- [ ] 準備部署檔案
- [ ] 設定正式環境參數
- [ ] 確認靜態檔案與 API 路徑設定在部署環境中正常運作
