import React from "react";
import { useTranslation } from "react-i18next";
import { formatDate } from "../../../../utils/formatDate";

const BinSubscriptions = ({ subscriptionRenewalDate }) => {
  const { t } = useTranslation("translation");

  return (
    <section className=" p-12 pt-5">
      {/* Panel header */}
      <div className="flex items-center justify-between py-6">
        <h2 className=" text-2xl font-medium">{t("subscriptions")}</h2>
      </div>

      {/* Panel content */}
      <div className="flex flex-col">
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("validTo")}</p>
          <div className="w-full">
            <p className="text-black">
              {subscriptionRenewalDate && subscriptionRenewalDate !== ""
                ? formatDate(subscriptionRenewalDate).split(" ")[0]
                : "-"}
            </p>
          </div>
        </div>
        {/* Row */}
        {/* <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("user")}</p>
          <div className="w-full">
            <p className="text-black">User name</p>
          </div>
        </div> */}
      </div>
    </section>
  );
};

export default BinSubscriptions;
