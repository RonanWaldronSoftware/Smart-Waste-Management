import React from "react";
import { useTranslation } from "react-i18next";

const BinReadings = ({ readingsPanelData }) => {
  const { t } = useTranslation("translation");
  return (
    <section className=" p-12 pt-5">
      {/* Panel header */}
      <div className="flex items-center justify-between py-6">
        <h2 className=" text-2xl font-medium">{t("devReadings")}</h2>
      </div>

      {/* Panel content */}
      <div className="flex flex-col">
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("binLevel")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.binLevel}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("binLevelBC")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.binLevelBc}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">Sigfox Alt. ID</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.sigfox}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">
            {t("batteryVoltageReading")}
          </p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.batteryVoltage} V</p>
          </div>
        </div>
        {/* Row */}
        {/* <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">
            {t("binLevelStatus")}
          </p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.binLevelStatus}</p>
          </div>
        </div> */}
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("messageType")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.messageType}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("noCompactions")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.compaction}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("flapOpenings")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.flapOpenings}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">Rsrp</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.rsrp}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">Rsrq</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.rsrq}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">Rssi</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.rssi}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">Snr</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.snr}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">Src</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.src}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("binJustOn")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.binJustOn}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("firmware")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.firmware}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("nbiotSimIssue")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.nbIot}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">
            {t("regularPeriodicReporting")}
          </p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.periodicReport}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("timeDiff")}</p>
          <div className="w-full">
            <p className="text-black">{readingsPanelData.timeDifference}</p>
          </div>
        </div>
      </div>
    </section>
  );
};

export default BinReadings;
