import React, { useEffect, useState } from "react";
import { Loader, MainWrapper } from "../../components";
import { BiSave } from "react-icons/bi";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { logout } from "../../store/userSlice";
import { getSystemDetails, saveSystemDetails } from "../../services/system";
import { toast } from "react-toastify";
import { useTranslation } from "react-i18next";

const System = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const accessToken = useSelector((state) => state.user.accessToken);
  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [systemData, setSystemData] = useState(null);
  const [formData, setFormData] = useState(null);

  //data fetching
  useEffect(() => {
    fetchData();
  }, []);

  //refactor data
  useEffect(() => {
    if (systemData) {
      const refactoredData = systemData.reduce((acc, curr) => {
        acc[curr.key] = curr.value;

        return acc;
      }, {});

      setFormData(refactoredData);
      setLoading(false);
    }
  }, [systemData]);

  //function for fetching data
  const fetchData = async () => {
    try {
      const data = await getSystemDetails(accessToken);
      if (data === null) {
        // Perform the logout and redirect
        dispatch(logout());
        navigate("/login");
      } else {
        setSystemData(data);
      }
    } catch (error) {
      setLoading(false);
      console.log(error);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };
  const handleFormSubmit = async (e) => {
    e.preventDefault();

    const refactoredArray = Object.keys(formData).map((key) => {
      return { key: key, value: formData[key] };
    });
    try {
      // Create an array of promises by calling saveSystemDetails for each item
      const promises = refactoredArray.map((item) =>
        saveSystemDetails(accessToken, item)
      );

      // Wait for all promises to resolve using Promise.all
      const results = await Promise.all(promises);

      // Check if all requests were successful
      if (results.every((result) => result === true)) {
        // All requests were successful
        toast.success(t("systemConfigurationUpdated"));
      } else {
        toast.error(t("requestError"));
      }
    } catch (error) {
      console.error("Error:", error);
      toast.error(t("requestError"));
    }
    //refactoredArray.forEach((item) => saveSystemDetails(accessToken, item));
  };

  if (loading) {
    return (
      <MainWrapper loader={true}>
        <Loader />
      </MainWrapper>
    );
  }

  return (
    <MainWrapper>
      <div className="flex flex-col mx-auto w-full">
        {/* Header */}
        <div className="flex items-center justify-between bg-white py-3 px-6 mb-6">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor">
            {t("settings")}
          </h1>

          {/* Additional filters */}
          <div className="flex items-center gap-2">
            <button
              className="btn btn-primary flex items-center"
              onClick={handleFormSubmit}
            >
              <BiSave />
              <span>{t("save")}</span>
            </button>
          </div>
        </div>

        {/* Content */}
        <div className="px-6 flex flex-col gap-4 mb-4">
          <div className="card p-5 flex flex-col gap-4 bg-white w-full max-w-[385px] mx-auto">
            <div className=" my-6">
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="MobileVersion">{t("mobileVersion")}</label>
                <input
                  type="text"
                  id="MobileVersion"
                  name="MobileVersion"
                  className="input"
                  value={formData.MobileVersion}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMTP-HOST">{t("host")}</label>
                <input
                  type="text"
                  id="SMTP-HOST"
                  name="SMTP-HOST"
                  className="input"
                  value={formData["SMTP-HOST"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMTP-PORT">{t("port")}</label>
                <input
                  type="text"
                  id="SMTP-PORT"
                  name="SMTP-PORT"
                  className="input"
                  value={formData["SMTP-PORT"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMTP-USERNAME">{t("smtpUsername")}</label>
                <input
                  type="text"
                  id="SMTP-USERNAME"
                  name="SMTP-USERNAME"
                  className="input"
                  value={formData["SMTP-USERNAME"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMTP-PASSWORD">{t("smtpPassword")}</label>
                <input
                  type="text"
                  id="SMTP-PASSWORD"
                  name="SMTP-PASSWORD"
                  className="input"
                  value={formData["SMTP-PASSWORD"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMS-DLR-URL">{t("smsDlrUrl")}</label>
                <input
                  type="text"
                  id="SMS-DLR-URL"
                  name="SMS-DLR-URL"
                  className="input"
                  value={formData["SMS-DLR-URL"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMS-URL">{t("smsUrl")}</label>
                <input
                  type="text"
                  id="SMS-URL"
                  name="SMS-URL"
                  className="input"
                  value={formData["SMS-URL"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMS-USERNAME">{t("smsUsername")}</label>
                <input
                  type="text"
                  id="SMS-USERNAME"
                  name="SMS-USERNAME"
                  className="input"
                  value={formData["SMS-USERNAME"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="SMS-PASSWORD">{t("smsPassword")}</label>
                <input
                  type="text"
                  id="SMS-PASSWORD"
                  name="SMS-PASSWORD"
                  className="input"
                  value={formData["SMS-PASSWORD"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2 mb-4">
                <label htmlFor="FCM-API-URL">Fcm api url</label>
                <input
                  type="text"
                  id="FCM-API-URL"
                  name="FCM-API-URL"
                  className="input"
                  value={formData["FCM-API-URL"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
              <div className="flex flex-col gap-2">
                <label htmlFor="FCM-AUTH-KEY">Fcm auth key</label>
                <input
                  type="text"
                  id="FCM-AUTH-KEY"
                  name="FCM-AUTH-KEY"
                  className="input"
                  value={formData["FCM-AUTH-KEY"]}
                  onChange={handleInputChange}
                />
                {/* {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} */}
              </div>
            </div>
          </div>
        </div>
      </div>
    </MainWrapper>
  );
};

export default System;
