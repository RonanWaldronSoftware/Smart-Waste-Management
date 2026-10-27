import React from "react";

const TabList = ({ tabs, activeTab, handleTabChange }) => {
  return (
    <ul className="pl-5 border-b border-solid border-themeColors-borderColor flex flex-wrap">
      {tabs.map((tab) => (
        <li
          key={tab.id}
          className={`tab-btn ${activeTab === tab.id ? "active" : ""}`}
          onClick={() => handleTabChange(tab.id)}
        >
          {tab.title}
        </li>
      ))}
    </ul>
  );
};

export default TabList;
