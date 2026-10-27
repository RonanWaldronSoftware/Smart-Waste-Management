import React, { useEffect, useState } from "react";
import { Loader, MainWrapper } from "../../components";
import BinForm from "./components/BinForm";
import { useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { checkSerialNumber, getUnit, updateBin } from "../../services/units";
import { useTranslation } from "react-i18next";
import { toast } from "react-toastify";
import { getPossibleParents } from "../../services/users";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import LoaderSmall from "../../components/LoaderSmall";

const EditBin = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const accessToken = useSelector((state) => state.user.accessToken);
  const configData = useSelector((state) => state.user.configData);

  //const owners = useSelector((state) => state.mapFilters.owners);
  //const models = useSelector((state) => state.mapFilters.models);
  const { t } = useTranslation("translation");

  const [binData, setBinData] = useState(null);
  const [owners, setOwners] = useState([]);
  const [formData, setFormData] = useState({
    owner: "",
    serialNo: "",
    subscription: "",
    customSubscription: {
      from: "",
      to: "",
    },
    renewalDate: "",
    location: "",
    sigfox: "",
    latitude: "",
    longitude: "",
    model: "",
    deviceType: "nonCompacting",
    deviceInputType: "",
    wasteType: "mixed",
    capacity: "default",
    capacityLow: 40,
    capacityHigh: 80,
  });
  const [formErrors, setFormErrors] = useState({});
  const [loading, setLoading] = useState(true);

  const [formSubmitting, setFormSubmitting] = useState(false);

  //fetch data
  useEffect(() => {
    fetchData();
  }, []);

  //set form data
  useEffect(() => {
    if (binData && owners[0]) {
      const formattedSubscription = checkSubscriptionRenewalDate(
        binData.subscriptionRenewalDate
      );

      setFormData({
        owner: binData.user.id,
        serialNo: binData.serialNumber,
        subscription: !formattedSubscription ? "" : formattedSubscription,
        customSubscription: {
          from: "",
          to: "",
        },

        renewalDate:
          formattedSubscription === "custom"
            ? binData.subscriptionRenewalDate
            : "",
        location: binData.location,
        sigfox: binData.serialNumber,
        latitude: binData.latitude,
        longitude: binData.longitude,
        model: binData.unitModelType.id,
        deviceType: binData.unitType.id,
        deviceInputType: binData.deviceInputTypeId ?? configData.deviceInputTypes?.[0]?.id ?? "",
        wasteType: binData.contentType.id,
        capacity:
          binData.emptyLevel === 40 && binData.fullLevel === 80
            ? "default"
            : "custom",
        capacityLow: binData.emptyLevel,
        capacityHigh: binData.fullLevel,
      });

      setLoading(false);
    }
  }, [owners, binData]);

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
      setLoading(false);
      console.error(error);
    }
  };

  const fetchData = async () => {
    try {
      const unitData = await fetchWithRetry(
        getUnit,
        accessToken,
        navigate,
        t,
        id
      );
      // Check if the returned value is null, indicating a logout
      if (unitData.status === 200) {
        setBinData(unitData.data);
        await fetchOwners();
      }
    } catch (error) {
      setLoading(false);
      console.error(error);
    }
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

      case "deviceInputType":
        selectedOption = configData.deviceInputTypes.find(
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

  const handleSubmit = async (e) => {
    e.preventDefault();
    setFormSubmitting(true);

    const isValidated = await validateForm(formData);
    /* const doesSerianNoExists = validateSerialNo(formData.serialNo);
    if (doesSerianNoExists) {
      toast.error(t("serialDuplicated"));
      return;
    } */

    if (isValidated) {
      const subscriptionRenewalDate =
        formData.subscription !== "" && formData.subscription !== "custom"
          ? generateRenewalDate(formData.subscription)
          : formData.subscription === "custom"
          ? formData.renewalDate
          : null;
      const binObj = {
        id,
        userId: formData.owner,
        serialNumber: formData.serialNo,
        unitModelTypeId: formData.model,
        location: formData.location,
        latitude: formData.latitude,
        longitude: formData.longitude,
        unitTypeId: formData.deviceType,
        contentTypeId: formData.wasteType,
        deviceInputTypeId: formData.deviceInputType,
        emptyLevel: formData.capacity === "default" ? 40 : formData.capacityLow,
        fullLevel: formData.capacity === "default" ? 80 : formData.capacityHigh,
        subscriptionRenewalDate,
      };

      try {
        const res = await fetchWithRetry(
          updateBin,
          accessToken,
          navigate,
          t,
          binObj
        );

        if (res.status === 200) {
          toast.success(t("unitSuccessfullyUpdated"));
          setFormSubmitting(false);
        } else {
          setFormSubmitting(false);
        }
      } catch (error) {
        toast.error(t("requestError"));
        console.error(error);
        setFormSubmitting(false);
      }
    } else {
      setFormSubmitting(false);
    }
  };

  const validateForm = async (data) => {
    const errors = {};

    if (data.serialNo === "") {
      toast.error(t("addAtLeastOneSerialNumber"));
      return false;
    }
    if (data.location === "") {
      toast.error(t("locationFieldIsRequired"));
      return false;
    }
    if (data.latitude === "" || data.longitude === "") {
      //errors.locationErrorMsg = "Please place a pin on the map";
      toast.error(t("pleaseSelectPinOnGoogleMap"));
      return false;
    }
    if (data.subscription === "custom" && data.renewalDate === "") {
      errors.renewalDateErrorMsg =
        "End date of subscription needs to be defined";
      return false;
    }
    /* if (
      data.subscription === "custom" &&
      (data.customSubscription.from === "" || data.customSubscription.to === "")
    ) {
      errors.customSubscriptionErrorMsg =
        "Start and end date of subscription needs to be defined";
    } */
    const isSerialValid =
      binData.serialNumber !== data.serialNo
        ? await validateSerialNo(data.serialNo)
        : true;
    if (!isSerialValid) {
      return false;
    }

    return true;

    /*  setFormErrors((prev) => ({
      ...prev,
      ...errors,
    }));

    return Object.keys(errors).length === 0; */
  };

  const checkSubscriptionRenewalDate = (renewalDate) => {
    if (renewalDate) {
      const currentDate = new Date();
      const receivedDate = new Date(renewalDate);
      const remainingMonths = Math.floor(
        (receivedDate - currentDate) / (30 * 24 * 60 * 60 * 1000) // Approximate months
      );

      if (
        remainingMonths === 12 ||
        remainingMonths === 24 ||
        remainingMonths === 36 ||
        remainingMonths === 48
      ) {
        return remainingMonths.toString();
      } else {
        return "custom";
      }
    } else {
      return null;
    }
  };

  const generateRenewalDate = (subscriptionPeriod) => {
    // Create a Date object for the custom renewal date if provided
    const renewalDate = new Date();

    // Calculate the subscription renewal date based on the selected period
    if (subscriptionPeriod === "12") {
      renewalDate.setMonth(renewalDate.getMonth() + 12);
    } else if (subscriptionPeriod === "24") {
      renewalDate.setMonth(renewalDate.getMonth() + 24);
    } else if (subscriptionPeriod === "36") {
      renewalDate.setMonth(renewalDate.getMonth() + 36);
    } else if (subscriptionPeriod === "48") {
      renewalDate.setMonth(renewalDate.getMonth() + 48);
    } // Add more conditions if needed for other subscription periods

    // Convert the renewalDate to "yyyy-mm-dd" format
    return renewalDate.toISOString().slice(0, 10);
  };
  const validateSerialNo = async (serialNo) => {
    const data = await checkSerialNumber(accessToken, serialNo);
    if (data.status !== 200) {
      toast.error(t("serialDuplicated"));
      return false;
    } else return true;
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
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-center gap-4">
            {t("editBin")}
            {formSubmitting && <LoaderSmall />}
          </h1>
        </div>

        {/* Content */}
        <div className="px-6 flex flex-col gap-4 w-full md:w-5/6 lg:w-2/3 mx-auto">
          <BinForm
            owners={owners}
            models={configData.unitModelTypes}
            deviceTypes={configData.unitTypes}
            deviceInputTypes={configData.deviceInputTypes}
            wasteTypes={configData.contentTypes}
            formData={formData}
            formErrors={formErrors}
            loading={formSubmitting}
            handleInputChange={handleInputChange}
            handleSelectChange={handleSelectChange}
            setFormData={setFormData}
            handleSubmit={handleSubmit}
          />
        </div>
      </div>
    </MainWrapper>
  );
};

export default EditBin;
