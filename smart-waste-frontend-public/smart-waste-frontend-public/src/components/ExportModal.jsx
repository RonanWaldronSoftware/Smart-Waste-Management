import React, { useState } from "react";
import { FaFileExcel, FaFilePdf } from "react-icons/fa";
import Loader from "./Loader";
import { useTranslation } from "react-i18next";
import { exportData } from "../services/lookup";
import { toast } from "react-toastify";

const ExportModal = ({
  binId,
  handleModal,
  data,
  resetExportData,
  exportDataId,
  exportDates,
}) => {
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const { t } = useTranslation("translation");
  const [submitting, setSubmitting] = useState(false);

  const handleExport = async (exporFormatId) => {
    setSubmitting(true);
    const date = new Date();
    const offsetInHrs = date.getTimezoneOffset() / 60;
    try {
      let reqData;
      if (exportDataId === 3) {
        if (exportDates.from && exportDates.to) {
          reqData = await exportData(
            accessToken,
            exportDataId,
            exporFormatId,
            data,
            offsetInHrs,
            exportDates.from,
            exportDates.to,
            binId
          );
        } else {
          reqData = await exportData(
            accessToken,
            exportDataId,
            exporFormatId,
            data,
            offsetInHrs
          );
        }
      } else {
        reqData = await exportData(
          accessToken,
          exportDataId,
          exporFormatId,
          data,
          offsetInHrs
        );
      }
      if (reqData?.status === 200) {
        resetExportData();
      } else {
        toast.error(t("requestError"));
      }
    } catch (error) {
      console.error(error);
      toast.error(t("requestError"));
    } finally {
      setSubmitting(false);
    }
  };

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
          <h3 className="font-medium text-[1.75rem]">{t("exportAs")}</h3>

          <button
            className=" text-2xl font-bold p-4 -my-4 -mr-4 ml-auto"
            onClick={() => handleModal("")}
          >
            x
          </button>
        </div>
        {/* Modal body */}
        {!submitting ? (
          <div className="flex justify-center gap-6">
            <button
              onClick={() => handleExport(2)}
              className=" w-48 h-48 flex flex-col items-center justify-center gap-6 bg-themeColors-bgLightColor text-2xl font-medium rounded group hover:bg-themeColors-successColor"
            >
              <FaFileExcel className=" text-themeColors-successColor group-hover:text-white" />
              <p className="text-[#212529] group-hover:text-white">Excel</p>
            </button>
            <button
              onClick={() => handleExport(1)}
              className=" w-48 h-48 flex flex-col items-center justify-center gap-6 bg-themeColors-bgLightColor text-2xl font-medium rounded group hover:bg-themeColors-dangerColor"
            >
              <FaFilePdf className=" text-themeColors-dangerColor group-hover:text-white" />
              <p className="text-[#212529] group-hover:text-white">Pdf</p>
            </button>
          </div>
        ) : (
          <Loader />
        )}

        {/*  {modalType === "bins-export" && isPdfRendering && (
          <PDFDownloadLink
            document={<BinsDoc data={data} />}
            fileName="units.pdf"
            className="btn btn-primary"
          >
            {({ blob, url, loading, error }) =>
              loading ? <Loader /> : t("downloadNow")
            }
          </PDFDownloadLink>
        )} 
        {modalType === "bins-export" && isExcelRendering && (
          <CSVLink
            data={excelData}
            headers={excelHeaders}
            className="btn btn-primary"
            filename="UnitsReport.csv"
          >
            {" "}
            {t("downloadNow")}
          </CSVLink>
        )}
        {modalType === "users-export" && isPdfRendering && (
          <PDFDownloadLink
            document={<UsersDoc data={data} />}
            fileName="users.pdf"
            className="btn btn-primary"
          >
            {({ blob, url, loading, error }) =>
              loading ? <Loader /> : t("downloadNow")
            }
          </PDFDownloadLink>
        )}
        {modalType === "users-export" && isExcelRendering && (
          <CSVLink
            data={excelData}
            headers={excelHeaders}
            className="btn btn-primary"
            filename="usersReport.csv"
          >
            {" "}
            {t("downloadNow")}
          </CSVLink>
        )}
        {modalType === "history-export" && isPdfRendering && (
          <PDFDownloadLink
            document={<HistoryDoc data={data} />}
            fileName="history-readings.pdf"
            className="btn btn-primary"
          >
            {({ blob, url, loading, error }) =>
              loading ? <Loader /> : t("downloadNow")
            }
          </PDFDownloadLink>
        )}
        {modalType === "history-export" && isExcelRendering && (
          <CSVLink
            data={excelData}
            headers={excelHeaders}
            className="btn btn-primary"
            filename={"history-readings.csv"}
            onClick={resetExportData}
          >
            {" "}
            {t("downloadNow")}
          </CSVLink>
        )}*/}
      </div>
    </div>
  );
};

export default ExportModal;
