import { toast } from "react-toastify";
import { getNewToken } from "../services/login";

export const fetchWithRetry = async (fetchFunction, accessToken, navigate, t, ...args) => {
    let response = await fetchFunction(accessToken, ...args);

    if(response) {
      if(response.status !== 200) {
        if (response.status === 401) {
            const newResponse = await refreshAccessToken(accessToken, fetchFunction, navigate, t, ...args);
            return newResponse
        } else {
          if(response?.message) {
            toast.error(response?.message);
          } else {
            toast.error(t("requestError"));
          }
          return response
        }
      } else {
        return response
      }
    } else {
      return {status: 500}
    }
  };

export const refreshAccessToken = async (accessToken, fetchFunction, navigate, t, ...args) => {
    const refreshToken = JSON.parse(localStorage.getItem("smartWasteRefreshToken"))
  
    if (!refreshToken) {
      localStorage.removeItem("smartWasteAccessToken");
      navigate("/login"); // Redirect to login page if refreshToken is missing
      return;
    }
  
    try {
      const response = await getNewToken(accessToken, refreshToken.tokenString);
      if(response) {
        if (response?.status !== 200) {
          console.log("refresh token status not ok, redirecting")
          localStorage.removeItem("smartWasteRefreshToken");
          localStorage.removeItem("smartWasteAccessToken");
          navigate("/login"); // Redirect to login page if fetching new token fails
          return;
        } else {
          localStorage.setItem("smartWasteRefreshToken", JSON.stringify(response.data.jwtAuthResult.refreshToken));
          localStorage.setItem("smartWasteAccessToken", response.data.jwtAuthResult.accessToken);
          console.log("New token generated")
  
          let newResponse = await fetchFunction(response.data.jwtAuthResult.accessToken, ...args);
          return newResponse
        }
      } else {
        return {status: 500}
      }


    } catch (error) {
      console.error(error);
      return {status: 500}
      /* navigate("/login"); */ // Redirect to login page if any other error occurs
    }
  };