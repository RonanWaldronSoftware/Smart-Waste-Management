import React from "react";
import { useTranslation } from "react-i18next";

const AlertModal = ({ handleModal, data }) => {
  const { t } = useTranslation("translation");

  return (
    <div className="fixed top-0 left-0 w-screen h-screen grid place-items-center p-4 z-50">
      {/* Overlay */}
      <div
        className="absolute top-0 left-0 w-full h-full bg-black bg-opacity-50 z-50"
        onClick={() => handleModal("")}
      ></div>

      {/* Modal */}
      <div
        className="bg-white flex flex-col p-6 rounded z-50 gap-8 w-full h-full max-w-[480px] max-h-[350px] overflow-auto"
        style={{ boxShadow: "0 2px 2px rgba(0,0,0,.25)" }}
      >
        {/* Modal header */}
        <div className="flex items-center justify-between">
          <h3 className="font-medium text-[1.75rem]">{data.label}</h3>

          <button
            className=" text-2xl font-bold p-4 -my-4 -mr-4 ml-auto"
            onClick={() => handleModal("")}
          >
            x
          </button>
        </div>
        {/* Modal body */}

        <div className="flex flex-col gap-6">
          <div className="flex flex-col gap-2">
            <div class="font-bold text-themeColors-dangerColor mt-3">
              {t("description")}
            </div>
            <p>{data.desc}</p>
          </div>
          <div className="flex flex-col gap-2">
            <div class="font-bold text-themeColors-successColor mt-3">
              {t("followUpAction")}
            </div>
            <p>{data.action}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AlertModal;
