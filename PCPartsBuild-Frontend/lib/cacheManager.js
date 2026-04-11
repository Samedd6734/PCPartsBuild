// cacheManager.js
// Utility for managing localStorage cache with Time-To-Live (TTL) functionality

export const cacheManager = {
  /**
   * Generates a standardized cache key
   * @param {string} category - The category of the PC part (e.g., 'processors', 'motherboards')
   * @param {number|string} page - The page number
   * @returns {string} The standardized cache key
   */
  generateKey: (category, page) => {
    return `v2_pcparts_${category}_page_${page}`;
  },

  /**
   * Saves data to localStorage with a TTL
   * @param {string} key - The cache key
   * @param {any} data - The data to cache
   * @param {number} ttlInHours - The Time-To-Live in hours (defaults to 1)
   */
  saveToCache: (key, data, ttlInHours = 1) => {
    try {
      if (typeof window === 'undefined') return;
      
      const expiry = Date.now() + (ttlInHours * 60 * 60 * 1000);
      const cacheItem = {
        data,
        expiry
      };
      
      localStorage.setItem(key, JSON.stringify(cacheItem));
    } catch (error) {
      console.warn('Failed to save to localStorage cache. Quota may be exceeded.', error);
      // Optional: Implement clearing logic here if QuotaExceededError is thrown
      if (error.name === 'QuotaExceededError') {
        cacheManager.clearExpiredCache();
        try {
          // Attempt to save again after clearing expired ones
          const expiry = Date.now() + (ttlInHours * 60 * 60 * 1000);
          localStorage.setItem(key, JSON.stringify({ data, expiry }));
        } catch (e) {
          console.error('Cache save completely failed after cleanup', e);
        }
      }
    }
  },

  /**
   * Retrieves valid data from the cache
   * @param {string} key - The cache key
   * @returns {any|null} The cached data if valid, otherwise null
   */
  getFromCache: (key) => {
    try {
      if (typeof window === 'undefined') return null;
      
      const itemStr = localStorage.getItem(key);
      if (!itemStr) return null;
      
      const item = JSON.parse(itemStr);
      
      // Check if cache expired
      if (Date.now() > item.expiry) {
        localStorage.removeItem(key);
        return null;
      }
      
      return item.data;
    } catch (error) {
      console.warn('Failed to read from localStorage cache.', error);
      return null;
    }
  },

  /**
   * Clear all expired cache items related to this utility
   */
  clearExpiredCache: () => {
    try {
      if (typeof window === 'undefined') return;
      
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key && key.startsWith('pcparts_')) {
          const itemStr = localStorage.getItem(key);
          if (itemStr) {
            try {
              const item = JSON.parse(itemStr);
              if (Date.now() > item.expiry) {
                localStorage.removeItem(key);
              }
            } catch (e) {
              // invalid json, just remove
              localStorage.removeItem(key);
            }
          }
        }
      }
    } catch (error) {
      console.warn('Failed to clean up localStorage cache', error);
    }
  }
};
