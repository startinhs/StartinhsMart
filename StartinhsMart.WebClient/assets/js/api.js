// API Configuration
const API_CONFIG = {
    // Backend base URL - Update this to match your backend server
    baseUrl: 'https://localhost:44333',
    
    endpoints: {
        pets: '/api/app/pets',
        categories: '/api/app/categories',
        petImage: '/api/app/pets/image'
    }
};

// API Service
const ApiService = {
    /**
     * Fetch pets from backend with optional filters
     * @param {Object} params - Query parameters (MaxResultCount, SkipCount, Sorting, CategoryId, etc.)
     * @returns {Promise<Object>} Response with items array and totalCount
     */
    async getPets(params = {}) {
        try {
            const defaultParams = {
                MaxResultCount: 50,
                SkipCount: 0,
                ...params
            };
            
            const queryString = new URLSearchParams(defaultParams).toString();
            const url = `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.pets}?${queryString}`;
            
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Error fetching pets:', error);
            throw error;
        }
    },
    
    /**
     * Fetch categories from backend
     * @param {Object} params - Query parameters
     * @returns {Promise<Object>} Response with categories
     */
    async getCategories(params = {}) {
        try {
            const defaultParams = {
                MaxResultCount: 100,
                SkipCount: 0,
                ...params
            };
            
            const queryString = new URLSearchParams(defaultParams).toString();
            const url = `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.categories}?${queryString}`;
            
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Error fetching categories:', error);
            throw error;
        }
    },
    
    /**
     * Get pet image URL
     * @param {string} imageId - Image GUID
     * @returns {string} Full image URL
     */
    getPetImageUrl(imageId) {
        if (!imageId) {
            return 'assets/img/no-image.png'; // Fallback image
        }
        return `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.petImage}?ImageId=${imageId}`;
    },
    
    /**
     * Format price in Vietnamese dong
     * @param {number} price - Price value
     * @returns {string} Formatted price
     */
    formatPrice(price) {
        if (!price) return '0₫';
        return new Intl.NumberFormat('vi-VN').format(price) + '₫';
    },
    
    /**
     * Get category name by ID from cached categories
     * @param {string} categoryId - Category GUID
     * @param {Array} categories - Categories array
     * @returns {string} Category name
     */
    getCategoryName(categoryId, categories) {
        if (!categoryId || !categories) return 'Chưa phân loại';
        const category = categories.find(cat => cat.id === categoryId);
        return category ? category.name : 'Chưa phân loại';
    }
};

const CATEGORY_IMAGE_MAP = {
    'cho-phoc-soc': 'https://pethouse.com.vn/wp-content/uploads/2023/03/02c1b1ad8d3b5065092a-1783x2048.jpg',
    'cho-corgi': 'https://pethouse.com.vn/wp-content/uploads/2023/03/ddecec08596d8533dc7c.jpg',
    'cho-lap-xuong': 'https://cdn.eva.vn/upload/3-2022/images/2022-09-05/image1-1662344394-443-width768height950.jpg',
    'cho-shiba': 'https://pethouse.com.vn/wp-content/uploads/2023/02/anh-cho-akita-383838877788.jpg',
    'cho-poodle': 'https://pethouse.com.vn/wp-content/uploads/2024/04/1-pd-1656-1872x2048.jpg'
};

function getCategoryImageUrl(category) {
    const slug = (category.slug || '').toLowerCase().trim();
    if (slug && CATEGORY_IMAGE_MAP[slug]) {
        return CATEGORY_IMAGE_MAP[slug];
    }
    return 'https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEjRomNX3h1-lXrBk0fJmDRbOVJgRg2TlaKUSS41CNdBSJY8p6Wf7c7MDdGWZxt_oeba4qh4xmsqXHq1ypxdVvb3nfCba8UgD9vGug5kUwWEFPVZZLmD-S5u-QbrCr9gC_hpj4c7gheltRxoUDU8fkVWUAgiv06vH_PLCEMYLnNtAWwXYcmkIT0Oa1PmTcw/s16000/Pawsome.png';
}

