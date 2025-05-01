/**
 * 城市管理模組 - 處理城市資料的展示與操作
 */
class CityManager {
    constructor(apiService) {
        this.apiService = apiService;
        this.cities = [];
        
        // DOM 元素
        this.citiesListElement = document.getElementById('cities-list');
        this.citySelectElement = document.getElementById('city-select');
        this.addCityForm = document.getElementById('add-city-form');
        
        // 事件繫結
        this.addCityForm.addEventListener('submit', this.handleAddCity.bind(this));
        
        // 初始化
        this.init();
    }

    /**
     * 初始化模組
     */
    async init() {
        try {
            await this.loadCities();
            this.renderCitySelect();
        } catch (error) {
            console.error('城市管理初始化失敗:', error);
            this.showError('無法載入城市資料，請重新整理頁面。');
        }
    }

    /**
     * 載入所有城市資料
     */
    async loadCities() {
        try {
            this.cities = await this.apiService.getAllCities();
            this.renderCitiesList();
        } catch (error) {
            console.error('載入城市失敗:', error);
            throw error;
        }
    }

    /**
     * 渲染城市列表
     */
    renderCitiesList() {
        if (!this.citiesListElement) return;
        
        // 清空現有列表
        this.citiesListElement.innerHTML = '';
        
        if (this.cities.length === 0) {
            this.citiesListElement.innerHTML = '<div class="col-span-full text-center text-gray-500">尚未新增關注城市</div>';
            return;
        }
        
        // 建立城市卡片
        this.cities.forEach(city => {
            const cityCard = this.createCityCard(city);
            this.citiesListElement.appendChild(cityCard);
        });
    }

    /**
     * 建立城市卡片 DOM 元素
     * @param {object} city - 城市資料
     * @returns {HTMLElement} - 城市卡片元素
     */
    createCityCard(city) {
        const card = document.createElement('div');
        card.className = 'city-card bg-blue-50 border border-blue-200 rounded-lg p-4 flex flex-col justify-between';
        card.dataset.id = city.id;
        
        card.innerHTML = `
            <div>
                <h3 class="font-semibold text-lg">${city.name}</h3>
            </div>
            <div class="flex justify-between items-center mt-3">
                <button class="view-forecast-btn text-blue-600 hover:text-blue-800" data-id="${city.id}">
                    查看預報
                </button>
                <button class="delete-city-btn text-red-600 hover:text-red-800" data-id="${city.id}">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                </button>
            </div>
        `;
        
        // 繫結事件
        card.querySelector('.view-forecast-btn').addEventListener('click', (e) => {
            const cityId = e.target.dataset.id;
            this.selectCity(cityId);
        });
        
        card.querySelector('.delete-city-btn').addEventListener('click', (e) => {
            const cityId = e.target.closest('.delete-city-btn').dataset.id;
            this.handleDeleteCity(cityId);
        });
        
        return card;
    }

    /**
     * 渲染城市選擇下拉選單
     */
    renderCitySelect() {
        if (!this.citySelectElement) return;
        
        // 清空現有選項，保留預設選項
        this.citySelectElement.innerHTML = '<option value="">-- 選擇城市查看預報 --</option>';
        
        // 新增城市選項
        this.cities.forEach(city => {
            const option = document.createElement('option');
            option.value = city.id;
            option.textContent = city.name;
            this.citySelectElement.appendChild(option);
        });
        
        // 繫結變更事件
        this.citySelectElement.addEventListener('change', (e) => {
            const cityId = e.target.value;
            if (cityId) {
                // 觸發天氣預報顯示
                const weatherEvent = new CustomEvent('city-selected', { detail: { cityId } });
                document.dispatchEvent(weatherEvent);
            }
        });
    }

    /**
     * 處理新增城市表單提交
     * @param {Event} event - 表單提交事件
     */
    async handleAddCity(event) {
        event.preventDefault();
        
        const cityNameSelect = document.getElementById('city-name');
        const cityName = cityNameSelect.value.trim();
        
        if (!cityName) {
            alert('請選擇城市');
            return;
        }
        
        // 檢查是否已經存在相同城市
        if (this.cities.some(city => city.name === cityName)) {
            alert('此城市已經在關注列表中');
            return;
        }
        
        try {
            const newCity = {
                name: cityName
            };
            
            // 呼叫 API 新增城市
            const addedCity = await this.apiService.addCity(newCity);
            
            // 更新本地城市列表
            this.cities.push(addedCity);
            
            // 更新 UI
            this.renderCitiesList();
            this.renderCitySelect();
            
            // 重設下拉式選單
            cityNameSelect.selectedIndex = 0;
            
            this.showSuccess('城市新增成功');
        } catch (error) {
            console.error('新增城市失敗:', error);
            this.showError('新增城市失敗，請稍後再試。');
        }
    }

    /**
     * 處理刪除城市
     * @param {string} cityId - 城市 ID
     */
    async handleDeleteCity(cityId) {
        if (!confirm('確定要刪除這個關注城市嗎？')) {
            return;
        }
        
        try {
            // 呼叫 API 刪除城市
            await this.apiService.deleteCity(cityId);
            
            // 更新本地城市列表
            this.cities = this.cities.filter(city => city.id !== parseInt(cityId));
            
            // 更新 UI
            this.renderCitiesList();
            this.renderCitySelect();
            
            this.showSuccess('城市已刪除');
            
            // 如果目前選中的是被刪除的城市，清空預報顯示
            if (this.citySelectElement.value === cityId) {
                this.citySelectElement.value = '';
                document.dispatchEvent(new Event('clear-forecast'));
            }
        } catch (error) {
            console.error('刪除城市失敗:', error);
            this.showError('刪除城市失敗，請稍後再試。');
        }
    }

    /**
     * 選擇城市
     * @param {string} cityId - 城市 ID
     */
    selectCity(cityId) {
        if (this.citySelectElement) {
            this.citySelectElement.value = cityId;
            
            // 觸發變更事件
            const event = new Event('change');
            this.citySelectElement.dispatchEvent(event);
        }
    }
    
    /**
     * 顯示成功訊息
     * @param {string} message - 訊息內容
     */
    showSuccess(message) {
        // 可以實作更好的通知機制，這裡簡單使用 alert
        alert(message);
    }
    
    /**
     * 顯示錯誤訊息
     * @param {string} message - 訊息內容
     */
    showError(message) {
        // 可以實作更好的通知機制，這裡簡單使用 alert
        alert(message);
    }
}
