import { getBaseUrl } from "../../utils/getEnv";

export const createRoute = async (accessToken, data) => {
    const url = getBaseUrl();
    try {
      const response = await fetch(`${url}Unit/Route`,{
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
};
export const updateRoute = async (accessToken, data) => {
    const url = getBaseUrl();
    try {
      const response = await fetch(`${url}Unit/Route`,{
        method: "PUT",
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
};
export const getAllRoutes = async (accessToken,search = "", page = 1, perPage = 25, sortBy = "", sortByDirection = "asc") => {
    const url = getBaseUrl();
    try {
      const response = await fetch(`${url}Unit/Route/Routes?search=${search}&page=${page}&perPage=${perPage}&sortBy=${sortBy}&sortByDirection=${sortByDirection}`,{
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
      });
      
      const data = await response.json();
      if (!response.ok) {
        
        return {status: response.status, data: null, message: data.message};
      }

      return {status: response.status, data}
    } catch (error) {
      console.error('Error fetching data:', error);
    }
};
export const getRoute = async (accessToken, routeId) => {
  const url = getBaseUrl();
  try {
    const response = await fetch(`${url}unit/route/${routeId}`,{
      method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`
    }
    });
    
    const data = await response.json();
    
    if (!response.ok) {
      return {status: response.status, data: null, message: data.message};
    }

    return {status: response.status, data}

  } catch (error) {
    console.error('Error fetching data:', error);
    return []
  }
};
export const getUserUnits = async (accessToken, userId) => {
    const url = getBaseUrl();
    try {
      const response = await fetch(`${url}unit/unit/GetUserUnits?userId=${userId}`,{
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
      });

      const data = await response.json();
      
      if (!response.ok) {
        return {status: response.status, data: null, message: data.message};
      }

      return {status: response.status, data}
    } catch (error) {
      console.error('Error fetching data:', error);
    }
};
export const deleteRoute = async (accessToken, routeId) => {
  try {
    const url = getBaseUrl();
      const response = await fetch(`${url}unit/route?routeId=${routeId}`,{
        method: "DELETE",
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${accessToken}`,
          
        },
      });
  
      return {status: response.status}
  
      return true
    } catch (error) {
      console.error('Network response was not ok:', error);
    }
      
  };