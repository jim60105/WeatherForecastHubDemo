/**
 * 天氣預報模組 - 處理天氣資料的展示
 */
class WeatherManager {
    constructor(apiService) {
        this.apiService = apiService;
        
        // DOM 元素
        this.forecastContainer = document.getElementById('forecast-container');
        
        // 事件繫結
        document.addEventListener('city-selected', this.handleCitySelected.bind(this));
        document.addEventListener('clear-forecast', this.clearForecast.bind(this));
    }

    /**
     * 處理城市選擇事件
     * @param {CustomEvent} event - 自訂事件，包含城市 ID
     */
    async handleCitySelected(event) {
        const cityId = event.detail.cityId;
        if (!cityId) return;
        
        try {
            // 顯示載入中
            this.showLoading();
            
            // 取得天氣資料
            const weatherData = await this.apiService.getWeatherForecast(cityId);
            
            // 顯示預報
            this.renderForecast(weatherData);
        } catch (error) {
            console.error('載入天氣預報失敗:', error);
            this.showError('無法載入天氣預報，請稍後再試。');
        }
    }

    /**
     * 顯示載入中狀態
     */
    showLoading() {
        if (!this.forecastContainer) return;
        
        this.forecastContainer.innerHTML = `
            <div class="col-span-full flex justify-center items-center p-10">
                <div class="loading"></div>
                <span class="ml-3">載入天氣資料中...</span>
            </div>
        `;
    }

    /**
     * 渲染天氣預報
     * @param {object} weatherData - 天氣預報資料
     */
    renderForecast(weatherData) {
        if (!this.forecastContainer || !weatherData) {
            this.clearForecast();
            return;
        }
        
        // 清空容器
        this.forecastContainer.innerHTML = '';
        
        // 如果沒有預報資料
        if (weatherData.length === 0) {
            this.forecastContainer.innerHTML = '<div class="col-span-full text-center text-gray-500">無可用的預報資料</div>';
            return;
        }
        
        // 顯示城市資訊
        const cityName = weatherData[0].cityName;
        const cityInfoElement = document.createElement('div');
        cityInfoElement.className = 'col-span-full mb-4';
        cityInfoElement.innerHTML = `
            <h3 class="text-xl font-semibold">${cityName}</h3>
        `;
        this.forecastContainer.appendChild(cityInfoElement);
        
        // 建立每日預報卡片
        weatherData.forEach(forecast => {
            const forecastCard = this.createForecastCard(forecast);
            this.forecastContainer.appendChild(forecastCard);
        });
    }

    /**
     * 建立天氣預報卡片
     * @param {object} forecast - 單日預報資料
     * @returns {HTMLElement} - 預報卡片元素
     */
    createForecastCard(forecast) {
        const card = document.createElement('div');
        card.className = 'forecast-card bg-white border border-gray-200 rounded-lg p-4 shadow-sm';
        
        // 取得天氣圖示
        const weatherIcon = this.getWeatherIcon(forecast.weatherCondition);
        
        // 格式化日期
        const forecastDate = new Date(forecast.date);
        const dayOfWeek = this.getDayOfWeek(forecastDate);
        
        card.innerHTML = `
            <div class="text-center mb-4">
                <div class="text-lg font-semibold">${dayOfWeek}</div>
                <div class="text-sm text-gray-500">${this.formatDate(forecastDate)}</div>
            </div>
            
            <div class="flex justify-center mb-3">
                ${weatherIcon}
            </div>
            
            <div class="text-center mb-4">
                <div class="text-lg font-medium">${forecast.weatherCondition}</div>
            </div>
            
            <div class="grid grid-cols-2 gap-2 text-sm">
                <div class="bg-blue-50 p-2 rounded">
                    <div class="text-gray-600">溫度</div>
                    <div class="font-medium">${forecast.temperature}°C</div>
                </div>
                <div class="bg-blue-50 p-2 rounded">
                    <div class="text-gray-600">降雨機率</div>
                    <div class="font-medium">${forecast.rainProbability}%</div>
                </div>
                <div class="bg-blue-50 p-2 rounded">
                    <div class="text-gray-600">濕度</div>
                    <div class="font-medium">${forecast.humidity}%</div>
                </div>
                <div class="bg-blue-50 p-2 rounded">
                    <div class="text-gray-600">風速</div>
                    <div class="font-medium">${forecast.windSpeed} m/s</div>
                </div>
            </div>
        `;
        
        return card;
    }

