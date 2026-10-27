import { getBaseUrl } from "../../utils/getEnv";

export const getDamages = async(accessToken, search = "", page = 1, perPage = 25, sortBy = "", sortByDirection = "asc") => {
    try {
        const url = getBaseUrl();
        const response = await fetch(`${url}unit/damage?search=${search}&page=${page}&perPage=${perPage}&sortBy=${sortBy}&sortByDirection=${sortByDirection}`,{
          method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
        });
  
        const damageData = await response.json();
        if (!response.ok) {
        
          return {status: response.status, data: null, message: damageData.message};
        }
        return {status: response.status, data: damageData}
      } catch (error) {
        console.error('Error fetching data:', error);
      }
}
export const getUnitDamages = async(accessToken, unitId) => {
    try {
        const url = getBaseUrl();
        const response = await fetch(`${url}unit/damage/UnitDamages?unitId=${unitId}`,{
          method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
        });
  
        if (!response.ok) {
        
          return {status: response.status, data: null};
        }
  
        const damageData = await response.json();
        return {status: response.status, data: damageData}
      } catch (error) {
        console.error('Error fetching data:', error);
      }
}
export const updateDamages = async(accessToken, data) => {
    try {
        const url = getBaseUrl();
        const response = await fetch(`${url}unit/damage`,{
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
}
export const getPossibleAssignedUsers = async (accessToken, userId) => {
  //typeID: 1 - users, 2 - unit, 3 - routes
    try {
      const url = getBaseUrl();
      const response = await fetch(`${url}account/user/PossibleAssignedUsers?userId=${userId}`,{
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
      });

      if (!response.ok) {
        
        return {status: response.status, data: null};
      }

      const data = await response.json();
      return {status: response.status, data}
    } catch (error) {
      console.error('Error fetching data:', error);
    }
};
