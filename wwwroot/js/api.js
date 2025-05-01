/**
 * API 服務類別 - 處理所有與後端 API 的互動
 */
class ApiService {
    constructor() {
        this.baseUrl = '/api';
    }

    /**
     * 執行 API 請求
     * @param {string} endpoint - API 端點
     * @param {string} method - HTTP 方法 (GET, POST, PUT, DELETE)
     * @param {object} data - 請求資料 (可選)
     * @returns {Promise<any>} - 回傳 Promise 物件
     */
    async fetchApi(endpoint, method = 'GET', data = null) {
        const url = `${this.baseUrl}${endpoint}`;
        
        const options = {
            method,
            headers: {
                'Content-Type': 'application/json'
            }
        };

        if (data && (method === 'POST' || method === 'PUT')) {
            options.body = JSON.stringify(data);
        }

        try {
            const response = await fetch(url, options);
            
            // 檢查回應狀態
            if (!response.ok) {
                throw new Error(`API 請求失敗: ${response.status} ${response.statusText}`);
            }
            
            // 處理無內容回應 (204 No Content)
            if (response.status === 204) {
                return;
            }
            // 解析 JSON 回應
            return await response.json();
        } catch (error) {
            console.error('API 請求錯誤:', error);
            throw error;
        }
    }

    // 城市相關 API
    async getAllCities() {
        return this.fetchApi('/cities');
    }

    async getCityById(id) {
        return this.fetchApi(`/cities/${id}`);
    }

    async addCity(city) {
        // 確保只發送城市名稱
        const cityData = { name: city.name };
        return this.fetchApi('/cities', 'POST', cityData);
    }

    async updateCity(id, city) {
        return this.fetchApi(`/cities/${id}`, 'PUT', city);
    }

    async deleteCity(id) {
        return this.fetchApi(`/cities/${id}`, 'DELETE');
    }

    // 天氣相關 API
    async getWeatherForecast(cityId) {
        return this.fetchApi(`/weather/${cityId}`);
    }
}