// Product Renderer
const ProductRenderer = {
    /**
     * Render a single product card
     * @param {Object} pet - Pet DTO from backend
     * @param {Array} categories - Categories array
     * @returns {string} HTML string for product card
     */
    renderProductCard(pet, categories = []) {
        const imageUrl = ApiService.getPetImageUrl(pet.imageId);
        const price = ApiService.formatPrice(pet.price);
        const comparePrice = pet.compareAtPrice ? ApiService.formatPrice(pet.compareAtPrice) : null;
        const categoryName = ApiService.getCategoryName(pet.categoryId, categories);
        const discountPercent = pet.compareAtPrice && pet.price 
            ? Math.round((1 - pet.price / pet.compareAtPrice) * 100) 
            : 0;
        
        // Build product detail page URL (you may need to adjust this)
        const productUrl = `./pet-detail.html?id=${pet.id}`;
        
        return `
            <div class="item swiper-slide">
                <div class="item_product_main">
                    <form class="variants wishItem" data-id="product-${pet.id}" enctype="multipart/form-data">
                        <div class="product-thumbnail">
                            <a class="product_overlay" href="${productUrl}"></a>
                            <a class="image_thumb" href="${productUrl}">
                                ${discountPercent > 0 ? `
                                <div class="rectangle">
                                    -${discountPercent}%
                                </div>
                                ` : ''}
                                <img class="lazyload" width="10" height="10"
                                    src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXYzh8+PB/AAffA0nNPuCLAAAAAElFTkSuQmCC"
                                    data-src="${imageUrl}"
                                    alt="${pet.name}">
                            </a>
                            <div class="product-action">
                                <div class="group_action">
                                    <a title="Xem nhanh" href="${productUrl}"
                                        class="xem_nhanh btn-circle btn-views btn_view btn right-to quick-view">
                                        <svg aria-hidden="true" focusable="false" data-prefix="far"
                                            data-icon="search-plus" role="img"
                                            xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"
                                            class="svg-inline--fa fa-search-plus fa-w-16">
                                            <path fill="currentColor"
                                                d="M312 196v24c0 6.6-5.4 12-12 12h-68v68c0 6.6-5.4 12-12 12h-24c-6.6 0-12-5.4-12-12v-68h-68c-6.6 0-12-5.4-12-12v-24c0-6.6 5.4-12 12-12h68v-68c0-6.6 5.4-12 12-12h24c6.6 0 12 5.4 12 12v68h68c6.6 0 12 5.4 12 12zm196.5 289.9l-22.6 22.6c-4.7 4.7-12.3 4.7-17 0L347.5 387.1c-2.3-2.3-3.5-5.3-3.5-8.5v-13.2c-36.5 31.5-84 50.6-136 50.6C93.1 416 0 322.9 0 208S93.1 0 208 0s208 93.1 208 208c0 52-19.1 99.5-50.6 136h13.2c3.2 0 6.2 1.3 8.5 3.5l121.4 121.4c4.7 4.7 4.7 12.3 0 17zM368 208c0-88.4-71.6-160-160-160S48 119.6 48 208s71.6 160 160 160 160-71.6 160-160z"
                                                class=""></path>
                                        </svg>
                                        <span class="text">Xem nhanh</span>
                                    </a>
                                    <a href="${productUrl}"
                                        class="btn-buy btn-cart btn-left btn btn-views left-to add_to_cart active"
                                        title="Xem chi tiết">
                                        <svg xmlns="http://www.w3.org/2000/svg" version="1.1" x="0"
                                            y="0" viewBox="0 0 490.666 490.666"
                                            style="enable-background:new 0 0 512 512"
                                            xml:space="preserve" class="">
                                            <g>
                                                <path
                                                    d="M394.667,373.333c-29.397,0-53.333,23.936-53.333,53.333S365.269,480,394.667,480S448,456.064,448,426.666    S424.064,373.333,394.667,373.333z M394.667,458.666c-17.643,0-32-14.357-32-32c0-17.643,14.357-32,32-32    c17.643,0,32,14.357,32,32C426.667,444.309,412.309,458.666,394.667,458.666z"
                                                    fill="#ffffff" data-original="#000000" style=""
                                                    class=""></path>
                                                <path
                                                    d="M181.333,373.333c-29.397,0-53.333,23.936-53.333,53.333S151.936,480,181.333,480s53.333-23.936,53.333-53.333    S210.731,373.333,181.333,373.333z M181.333,458.666c-17.643,0-32-14.357-32-32c0-17.643,14.357-32,32-32s32,14.357,32,32    C213.333,444.309,198.976,458.666,181.333,458.666z"
                                                    fill="#ffffff" data-original="#000000" style=""
                                                    class=""></path>
                                                <path
                                                    d="M437.333,330.666H191.125c-25.323,0-47.317-18.027-52.288-42.88L85.12,19.242c-1.003-4.992-5.376-8.576-10.453-8.576h-64    C4.779,10.666,0,15.445,0,21.333S4.779,32,10.667,32H65.92l51.989,259.989c6.955,34.773,37.76,60.011,73.216,60.011h246.208    c5.888,0,10.667-4.779,10.667-10.667C448,335.445,443.221,330.666,437.333,330.666z"
                                                    fill="#ffffff" data-original="#000000" style=""
                                                    class=""></path>
                                                <path
                                                    d="M488,78.272c-2.027-2.283-4.928-3.605-8-3.605H96c-5.888,0-10.667,4.779-10.667,10.667S90.112,96,96,96h371.925    l-15.168,121.301c-2.005,15.979-15.659,28.032-31.765,28.032H128c-5.888,0-10.667,4.779-10.667,10.667    c0,5.888,4.779,10.667,10.667,10.667h292.992c26.837,0,49.6-20.075,52.928-46.72l16.661-133.291    C490.965,83.626,490.027,80.554,488,78.272z"
                                                    fill="#ffffff" data-original="#000000" style=""
                                                    class=""></path>
                                            </g>
                                        </svg>
                                        <span class="text">Mua ngay</span>
                                    </a>
                                </div>
                            </div>
                        </div>
                        <div class="product-info">
                            <h3 class="product-name"><a href="${productUrl}">${pet.name}</a></h3>
                            <div class="price-box">
                                <span class="price">${price}</span>
                                ${comparePrice ? `<span class="compare-price">${comparePrice}</span>` : ''}
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        `;
    },
    
    /**
     * Load and render products to a container
     * @param {string} containerSelector - CSS selector for container
     * @param {Object} filterParams - Filter parameters for API call
     */
    async loadProducts(containerSelector, filterParams = {}) {
        try {
            const container = document.querySelector(containerSelector);
            if (!container) {
                console.error('Container not found:', containerSelector);
                return;
            }
            
            // Show loading state
            container.innerHTML = '<div class="loading-products">Đang tải sản phẩm...</div>';
            
            // Fetch data
            const [petsResponse, categoriesResponse] = await Promise.all([
                ApiService.getPets(filterParams),
                ApiService.getCategories()
            ]);
            
            const pets = petsResponse.items || [];
            const categories = categoriesResponse.items || [];
            
            // Render products
            if (pets.length === 0) {
                container.innerHTML = '<div class="no-products">Không tìm thấy sản phẩm nào.</div>';
                return;
            }
            
            const productsHtml = pets.map(pet => this.renderProductCard(pet, categories)).join('');
            container.innerHTML = productsHtml;
            
            // Re-initialize lazy load images
            if (typeof awe_lazyloadImage === 'function') {
                awe_lazyloadImage();
            }
            
            // Re-initialize swiper if needed
            if (container.closest('.swiper-container')) {
                // Swiper will auto-detect and initialize
            }
            
        } catch (error) {
            console.error('Error loading products:', error);
            const container = document.querySelector(containerSelector);
            if (container) {
                container.innerHTML = `
                    <div class="error-products">
                        <p>Không thể tải sản phẩm. Vui lòng thử lại sau.</p>
                        <button onclick="ProductRenderer.loadProducts('${containerSelector}', ${JSON.stringify(filterParams)})">
                            Thử lại
                        </button>
                    </div>
                `;
            }
        }
    }
};

