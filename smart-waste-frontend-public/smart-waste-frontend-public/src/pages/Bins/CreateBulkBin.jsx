import React, { useState, useEffect } from "react";
import { FaSave } from "react-icons/fa";
import BinFormMap from "../../components/maps/BinFormMap";
import { Loader, MainWrapper } from "../../components";
import { useSelector } from "react-redux";
import { checkSerialNumber, createBin } from "../../services/units";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { getPossibleParents } from "../../services/users";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import LoaderSmall from "../../components/LoaderSmall";

const CreateBulkBin = () => {
  const navigate = useNavigate();
  const accessToken = useSelector((state) => state.user.accessToken);
  const configData = useSelector((state) => state.user.configData);
  const { t } = useTranslation("translation");

  const [owners, setOwners] = useState([]);
  const [formErrors, setFormErrors] = useState({});
  const [loading, setLoading] = useState(true);
  const [formSubmitting, setFormSubmitting] = useState(false);

  const [formData, setFormData] = useState({
    owner: "",
    serialNo: [""],
    /* subscription: "12",
    customSubscription: {
      from: "",
      to: "",
    }, */
    location: "",
    latitude: "",
    longitude: "",
    model: "",
    deviceType: "",
    wasteType: "",
    capacity: "default",
    capacityLow: 40,
    capacityHigh: 80,
  });

  useEffect(() => {
    fetchOwners();
  }, []);

  //fill form data when select options are fetched and set
  useEffect(() => {
    if (owners && owners.length > 0) {
      setFormData((prev) => ({
        ...prev,
        owner: owners[0].id,
        model: configData.unitModelTypes[0].id,
        deviceType: 2,
        wasteType: configData.contentTypes[0].id,
      }));
    }
  }, [owners]);

  const [pinPosition, setPinPosition] = useState(null);
  useEffect(() => {
    if (formData.latitude !== "" && formData.longitude !== "") {
      setPinPosition({
        lat: Number(formData.latitude),
        lng: Number(formData.longitude),
      });
    }
  }, [formData.latitude, formData.longitude]);

  //add new empty input when the last serial number lenght is greater than 3
  useEffect(() => {
    if (
      formData.serialNo.length > 0 &&
      formData.serialNo[formData.serialNo.length - 1].length >= 3
    ) {
      formData.serialNo.push("");
    }
  }, [formData.serialNo]);

  const fetchOwners = async () => {
    try {
      const data = await fetchWithRetry(
        getPossibleParents,
        accessToken,
        navigate,
        t,
        2
      );
      if (data.status === 200) {
        setOwners(data.data);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleMapClick = (event) => {
    const { lat, lng } = event.latLng.toJSON();
    setPinPosition({ lat, lng });
    setFormData((prev) => ({ ...prev, latitude: lat, longitude: lng }));
  };

  const handleInputChange = (e) => {
    const { name, value, id } = e.target;
    let newData = {};

    if (name === "customSubscription") {
      newData = {
        customSubscription: {
          from:
            id === "customSubscriptionFrom"
              ? value
              : formData.customSubscription.from,
          to:
            id === "customSubscriptionTo"
              ? value
              : formData.customSubscription.to,
        },
      };
      setFormErrors((prev) => ({ ...prev, customSubscriptionErrorMsg: null }));
    } else {
      newData = { [name]: value };
      setFormErrors((prev) => ({
        ...prev,
        [`${name}ErrorMsg`]: null,
      }));
    }

    setFormData((prev) => ({
      ...prev,
      ...newData,
    }));
  };
  const handleSelectChange = (e, selectType) => {
    const { name, value } = e.target;
    let selectedOption;
    switch (selectType) {
      case "owner":
        selectedOption = owners.find((option) => option.id === Number(value));
        break;
      case "model":
        selectedOption = configData.unitModelTypes.find(
          (option) => option.id === Number(value)
        );
        break;
      case "deviceType":
        selectedOption = configData.unitTypes.find(
          (option) => option.id === Number(value)
        );

        break;
      case "wasteType":
        selectedOption = configData.contentTypes.find(
          (option) => option.id === Number(value)
        );

        break;

      default:
        break;
    }
    setFormData((prevData) => ({
      ...prevData,
      [name]: selectedOption.id,
    }));
  };
  const handleSerialNumberInputs = (index, value) => {
    const updatedSerialNos = [...formData.serialNo];
    updatedSerialNos[index] = value;

    /*    if (value.length > 3) {
      updatedSerialNos.push("");
    } */

    if (hasDuplicates(updatedSerialNos.filter((no) => no !== ""))) {
      toast.error(t("serialDuplicated"));
      return false;
    }

    setFormData({
      ...formData,
      serialNo: updatedSerialNos,
    });
  };

  const handleRemoveInput = (index) => {
    const updatedSerialNos = formData.serialNo.filter((_, i) => i !== index);
    setFormData({
      ...formData,
      serialNo: updatedSerialNos,
    });
  };
  const handleSubmit = async (e) => {
    e.preventDefault();
    setFormSubmitting(true);

    const refactoredFormData = {
      ...formData,
      serialNo: formData.serialNo.filter((item) => item !== ""),
    };
    const isValidated = await validateForm(refactoredFormData);
    /*  const doesSerianNoExists = validateSerialNo(
      refactoredFormData.serialNo,
      true
    );
    if (doesSerianNoExists) {
      toast.error(t("serialDuplicated"));
      return;
    } */
    if (isValidated) {
      /* "{\n  \"binType\": {\n    \"emptyLevel\": 40,\n    \"fullLevel\": 90,\n    \"id\": 1,\n    \"name\": \"120 Model\"\n  },\n  \"contentType\": {\n    \"id\": 1,\n    \"name\": \"Mixed Waste\"\n  },\n  \"deviceType\": {\n    \"id\": 1,\n    \"name\": \"Non-Compacting\"\n  },\n  \"emptyLevel\": 40,\n  \"fullLevel\": 90,\n  \"id\": 0,\n  \"insertBy\": 290,\n  \"insertDate\": \"2023-08-24T10:49:11.334453Z\",\n  \"lastActivity\": \"0001-01-01T00:00:00Z\",\n  \"latitude\": 45.79952129265749,\n  \"location\": \"test\",\n  \"longitude\": 15.992790222167951,\n  \"modifiedBy\": 0,\n  \"modifiedDate\": \"0001-01-01T00:00:00Z\",\n  \"owner\": {\n    \"binLevelAlert\": 0,\n    \"id\": 151,\n    \"insertBy\": 148,\n    \"modifiedBy\": 1,\n    \"name\": \"Serial Number Storage\",\n    \"email\": null\n  },\n  \"reading100percent\": 0,\n  \"reading40percent\": 0,\n  \"serialNo\": \"test34243\",\n  \"useBinTypeLevel\": false,\n  \"status\": null,\n  \"sigfoxAltId\": null\n}" */
      const binObj = {
        userId: refactoredFormData.owner,
        serialNumbers: refactoredFormData.serialNo,
        unitModelTypeId: refactoredFormData.model,
        location: refactoredFormData.location,
        latitude: refactoredFormData.latitude,
        longitude: refactoredFormData.longitude,
        unitTypeId: refactoredFormData.deviceType,
        contentTypeId: refactoredFormData.wasteType,
        emptyLevel:
          refactoredFormData.capacity === "default"
            ? 40
            : refactoredFormData.capacityLow,
        fullLevel:
          refactoredFormData.capacity === "default"
            ? 80
            : refactoredFormData.capacityHigh,
        subscriptionRenewalDate: null,
      };
      try {
        const res = await fetchWithRetry(
          createBin,
          accessToken,
          navigate,
          t,
          binObj
        );
        if (res.status === 200) {
          toast.success(t("newUnitSuccessfullyCreated"));
          setFormSubmitting(false);
          navigate("/bins");
        } else {
          setFormSubmitting(false);
        }
      } catch (error) {
        toast.error(t("requestError"));
        console.error(error);
      }
    } else {
      setFormSubmitting(false);
      return;
    }
  };

  const validateForm = async (data) => {
    const errors = {};

    if (data.serialNo.length === 0) {
      //errors.serialNoErrorMsg = "Serial number needs to be defined";
      toast.error(t("addAtLeastOneSerialNumber"));
      return false;
    }
    if (data.serialNo.some((serialNo) => serialNo.length < 8)) {
      //errors.serialNoErrorMsg =
      //"All serial numbers must be at least 8 characters long";
      toast.error("All serial numbers must be at least 8 characters long");
      return false;
    }
    if (data.location === "") {
      //errors.locationErrorMsg = "Location needs to be defined";
      toast.error(t("locationFieldIsRequired"));
      return false;
    }
    if (data.latitude === "" || data.longitude === "") {
      //errors.locationErrorMsg = "Please place a pin on the map";
      toast.error(t("pleaseSelectPinOnGoogleMap"));
      return false;
    }

    for (const no of data.serialNo) {
      const isValid = await validateSerialNo(no);
      if (!isValid) {
        return false;
      }
    }

    return true;

    /* if (
      data.subscription === "custom" &&
      (data.customSubscription.from === "" || data.customSubscription.to === "")
    ) {
      errors.customSubscriptionErrorMsg =
        "Start and end date of subscription needs to be defined";
    } */

    /*  setFormErrors((prev) => ({
      ...prev,
      ...errors,
    })); */

    //return Object.keys(errors).length === 0;
  };
  const hasDuplicates = (array) => {
    const set = new Set(array);
    return set.size !== array.length;
  };
  const validateSerialNo = async (serialNo) => {
    const data = await checkSerialNumber(accessToken, serialNo);
    if (data.status !== 200) {
      toast.error(serialNo + " " + t("serialDuplicated"));
      return false;
    }
    return true;
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
      <div className="flex flex-col mx-auto w-full mb-6">
        {/* Header */}
        <div className="flex items-center justify-between bg-white py-3 px-6 mb-6">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-center gap-4">
            {t("createBin")}
            {formSubmitting && <LoaderSmall />}
          </h1>

          <button
            className="btn btn-primary flex items-center gap-2"
            onClick={handleSubmit}
          >
            <FaSave />
            <span>{t("save")}</span>
          </button>
        </div>

        {/* Content */}
        <form className="px-6 flex flex-col md:flex-row gap-4 w-full ">
          <div className="card p-5 flex flex-col gap-4 bg-white w-full md:w-1/2">
            <h5 className="border-b border-solid border-b-themeColors-borderColor pb-4 mb-6 text-xl font-medium">
              {t("data")}
            </h5>

            <div className="flex gap-6 flex-col md:flex-row">
              <div className="flex flex-col gap-6 w-full md:w-1/2">
                {/* Form input wrapper */}
                <div className="flex flex-col gap-2">
                  <label htmlFor="owner">{t("customer")}</label>
                  <select
                    name="owner"
                    id="owner"
                    value={formData.owner.id}
                    onChange={(e) => handleSelectChange(e, "owner")}
                    className="input"
                  >
                    {owners.map((owner) => (
                      <option key={owner.id} value={owner.id}>
                        {owner.name}
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
                    value={formData.wasteType.id}
                    onChange={(e) => handleSelectChange(e, "wasteType")}
                  >
                    {configData.contentTypes.map((type) => (
                      <option key={type.id} value={type.id}>
                        {type.name}
                      </option>
                    ))}
                  </select>
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
                  />
                  {formErrors.sigfoxErrorMsg && (
                    <p>{formErrors.sigfoxErrorMsg}</p>
                  )}
                </div> */}
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
                  />
                  {formErrors.locationErrorMsg && (
                    <p>{formErrors.locationErrorMsg}</p>
                  )}
                </div>
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
                    value={formData.model.id}
                    onChange={(e) => handleSelectChange(e, "model")}
                  >
                    {configData.unitModelTypes.map((model) => (
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
                  >
                    {configData.unitTypes.map((type) => (
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
                    <div class="flex items-center mb-4">
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
                        class="w-4 h-4 text-themeColors-accentColor bg-gray-100 border-gray-300 focus:ring-themeColors-accentColor accent-themeColors-accentColor focus:ring-1"
                      />
                      <label htmlFor="default" className="ml-2">
                        {t("modelDefault")}
                      </label>
                    </div>
                    <div class="flex items-center mb-4">
                      <input
                        id="custom"
                        type="radio"
                        value="custom"
                        checked={formData.capacity === "custom"}
                        onChange={handleInputChange}
                        name="capacity"
                        class="w-4 h-4 text-themeColors-accentColor bg-gray-100 border-gray-300 focus:ring-themeColors-accentColor accent-themeColors-accentColor focus:ring-1"
                      />
                      <label for="custom" className="ml-2">
                        {t("custom")}
                      </label>
                    </div>
                  </div>

                  <div className="flex items-center gap-5">
                    <div class="flex flex-col gap-2">
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
                          disabled={formData.capacity === "default"}
                        />
                        <span
                          className="py-[.375rem] px-3 text-base text-center border border-solid border-themeColors-borderColor border-l-0 rounded-tr rounded-br"
                          style={{ color: "#495057" }}
                        >
                          %
                        </span>
                      </div>
                    </div>
                    <div class="flex flex-col gap-2">
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
                          disabled={formData.capacity === "default"}
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
          </div>
          <div className="card p-5 flex flex-col gap-4 bg-white w-full md:w-1/2">
            <h5 className="border-b border-solid border-b-themeColors-borderColor pb-4 mb-6 text-xl font-medium">
              {t("serialNumber")}
            </h5>
            <div className="flex flex-col gap-2">
              {/* Input wrapper */}
              {formData.serialNo.map((value, index) => (
                <div className="relative flex">
                  <input
                    type="text"
                    id={`serialNo-${index}`}
                    name={`serialNo-${index}`}
                    placeholder="Serial number"
                    className="input"
                    value={value}
                    onChange={(e) =>
                      handleSerialNumberInputs(index, e.target.value)
                    }
                  />
                  {value.length >= 1 && (
                    <button
                      type="button"
                      className="py-[.375rem] px-3 text-base text-center border border-solid border-themeColors-borderColor border-l-0 rounded-tr rounded-br bg-themeColors-borderColor bg-opacity-50 hover:bg-opacity-75"
                      style={{ color: "#495057" }}
                      onClick={() => handleRemoveInput(index)}
                    >
                      -
                    </button>
                  )}
                </div>
              ))}
              {formErrors.serialNoErrorMsg && (
                <p>{formErrors.serialNoErrorMsg}</p>
              )}
            </div>
            <h5 className="border-b border-solid border-b-themeColors-borderColor pb-4 my-6 text-xl font-medium">
              {t("location")}
            </h5>
            <div className="w-full h-80 bg-themeColors-borderColor">
              <BinFormMap
                pinPosition={pinPosition}
                handleMapClick={handleMapClick}
              />
            </div>
          </div>
        </form>
      </div>
    </MainWrapper>
  );
};

export default CreateBulkBin;
