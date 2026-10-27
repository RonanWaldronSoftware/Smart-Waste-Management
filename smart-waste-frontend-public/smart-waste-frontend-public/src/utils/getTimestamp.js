export const getTimestamp = () => {
    const currentDate = new Date();
      const formattedTimestamp = currentDate.toLocaleString("hr-HR", {
        day: "2-digit",
        month: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
      });

      return formattedTimestamp
}