import React from "react";
import { BinIcon, RecycleIcon, WarningIcon } from "../../../assets";
import { useTranslation } from "react-i18next";

const DashboardCards = ({
  recycleBinsCount,
  warningsCount,
  mixBinsCount,
  fullBins,
  handleRedirect,
}) => {
  const { t } = useTranslation("translation");

  function formatNumber(num) {
    if (1 <= num && num <= 999) {
      return num;
    } else if (1000 <= num && num <= 9999) {
      return num.toLocaleString("hr-HR", { minimumFractionDigits: 0 });
    } else if (10000 <= num && num <= 99999) {
      return num.toLocaleString("hr-HR", { minimumFractionDigits: 0 });
    } else {
      return num;
    }
  }
  return (
    <div className="flex flex-wrap w-full">
      <div className="w-full md:w-1/2 xl:w-1/4 px-4 mb-4 xl:mb-0 min-h-[170px]">
        {/* Card */}
        <article
          className="card bg-white h-full justify-between px-5 pt-5 pb-3 border-b-4 border-solid w-full cursor-pointer border-b-themeColors-successColor"
          onClick={() => handleRedirect("recyclable")}
        >
          <h3 className="uppercase text-xs leading-tight text-themeColors-textColor font-bold mb-3">
            {t("recycle")}
          </h3>

          <div className="flex items-end justify-between">
            <RecycleIcon />
            <p
              className=" font-medium text-themeColors-textColor"
              style={{ fontSize: "2rem" }}
            >
              {recycleBinsCount}
            </p>
          </div>
        </article>
      </div>
      <div className="w-full md:w-1/2 xl:w-1/4 px-4 mb-4 xl:mb-0 min-h-[170px]">
        {/* Card */}
        <article
          className="card bg-white h-full justify-between px-5 pt-5 pb-3 border-b-4 border-solid w-full cursor-pointer border-b-themeColors-bgColor"
          onClick={() => handleRedirect("mixed")}
        >
          <h3 className="uppercase text-xs leading-tight text-themeColors-textColor font-bold mb-3">
            {t("mixed")}
          </h3>

          <div className="flex items-end justify-between">
            <BinIcon />
            <p
              className=" font-medium text-themeColors-textColor"
              style={{ fontSize: "2rem" }}
            >
              {mixBinsCount}
            </p>
          </div>
        </article>
      </div>
      <div className="w-full md:w-1/2 xl:w-1/4 px-4 mb-4 xl:mb-0 min-h-[170px]">
        {/* Card */}
        <article
          className="card bg-white h-full justify-between px-5 pt-5 pb-3 border-b-4 border-solid w-full cursor-pointer border-b-themeColors-dangerColor"
          onClick={() => handleRedirect("alerts")}
        >
          <h3 className="uppercase text-xs leading-tight text-themeColors-textColor font-bold mb-3">
            {t("alerts")}
          </h3>

          <div className="flex items-end justify-between">
            <WarningIcon />
            <p
              className=" font-medium text-themeColors-textColor"
              style={{ fontSize: "2rem" }}
            >
              {warningsCount}
            </p>
          </div>
        </article>
      </div>
      <div className="w-full md:w-1/2 xl:w-1/4 px-4 mb-4 xl:mb-0 min-h-[170px]">
        {/* Card */}
        <article
          className="card bg-white h-full justify-between px-5 pt-5 pb-3 border-b-4 border-solid w-full cursor-pointer border-b-themeColors-dangerColor"
          onClick={() => handleRedirect("capacity")}
        >
          <h3 className="uppercase text-xs leading-tight text-themeColors-textColor font-bold mb-3">
            {t("fullBins")}
          </h3>

          <div className="flex items-end justify-between">
            <RecycleIcon />
            <p className=" font-medium  text-themeColors-successColor text-2xl">
              {fullBins?.recycle.count}
            </p>
            <p className=" font-medium text-themeColors-successColor text-2xl">
              {fullBins?.recycle.liters.toFixed(0)} l
            </p>
          </div>
          <div className="flex items-end justify-between">
            <BinIcon />
            <p className=" font-medium text-themeColors-textColor text-2xl">
              {fullBins?.mixed.count}
            </p>
            <p className=" font-medium text-themeColors-textColor text-2xl">
              {fullBins?.mixed.liters.toFixed(0)} l
            </p>
          </div>
        </article>
      </div>
    </div>
  );
};

export default DashboardCards;
