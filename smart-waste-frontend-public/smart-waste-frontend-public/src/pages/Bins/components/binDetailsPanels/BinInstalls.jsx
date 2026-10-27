import React from "react";
import { useTranslation } from "react-i18next";
import { formatDate } from "../../../../utils/formatDate";

const BinInstalls = ({ installationData }) => {
  const { t } = useTranslation("translation");
  return (
    <section className=" p-12 pt-5">
      {/* Panel header */}
      <div className="flex items-center justify-between py-6">
        <h2 className=" text-2xl font-medium">{t("installs")}</h2>
      </div>

      {/* Panel content */}
      <div className="flex flex-col">
        {/* Row */}
        {/*  <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("image")}</p>
          <div className="w-full">
            <img src={logo} />
          </div>
        </div> */}
        {/* Row */}
        {installationData ? (
          <div className="flex flex-col gap-3 border-b border-solid border-b-[#e9ecef] py-4">
            <div className="flex items-center gap-6">
              <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("date")}</p>
              <div className="w-full">
                <p className="text-black">
                  {formatDate(installationData.date) || "-"}
                </p>
              </div>
            </div>
            {installationData.image && installationData.image !== "" && (
              <div className="w-full max-w-md">
                <img src={installationData.image} className="w-full" />
              </div>
            )}
          </div>
        ) : (
          <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
            <div className="w-full">
              <p className="text-black">{t("noInstalls")}</p>
            </div>
          </div>
        )}
        {/* Row */}
        {/* <div className="flex items-center gap-6 py-4">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("checklist")}</p>
          <div className="w-full">
            <ul className=" list-disc">
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
            </ul>
          </div>
        </div> */}
      </div>

      {/* <div className="flex items-center justify-between py-6 mt-12">
        <h2 className=" text-2xl font-medium">{t("uninstalls")}</h2>
      </div>

     
      <div className="flex flex-col">
       
       <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("image")}</p>
          <div className="w-full">
            <img src={logo} />
          </div>
        </div>
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("date")}</p>
          <div className="w-full">
            <p className="text-black">25/08/2025</p>
          </div>
        </div>
        <div className="flex items-center gap-6 py-4">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("checklist")}</p>
          <div className="w-full">
            <ul className=" list-disc">
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
              <li>List item</li>
            </ul>
          </div>
        </div> 
      </div>*/}
    </section>
  );
};

export default BinInstalls;
