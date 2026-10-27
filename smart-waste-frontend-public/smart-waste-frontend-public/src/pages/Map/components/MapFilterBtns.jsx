import React from "react";
import { useTranslation } from "react-i18next";

const MapFilterBtns = ({ selectedFilter, handleFilterChange }) => {
  const { t } = useTranslation("translation");
  return (
    <div className="flex items-center gap-3">
      <button
        className={`filter-btn ${
          selectedFilter === "fillLevel" ? "active" : ""
        }`}
        onClick={() => handleFilterChange("fillLevel")}
      >
        <svg
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            fillRule="evenodd"
            clipRule="evenodd"
            d="M11 5H14V7H2V5H5L6 4H10L11 5ZM3 18C3 19.1 3.9 20 5 20H11C12.1 20 13 19.1 13 18V8H3V18ZM15 8H22V10H15V8ZM15 16H19V18H15V16ZM21 12H15V14H21V12Z"
            fill="black"
            fillOpacity="0.54"
          ></path>
        </svg>
        {t("fillLevel")}
      </button>
      <button
        className={`filter-btn ${selectedFilter === "battery" ? "active" : ""}`}
        onClick={() => handleFilterChange("battery")}
      >
        <svg
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            fillRule="evenodd"
            clipRule="evenodd"
            d="M14 4H15.67C16.4 4 17 4.6 17 5.33V20.67C17 21.4 16.4 22 15.66 22H8.33C7.6 22 7 21.4 7 20.66V5.33C7 4.6 7.6 4 8.33 4H10V2H14V4ZM11 14.5V20L15 12.5H13V7L9 14.5H11Z"
            fill="black"
            fillOpacity="0.54"
          ></path>
        </svg>
        {t("battery")}
      </button>
      <button
        className={`filter-btn ${selectedFilter === "alerts" ? "active" : ""}`}
        onClick={() => handleFilterChange("alerts")}
      >
        <svg
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M12.09 2.90995C10.08 0.899954 7.07 0.489954 4.65 1.66995L8.99 6.00995L5.99 9.00995L1.65 4.66995C0.480003 7.09995 0.890003 10.09 2.9 12.1C4.76 13.96 7.48 14.45 9.79 13.58L18.9 22.69C19.29 23.08 19.92 23.08 20.31 22.69L22.61 20.39C23 20 23 19.37 22.61 18.98L13.54 9.89995C14.46 7.55995 13.98 4.79995 12.09 2.90995Z"
            fill="black"
            fillOpacity="0.54"
          ></path>
        </svg>
        {t("alerts")}
      </button>
    </div>
  );
};

export default MapFilterBtns;
