# 氣象預報應用 - 入門學習專案

## 專案簡介

這是一個簡單的氣象預報應用，專為學習 RESTful API 和 Web 應用開發而設計。透過這個專案，您將了解如何建立一個完整的網頁應用，從前端介面到後端 API，再到資料庫儲存。

使用者可以：
- 選擇並關注台灣的城市
- 查看這些城市未來 3 天的天氣預報（溫度、濕度、風速、降雨機率）
- 管理自己的關注城市清單

> 💡 **學習提示**：這是一個概念驗證 (POC) 專案，適合初學者了解基本的網頁應用架構和開發流程。

## 技術架構說明

### 前端技術 (使用者看得到的部分)

- **基礎技術**：純 JavaScript (Vanilla JS)，無需額外框架
  > 💡 **為何選擇**：簡單易學，無需了解複雜框架，適合初學者
- **版面設計**：Tailwind CSS
  > 💡 **為何選擇**：快速套用樣式，無需寫複雜的 CSS
- **頁面結構**：單頁應用 (SPA)，所有功能都在一個頁面上
- **檔案組成**：單一 HTML 檔案，內含 CSS 和 JavaScript
- **檔案存放**：放在 .NET 專案的 `wwwroot` 資料夾中

### 後端技術 (伺服器端程式)

- **開發框架**：C# .NET 9 Web API
  > 💡 **學習重點**：了解 API 設計和 HTTP 請求處理
- **程式架構**：Repository 模式（資料存取層分離）
  > 💡 **為何重要**：讓程式碼更有組織性，易於維護
- **相依性注入**：使用 .NET 內建 DI 容器
  > 💡 **概念說明**：一種讓程式碼更靈活、更易測試的技術
- **程式分層**：Controller 層 (處理請求) 和 Service 層 (處理業務邏輯)
- **API 文件**：使用 Swagger 自動產生
  > 💡 **實用工具**：可視化 API 介面，方便測試
- **API 路徑**：所有 API 都以 `/api` 開頭

### 資料庫 (資料儲存)

- **ORM 技術**：Entity Framework Core
  > 💡 **概念說明**：Object-Relational Mapping，用程式碼物件操作資料庫
- **資料庫選擇**：SQLite
  > 💡 **為何選擇**：輕量級、無需額外安裝，檔案式資料庫

## 功能規格

### 前端功能 (使用者介面)

1. **關注城市管理**
   - 使用下拉選單選擇台灣的城市
   - 顯示已關注城市的清單
   - 新增或移除關注的城市
   > 💡 **學習重點**：DOM 操作、事件處理、API 呼叫

2. **天氣預報顯示**
   - 顯示所選城市未來 3 天的預報資料
   - 包含的資訊：溫度、濕度、風速、降雨機率
   > 💡 **學習重點**：資料呈現、API 資料處理

### 後端 API (伺服器端介面)

> 💡 **RESTful API 說明**：以下是遵循 REST 設計的 API 端點，使用標準 HTTP 方法對應 CRUD 操作

1. **關注城市管理 API**
   - `GET /api/cities` - 取得所有關注的城市
   - `GET /api/cities/{id}` - 取得指定 ID 的城市
   - `POST /api/cities` - 新增關注城市
   - `PUT /api/cities/{id}` - 更新城市資訊
   - `DELETE /api/cities/{id}` - 刪除關注城市

2. **氣象資料 API**
   - `GET /api/weather/{cityId}` - 取得指定城市的天氣預報
   > 💡 **學習重點**：API 參數傳遞、外部 API 整合

## 資料模型 (程式中的資料結構)

### 關注城市模型 (City 類別)

```csharp
public class City
{
    public int Id { get; set; }        // 城市唯一識別碼
    public string Name { get; set; }   // 城市名稱
}
```
> 💡 **學習重點**：資料模型設計、屬性定義

### 資料庫上下文 (DbContext)

```csharp
public class WeatherDbContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    
    // 其他設定...
}
```
> 💡 **學習重點**：Entity Framework Core 基本用法

## 外部資料整合

本專案使用中央氣象署開放資料平臺的 API 取得天氣預報。

- **API 位址**：`https://opendata.cwa.gov.tw/api/v1/`
- **認證方式**：透過 appsettings.json 設定 API 金鑰
- **使用的 API**：鄉鎮天氣預報-臺灣未來3天天氣預報 (`/F-D0047-089`)
- **請求參數**：
  - `Authorization`：API 金鑰
  - `LocationName`：台灣各縣市名稱

> 💡 **學習重點**：第三方 API 整合、HTTP 請求處理

## 程式組成詳解

### 設定檔 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=WeatherForecast.db"
  },
  "CWAApi": {
    "BaseUrl": "https://opendata.cwa.gov.tw/api/v1/",
    "ApiKey": "YourApiKeyHere"
  }
}
```
> 💡 **學習重點**：應用程式設定管理、敏感資訊處理

### 程式碼結構說明

- **靜態檔案**：前端檔案放在 `wwwroot` 資料夾
- **資料存取層**：`ICityRepository` 處理資料庫操作
- **業務邏輯層**：
  - `ICityService` - 處理城市管理邏輯
  - `IWeatherService` - 處理氣象資料取得邏輯
- **API 控制器**：
  - `CitiesController` - 管理城市的 CRUD API
  - `WeatherController` - 提供氣象資料 API

> 💡 **學習重點**：分層架構、關注點分離

## 專案限制說明

為了保持學習專案的簡單性，以下功能未實作：

1. 多使用者功能 - 這是單一使用者專案
2. 資料快取機制 - 每次請求都會即時獲取資料
3. 自動更新功能 - 需手動重整獲取最新資料
4. 響應式設計 - 未特別針對不同螢幕大小優化
5. 容器化部署 - 未使用 Docker 等容器技術

> 💡 **進階學習**：以上是您學完基礎後可以嘗試自行擴充的功能

## 開發流程指南

對於初學者，建議按照以下步驟學習：

1. 了解專案結構與設計理念
2. 學習設定資料庫與後端 API
3. 實作城市管理功能
4. 整合氣象資料 API
5. 開發前端介面與 API 整合
6. 測試與改進

## 學習資源

- [.NET Web API 官方文件](https://learn.microsoft.com/zh-tw/aspnet/core/web-api/?view=aspnetcore-9.0)
- [Entity Framework Core 基礎教學](https://learn.microsoft.com/zh-tw/ef/core/)
- [Tailwind CSS 入門](https://tailwindcss.com/docs)
- [中央氣象署開放資料平臺](https://opendata.cwa.gov.tw/)

