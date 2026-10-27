import { getBaseUrl } from "../../utils/getEnv";

export const fetchLoginData = async (formData) => {
  try {
    const baseUrl = getBaseUrl();
    const res = await fetch(`${baseUrl}account/authentication/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(formData),
    });
    if (!res.ok) {
      
    
      return {status: res.status};
    }
    const data = await res.json();
    return {status: res.status, data};
  } catch (error) {
    console.error(error);
    throw error;
  }
};
export const checkToken = async (token) => {
  try {
    const baseUrl = getBaseUrl();
    const res = await fetch(`${baseUrl}account/authentication/CheckToken?token=${token}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      }
    });
    return {status: res.status};
  } catch (error) {
    console.error(error);
    throw error;
  }
};
export const forgotPassword = async (email) => {
  try {
    const url = getBaseUrl();
    const response = await fetch(`${url}account/authentication/forgotPassword`,{
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({email})
    });

    if (!response.ok) {
      // Handle logout for 401 Unauthorized response
      /* if (response.status === 401) {
        return null;
      } */
      return false
    }

    return true
  } catch (error) {
    console.error('Network response was not ok:', error);
  }
    
};
export const resetPassword = async (token, password) => {
  try {
    const url = getBaseUrl();
    const res = await fetch(`${url}account/authentication/PasswordReset`,{
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({token, password})
    });
    if(res.ok) {
      const data = await res.json()
      return {status: res.status,
        data
      }
    }
    return {status: res.status};
  } catch (error) {
    console.error(error);
    throw error;
  }
};

export const getNewToken = async (accessToken, refreshToken) => {
  try {
    const baseUrl = getBaseUrl();
    const res = await fetch(`${baseUrl}account/authentication/RefreshToken`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        accessToken,
        refreshToken
      }) 
    });
    const data = await res.json()
    if(res.ok) {
      return {
        status: res.status,
        data
      }
    }
    return {status: res.status, data: null};
  } catch (error) {
    console.error(error);
  }
};