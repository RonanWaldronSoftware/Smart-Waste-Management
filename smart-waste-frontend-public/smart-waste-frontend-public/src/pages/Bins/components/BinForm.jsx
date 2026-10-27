import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Link } from "react-router-dom";
import BinFormMap from "../../../components/maps/BinFormMap";
import { useTranslation } from "react-i18next";
import LoaderSmall from "../../../components/LoaderSmall";

const BinForm = ({
  owners,
  models,
  deviceTypes,
  deviceInputTypes = [],
  wasteTypes,
  formData,
  formErrors,
  loading,
  setFormData,
  handleInputChange,
  handleSelectChange,
  handleSubmit,
}) => {
  const { t } = useTranslation("translation");
  const userData = useSelector((state) => state.user.userData);

  const [pinPosition, setPinPosition] = useState(null);

  const handleMapClick = (event) => {
    const { lat, lng } = event.latLng.toJSON();
    setPinPosition({ lat, lng });
    setFormData((prev) => ({ ...prev, latitude: lat, longitude: lng }));
  };

  useEffect(() => {
    if (formData.latitude !== "" && formData.longitude !== "") {
      setPinPosition({
        lat: Number(formData.latitude),
        lng: Number(formData.longitude),
      });
    }
  }, [formData.latitude, formData.longitude]);

  return (
    <form
      onSubmit={handleSubmit}
      className="card p-5 flex flex-col gap-4 bg-white"
    >
      <h5 className="border-b border-solid capitalize border-b-themeColors-borderColor pb-4 mb-6 text-xl font-medium">
        {t("data")}
      </h5>

      <div className="flex gap-6">
        <div className="flex flex-col gap-6 w-full md:w-1/2">
          {/* Form input wrapper */}
          <div className="flex flex-col gap-2">
            <label htmlFor="owner">{t("customer")}</label>
            <select
              name="owner"
              id="owner"
              value={formData.owner}
              onChange={(e) => handleSelectChange(e, "owner")}
              className="input"
              disabled={userData?.role?.id === 5}
            >
              {owners.map((owner) => (
                <option key={owner.id} value={owner.id}>
                  {owner.name}
                </option>
              ))}
            </select>
          </div>
          <div className="flex flex-col gap-2">
            <label htmlFor="serialNo">{t("serialNumber")}</label>
            {/* Input wrapper */}
            <div className="relative flex">
              <span
                className="py-[.375rem] px-3 text-base text-center border border-solid border-themeColors-borderColor border-r-0 rounded-tl rounded-bl"
                style={{ color: "#495057" }}
              >
                #
              </span>
              <input
                type="text"
                id="serialNo"
                name="serialNo"
                placeholder="Serial number"
                className="input"
                value={formData.serialNo}
                onChange={handleInputChange}
                disabled={userData?.role?.id !== 1}
              />
            </div>
            {formErrors.serialNoErrorMsg && (
              <p>{formErrors.serialNoErrorMsg}</p>
            )}
          </div>
          {userData?.role?.id === 1 && (
            <div className="flex flex-col gap-2">
              <label htmlFor="subscription">{t("subscriptions")}</label>
              <select
                name="subscription"
                id="subscription"
                className="input"
                value={formData.subscription}
                onChange={handleInputChange}
              >
                <option value="">{t("selectSubscriptionPeriod")}</option>
                <option value="12">12 {t("months")}</option>
                <option value="24">24 {t("months")}</option>
                <option value="36">36 {t("months")}</option>
                <option value="48">48 {t("months")}</option>
                <option value="custom">{t("custom")}</option>
              </select>

              {/* {formData.subscription === "custom" && (
              <>
                <label htmlFor="customSubscriptionFrom">
                  {t("selectSubscriptionPeriod")}
                </label>
                <div className="flex gap-1">
                  <input
                    type="date"
                    name="customSubscription"
                    id="customSubscriptionFrom"
                    className="input"
                    value={formData.customSubscription.from}
                    onChange={handleInputChange}
                  />
                  <input
                    type="date"
                    name="customSubscription"
                    id="customSubscriptionTo"
                    className="input"
                    value={formData.customSubscription.to}
                    onChange={handleInputChange}
                  />
                </div>
              </>
            )}
            {formErrors.customSubscriptionErrorMsg && (
              <p>{formErrors.customSubscriptionErrorMsg}</p>
            )} */}

              {formData.subscription === "custom" && (
                <>
                  <label htmlFor="customSubscriptionFrom">
                    Select renewal date
                  </label>
                  <div className="flex gap-1">
                    <input
                      type="date"
                      name="renewalDate"
                      id="renewalDate"
                      className="input"
                      value={formData.renewalDate}
                      onChange={handleInputChange}
                    />
                  </div>
                </>
              )}
              {formErrors.renewalDateErrorMsg && (
                <p>{formErrors.renewalDateErrorMsg}</p>
              )}
            </div>
          )}
          <div className="flex flex-col gap-2">
            <label htmlFor="location">{t("location")}</label>
            <input
              type="text"
              id="location"
              name="location"
              placeholder="Location"
              className="input"
              value={formData.location}
              onChange={handleInputChange}
              disabled={userData?.role?.id === 5}
            />
            {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )}
          </div>
          {/* <div className="flex flex-col gap-2">
            <label htmlFor="sigfox">Sigfox Alt. ID</label>
            <input
              type="text"
              id="sigfox"
              name="sigfox"
              placeholder="Sigfox Alt. ID"
              className="input"
              value={formData.sigfox}
              onChange={handleInputChange}
              disabled={userData.role.id === 5}
            />
            {formErrors.sigfoxErrorMsg && <p>{formErrors.sigfoxErrorMsg}</p>}
          </div> */}
          <div className="flex flex-col gap-2">
            <label htmlFor="latitude">{t("latitude")}</label>
            <input
              type="text"
              id="latitude"
              name="latitude"
              placeholder="Latitude"
              className="input"
              value={formData.latitude}
              onChange={handleInputChange}
              disabled={userData?.role?.id === 5}
            />
          </div>
          <div className="flex flex-col gap-2">
            <label htmlFor="longitude">{t("longitude")}</label>
            <input
              type="text"
              id="longitude"
              name="longitude"
              placeholder="Longitude"
              className="input"
              value={formData.longitude}
              onChange={handleInputChange}
              disabled={userData?.role?.id === 5}
            />
          </div>
        </div>
        <div className="flex flex-col gap-6 w-full md:w-1/2">
          <div className="flex flex-col gap-2">
            <label htmlFor="model">{t("binModel")}</label>
            <select
              name="model"
              id="model"
              className="input"
              value={formData.model}
              onChange={(e) => handleSelectChange(e, "model")}
              disabled={userData?.role?.id === 5}
            >
              {models.map((model) => (
                <option key={model.id} value={model.id}>
                  {model.name}
                </option>
              ))}
            </select>
          </div>
          <div className="flex flex-col gap-2">
            <label htmlFor="deviceType">{t("deviceType")}</label>
            <select
              name="deviceType"
              id="deviceType"
              className="input"
              value={formData.deviceType}
              onChange={(e) => handleSelectChange(e, "deviceType")}
              disabled={userData?.role?.id === 5}
            >
              {deviceTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>
          </div>
          <div className="flex flex-col gap-2">
            <label htmlFor="wasteType">{t("wasteType")}</label>
            <select
              name="wasteType"
              id="wasteType"
              className="input"
              value={formData.wasteType}
              onChange={(e) => handleSelectChange(e, "wasteType")}
              disabled={userData?.role?.id === 5}
            >
              {wasteTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>
          </div>
          <div className="flex flex-col gap-2">
            <label htmlFor="deviceInputType">{t("deviceInputType")}</label>
            <select
              name="deviceInputType"
              id="deviceInputType"
              className="input"
              value={formData.deviceInputType}
              onChange={(e) => handleSelectChange(e, "deviceInputType")}
              disabled={userData?.role?.id === 5}
            >
              {deviceInputTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <h5 className="border-b border-solid border-b-themeColors-borderColor pb-4 my-6 text-xl font-medium">
        {t("binCapacityIndicator")}
      </h5>
      <div className="flex gap-6">
        <div className="flex flex-col gap-6 w-full md:w-1/2">
          {/* Form input wrapper */}
          <div className="flex flex-col gap-2">
            <div className="flex items-center gap-5">
              <div className="flex items-center mb-4">
                <input
                  id="default"
                  type="radio"
                  value="default"
                  checked={formData.capacity === "default"}
                  onChange={(e) => {
                    setFormData((prev) => ({
                      ...prev,
                      capacityLow: 40,
                      capacityHigh: 80,
                    }));
                    handleInputChange(e);
                  }}
                  name="capacity"
                  disabled={userData?.role?.id === 5}
                  className="w-4 h-4 text-themeColors-accentColor bg-gray-100 border-gray-300 focus:ring-themeColors-accentColor accent-themeColors-accentColor focus:ring-1"
                />
                <label htmlFor="default" className="ml-2">
                  {t("modelDefault")}
                </label>
              </div>
              <div className="flex items-center mb-4">
                <input
                  id="custom"
                  type="radio"
                  value="custom"
                  checked={formData.capacity === "custom"}
                  onChange={handleInputChange}
                  name="capacity"
                  disabled={userData?.role?.id === 5}
                  className="w-4 h-4 text-themeColors-accentColor bg-gray-100 border-gray-300 focus:ring-themeColors-accentColor accent-themeColors-accentColor focus:ring-1"
                />
                <label htmlFor="custom" className="ml-2">
                  {t("custom")}
                </label>
              </div>
            </div>

            <div className="flex items-center gap-5">
              <div className="flex flex-col gap-2">
                <label htmlFor="capacityLow" className="ml-2">
                  {t("low")}
                </label>
                <div className="relative flex w-[115px]">
                  <input
                    type="number"
                    id="capacityLow"
                    name="capacityLow"
                    className="input disabled:opacity-70 disabled:bg-themeColors-borderColor"
                    value={formData.capacityLow}
                    onChange={handleInputChange}
                    disabled={
                      formData.capacity === "default" || userData?.role?.id === 5
                    }
                  />
                  <span
                    className="py-[.375rem] px-3 text-base text-center border border-solid border-themeColors-borderColor border-l-0 rounded-tr rounded-br"
                    style={{ color: "#495057" }}
                  >
                    %
                  </span>
                </div>
              </div>
              <div className="flex flex-col gap-2">
                <label htmlFor="capacityHigh" className="ml-2">
                  {t("high")}
                </label>
                <div className="relative flex w-[115px]">
                  <input
                    type="number"
                    id="capacityHigh"
                    name="capacityHigh"
                    className="input disabled:opacity-70 disabled:bg-themeColors-borderColor"
                    value={formData.capacityHigh}
                    onChange={handleInputChange}
                    disabled={
                      formData.capacity === "default" || userData?.role?.id === 5
                    }
                  />
                  <span
                    className="py-[.375rem] px-3 text-base text-center border border-solid border-themeColors-borderColor border-l-0 rounded-tr rounded-br"
                    style={{ color: "#495057" }}
                  >
                    %
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <h5 className="border-b border-solid border-b-themeColors-borderColor pb-4 my-6 text-xl font-medium">
        {t("location")}
      </h5>
      <div className="w-full h-80">
        <BinFormMap pinPosition={pinPosition} handleMapClick={handleMapClick} />
      </div>

      <div className="flex items-center justify-end gap-3 pt-4 border-t border-solid border-themeColors-bgLightColor px-4">
        <Link to="/bins" className="btn btn-secondary">
          {t("back")}
        </Link>
        <button className="btn btn-primary min-w-[80px] h-full min-h-[38px]">
          {loading ? <LoaderSmall /> : t("save")}
        </button>
      </div>
    </form>
  );
};

export default BinForm;
