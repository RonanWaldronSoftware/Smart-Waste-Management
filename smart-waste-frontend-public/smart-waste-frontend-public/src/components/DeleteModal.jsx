import React from "react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import LoaderSmall from "./LoaderSmall";

const DeleteModal = ({ handleCancel, handleConfirm, submitting }) => {
  const { t } = useTranslation("translation");
  const isExpanded = useSelector((state) => state.nav.isExpanded);

  return (
    <div
      className="fixed top-0 left-0 w-screen h-screen grid place-items-center p-4"
      style={{ paddingLeft: isExpanded ? "240px" : "80px" }}
    >
      {/* Overlay */}
      <div
        className="absolute top-0 left-0 w-full h-full bg-black bg-opacity-50 z-30"
        onClick={handleCancel}
      ></div>

      {/* Modal */}
      <div
        className="bg-white flex flex-col p-6 rounded z-50 gap-8 w-full h-fit max-w-[500px] max-h-[300px] overflow-auto"
        style={{ boxShadow: "0 2px 2px rgba(0,0,0,.25)" }}
      >
        {/* Modal header */}
        <div className="flex items-center justify-between">
          <h3 className="font-medium text-[1.75rem] text-themeColors-dangerColor flex items-centerg gap-4">
            {t("areYouSure")}
            {submitting && <LoaderSmall />}
          </h3>

          <button
            className=" text-2xl font-bold p-4 -my-4 -mr-4 ml-auto"
            onClick={handleCancel}
          >
            x
          </button>
        </div>

        {/* Modal body */}
        <div className="flex flex-col gap-6">
          <p className="text-left">{t("actionPermanent")}</p>

          {/* Separator */}
          <span className="block col-span-1 md:col-span-2 lg:col-span-3 border-t border-solid border-themeColors-bgLightColor"></span>

          <div className="flex  col-span-1 md:col-span-2 lg:col-span-3 items-center justify-end gap-3">
            <button className="btn btn-secondary" onClick={handleCancel}>
              {t("back")}
            </button>
            <button
              onClick={handleConfirm}
              className="btn btn-primary"
              disabled={submitting}
              style={{ opacity: submitting ? 0.5 : 1 }}
            >
              {t("confirm")}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DeleteModal;
