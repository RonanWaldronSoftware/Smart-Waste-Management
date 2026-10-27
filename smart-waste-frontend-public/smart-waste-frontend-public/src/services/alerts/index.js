import { getBaseUrl } from "../../utils/getEnv";

export const getUserAlerts = async(accessToken, customerId) => {
  try {
      const url = getBaseUrl();
      const response = await fetch(`${url}unit/alert?userId=${customerId}`,{
        method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      }
      });

      if (!response.ok) {
        
        return {status: response.status, data: null};
      }
  
      const alertsData = await response.json();
      return {status: response.status, data: alertsData}
    } catch (error) {
      console.error('Error fetching data:', error);
    }
}
export const createAlert = async(accessToken, data) => {
  try {
      const url = getBaseUrl();
      const response = await fetch(`${url}unit/alert`,{
        method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify(data)
      });

      return {status: response.status}
    } catch (error) {
      console.error('Error fetching data:', error);
    }
}
