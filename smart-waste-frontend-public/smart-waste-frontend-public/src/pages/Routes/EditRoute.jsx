import React, { useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Loader, MainWrapper, NoResults, RoutesMap } from "../../components";
import { useSelector } from "react-redux";
import { getRoute, getUserUnits, updateRoute } from "../../services/routes";
import { toast } from "react-toastify";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import { getPossibleParents } from "../../services/users";
import LoaderSmall from "../../components/LoaderSmall";

const EditRoute = () => {
  const accessToken = useSelector((state) => state.user.accessToken);
  const userData = useSelector((state) => state.user.userData);
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useTranslation("translation");

  const [routeData, setRouteData] = useState(null);
  const [customerData, setCustomerData] = useState(null);
  const [customerUnitsData, setCustomerUnitsData] = useState(null);
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const [selectedBins, setSelectedBins] = useState([]);
  const [loading, setLoading] = useState(true);
  const [fetchError, setFetchError] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const [formData, setFormData] = useState({
    name: "",
    ownerId: "",
    routeUnits: [],
  });

  //fetch owners on first load
  useEffect(() => {
    fetchPossibleParents();
  }, []);

  //fetch route data
  useEffect(() => {
    if (customerData) {
      const pathnames = location.pathname.split("/");
      const routeId = Number(pathnames[pathnames.length - 1]);
      fetchRouteData(routeId);
    }
  }, [customerData]);

  //when customer changes, fetch customer units
  useEffect(() => {
    if (selectedCustomer !== null) {
      fetchOwnersUnits(selectedCustomer);
    }
    setLoading(false);
  }, [selectedCustomer]);

  const fetchRouteData = async (routeId) => {
    try {
      const fetchedRouteData = await fetchWithRetry(
        getRoute,
        accessToken,
        navigate,
        t,
        routeId
      );
      if (fetchedRouteData.status === 200) {
        const routeCustomer = customerData.find(
          (customer) => customer.id === fetchedRouteData.data.userId
        );
        setSelectedCustomer(routeCustomer.id);
        setRouteData(fetchedRouteData.data);
        setFormData({
          name: fetchedRouteData.data.name,
          ownerId: fetchedRouteData.data.userId,
          routeUnits: fetchedRouteData.data.routeUnits,
        });
        setSelectedBins(fetchedRouteData.data.routeUnits);
      } else {
        setFetchError(true);
        setLoading(false);
      }
    } catch (error) {
      console.error(error);
    }
  };
  const fetchPossibleParents = async () => {
    try {
      const possibleParents = await fetchWithRetry(
        getPossibleParents,
        accessToken,
        navigate,
        t,
        3
      );

      if (possibleParents.status === 200) {
        setCustomerData(possibleParents.data);
      } else {
        setFetchError(true);
        setLoading(false);
      }
    } catch (error) {
      console.error(error);
    }
  };
  const fetchOwnersUnits = async (selectedCustomer) => {
    try {
      const fetchedCustomerUnits = await fetchWithRetry(
        getUserUnits,
        accessToken,
        navigate,
        t,
        selectedCustomer
      );
      if (fetchedCustomerUnits.status === 200) {
        setCustomerUnitsData(fetchedCustomerUnits.data);
      } else {
        setFetchError(true);
        setLoading(false);
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleCustomerChange = (e) => {
    setSelectedBins([]);
    if (e.target.value !== "") {
      setSelectedCustomer(Number(e.target.value));
      setFormData((prev) => ({ ...prev, ownerId: Number(e.target.value) }));
    } else {
      setSelectedCustomer(null);
    }
  };

  const handleSubmit = async () => {
    setSubmitting(true);
    const pathnames = location.pathname.split("/");
    const routeId = Number(pathnames[pathnames.length - 1]);
    const routeObj = {
      id: routeId,
      name: formData.name,
      userId: selectedCustomer,
      routeUnits: selectedBins,
    };

    if (routeObj.name === "") {
      toast.error("Route name needs to be defined!");
      setSubmitting(false);
      return;
    }
    if (!routeObj.userId || routeObj.userId === "") {
      toast.error("Route owner needs to be selected!");
      setSubmitting(false);
      return;
    }

    try {
      const res = await fetchWithRetry(
        updateRoute,
        accessToken,
        navigate,
        t,
        routeObj
      );

      if (res.status !== 200) {
        toast.error(t("requestError"));
      } else {
        toast.success("Route updated successfully");
      }
    } catch (error) {
      console.error("error", error);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <MainWrapper loader={true}>
        <Loader />
      </MainWrapper>
    );
  }

  if (fetchError) {
    return (
      <MainWrapper>
        <div className="flex flex-col w-full">
          <NoResults />
        </div>
      </MainWrapper>
    );
  }

  return (
    <MainWrapper>
      <div className="flex flex-col w-full">
        {/* Header */}
        <div className="flex items-center justify-between bg-white py-3 px-6 w-full h-fit">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-centerg gap-4">
            {userData?.role?.id < 5
              ? `${t("editRoute")} ${routeData?.name}`
              : routeData?.name}

            {submitting && <LoaderSmall />}
          </h1>

          <div className="flex items-center gap-2">
            <Link to="/routes" className="btn btn-secondary">
              {t("back")}
            </Link>
            {userData?.role?.id < 5 && (
              <button
                onClick={handleSubmit}
                className="btn btn-primary flex items-center"
                disabled={submitting}
              >
                <span>{t("save")}</span>
              </button>
            )}
          </div>
        </div>

        <div className="px-6 flex flex-col gap-4 w-full md:w-5/6 lg:w-2/3 mx-auto mt-10">
          {/* Form */}
          <div className="card p-5 flex flex-col gap-4 bg-white">
            <div className="flex gap-6">
              <div className="flex flex-col gap-6 w-full">
                <div className="flex flex-col gap-2">
                  <label htmlFor="routeName">{t("routeName")}</label>
                  {/* Input wrapper */}
                  <div className="relative flex">
                    <input
                      type="text"
                      id="routeName"
                      name="routeName"
                      className="input"
                      disabled={
                        userData?.role?.id === 6 || userData?.role?.id === 5
                      }
                      value={formData.name}
                      onChange={(e) =>
                        setFormData((prev) => ({
                          ...prev,
                          name: e.target.value,
                        }))
                      }
                    />
                  </div>
                  {/* {formErrors.serialNoErrorMsg && (
              <p>{formErrors.serialNoErrorMsg}</p>
            )} */}
                </div>
                {/* Form input wrapper */}
                <div className="flex flex-col gap-2">
                  <label htmlFor="customer">{t("customer")}</label>
                  <select
                    name="customer"
                    id="customer"
                    className="input"
                    value={selectedCustomer}
                    onChange={handleCustomerChange}
                    disabled={
                      userData?.role?.id === 6 || userData?.role?.id === 5
                    }
                  >
                    <option value="">Select customer...</option>
                    {customerData?.map((customer) => (
                      <option key={customer.id} value={customer.id}>
                        {customer.name}
                      </option>
                    ))}
                  </select>
                </div>
                {customerUnitsData && (
                  <RoutesMap
                    binsData={customerUnitsData}
                    selectedBins={selectedBins}
                    setSelectedBins={setSelectedBins}
                    isDisabled={
                      userData?.role?.id === 6 || userData?.role?.id === 5
                    }
                  />
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </MainWrapper>
  );
};

export default EditRoute;
