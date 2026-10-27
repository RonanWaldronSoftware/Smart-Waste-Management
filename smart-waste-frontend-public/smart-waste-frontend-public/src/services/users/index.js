import { getBaseUrl } from "../../utils/getEnv";

export const getAllUsers = async (accessToken, search = "", page = 1, perPage = 25, sortBy = "", sortByDirection = "asc", includeInactive = false, simpleVersion = false) => {
    try {
      const url = getBaseUrl();
      const response = await fetch(`${url}account/user/Users?search=${search}&page=${page}&perPage=${perPage}&sortBy=${sortBy}&sortByDirection=${sortByDirection}&includeInactive=${includeInactive}&simpleVersion=${simpleVersion}`,{
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        },
      });

      const usersData = await response.json();
      
      if (!response.ok) {
        return {status: response.status, data: null, message: usersData.message};
      }
      return {status: response.status, data: usersData}
    } catch (error) {
      console.error('Error fetching data:', error);
    }
};
export const getUser = async (accessToken, userId) => {
  try {
    const url = getBaseUrl();
    const response = await fetch(userId ? `${url}account/user?userId=${userId}` : `${url}account/user`,{
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      }
    });
    const usersData = await response.json();

    if (!response.ok) {
      return {status: response.status, data: null, message: usersData.message};
    }
    return {status: response.status, data: usersData}
  } catch (error) {
    console.error('Network response was not ok:', error);
  }
    
};
export const getPossibleParents = async (accessToken, typeId, roleId = "") => {
  //typeID: 1 - users, 2 - unit, 3 - routes
    try {
      const url = getBaseUrl();
      const response = await fetch(`${url}account/user/PossibleParents?typeId=${typeId}&roleId=${roleId}`,{
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
export const createUser = async (accessToken, data) => {
  const url = getBaseUrl();
  const response = await fetch(`${url}account/user`,{
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`
    },
    body: JSON.stringify(data)
  });
  if(!response.ok) {
    const resData = await response.json()
    return {status: response.status, message: resData.message}
  } else {
    return {status: response.status, message: null}
  }  
};
export const updateUser = async (accessToken, data) => {
    const url = getBaseUrl();
    const response = await fetch(`${url}account/user`,{
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify(data)
    });

    if(!response.ok) {
      const resData = await response.json()
      return {status: response.status, message: resData.message}
    } else {
      return {status: response.status, message: null}

    } 
};
export const changeUserStatus = async (accessToken, userId) => {
    const url = getBaseUrl();
    const response = await fetch(`${url}account/user/changeUserStatus?userId=${userId}`,{
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      }
    });

    if(!response.ok) {
      const resData = await response.json()
      return {status: response.status, message: resData.message}
    } else {
      return {status: response.status, message: null}

    } 
};
export const changeUserPassword = async (accessToken, email, password) => {
  const url = getBaseUrl();
  try {
    const response = await fetch(`${url}account/authentication/ChangePassword?email=${email}&password=${password}`,{
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      },
    });

    return {status: response.status}
  } catch (error) {
    console.error('Network response was not ok:', error);
  }
    
};
export const changeUserLanguage = async (accessToken, languageId) => {
  const url = getBaseUrl();
  const response = await fetch(`${url}account/user/UpdateLanguage?languageId=${languageId}`,{
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`
    }
  });

  const resData = await response.json()
  if(!response.ok) {
    return {status: response.status, message: resData.message && resData.message !== "" ? resData.message : null, data: null}
  } else {
    return {status: response.status, message: null, data: resData}

  } 
};
export const deleteUser = async (accessToken, email) => {
  const url = getBaseUrl();
  
    const response = await fetch(`${url}account/user/${email}`,{
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      },
    });

    if(!response.ok) {
      const resData = await response.json()
      return {status: response.status, message: resData.message}
    } else {
      return {status: response.status, message: null}
    } 
};