    /**
     * 根據天氣描述取得對應圖示
     * @param {string} description - 天氣描述
     * @returns {string} - 圖示 HTML
     */
    getWeatherIcon(description) {
        // 根據天氣描述判斷使用哪種圖示
        if (description.includes('晴') && !description.includes('雨')) {
            return `
                <svg class="weather-icon weather-sunny" fill="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path d="M12 2.25a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0V3a.75.75 0 01.75-.75zM7.5 12a4.5 4.5 0 119 0 4.5 4.5 0 01-9 0zM18.894 6.166a.75.75 0 00-1.06-1.06l-1.591 1.59a.75.75 0 101.06 1.061l1.591-1.59zM21.75 12a.75.75 0 01-.75.75h-2.25a.75.75 0 010-1.5H21a.75.75 0 01.75.75zM17.834 18.894a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 10-1.061 1.06l1.59 1.591zM12 18a.75.75 0 01.75.75V21a.75.75 0 01-1.5 0v-2.25A.75.75 0 0112 18zM7.758 17.303a.75.75 0 00-1.061-1.06l-1.591 1.59a.75.75 0 001.06 1.061l1.591-1.59zM6 12a.75.75 0 01-.75.75H3a.75.75 0 010-1.5h2.25A.75.75 0 016 12zM6.697 7.757a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 00-1.061 1.06l1.59 1.591z"/>
                </svg>
            `;
        } else if (description.includes('雨') && description.includes('雷')) {
            return `
                <svg class="weather-icon weather-thunder" fill="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path d="M11.983 1.907a.75.75 0 00-1.292.658l.591 1.61-1.3.138a.75.75 0 00-.376 1.3l1.679 1.276-2.297.442a.75.75 0 00-.514.788c.077.436.277.933.633 1.359.356.425.883.788 1.555.788a.75.75 0 000-1.5c-.246 0-.383-.145-.54-.332-.151-.182-.257-.401-.296-.583l2.038-.393a.75.75 0 00.402-1.241l-1.225-.93 1.321-.141a.75.75 0 00.597-.843l-.542-1.477.647 1.038a.75.75 0 001.269-.796l-1.35-2.16z"/>
                    <path d="M15.22 9.375a.75.75 0 01-.055 1.06L10.59 14.28a.75.75 0 01-1.005-1.115l4.574-3.844a.75.75 0 011.06.054zm-4.952 7.298a.75.75 0 01.546.905l-.872 4.06a.75.75 0 01-1.45-.31l.872-4.062a.75.75 0 01.904-.593z"/>
                </svg>
            `;
        } else if (description.includes('雨')) {
            return `
                <svg class="weather-icon weather-rainy" fill="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path fill-rule="evenodd" d="M4.5 9.75a6 6 0 0111.573-2.226 3.75 3.75 0 014.133 4.303A4.5 4.5 0 0118 20.25H6.75a5.25 5.25 0 01-2.23-10.004 6.072 6.072 0 01-.02-.496z" clip-rule="evenodd"/>
                    <path d="M3.75 14.25a.75.75 0 01.75-.75h1.5a.75.75 0 010 1.5h-1.5a.75.75 0 01-.75-.75zm8.25-8.25a.75.75 0 01.75-.75h1.5a.75.75 0 010 1.5h-1.5a.75.75 0 01-.75-.75zM6.75 14.25a.75.75 0 01.75-.75h1.5a.75.75 0 010 1.5h-1.5a.75.75 0 01-.75-.75zm8.25 0a.75.75 0 01.75-.75h1.5a.75.75 0 010 1.5h-1.5a.75.75 0 01-.75-.75zm-8.25 3a.75.75 0 01.75-.75h1.5a.75.75 0 010 1.5h-1.5a.75.75 0 01-.75-.75zm8.25 0a.75.75 0 01.75-.75h1.5a.75.75 0 010 1.5h-1.5a.75.75 0 01-.75-.75z"/>
                </svg>
            `;
        } else if (description.includes('陰') || description.includes('雲')) {
            return `
                <svg class="weather-icon weather-cloudy" fill="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path fill-rule="evenodd" d="M4.5 9.75a6 6 0 0111.573-2.226 3.75 3.75 0 014.133 4.303A4.5 4.5 0 0118 20.25H6.75a5.25 5.25 0 01-2.23-10.004 6.072 6.072 0 01-.02-.496z" clip-rule="evenodd"/>
                </svg>
            `;
        } else {
            // 預設圖示
            return `
                <svg class="weather-icon" fill="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path fill-rule="evenodd" d="M4.5 9.75a6 6 0 0111.573-2.226 3.75 3.75 0 014.133 4.303A4.5 4.5 0 0118 20.25H6.75a5.25 5.25 0 01-2.23-10.004 6.072 6.072 0 01-.02-.496z" clip-rule="evenodd"/>
                </svg>
            `;
        }
    }

    /**
     * 格式化日期為 'YYYY-MM-DD' 格式
     * @param {Date} date - 日期物件
     * @returns {string} - 格式化後的日期字串
     */
    formatDate(date) {
        return date.toISOString().split('T')[0];
    }

    /**
     * 取得星期幾
     * @param {Date} date - 日期物件
     * @returns {string} - 星期幾的字串
     */
    getDayOfWeek(date) {
        const days = ['週日', '週一', '週二', '週三', '週四', '週五', '週六'];
        return days[date.getDay()];
    }

    /**
     * 清空預報顯示
     */
    clearForecast() {
        if (this.forecastContainer) {
            this.forecastContainer.innerHTML = '<div class="col-span-full text-center text-gray-500">請選擇城市查看天氣預報</div>';
        }
    }

    /**
     * 顯示錯誤訊息
     * @param {string} message - 錯誤訊息
     */
    showError(message) {
        if (this.forecastContainer) {
            this.forecastContainer.innerHTML = `
                <div class="col-span-full text-center text-red-500">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10 mx-auto mb-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                    <p>${message}</p>
                </div>
            `;
        }
    }
}
