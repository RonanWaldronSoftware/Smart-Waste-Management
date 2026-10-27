import React from "react";
import { useTranslation } from "react-i18next";

const GraphCards = ({ handleExternalPieHover }) => {
  const { t } = useTranslation("translation");
  return (
    <div className="p-5 lg:pl-0 w-full h-full flex flex-col gap-5">
      <div
        className="card text-white bg-themeColors-successColor  cursor-pointer"
        onMouseEnter={() => handleExternalPieHover(0)}
      >
        <h5 className=" px-5 py-3 bg-black bg-opacity-5 text-2xl font-medium leading-tight">
          {t("low")}
        </h5>
        <p className="p-5 text-sm font-light">
          {t("binIsEmptyAndNoNeedToTakeCareAboutIt")}
        </p>
      </div>
      <div
        className="card text-white bg-themeColors-warningColor cursor-pointer"
        onMouseEnter={() => handleExternalPieHover(1)}
      >
        <h5 className=" px-5 py-3 bg-black bg-opacity-5 text-2xl font-medium leading-tight">
          {t("medium")}
        </h5>
        <p className="p-5 text-sm font-light">
          {t("binIsStartingToFillAndSoonWIllNeedToBeEmptied")}
        </p>
      </div>
      <div
        className="card text-white bg-themeColors-dangerColor cursor-pointer"
        onMouseEnter={() => handleExternalPieHover(2)}
      >
        <h5 className=" px-5 py-3 bg-black bg-opacity-5 text-2xl font-medium leading-tight">
          {t("high")}
        </h5>
        <p className="p-5 text-sm font-light">
          {t("binIsFullAndSomeoneShouldBeSentToEmptyIt")}
        </p>
      </div>
    </div>
  );
};

export default GraphCards;
