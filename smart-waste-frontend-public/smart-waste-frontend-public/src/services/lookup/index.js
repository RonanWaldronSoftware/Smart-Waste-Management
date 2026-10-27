import { getBaseUrl } from "../../utils/getEnv";

export const getConfig = async( accessToken) => {
    try {
        const url = getBaseUrl();
        const response = await fetch(`${url}account/Configuration`,{
          method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
        });
  
        if (response.ok) {
            const data = await response.json();
            return {data, status: response.status}
        }
        return {status: response.status}
  
      } catch (error) {
        console.error('Error fetching data:', error);
      }
}
export const getNavCount = async( accessToken) => {
    try {
        const url = getBaseUrl();
        const response = await fetch(`${url}account/Configuration/NavigationCount`,{
          method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
        });
  
        if (response.ok) {
            const data = await response.json();
            return {data, status: response.status}
        }
        return {status: response.status}
  
      } catch (error) {
        console.error('Error fetching data:', error);
      }
}
export const getDashboardData = async( accessToken) => {
    try {
        const url = getBaseUrl();
        const response = await fetch(`${url}account/Configuration/dashboard`,{
          method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`
        }
        });
  
        if (response.ok) {
          const data = await response.json();
          return {data, status: response.status}
        }

        let message = null;
        try {
          const errorData = await response.json();
          message = errorData?.message || errorData?.Message || null;
        } catch (e) {
          // Response had no JSON body
        }
        return {status: response.status, data: null, message}

      } catch (error) {
        console.error('Error fetching data:', error);
        return {status: 0, data: null, message: error?.message || 'Network error'};
      }
}
export const exportData = async( accessToken, exportDataTypeId, exportFormatId, dataIds = [], timezoneOffset = 0, from = null, to = null, unitId = null) => {

  try {
        const url = getBaseUrl();
        const response = await fetch(`${url}handler/export`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json", // Add Content-Type header
            Authorization: `Bearer ${accessToken}`
          },
          body: JSON.stringify({
            unitId,
            exportFormatId,
            exportDataTypeId,
            dataIds,
            from, to, timezoneOffset
          })
        });
  
        if (!response.ok) {
          let message = null;
          try {
            const errorData = await response.json();
            message = errorData?.message || errorData?.Message || null;
          } catch (e) {
            // Response had no JSON body
          }
          return { status: response.status, data: null, message };
        }


        if (exportFormatId === 1) {
          // Get PDF
          const blob = await response.blob();
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', 'exported_data.pdf'); // You can specify a different file name here
          document.body.appendChild(link);
          link.click();
          link.remove();
          window.URL.revokeObjectURL(url);
  
          return { status: response.status, data: null, message: 'PDF downloaded successfully' };
        }
        if (exportFormatId === 2) {
          // Handle CSV
          const blob = await response.blob();
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', 'exported_data.xlsx'); // Specify file name
          document.body.appendChild(link);
          link.click();
          link.remove();
          window.URL.revokeObjectURL(url);
  
          return { status: response.status, data: null, message: 'Excel downloaded successfully' };
        }

        return { status: response.status, data: null, message: 'Unsupported export format' };

      } catch (error) {
        console.error('Error fetching data:', error);
        return { status: 0, data: null, message: error?.message || 'Network error' };
      }
}