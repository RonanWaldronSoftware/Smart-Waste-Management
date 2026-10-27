

import { getBaseUrl } from "../../utils/getEnv";

export const getBinsList = async (accessToken, search = "", page = 1, perPage = 25, sortBy = "", sortByDirection = "asc", includeInactive = false, filter = null) => {
  try {
    const url = getBaseUrl();
    const response = await fetch(`${url}unit/unit/getUnits`,{
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify({
        dataTableOptions: {
          search,
          page,
          perPage,
          sortBy,
          sortByDirection
        },
        includeInactive,
        filter
      })
    });

    if (!response.ok) {
        
      return {status: response.status, data: null};
    }

    const binsData = await response.json();
    return {status: response.status, data: binsData}
  } catch (error) {
    console.error('Error fetching data:', error);
  }
};
export const getUnit = async(accessToken, binId) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit/${binId}`, {
      method: "GET",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      }
     })
    const data = await response.json();
     if (!response.ok) {
      // Handle logout for 401 Unauthorized response
      return {status: response.status, data: null, message: data.message}
    }

    
    return {status: response.status, data}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const createBin = async(accessToken, binData) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit`, {
      method: "POST",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify(binData)
     })

    return {status: response.status}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const updateBin = async(accessToken, binData) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit`, {
      method: "PUT",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify(binData)
     })

    return {status: response.status}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const setUnitStatus = async(accessToken, unitId) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit/setStatus?unitId=${unitId}`, {
      method: "PUT",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      }
     })

    return {status: response.status}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const checkSerialNumber = async(accessToken, serialNumber) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit/CheckSerialNumber?serialNumber=${serialNumber}`, {
      method: "GET",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      }
     })

     if(response.status === 404) {
      return {status: 200}
     } else {
      return {status: 400}
     }
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const getUnitReadings = async(accessToken, binId,last7days = "",search = "", page = 1, perPage = 25, sortBy = "id", sortByDirection = "asc",) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit/readings?unitId=${binId}&search=${search}&page=${page}&perPage=${perPage}&sortBy=${sortBy}&sortByDirection=${sortByDirection}&ForLastDays=${last7days}`, {
      method: "GET",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      }
     })

    const binsData = await response.json();
     if (!response.ok) {
        
      return {status: response.status, data: null, message: binsData.message}
    }
    return {status: response.status, data: binsData}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const deleteBin = async(accessToken, unitId) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit?unitId=${unitId}`, {
      method: "DELETE",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      },
     })

     if(response.status !== 200) {
      const data = await response.json()
      return {status: response.status, message: data.message}
     }

    return {status: response.status}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}
export const resetLocationToDevice = async(accessToken, binId) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit/${binId}/ResetLocationToDevice`, {
      method: "POST",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      }
     })

     if(response.status !== 200) {
      // Surface the localized backend message (e.g. no device position recorded)
      const data = await response.json().catch(() => null)
      return {status: response.status, data: null, message: data?.message}
     }

    return {status: response.status, data: null}

 } catch (error) {
  console.error('Error resetting location:', error);
 }
}
export const handleBinLock = async(accessToken, unitId, lockUntil, binLock = true) => {
  try {
     const baseUrl = getBaseUrl()
     const response = await fetch(`${baseUrl}unit/unit/LockUnit`, {
      method: "PUT",
      headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify({
        unitId, lockUntil, binLock
      })
     })

     return {status: response.status}
  
 } catch (error) {
  console.error('Error fetching data:', error);
 } 
}