const CategoryRenderer = {
    renderCategoryCard(category) {
        const imageUrl = getCategoryImageUrl(category);
        const categoryUrl = category.slug ? `./${category.slug}` : '#';
        const description = category.description || 'Xem chi tiet';

        return `
            <div class="swiper-slide item">
                <div class="box_welcome">
                    <div class="image_welcome">
                        <a href="${categoryUrl}">
                            <img class="lazyload"
                                src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXYzh8+PB/AAffA0nNPuCLAAAAAElFTkSuQmCC"
                                data-src="${imageUrl}" />
                        </a>
                    </div>
                    <div class="content_welcome">
                        <h3 class="title_welcome">
                            <a href="${categoryUrl}">${category.name}</a>
                        </h3>
                        <p>${description}</p>
                    </div>
                </div>
            </div>
        `;
    },

    async loadCategories(containerSelector, filterParams = {}) {
        try {
            const container = document.querySelector(containerSelector);
            if (!container) {
                console.error('Container not found:', containerSelector);
                return;
            }

            container.innerHTML = '<div class="loading-products">Dang tai danh muc...</div>';

            const response = await ApiService.getCategories(filterParams);
            const categories = response.items || [];

            if (categories.length === 0) {
                container.innerHTML = '<div class="no-products">Khong tim thay danh muc nao.</div>';
                return;
            }

            const categoriesHtml = categories.map(category => this.renderCategoryCard(category)).join('');
            container.innerHTML = categoriesHtml;

            if (typeof awe_lazyloadImage === 'function') {
                awe_lazyloadImage();
            }
        } catch (error) {
            console.error('Error loading categories:', error);
            const container = document.querySelector(containerSelector);
            if (container) {
                container.innerHTML = `
                    <div class="error-products">
                        <p>Khong the tai danh muc. Vui long thu lai sau.</p>
                        <button onclick="CategoryRenderer.loadCategories('${containerSelector}', ${JSON.stringify(filterParams)})">
                            Thu lai
                        </button>
                    </div>
                `;
            }
        }
    }
};

// Initialize on page load
window.ApiService = ApiService;
window.ProductRenderer = ProductRenderer;
window.CategoryRenderer = CategoryRenderer;
window.API_CONFIG = API_CONFIG;
