export const getBaseUrl = () => {
    const baseUrl = import.meta.env.VITE_SMARTWASTE_BASE_URL || process.env.VITE_SMARTWASTE_BASE_URL;
    if (!baseUrl) {
      throw new Error('VITE_SMARTWASTE_BASE_URL not defined in .env file.');
    }
    return baseUrl;
  };
export const getGoogleMapsApiKey = () => {
    const baseUrl = import.meta.env.VITE_GOOGLE_MAPS_KEY || process.env.VITE_GOOGLE_MAPS_KEY;
    if (!baseUrl) {
      throw new Error('VITE_GOOGLE_MAPS_KEY not defined in .env file.');
    }
    return baseUrl;
  };