import React, { useEffect, useState } from "react";
import { Loader, MainWrapper } from "../../components";
import BinForm from "./components/BinForm";
import { useSelector } from "react-redux";
import { checkSerialNumber, createBin } from "../../services/units";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { getPossibleParents } from "../../services/users";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import LoaderSmall from "../../components/LoaderSmall";

const CreateBin = () => {
  const navigate = useNavigate();
  const { t } = useTranslation("translation");

  const accessToken = useSelector((state) => state.user.accessToken);
  const configData = useSelector((state) => state.user.configData);

  const [owners, setOwners] = useState([]);
  const [formErrors, setFormErrors] = useState({});
  const [loading, setLoading] = useState(true);
  const [formSubmitting, setFormSubmitting] = useState(false);

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
    deviceType: "",
    wasteType: "",
    deviceInputType: "",
    capacity: "default",
    capacityLow: 40,
    capacityHigh: 80,
  });

  useEffect(() => {
    fetchOwners();
  }, []);

  useEffect(() => {
    if (owners && owners.length > 0) {
      setFormData((prev) => ({
        ...prev,
        owner: owners[0].id,
        model: configData.unitModelTypes[0].id,
        deviceType: configData.unitTypes[1].id,
        wasteType: configData.contentTypes[0].id,
        deviceInputType: configData.deviceInputTypes[1].id,
      }));
    }
  }, [owners]);

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

      case "deviceInputType":
        selectedOption = configData.deviceInputTypes.find(
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
        userId: formData.owner,
        serialNumbers: [formData.serialNo],
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
      } finally {
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
      errors.customSubscriptionErrorMsg =
        "End date of subscription needs to be defined";
      return false;
    }
    const isSerialValid = await validateSerialNo(data.serialNo);
    if (!isSerialValid) {
      return false;
    }

    return true;
  };

  const validateSerialNo = async (serialNo) => {
    const data = await checkSerialNumber(accessToken, serialNo);
    if (data.status !== 200) {
      toast.error(t("serialDuplicated"));
      return false;
    } else return true;
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
            {t("createBin")}
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

export default CreateBin;
