import { getBaseUrl } from "../../utils/getEnv";

export const getSystemDetails = async ( accessToken) => {
    try {
      const url = getBaseUrl();
      const response = await fetch(`${url}system/getSysConfigValues`,{
        method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      }
      });

      if (!response.ok) {
        // Handle logout for 401 Unauthorized response
      if (response.status === 401) {
        return null;
      }
        throw new Error('Network response was not ok.');
      }

      const data = await response.json();
      return data
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };
export const saveSystemDetails = async ( accessToken, data) => {
    try {
      const url = getBaseUrl();
      const response = await fetch(`${url}system/saveSysConfigValue`,{
        method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify(data)
      });

      if (!response.ok) {
        // Handle logout for 401 Unauthorized response
        if (response.status === 401) {
          return null;
        }
        throw new Error('Network response was not ok.');
      }

      
      return true
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };