我正在設計一個簡單的POC程式，它要包含以下架構設計:

## 前端程式

- 使用 Vanilla JS
- Tailwind CSS
- 一頁式設計，沒有 route
- 單一 html 檔案，沒有 .css 和 .js 檔案

## 後端程式

- 使用 C# .NET 9 Web API
- 使用 Repository Pattern
- 使用 Dependency Injection
- 建立 Controller 和 Service
- 使用 Swagger

## 資料庫

- 使用 Entity Framework Core 介接 SQLite 資料庫

主題是「氣象預報程式」，前端程式要能夠顯示氣象預報的資料，後端程式要包含以下的 API:

- CRUD 關注的城市
- 取得指定城市的氣象預報資料

氣象資料的 API 可以參考以下的 OpenAPI 規格: #file:中央氣象署開放資料平臺之資料擷取API.yaml
端點位置為: https://opendata.cwa.gov.tw/api/


## 問題與回答
Q: 使用者如何管理關注城市？是透過下拉選單選擇，還是搜尋欄位輸入？

A: 使用下拉選單。選單內容為台灣的城市地區，在 中央氣象署開放資料平臺之資料擷取API 內有清單。

Q: 前端需要顯示哪些具體氣象指標？（溫度、濕度、風速、降雨機率等）
A: 溫度、濕度、風速、降雨機率 即可

Q: 氣象預報需要顯示多少天的資料？（今日、未來3天或一週）
A: 3天

Q: 需要永久保存使用者關注的城市嗎？還是僅在瀏覽器會話中保存？
A: 保存在 SQLite 資料庫之內

Q: 是否需要緩存氣象資料來減少對中央氣象署API的呼叫次數？
A: 不用

Q: 中央氣象署API需要授權碼，這部分要如何管理？（後端存儲還是前端輸入）
A: 後端使用 appsettings.json 設定值帶入


Q: 前端是否需要響應式設計以支援不同裝置？
A: 不用

Q: 對API回應時間有什麼具體要求？
A: 沒有具體要求，即時取得，並在失敗時將 HTTP Status Code 拋給前端

Q: 是否需要實作資料更新機制（例如自動定時更新天氣資料）？
A: 不用

Q: 系統的預期使用者數量或規模？
A: 單人使用，這是簡單的 POC 專案

Q: 是否需要考慮Docker容器化部署？
A: 不用

請你在完全理解之後，將規格文件撰寫在 #file:README.md 中
