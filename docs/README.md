# 氣象預報程式 POC 規格文件

## 專案概述

本專案是一個簡單的氣象預報概念驗證(POC)程式，可讓使用者查看並管理關注城市的氣象預報資料。使用者可以透過下拉選單選擇台灣的城市地區，並查看該地區未來 3 天的天氣預報，包括溫度、濕度、風速和降雨機率等指標。

## 技術架構

### 前端技術

- **使用技術**：Vanilla JavaScript
- **CSS 框架**：Tailwind CSS
- **頁面設計**：一頁式設計，無路由
- **檔案結構**：單一 HTML 檔案，內嵌 CSS 和 JavaScript
- **檔案提供**：由後端 .NET 靜態檔案中介軟體提供，位於路徑根目錄

### 後端技術

- **開發框架**：C# .NET 9 Web API
- **架構模式**：Repository Pattern
- **相依性注入**：使用內建 DI 容器
- **層次結構**：Controller 和 Service 層
- **API 文件**：Swagger
- **API 路徑**：所有 API 端點統一放在 `/api` 路徑之下

### 資料庫

- **ORM**：Entity Framework Core
- **資料庫**：SQLite

## 功能規格

### 前端功能

1. **關注城市管理**

- 使用下拉選單選擇台灣的城市地區
- 顯示已關注城市的列表
- 可新增、移除關注城市

2. **天氣預報顯示**

- 顯示選定城市未來 3 天的天氣預報
- 顯示的氣象指標：
  - 溫度
  - 濕度
  - 風速
  - 降雨機率

### 後端 API

1. **關注城市管理**

- `GET /api/cities` - 取得所有關注的城市
- `GET /api/cities/{id}` - 取得指定 ID 的關注城市
- `POST /api/cities` - 新增關注的城市
- `PUT /api/cities/{id}` - 更新關注的城市
- `DELETE /api/cities/{id}` - 刪除關注的城市

2. **氣象預報資料**

- `GET /api/weather/{cityId}` - 取得指定城市的氣象預報資料

## 資料模型

### 關注城市 (City)

```csharp
public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LocationId { get; set; }  // 對應中央氣象署API的位置ID
}
```

### 資料庫上下文 (DbContext)

```csharp
public class WeatherDbContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    
    // 其他設定...
}
```

## 外部 API 整合

系統將整合中央氣象署開放資料平臺的 API 以獲取天氣預報資料。

- **API 端點**：`https://opendata.cwa.gov.tw/api/`
- **授權方式**：透過 appsettings.json 設定 API 授權碼
- **主要使用的 API**：一般天氣預報-今明 36 小時天氣預報 (`F-C0032-001`)
- **請求參數**：
  - `Authorization`：API 授權碼
  - `locationName`：台灣各縣市名稱
  - `elementName`：氣象要素（溫度、濕度等）

## 實作細節

### 設定檔 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=weather.db"
  },
  "WeatherApi": {
    "BaseUrl": "https://opendata.cwa.gov.tw/api",
    "ApiKey": "您的API授權碼"
  }
}
```

### 靜態檔案設定

- 前端 HTML、CSS 與 JavaScript 檔案放在專案的 `wwwroot` 資料夾中
- 透過 .NET 的靜態檔案中介軟體在應用程式啟動時設定
- 使用者可透過根路徑 (`/`) 存取前端頁面

### Repository 層

- `ICityRepository` - 處理城市資料的存取
- `IWeatherRepository` - 處理氣象資料的存取

### Service 層

- `ICityService` - 處理城市相關的業務邏輯
- `IWeatherService` - 處理氣象資料的業務邏輯，包括從中央氣象署 API 獲取資料

### Controller 層

- `CitiesController` - 處理關注城市的 CRUD 操作
- `WeatherController` - 處理氣象資料的查詢

## 限制與考量

1. 本系統為單人使用的 POC 專案，不考慮多用戶情境
2. 不實現氣象資料緩存，每次請求都會即時從中央氣象署獲取資料
3. 不需要實作自動更新機制
4. 不需要響應式設計以支援不同裝置
5. 不考慮 Docker 容器化部署

## 開發工作流程

1. 設定後端專案結構和資料庫
2. 實作關注城市的 CRUD API
3. 實作氣象資料查詢 API
4. 開發前端頁面並整合 API
5. 測試與調整

## 錯誤處理

- 當 API 請求失敗時，將中央氣象署 API 的 HTTP 狀態碼直接傳遞給前端
- 前端需處理可能的錯誤情況並顯示適當的提示訊息
