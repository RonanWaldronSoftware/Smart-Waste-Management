import React, { useEffect, useState } from "react";
import {
  FaBatteryQuarter,
  FaBell,
  FaCrown,
  FaDoorOpen,
  FaEnvelope,
  FaExclamationCircle,
  FaFileExport,
  FaLock,
  FaSms,
  FaTrashAlt,
  FaTrashRestoreAlt,
  FaUser,
} from "react-icons/fa";
import { Loader, MainWrapper } from "../../components";
import { BiSave, BiWindowOpen } from "react-icons/bi";
import { useSelector } from "react-redux";
import { getPossibleParents } from "../../services/users";
import { useNavigate } from "react-router-dom";
import { createAlert, getUserAlerts } from "../../services/alerts";
import { toast } from "react-toastify";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import LoaderSmall from "../../components/LoaderSmall";
import BinDamaged from "../../assets/icons/BinDamaged";

const AlertTable = ({
  customers,
  type,
  title,
  handleCheckboxChange,
  t,
  userData,
}) => (
  <div className="card flex flex-col gap-3 bg-white p-5">
    <h3 className="font-medium text-xl flex items-center gap-2">
      {type.includes("admin") ? (
        <FaCrown color="#E7D00A" />
      ) : type.includes("distr") ? (
        <svg
          xmlns="http://www.w3.org/2000/svg"
          height="20"
          viewBox="0 0 448 512"
        >
          <path
            d="M224 0c70.7 0 128 57.3 128 128s-57.3 128-128 128s-128-57.3-128-128S153.3 0 224 0zM209.1 359.2l-18.6-31c-6.4-10.7 1.3-24.2 13.7-24.2H224h19.7c12.4 0 20.1 13.6 13.7 24.2l-18.6 31 33.4 123.9 39.5-161.2c77.2 12 136.3 78.8 136.3 159.4c0 17-13.8 30.7-30.7 30.7H265.1 182.9 30.7C13.8 512 0 498.2 0 481.3c0-80.6 59.1-147.4 136.3-159.4l39.5 161.2 33.4-123.9z"
            fill="#bcbcbc"
          />
        </svg>
      ) : type.includes("driver") ? (
        <svg
          xmlns="http://www.w3.org/2000/svg"
          height="20"
          viewBox="0 0 448 512"
        >
          <path
            d="M48 32H352c26.5 0 48 21.5 48 48V240c0 26.5-21.5 48-48 48H48c-26.5 0-48-21.5-48-48V80C0 53.5 21.5 32 48 32zM80 80c-8.8 0-16 7.2-16 16V224c0 8.8 7.2 16 16 16s16-7.2 16-16V96c0-8.8-7.2-16-16-16zM224 96V224c0 8.8 7.2 16 16 16s16-7.2 16-16V96c0-8.8-7.2-16-16-16s-16 7.2-16 16zM160 80c-8.8 0-16 7.2-16 16V224c0 8.8 7.2 16 16 16s16-7.2 16-16V96c0-8.8-7.2-16-16-16zM304 96V224c0 8.8 7.2 16 16 16s16-7.2 16-16V96c0-8.8-7.2-16-16-16s-16 7.2-16 16zm128 72c0-13.3 10.7-24 24-24h72.8c16.8 0 32.7 7.5 43.3 20.5L631 236.4c5.8 7.1 9 16.1 9 25.3V272v16 64c0 17.7-14.3 32-32 32h-1.6c1 5.2 1.6 10.5 1.6 16c0 44.2-35.8 80-80 80s-80-35.8-80-80c0-5.5 .6-10.8 1.6-16H432 286.4c1 5.2 1.6 10.5 1.6 16c0 44.2-35.8 80-80 80c-26.2 0-49.4-12.6-64-32c-14.6 19.4-37.8 32-64 32c-44.2 0-80-35.8-80-80V384 352c0-17.7 14.3-32 32-32H80 208 432V288 256 168zm48 88H585l-50-61.1c-1.5-1.9-3.8-2.9-6.2-2.9H480v64zm80 144c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM208 432c17.7 0 32-14.3 32-32s-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32zm-96-32c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32z"
            fill="#bcbcbc"
          />
        </svg>
      ) : type.includes("tech") ? (
        <svg
          xmlns="http://www.w3.org/2000/svg"
          height="20"
          viewBox="0 0 448 512"
        >
          <path
            d="M216 0c-13.3 0-24 10.7-24 24V80L163.6 23.1C125.6 43.5 99 82.6 96.2 128H96c-13.3 0-24 10.7-24 24s10.7 24 24 24H352c13.3 0 24-10.7 24-24s-10.7-24-24-24h-.2C349 82.6 322.4 43.5 284.4 23.1L256 80V24c0-13.3-10.7-24-24-24H216zm8 320c65.3 0 119.1-48.9 127-112H97c7.9 63.1 61.7 112 127 112zm-89.6 32C60.2 352 0 412.2 0 486.4C0 500.5 11.5 512 25.6 512H422.4c14.1 0 25.6-11.5 25.6-25.6C448 412.2 387.8 352 313.6 352H134.4z"
            fill="#bcbcbc"
          />
        </svg>
      ) : type.includes("corp") ? (
        <svg
          xmlns="http://www.w3.org/2000/svg"
          height="20"
          viewBox="0 0 448 512"
        >
          <path
            d="M216 0c-13.3 0-24 10.7-24 24V80L163.6 23.1C125.6 43.5 99 82.6 96.2 128H96c-13.3 0-24 10.7-24 24s10.7 24 24 24H352c13.3 0 24-10.7 24-24s-10.7-24-24-24h-.2C349 82.6 322.4 43.5 284.4 23.1L256 80V24c0-13.3-10.7-24-24-24H216zm8 320c65.3 0 119.1-48.9 127-112H97c7.9 63.1 61.7 112 127 112zm-89.6 32C60.2 352 0 412.2 0 486.4C0 500.5 11.5 512 25.6 512H422.4c14.1 0 25.6-11.5 25.6-25.6C448 412.2 387.8 352 313.6 352H134.4z"
            fill="#bcbcbc"
          />
        </svg>
      ) : (
        <FaUser color="#BCBCBC" />
      )}
      {title}
    </h3>
    <div className="table-wrapper">
      <table className="table w-full" style={{ borderInline: 0 }}>
        <thead>
          <tr>
            <th
              className="p-3 pr-9 relative text-left"
              style={{ borderInline: 0 }}
            >
              {t("name")}
            </th>
            <th
              className="p-3 pr-9 relative text-left"
              style={{ borderInline: 0 }}
            >
              {t("personalAlerts")}
            </th>
            <th
              className="w-40 p-3 pr-9 relative text-left min-w-[200px]"
              style={{ borderInline: 0 }}
            >
              {t("alertMethod")}
            </th>
          </tr>
        </thead>
        <tbody>
          {customers.map((customer, index) => (
            <tr key={customer.id}>
              <td style={{ borderInline: 0 }}>{customer.name}</td>
              <td style={{ borderInline: 0 }}>
                <div className="flex flex-wrap gap-3">
                  {userData?.role?.id === 1 && (
                    <AlertCheckbox
                      key={1}
                      alertId={1}
                      customerId={customer.id}
                      checked={customer.alerts.some((a) => a.id === 1)}
                      onChange={(e) =>
                        handleCheckboxChange(e, type, customer.id, 1)
                      }
                      t={t}
                    />
                  )}

                  {[2, 4, 5, 6, 7, 8, 10].map((alertId) => (
                    <AlertCheckbox
                      key={alertId}
                      alertId={alertId}
                      customerId={customer.id}
                      checked={customer.alerts.some((a) => a.id === alertId)}
                      onChange={(e) =>
                        handleCheckboxChange(e, type, customer.id, alertId)
                      }
                      t={t}
                    />
                  ))}
                  {userData?.role?.id === 1 && (
                    <AlertCheckbox
                      key={3}
                      alertId={3}
                      customerId={customer.id}
                      checked={customer.alerts.some((a) => a.id === 3)}
                      onChange={(e) =>
                        handleCheckboxChange(e, type, customer.id, 3)
                      }
                      t={t}
                    />
                  )}
                  {userData?.role?.id === 1 && (
                    <AlertCheckbox
                      key={9}
                      alertId={9}
                      customerId={customer.id}
                      checked={customer.alerts.some((a) => a.id === 9)}
                      onChange={(e) =>
                        handleCheckboxChange(e, type, customer.id, 9)
                      }
                      t={t}
                    />
                  )}
                </div>
              </td>
              <td style={{ borderInline: 0, verticalAlign: "top" }}>
                <div className="flex flex-wrap gap-3 min-h-full mb-auto">
                  {[12, 11, 13].map((alertId) => (
                    <AlertCheckbox
                      key={alertId}
                      alertId={alertId}
                      customerId={customer.id}
                      checked={customer.alerts.some((a) => a.id === alertId)}
                      onChange={(e) =>
                        handleCheckboxChange(e, type, customer.id, alertId)
                      }
                      t={t}
                    />
                  ))}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  </div>
);
const AlertCheckbox = ({ alertId, customerId, checked, onChange, t }) => {
  const icons = {
    1: <FaBatteryQuarter />,
    2: <FaExclamationCircle />,
    3: <FaBatteryQuarter className=" -rotate-90" />,
    4: <FaLock />,
    5: <FaTrashAlt className=" rotate-12" />,
    6: <FaDoorOpen />,
    7: <BiWindowOpen />,
    8: <FaTrashRestoreAlt />,
    9: <BinDamaged />,
    10: <FaFileExport />,
    12: <FaEnvelope />,
    11: <FaSms />,
    13: <FaBell />,
  };
  const titles = {
    1: t("batteryUVLO"),
    2: t("binEmptiedLastPeriod"),
    3: t("batteryOverTempLO"),
    4: t("binLocked"),
    5: t("binTilted"),
    6: t("serviceDoorOpen"),
    7: t("flapStuckOpen"),
    8: t("binFull"),
    9: t("damages"),
    10: t("notReporting"),
    12: t("email"),
    11: "SMS",
    13: t("pushNotification"),
  };
  return (
    <label
      htmlFor={`${customerId}alert-${alertId}`}
      className="custom-checkbox"
      title={titles[alertId]}
    >
      <input
        type="checkbox"
        id={`${customerId}alert-${alertId}`}
        checked={checked}
        onChange={onChange}
      />
      <span>{icons[alertId]}</span>
    </label>
  );
};

const Alerts = () => {
  const navigate = useNavigate();
  const accessToken = useSelector((state) => state.user.accessToken);
  const userData = useSelector((state) => state.user.userData);

  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [refetching, setRefetching] = useState(false);
  const [formSubmitting, setFormSubmitting] = useState(false);

  const [allUsers, setAllUsers] = useState(null);
  const [selectedCustomer, setSelectedCustomer] = useState(null);

  const [alertsData, setAlertsData] = useState({
    adminAlerts: [],
    distributorAlerts: [],
    technicianAlerts: [],
    corporateCustomerAlerts: [],
    customerAlerts: [],
    driverAlerts: [],
  });

  const [NoResults, setNoResults] = useState(null);

  //fetch data
  useEffect(() => {
    //fetchUserData();
    fetchAllCustomers();
  }, []);
  //set selected user
  useEffect(() => {
    if (allUsers && allUsers.length) {
      setSelectedCustomer(allUsers[0].id);
    }
  }, [allUsers]);
  //update alerts on selected user change
  useEffect(() => {
    if (selectedCustomer) {
      setRefetching(true);
      fetchAlerts();
    }
  }, [selectedCustomer]);

  const fetchAllCustomers = async () => {
    try {
      const possibleParents = await fetchWithRetry(
        getPossibleParents,
        accessToken,
        navigate,
        t,
        5
      );

      if (possibleParents.status === 200) {
        if (possibleParents.data.length > 0) {
          setAllUsers(possibleParents.data);
          setNoResults(false);
        } else {
          setNoResults(true);
          setLoading(false);
        }
      }
    } catch (error) {
      console.error(error);
    }
  };

  const fetchAlerts = async () => {
    try {
      const data = await fetchWithRetry(
        getUserAlerts,
        accessToken,
        navigate,
        t,
        selectedCustomer
      );
      if (data.status === 200) {
        const alertCategories = [
          "admin",
          "distributor",
          "corporateCustomer",
          "customer",
          "driver",
          "technician",
        ];
        const alertsData = alertCategories.reduce((acc, category) => {
          acc[`${category}Alerts`] = data.data.filter(
            (item) => item.roleId === getRoleId(category)
          );
          return acc;
        }, {});
        setAlertsData(alertsData);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setRefetching(false);
      setLoading(false);
    }
  };
  const getRoleId = (category) => {
    const roleIds = {
      admin: 1,
      distributor: 2,
      corporateCustomer: 3,
      customer: 4,
      driver: 5,
      technician: 6,
    };
    return roleIds[category];
  };

  const handleCheckboxChange = (e, alertType, customerId, alertId) => {
    if (alertType.includes("admin")) {
      setAlertsData((prevState) => {
        // Find the customer
        const customerIndex = prevState.adminAlerts.findIndex(
          (cust) => cust.id === customerId
        );
        const customer = prevState.adminAlerts[customerIndex];

        // Determine if the alert is already in the array
        const alertExists = customer.alerts.some(
          (alert) => alert.id === alertId
        );

        // Create a new alerts array based on the checkbox state
        let newAlerts;
        if (alertExists) {
          newAlerts = customer.alerts.filter((alert) => alert.id !== alertId);
        } else {
          newAlerts = [...customer.alerts, { id: alertId }];
        }

        // Create a new adminAlerts array with the updated alerts for the customer
        const newAdminAlerts = [...prevState.adminAlerts];
        newAdminAlerts[customerIndex] = {
          ...customer,
          alerts: newAlerts,
        };

        // Return the updated state
        return {
          ...prevState,
          adminAlerts: newAdminAlerts,
        };
      });
    }
    if (alertType.includes("distributor")) {
      setAlertsData((prevState) => {
        // Find the customer
        const customerIndex = prevState.distributorAlerts.findIndex(
          (cust) => cust.id === customerId
        );
        const customer = prevState.distributorAlerts[customerIndex];

        // Determine if the alert is already in the array
        const alertExists = customer.alerts.some(
          (alert) => alert.id === alertId
        );

        // Create a new alerts array based on the checkbox state
        let newAlerts;
        if (alertExists) {
          newAlerts = customer.alerts.filter((alert) => alert.id !== alertId);
        } else {
          newAlerts = [...customer.alerts, { id: alertId }];
        }

        // Create a new adminAlerts array with the updated alerts for the customer
        const newdistributorAlerts = [...prevState.distributorAlerts];
        newdistributorAlerts[customerIndex] = {
          ...customer,
          alerts: newAlerts,
        };

        // Return the updated state
        return {
          ...prevState,
          distributorAlerts: newdistributorAlerts,
        };
      });
    }
    if (alertType.includes("technician")) {
      setAlertsData((prevState) => {
        // Find the customer
        const customerIndex = prevState.technicianAlerts.findIndex(
          (cust) => cust.id === customerId
        );
        const customer = prevState.technicianAlerts[customerIndex];

        // Determine if the alert is already in the array
        const alertExists = customer.alerts.some(
          (alert) => alert.id === alertId
        );

        // Create a new alerts array based on the checkbox state
        let newAlerts;
        if (alertExists) {
          newAlerts = customer.alerts.filter((alert) => alert.id !== alertId);
        } else {
          newAlerts = [...customer.alerts, { id: alertId }];
        }

        // Create a new technicianAlerts array with the updated alerts for the customer
        const newtechnicianAlerts = [...prevState.technicianAlerts];
        newtechnicianAlerts[customerIndex] = {
          ...customer,
          alerts: newAlerts,
        };

        // Return the updated state
        return {
          ...prevState,
          technicianAlerts: newtechnicianAlerts,
        };
      });
    }
    if (alertType.includes("corporate")) {
      setAlertsData((prevState) => {
        // Find the customer
        const customerIndex = prevState.corporateCustomerAlerts.findIndex(
          (cust) => cust.id === customerId
        );
        const customer = prevState.corporateCustomerAlerts[customerIndex];

        // Determine if the alert is already in the array
        const alertExists = customer.alerts.some(
          (alert) => alert.id === alertId
        );

        // Create a new alerts array based on the checkbox state
        let newAlerts;
        if (alertExists) {
          newAlerts = customer.alerts.filter((alert) => alert.id !== alertId);
        } else {
          newAlerts = [...customer.alerts, { id: alertId }];
        }

        // Create a new corporateCustomerAlerts array with the updated alerts for the customer
        const newcorporateCustomerAlerts = [
          ...prevState.corporateCustomerAlerts,
        ];
        newcorporateCustomerAlerts[customerIndex] = {
          ...customer,
          alerts: newAlerts,
        };

        // Return the updated state
        return {
          ...prevState,
          corporateCustomerAlerts: newcorporateCustomerAlerts,
        };
      });
    }
    if (alertType === "customerAlerts") {
      setAlertsData((prevState) => {
        // Find the customer
        const customerIndex = prevState.customerAlerts.findIndex(
          (cust) => cust.id === customerId
        );
        const customer = prevState.customerAlerts[customerIndex];

        // Determine if the alert is already in the array
        const alertExists = customer.alerts.some(
          (alert) => alert.id === alertId
        );

        // Create a new alerts array based on the checkbox state
        let newAlerts;
        if (alertExists) {
          newAlerts = customer.alerts.filter((alert) => alert.id !== alertId);
        } else {
          newAlerts = [...customer.alerts, { id: alertId }];
        }

        // Create a new customerAlerts array with the updated alerts for the customer
        const newcustomerAlerts = [...prevState.customerAlerts];
        newcustomerAlerts[customerIndex] = {
          ...customer,
          alerts: newAlerts,
        };

        // Return the updated state
        return {
          ...prevState,
          customerAlerts: newcustomerAlerts,
        };
      });
    }
    if (alertType.includes("driver")) {
      setAlertsData((prevState) => {
        // Find the customer
        const customerIndex = prevState.driverAlerts.findIndex(
          (cust) => cust.id === customerId
        );
        const customer = prevState.driverAlerts[customerIndex];

        // Determine if the alert is already in the array
        const alertExists = customer.alerts.some(
          (alert) => alert.id === alertId
        );

        // Create a new alerts array based on the checkbox state
        let newAlerts;
        if (alertExists) {
          newAlerts = customer.alerts.filter((alert) => alert.id !== alertId);
        } else {
          newAlerts = [...customer.alerts, { id: alertId }];
        }

        // Create a new driverAlerts array with the updated alerts for the customer
        const newdriverAlerts = [...prevState.driverAlerts];
        newdriverAlerts[customerIndex] = {
          ...customer,
          alerts: newAlerts,
        };

        // Return the updated state
        return {
          ...prevState,
          driverAlerts: newdriverAlerts,
        };
      });
    }
  };

  const handleAlertUpdate = async (e) => {
    e.preventDefault();
    const alertsArr = [];
    setFormSubmitting(true);

    Object.keys(alertsData)
      .filter((key) => alertsData[key].length > 0)
      .forEach((key) =>
        alertsData[key].map(
          (customer) =>
            customer.alerts.length > 0 &&
            alertsArr.push({
              userId: customer.id,
              customerUserId: Number(selectedCustomer),
              alertTypeIds: customer.alerts.map((item) => item.id),
            })
        )
      );
    try {
      const data = await fetchWithRetry(
        createAlert,
        accessToken,
        navigate,
        t,
        alertsArr
      );
      if (data.status === 200) {
        toast.success(t("alertsSuccessfullyUpdated"));
      }
    } catch (error) {
      console.error(error);
    } finally {
      setFormSubmitting(false);
    }
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
            {t("alerts")}
            {formSubmitting || refetching ? <LoaderSmall /> : <></>}
          </h1>

          {/* Additional filters */}
          <div className="flex items-center gap-2">
            <select
              name="parent"
              id="parent"
              className="input"
              value={selectedCustomer}
              onChange={(e) => setSelectedCustomer(e.target.value)}
            >
              {allUsers?.map((user) => (
                <option key={user.id} value={user.id}>
                  {user.name}
                </option>
              ))}
            </select>
            {NoResults !== true && (
              <button
                className="btn btn-primary flex items-center gap-3 disabled:opacity-50"
                onClick={handleAlertUpdate}
                disabled={formSubmitting}
              >
                <BiSave />
                <span>{t("save")}</span>
              </button>
            )}
          </div>
        </div>

        {/* Content */}
        <div className="px-6 flex flex-col gap-4 mb-4">
          {NoResults !== true &&
            Object.keys(alertsData)
              .filter((key) => alertsData[key].length > 0)
              .map((key) => (
                <AlertTable
                  key={key}
                  customers={alertsData[key]}
                  type={key}
                  title={
                    key.includes("admin")
                      ? t("admins")
                      : key.includes("dist")
                      ? t("distributors")
                      : key.includes("driver")
                      ? t("drivers")
                      : key.includes("tech")
                      ? t("technicians")
                      : key.includes("corpor")
                      ? t("corporateCustomers")
                      : t("customer")
                  }
                  handleCheckboxChange={handleCheckboxChange}
                  t={t}
                  userData={userData}
                />
              ))}
        </div>
      </div>
    </MainWrapper>
  );
};

export default Alerts;
