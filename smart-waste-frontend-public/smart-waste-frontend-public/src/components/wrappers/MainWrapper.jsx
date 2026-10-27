import React from "react";
import { useSelector } from "react-redux";

const MainWrapper = ({ children, loader }) => {
  const isExpanded = useSelector((state) => state.nav.isExpanded);
  return (
    <main
      className={`min-h-screen w-full pt-[4.1rem] md:pl-[70px] ${
        isExpanded ? "lg:pl-[220px]" : ""
      } bg-themeColors-bgLightColor relative ${
        loader ? "grid place-items-center" : "flex"
      }`}
    >
      {children}
    </main>
  );
};

export default MainWrapper;
